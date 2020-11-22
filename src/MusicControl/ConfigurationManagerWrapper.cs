using System.Configuration;

namespace PeKaRaSa.MusicControl
{
    public static class ConfigurationManagerWrapper
    {
        public static string AppSettings(string name)
        {
            return ConfigurationManager.AppSettings[name];
        }
    }
}
