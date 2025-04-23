using System;

namespace ClientSDK.Data;

public class SDKMissconfiguredException(string msg) : Exception(msg);