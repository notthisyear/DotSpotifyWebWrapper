using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using DotSpotifyWebWrapper.ApiCalls.Shared;
using DotSpotifyWebWrapper.Types;
using DotSpotifyWebWrapper.Utilities;

namespace DotSpotifyWebWrapper.ApiCalls
{
    public enum SpotifyApiCallType
    {
        GetCurrentUserProfile,
        GetAvailableDevices,
        GetPlaybackState,
        StartOrResumePlayback,
        PausePlayback,
        SetPlaybackVolume,
        GetTrack,
        GetCurrentlyPlayingTrack
    }

    public abstract class SpotifyApiCallBase
    {
        #region Public properties
        public abstract SpotifyApiCallType ApiCall { get; }

        public bool SuccessfulStatusCode { get; private set; }

        public HttpStatusCode ReturnStatusCode { get; private set; }

        public string ErrorReasonString { get; private set; } = string.Empty;

        public virtual bool ReturnsErrorContent { get; } = true;

        public ErrorObject Error { get; private set; }
        #endregion

        #region Protected properties
        protected abstract HttpRequestMethod RequestMethod { get; }

        protected abstract List<AccessScopeType> Scopes { get; }

        protected abstract string Endpoint { get; }

        protected HttpResponseMessage? ResponseMessage { get; private set; }

        protected delegate Exception? CustomResponseHandler<T>(T response, string rawContent);
        #endregion

        #region Private constants
        private string _responseContent = string.Empty;
        private const string AuthorizationHeaderKey = "Authorization";
        #endregion

        #region Public method
        public string GetFormattedError()
            => ReturnsErrorContent ? $"{Error.Status} - {Error.Message}" : "No error info";
        #endregion

        #region Internal methods
        internal TokenStatus VerifyAccessToken(SpotifyAccessToken accessToken)
        {
            foreach (var requiredScope in Scopes)
            {
                if (!accessToken.HasScope(requiredScope))
                    return TokenStatus.TokenInsufficient;
            }

            return accessToken.HasExpired() ? TokenStatus.TokenExpired : TokenStatus.TokenOk;
        }

        internal HttpRequestMessage GetHttpRequestMessage(SpotifyAccessToken accessToken)
        {
            var request = new HttpRequestMessage(RequestMethod switch
            {
                HttpRequestMethod.Post => HttpMethod.Post,
                HttpRequestMethod.Put => HttpMethod.Put,
                HttpRequestMethod.Get => HttpMethod.Get,
                _ => throw new NotImplementedException(),
            }, GetEndpoint());

            request.Headers.Add(AuthorizationHeaderKey, GetAuthorizationHeaderValue(accessToken.TokenType!, accessToken.AccessToken!));
            AddBodyToRequestIfNeeded(request);
            return request;
        }

        internal async Task SetHttpResponseMessage(HttpResponseMessage? response)
        {
            SuccessfulStatusCode = response != default && response.IsSuccessStatusCode;
            ReturnStatusCode = response?.StatusCode ?? HttpStatusCode.Unused;
            ErrorReasonString = response?.ReasonPhrase ?? "No response received";

            if (response != default)
                _responseContent = await response.Content.ReadAsStringAsync();

            if (SuccessfulStatusCode)
            {
                ParseResponse();
            }
            else if (ReturnsErrorContent)
            {
                var (success, errorResponse) = ReadAndDeserializeJsonResponse<ErrorResponse>();
                if (success)
                    Error = errorResponse.Error;
            }

        }
        #endregion

        #region Protected methods
        protected virtual void AddBodyToRequestIfNeeded(HttpRequestMessage request) { }

        protected virtual void ParseResponse() { }

        protected virtual string GetEndpoint() => Endpoint;

        protected (bool success, T? response) ReadAndDeserializeJsonResponse<T>(bool emptyResponseValid = false, CustomResponseHandler<T>? customResponseHandler = default)
        {
            if (string.IsNullOrEmpty(_responseContent))
                return (emptyResponseValid, default);

            var (r, e) = _responseContent.DeserializeJsonString<T>(convertSnakeCaseToPascalCase: true);
            if (customResponseHandler != default)
                e = customResponseHandler.Invoke(r!, _responseContent);
            return (e == default, r);
        }
        #endregion

        private static string GetAuthorizationHeaderValue(string tokenType, string accessToken)
            => tokenType + " " + accessToken;
    }
}
