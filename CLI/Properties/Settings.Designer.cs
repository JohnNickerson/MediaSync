using System;
using System.Diagnostics;
using System.IO;
using Microsoft.Extensions.Configuration;

namespace AssimilationSoftware.MediaSync.CLI.Properties {


    internal sealed partial class Settings
    {

        private static Settings defaultInstance = new Settings();

        public static Settings Default
        {
            get
            {
                return defaultInstance;
            }
        }

        private bool _configured;
        public bool Configured
        {
            get
            {
                return _configured;
            }
            set
            {
                _configured = value;
            }
        }

        private string _machineName;
        public string MachineName
        {
            get
            {
                return _machineName;
            }
            set
            {
                _machineName = value;
            }
        }

        private string _metaDataFolder;
        public string MetadataFolder
        {
            get
            {
                return _metaDataFolder;
            }
            set
            {
                _metaDataFolder = value;
            }
        }

        private bool _upgradeRequired;
        public bool UpgradeRequired
        {
            get
            {
                return _upgradeRequired;
            }
            set
            {
                _upgradeRequired = value;
            }
        }

        public void Upgrade()
        {
            // Nothing for now.
        }
        
        static Settings()
        {
            Reload();
        }

        public static void Reload()
        {
            try
            {
                string settingsFile = "appsettings.json";
                string machineFile = $"appsettings.{Environment.MachineName}.json";
                var builder = new ConfigurationBuilder()
                    .SetBasePath(Directory.GetCurrentDirectory())
                    .AddJsonFile(settingsFile, optional: false, reloadOnChange: true)
                    .AddJsonFile(machineFile, optional: true, reloadOnChange: true);
                Debug.WriteLine("Builder initialised.");

                IConfigurationRoot configuration = builder.Build();
                Debug.WriteLine("Configuration built.");

                defaultInstance = new();
                configuration.GetSection("Default").Bind(defaultInstance);
                Debug.WriteLine("Bound to singleton instance.");
            }
            catch (System.Exception ex)
            {
                // Default configuration.
                Console.WriteLine($"Configuration file error: {ex.Message}. Using default config.");
                defaultInstance = new();
            }
        }

        public void Save()
        {
            string jsonPath = Path.Combine(Directory.GetCurrentDirectory(), $"appsettings.{Environment.MachineName}.json");
            System.Text.StringBuilder json = new();
            json.AppendLine("{");
            json.AppendLine("  \"Default\":");
            json.AppendLine(Newtonsoft.Json.JsonConvert.SerializeObject(Default, Newtonsoft.Json.Formatting.Indented));
            json.AppendLine("}");
            File.WriteAllText(jsonPath, json.ToString());
        }
    }
}
