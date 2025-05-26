using System.Collections.Generic;
using DotSpotifyWebWrapper.Types;
using DotSpotifyWebWrapper.Utilities;

namespace DotSpotifyWebWrapper.ApiCalls
{
    public sealed class SetPlaybackVolume(int percent, string deviceId = "") : SpotifyApiCallBase
    {
        public override SpotifyApiCallType ApiCall => SpotifyApiCallType.SetPlaybackVolume;

        protected override HttpRequestMethod RequestMethod => HttpRequestMethod.Put;

        protected override List<AccessScopeType> Scopes => [AccessScopeType.UserModifyPlaybackState];

        protected override string Endpoint => SpotifyEndpoint.PlayerBaseEndpoint +
            "/volume" +
            HttpRequestUriUtilities.GetQueryString(new()
            {
                {"volume_percent", $"{_percent}"},
                {"device_id", _deviceId }
            });

        private readonly int _percent = percent;

        private readonly string _deviceId = deviceId;
    }
}
