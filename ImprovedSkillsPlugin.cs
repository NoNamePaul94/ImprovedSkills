using System;
using System.Reflection;
using BepInEx;
using HarmonyLib;
using UnityEngine;
using System.Collections.Generic;
using System.Collections;
using BepInEx.Configuration;
using BepInEx.Logging;
using ModSettingsUI;

namespace ImprovedSkills
{
    [BepInPlugin(pluginGUID, pluginName, pluginVersion)]
    public class ImprovedSkillsPlugin : BaseUnityPlugin
    {
        public const string pluginGUID = "nonamepaul.plugins.improvedskills";
        public const string pluginName = "Improved Skills";
        public const string pluginVersion = "1.0.0";

        internal readonly Harmony harmony;
        internal readonly Assembly assembly;

        public static List<KeyValuePair<string, ConfigEntry<float>>> _skillConfigurations;
        public static ConfigEntry<bool> _configIsEnabled;
        public static ConfigEntry<bool> _configMultiplayerActive;

        public static ManualLogSource logger;

        public static ConfigFile config;
        public static bool initialized = false;

        public ImprovedSkillsPlugin()
        {
            assembly = Assembly.GetExecutingAssembly();
            harmony = new Harmony(pluginGUID);
        }
        public static float GetMultiplierForSkillBySkillType(Skills.SkillType skillType)
        {
            string key = skillType.ToString().ToLower();
            foreach(KeyValuePair<string, ConfigEntry<float>> pair in _skillConfigurations)
            {
                if (pair.Key == key)
                {
                    return pair.Value.Value;
                }
            }
            return 1f;
        }

        public static bool IsEnabled()
        {
            return _configIsEnabled.Value;
        }

        public static bool IsMultiplayerActive()
        {
            return _configMultiplayerActive.Value;
        }
        void Awake()
        {
            logger = Logger;
            config = Config;
            try
            {
                harmony.PatchAll(assembly);
            } catch(Exception e)
            {
                logger.LogError(e.Message);
            }

        }
    }    
}