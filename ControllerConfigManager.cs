using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

namespace PerControllerOffset
{
    class ControllerConfigManager
    {
        private static readonly string ConfigFolderPath = Environment.CurrentDirectory + "\\UserData\\PerControllerOffset\\";

        private static string GetConfigPath(string name)
        {
            // santise the name by removing invalid chars and moving to lowercase
            char[] invalids = Path.GetInvalidFileNameChars();
            name = name.ToLower();
            name = string.Join("_", name.Split(invalids, StringSplitOptions.RemoveEmptyEntries)).TrimEnd('.');
            return ConfigFolderPath + name + ".json";
        }

        public static void EnsureFolderExists()
        {
            Directory.CreateDirectory(ConfigFolderPath);
        }

        public static ControllerOffsetConfig GetControllerConfig(string name)
        {
            string config_path = GetConfigPath(name);
            PCOffsetPlugin.Log.Info($"Loading controller config from {config_path}");
            return FileHelpers.LoadFromJSONFile<ControllerOffsetConfig>(config_path);
        }

        public static void SaveControllerConfig(string name, ControllerOffsetConfig config)
        {
            string config_path = GetConfigPath(name);
            PCOffsetPlugin.Log.Info($"Saving controller config to {config_path}");
            config.controllerName = name;
            FileHelpers.SaveToJSONFile(config, config_path, true);
        }

        public static bool HasControllerConfig(string name)
        {
            string filepath = GetConfigPath(name);
            return File.Exists(filepath);
        }
    }
}
