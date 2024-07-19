using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using DotSpotifyWebWrapper.ApiCalls;
using DotSpotifyWebWrapper.Types;
using DotSpotifyWebWrapper.Utilities;
using static System.Net.Mime.MediaTypeNames;

namespace DotSpotifyWebWrapper
{
    public class SpotifyClient : IDisposable
    {
        public enum ImageFormat
        {
            None,
            Jpg,
            Png,
            Tiff
        }

        public bool HasAccessToken => _currentAccessToken != default;

        #region Private fields
        private readonly string _clientId;
        private readonly SpotifyHttpClient _client;
        private readonly SpotifyHttpListener _listener;
        private readonly bool _cacheAccessToken = true;
        private SpotifyAccessToken? _currentAccessToken;
        private bool _disposedValue;
        private static readonly string s_applicationDataPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), Path.GetFileNameWithoutExtension(AppDomain.CurrentDomain.FriendlyName));
        private static readonly string s_accessTokenCachePath = Path.Combine(s_applicationDataPath, "spotify_access_token.json");
        #endregion

        public SpotifyClient(string clientId, int localListenerPort, bool cacheAccessToken = false)
        {
            _clientId = clientId;
            _cacheAccessToken = cacheAccessToken;

            // From https://learn.microsoft.com/en-us/dotnet/fundamentals/networking/http/httpclient-guidelines
            var client = new HttpClient(new SocketsHttpHandler
            {
                PooledConnectionLifetime = TimeSpan.FromMinutes(15)
            });
            var listener = new HttpListener();

            _client = new(client);
            _listener = new(listener, localListenerPort);

            if (_cacheAccessToken)
            {
                if (!Path.Exists(s_applicationDataPath))
                    Directory.CreateDirectory(s_applicationDataPath);

                if (Path.Exists(s_accessTokenCachePath))
                    TryLoadCachedAccessToken();
            }
        }

        #region Public methods
        public async Task<bool> TryGetTokenForScopes(AuthorizationFlowType flowType, IProgress<string>? progressReporter = default, int pkceVerifierLength = 0, params AccessScopeType[] scopes)
        {
            if (scopes.Length == 0)
                return false;

            List<AccessScopeType> scopesToInclude = [];
            if (_currentAccessToken != default)
            {
                var missingScopes = scopes.Where(x => !_currentAccessToken.HasScope(x));
                if (!missingScopes.Any())
                {
                    if (_currentAccessToken.HasExpired())
                        return await RefreshToken(_currentAccessToken);
                    return true;
                }

                foreach (var existingScope in _currentAccessToken.Scopes!)
                    scopesToInclude.Add(existingScope);
            }

            foreach (var scope in scopes)
            {
                if (!scopesToInclude.Contains(scope))
                    scopesToInclude.Add(scope);
            }

            var token = flowType switch
            {
                AuthorizationFlowType.AuthorizationCodePkce =>
                    await Authorizer.TryAuthorizationUsingAuthorizationCodePkce(_listener, _client, scopesToInclude, _clientId, pkceVerifierLength, progressReporter),
                _ =>
                    throw new NotSupportedException($"Only AuthorizationFlowType '{AuthorizationFlowType.AuthorizationCodePkce}' is supported"),
            };

            if (token != default)
                SaveToken(token);

            return token != default;
        }

        public async Task<bool> SendSpotifyApiCall(SpotifyApiCallBase call)
        {
            if (_currentAccessToken == default)
                return false;

            var tokenStatus = call.VerifyAccessToken(_currentAccessToken);
            if (tokenStatus == TokenStatus.TokenInsufficient)
                return false;

            if (tokenStatus == TokenStatus.TokenExpired)
                return await RefreshToken(_currentAccessToken);

            var request = call.GetHttpRequestMessage(_currentAccessToken);
            try
            {
                var response = await _client.SendHttpRequest(request);
                await call.SetHttpResponseMessage(response);
                return response != default;
            }
            catch (TimeoutException)
            {
                return false;
            }
        }

        public async Task<(bool success, ImageFormat format)> DownloadImageAndSaveToFile(string url, string destinationPath)
        {
            var response = await _client.SendRequest(HttpRequestMethod.Get, url);
            if (!response.IsSuccessStatusCode)
                return (false, ImageFormat.None);

            var mediaType = response.Content.Headers.ContentType?.MediaType ?? string.Empty;
            if (string.IsNullOrEmpty(mediaType))
                return (false, ImageFormat.None);

            var format = mediaType switch
            {
                Image.Jpeg => ImageFormat.Jpg,
                Image.Png => ImageFormat.Png,
                Image.Tiff => ImageFormat.Tiff,
                _ => ImageFormat.None
            };

            if (format == ImageFormat.None)
                return (false, ImageFormat.None);

            var content = await response.Content.ReadAsByteArrayAsync();
            if (content == default || content.Length == 0)
                return (false, ImageFormat.None);

            await File.WriteAllBytesAsync(destinationPath, content);
            return (true, format);
        }
        #endregion

        #region Private methods
        private void TryLoadCachedAccessToken()
        {
            var (token, e) = JsonUtilities.DeserializeJsonString<SpotifyAccessToken>(File.ReadAllText(s_accessTokenCachePath));
            if (e != default)
                throw e;

            token!.SetExpiration(token.TokenFetchedAt);
            token.ParseScopes();
            _currentAccessToken = token;

        }

        private async Task<bool> RefreshToken(SpotifyAccessToken currentToken)
        {
            var newToken = await Authorizer.TryRefreshToken(_client, _clientId, currentToken);
            if (newToken == default)
                return false;

            SaveToken(newToken);
            return true;
        }

        private void SaveToken(SpotifyAccessToken token)
        {
            _currentAccessToken = token;
            if (_cacheAccessToken)
            {
                var (seralizedToken, e) = JsonUtilities.SerializeToJson(token);
                if (e != default)
                    throw e;
                File.WriteAllText(s_accessTokenCachePath, seralizedToken);
            }
        }
        #endregion

        #region Disposal
        protected virtual void Dispose(bool disposing)
        {
            if (!_disposedValue)
            {
                if (disposing)
                    _listener.Dispose();
                _disposedValue = true;
            }
        }

        public void Dispose()
        {
            Dispose(disposing: true);
            GC.SuppressFinalize(this);
        }
        #endregion
    }
}
