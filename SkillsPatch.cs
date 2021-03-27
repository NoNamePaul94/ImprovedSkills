using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using HarmonyLib;
using BepInEx;
using BepInEx.Configuration;
using UnityEngine;

namespace ImprovedSkills
{
    [HarmonyPatch]
    public static class SkillsPatch
    {
        [HarmonyPostfix]
        [HarmonyPatch(typeof(Skills), "Load")]
        private static void Initialize()
        {
            if (ImprovedSkillsPlugin.initialized) return;
            
            ImprovedSkillsPlugin._configIsEnabled = ImprovedSkillsPlugin.config.Bind("Settings", "is_enabled", true, "If the plugin is active at all");
            ModSettingsUI.ModSettingsUI.AddInputField(ImprovedSkillsPlugin.pluginName, ModSettingsUI.OptionType.Checkbox, "Enabled", ImprovedSkillsPlugin._configIsEnabled.Value, delegate (object value)
            {
                ImprovedSkillsPlugin._configIsEnabled.Value = (bool)value;
                ImprovedSkillsPlugin.config.Save();
            });

            ImprovedSkillsPlugin._configMultiplayerActive = ImprovedSkillsPlugin.config.Bind("Settings", "multiplayer_active", false, "If the plugin is active in multiplayer");
            ModSettingsUI.ModSettingsUI.AddInputField(ImprovedSkillsPlugin.pluginName, ModSettingsUI.OptionType.Checkbox, "Active during multiplayer", ImprovedSkillsPlugin._configMultiplayerActive.Value, delegate (object value)
            {
                ImprovedSkillsPlugin._configMultiplayerActive.Value = (bool)value;
                ImprovedSkillsPlugin.config.Save();
            });

            ImprovedSkillsPlugin._skillConfigurations = new List<KeyValuePair<string, ConfigEntry<float>>>();

            Skills skills = GameObject.FindObjectOfType<Skills>();

            foreach (Skills.Skill skill in skills.GetSkillList())
            {
                string skillKey = skill.m_info.m_skill.ToString().ToLower();

                ConfigEntry<float> skillConfig = ImprovedSkillsPlugin.config.Bind("Skills", skillKey, 1.0f, $"Multiplier for Skill '{skill.m_info.m_skill}'");
                ImprovedSkillsPlugin._skillConfigurations.Add(new KeyValuePair<string, ConfigEntry<float>>(skillKey, skillConfig));
            }

            foreach (KeyValuePair<string, ConfigEntry<float>> keypair in ImprovedSkillsPlugin._skillConfigurations)
            {
                string optionTitle = Localization.instance.Localize($"$skill_{keypair.Key.ToLower()}");
                ModSettingsUI.ModSettingsUI.AddInputField(ImprovedSkillsPlugin.pluginName, ModSettingsUI.OptionType.DecimalInput, optionTitle, keypair.Value.Value, delegate (object value)
                {
                    keypair.Value.Value = (float)value;
                    ImprovedSkillsPlugin.config.Save();
                });
            }

            ImprovedSkillsPlugin.initialized = true;
        }
    }
}
