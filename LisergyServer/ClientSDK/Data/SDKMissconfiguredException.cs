using System;

namespace ClientSDK.Data
{
    public class SDKMissconfiguredException : Exception
    {
        public SDKMissconfiguredException(string msg) : base(msg) { }
    }
}
