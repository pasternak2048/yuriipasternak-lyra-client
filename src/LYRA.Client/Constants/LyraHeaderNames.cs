namespace LYRA.Client.Constants
{
    /// <summary>
    /// Contains the standard LYRA HTTP header names used for signed requests.
    /// </summary>
    public static class LyraHeaderNames
    {
        public const string Caller = "x-lyra-caller";
        public const string Target = "x-lyra-target";
        public const string Method = "x-lyra-method";
        public const string Path = "x-lyra-path";
        public const string Payload = "x-lyra-payload";
        public const string PayloadHash = "x-lyra-payload-hash";
        public const string Timestamp = "x-lyra-timestamp";
        public const string Context = "x-lyra-context";
        public const string Signature = "x-lyra-signature";
    }
}
