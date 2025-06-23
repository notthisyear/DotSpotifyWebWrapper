using System.Collections.Generic;
using DotSpotifyWebWrapper.ApiCalls.Shared;
using DotSpotifyWebWrapper.Types;
using DotSpotifyWebWrapper.Utilities;

namespace DotSpotifyWebWrapper.ApiCalls
{
    public sealed class GetTrack(string trackId, string market = "") : SpotifyApiCallBase
    {
        public TrackObject? Response { get; private set; }

        public override SpotifyApiCallType ApiCall => SpotifyApiCallType.GetTrack;

        protected override HttpRequestMethod RequestMethod => HttpRequestMethod.Get;

        protected override List<AccessScopeType> Scopes => [];

        protected override string Endpoint =>
            SpotifyEndpoint.TracksEndpoint +
            "/" + _trackId +
            HttpRequestUriUtilities.GetQueryStringOrEmpty("market", _market);

        private readonly string _trackId = trackId;
        private readonly string _market = market;

        protected override void ParseResponse()
        {
            var (success, data) = ReadAndDeserializeJsonResponse<TrackObject>();
            if (success)
                Response = data;
        }
    }
}
