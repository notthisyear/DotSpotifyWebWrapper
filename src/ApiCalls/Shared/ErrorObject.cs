namespace DotSpotifyWebWrapper.ApiCalls.Shared
{
    public readonly struct ErrorObject
    {
        /// <summary>
        /// The HTTP status code.
        /// </summary>
        public int Status { get; init; }

        /// <summary>
        /// A short description of the cause of the error.
        /// </summary>
        public string Message { get; init; }

        /// <summary>
        /// A error reason string. Not always present.
        /// </summary>
        public string? Reason { get; init; }
    }

    public readonly struct ErrorResponse
    {
        /// <summary>
        /// The current error response.
        /// </summary>
        public ErrorObject Error { get; init; }
    }
}
