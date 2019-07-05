using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using System;

/// <summary>
/// Simple config reader/manager - NB
/// </summary>

public class ConfigManager
{
    //stores the section key & value as a dictionary set of 3
    private struct ConfigDictionary
    {
        private string section, key, value;
        
        public ConfigDictionary(string sectionValue, string keyValue, string valueValue)
        {
            section = sectionValue;
            key = keyValue;
            value = valueValue;
        }

        public string GetSection() { return section; }
        public string GetKey() { return key; }
        public string GetValue() { return value; }
    }

    private List<ConfigDictionary> configData;

    public ConfigManager(string configFile)
    {
        //read file, get values as a string
        string path = Application.streamingAssetsPath + "\\" + configFile;

        configData = new List<ConfigDictionary>();

        using (StreamReader reader = new StreamReader(path))
        {
            string currentLine = null;
            string sectionLine = null;
            while ((currentLine = reader.ReadLine()) != null)
            {
                if (currentLine.Contains("["))
                {
                    sectionLine = currentLine.Trim(new Char[] { '[', ']', ' '});
                }
                else if (currentLine.Length > 2 && !currentLine.Contains("//"))
                {
                    string trimmedLine = currentLine.Trim(new Char[] { ' ' });
                    string[] splitLine = trimmedLine.Split('=');
                    ConfigDictionary newConfigDict = new ConfigDictionary(sectionLine, splitLine[0], splitLine[1]);
                    configData.Add(newConfigDict);
                }
            }
        }
    }

    public string GetSetting(string sectionValue, string keyValue, string defaultValue = null)
    {
        foreach(ConfigDictionary configDict in configData)
        {
            if(configDict.GetSection() == sectionValue && configDict.GetKey() == keyValue)
            {
                return configDict.GetValue();
            }
        }
        Debug.Log("Could not find value for " + sectionValue + ", " + keyValue);
        return defaultValue;
    }

    public float GetValue(string sectionValue, string keyValue, float defaultValue = 0f)
    {
        foreach (ConfigDictionary configDict in configData)
        {
            if (configDict.GetSection() == sectionValue && configDict.GetKey() == keyValue)
            {
                float floatValue;
                if(float.TryParse(configDict.GetValue(), out floatValue))
                {
                    return floatValue;
                }
                else
                {
                    Debug.Log("Could not convert value to float for " + sectionValue + ", " + keyValue + ". Are there any non numerical symbols in the config value?");
                    return defaultValue;
                }
                
                
            }
        }
        Debug.Log("Could not find value for " + sectionValue + ", " + keyValue);
        return defaultValue;
    }



}
