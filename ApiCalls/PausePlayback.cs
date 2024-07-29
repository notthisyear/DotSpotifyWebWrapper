using System.Collections.Generic;
using DotSpotifyWebWrapper.Types;

namespace DotSpotifyWebWrapper.ApiCalls
{
    public sealed class PausePlayback(string deviceId = "") : SpotifyApiCallBase
    {
        public override SpotifyApiCallType ApiCall => SpotifyApiCallType.PausePlayback;

        protected override HttpRequestMethod RequestMethod => HttpRequestMethod.Put;

        protected override List<AccessScopeType> Scopes => [AccessScopeType.UserModifyPlaybackState];

        protected override string Endpoint => SpotifyEndpoint.PlayerBaseEndpoint + "/pause" + (string.IsNullOrEmpty(_deviceId) ? "" : ("?device_id=" + _deviceId));

        private readonly string _deviceId = deviceId;
    }
}
