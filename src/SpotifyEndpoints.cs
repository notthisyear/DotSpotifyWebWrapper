namespace DotSpotifyWebWrapper
{
    internal static class SpotifyEndpoint
    {
        private const string AccountBaseUrl = "https://accounts.spotify.com/";

        private const string ApiBaseUrl = "https://api.spotify.com/v1/";

        public const string AuthorizeEndpoint = AccountBaseUrl + "authorize";

        public const string OAuthTokenEndpoint = AccountBaseUrl + "api/token";

        public const string UserBaseEndpoint = ApiBaseUrl + "me";

        public const string PlayerBaseEndpoint = UserBaseEndpoint + "/player";

        public const string TracksEndpoint = ApiBaseUrl + "tracks";

        public const string EpisodesEndpoint = ApiBaseUrl + "episodes";
    }
}
