using System;
using System.Collections.Generic;
using System.Text;

namespace ClientSDK.Data
{
    public class SDKMissconfiguredException : Exception
    {
        public SDKMissconfiguredException(string msg) : base(msg) { }
    }
}
