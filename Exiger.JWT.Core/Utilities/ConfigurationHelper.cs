using System.Collections.Generic;
using System.Configuration;

namespace Exiger.JWT.Core.Utilities
{
    public static class ConfigurationHelper
    {
        public static string GetConnectionString(string connectionStringName)
        {
            string connectionString = ConfigurationManager.ConnectionStrings[connectionStringName].ConnectionString;
            if (string.IsNullOrWhiteSpace(connectionString))
            {
                throw new ConfigurationErrorsException(string.Format("Connection string {0} not found.", connectionStringName));
            }

            return connectionString;
        }

        public static string GetSetting(string settingName, bool throwIfEmptyOrMissing = true)
        {
            string settingValue = ConfigurationManager.AppSettings[settingName];
            if (string.IsNullOrWhiteSpace(settingValue) && throwIfEmptyOrMissing)
            {
                throw new ConfigurationErrorsException(string.Format("Configuration setting {0} not found.", settingName));
            }

            return settingValue;
        }

        public static bool GetBooleanSetting(string settingName)
        {
            string settingValue = GetSetting(settingName);
            bool booleanSettingValue;
            if (bool.TryParse(settingValue, out booleanSettingValue))
            {
                return booleanSettingValue;
            }
            else
            {
                throw new ConfigurationErrorsException(string.Format("Value {0} for configuration setting {1} cannot be parsed to Boolean.", settingValue, settingName));
            }
        }

        public static bool TryGetBooleanSetting(string settingName, out bool result)
        {
            try
            {
                result = GetBooleanSetting(settingName);
                return true;
            }
            catch
            {
                result = false;
                return false;
            }
        }

        public static int GetInt32Setting(string settingName)
        {
            string settingValue = GetSetting(settingName);
            int intSettingValue;
            if (int.TryParse(settingValue, out intSettingValue))
            {
                return intSettingValue;
            }
            else
            {
                throw new ConfigurationErrorsException(string.Format("Value {0} for configuration setting {1} cannot be parsed to Int32.", settingValue, settingName));
            }
        }

        public static List<int> GetListInt32Setting(string settingName)
        {
            List<int> parsedValues = new List<int>();
            string settingValue = GetSetting(settingName, false);

            if (string.IsNullOrEmpty(settingValue))
            {
                return parsedValues;
            }

            var commaSeparatedValues = settingValue.Split(',');

            foreach (var value in commaSeparatedValues)
            {
                int intSettingValue;
                if (int.TryParse(value, out intSettingValue))
                {
                    parsedValues.Add(intSettingValue);
                }
            }

            return parsedValues;
        }
    }
}
