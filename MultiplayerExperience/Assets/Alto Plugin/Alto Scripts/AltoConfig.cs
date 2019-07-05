using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Alto
{
public class AltoConfig : Singleton<AltoConfig> {

        ConfigManager config;

        static ConfigManager Config
        {
            get
            {
                if (Instance.config == null)
                {
                    Instance.config = new ConfigManager("Alto.cfg");
                }
                return Instance.config;
            }
        }

        public string GetSetting(string section, string key, string defaultValue = null)
        {
            return Config.GetSetting(section, key, defaultValue);
        }

        public float GetValue(string section, string key, float defaultValue = 0f)
        {
            return Config.GetValue(section, key, defaultValue);
        }
    }
}