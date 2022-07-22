using System;

namespace MerinoClient.Core.Managers
{
    public class ConfigManager
    {
        public string CategoryName { get; }

        public static ConfigManager Instance { get; private set; }

        public static ConfigManager Create(string categoryName)
        {
            if (Instance != null)
            {
                throw new Exception("ConfigManager already exists.");
            }

            return new ConfigManager(categoryName);
        }

        private ConfigManager(string categoryName)
        {
            Instance = this;
            CategoryName = categoryName;
        }
    }
}
