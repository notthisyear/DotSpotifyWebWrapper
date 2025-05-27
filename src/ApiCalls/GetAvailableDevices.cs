using System.Collections.Generic;
using DotSpotifyWebWrapper.ApiCalls.Shared;
using DotSpotifyWebWrapper.Types;


namespace DotSpotifyWebWrapper.ApiCalls
{
    public sealed class GetAvailableDevices : SpotifyApiCallBase
    {
        public record ResponseData
        {
            /// <summary>
            /// Array of DeviceObject.
            /// </summary>
            public List<DeviceObject>? Devices { get; init; }
        }

        public ResponseData? Response { get; private set; }

        public override SpotifyApiCallType ApiCall => SpotifyApiCallType.GetAvailableDevices;

        protected override HttpRequestMethod RequestMethod => HttpRequestMethod.Get;

        protected override List<AccessScopeType> Scopes => [AccessScopeType.UserReadPlaybackState];

        protected override string Endpoint => SpotifyEndpoint.PlayerBaseEndpoint + "/devices";

        protected override void ParseResponse()
        {
            var (success, data) = ReadAndDeserializeJsonResponse<ResponseData>(emptyResponseValid: true);
            if (success)
                Response = data;
        }
    }
}
