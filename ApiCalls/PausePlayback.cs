using System.Collections.Generic;
using DotSpotifyWebWrapper.Types;

namespace DotSpotifyWebWrapper.ApiCalls
{
    public sealed class PausePlayback : SpotifyApiCallBase
    {
        public override SpotifyApiCallType ApiCall => SpotifyApiCallType.PausePlayback;

        protected override HttpRequestMethod RequestMethod => HttpRequestMethod.Put;

        protected override List<AccessScopeType> Scopes => [AccessScopeType.UserModifyPlaybackState];

        protected override string Endpoint => SpotifyEndpoint.PlayerBaseEndpoint + "/pause";
    }
}
