using System;

namespace PeKaRaSa.MusicControl
{
    public static class AppSettings
    {
        public static string GetValueOrDefault(string name, string defaultValue)
        {
            try
            {
                return ConfigurationManagerWrapper.AppSettings(name)?? defaultValue;
            }
            catch
            {
                return defaultValue;
            }
        }

        public static Int32 GetInt32OrDefault(string name, Int32 defaultValue)
        {
            try
            {
                return Int32.Parse(GetValueOrDefault(name, defaultValue.ToString()));
            }
            catch
            {
                return defaultValue;
            }
        }
    }
}
