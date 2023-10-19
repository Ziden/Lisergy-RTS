using NUnit.Framework;
using PlayFab;

namespace ServerTests.Integration
{
    public class TestPlayfabIntegration
    {
        [SetUp]
        public void Setup()
        {
            // Test Playfab
            PlayFabSettings.staticSettings.TitleId = "45CE6";
            PlayFabSettings.staticSettings.DeveloperSecretKey = "IW1M1H6FSUZKAYS3PGI1PZK88UB777SROC7S68B4478PC37GMG";
        }

        [Test] 
        public void Test()
        {

        }
    }
}
