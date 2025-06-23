using System.Collections.Generic;
using DotSpotifyWebWrapper.ApiCalls.Shared;
using DotSpotifyWebWrapper.Types;
using DotSpotifyWebWrapper.Utilities;

namespace DotSpotifyWebWrapper.ApiCalls
{
    public sealed class GetEpisode(string episodeId, string market = "") : SpotifyApiCallBase
    {
        public EpisodeObject? Response { get; private set; }

        public override SpotifyApiCallType ApiCall => SpotifyApiCallType.GetEpisode;

        protected override HttpRequestMethod RequestMethod => HttpRequestMethod.Get;

        protected override List<AccessScopeType> Scopes => [AccessScopeType.UserReadPlaybackPosition];

        protected override string Endpoint =>
            SpotifyEndpoint.EpisodesEndpoint +
            "/" + _episodeId +
            HttpRequestUriUtilities.GetQueryStringOrEmpty("market", _market);

        private readonly string _episodeId = episodeId;
        private readonly string _market = market;

        protected override void ParseResponse()
        {
            var (success, data) = ReadAndDeserializeJsonResponse<EpisodeObject>();
            if (success)
                Response = data;
        }
    }
}
