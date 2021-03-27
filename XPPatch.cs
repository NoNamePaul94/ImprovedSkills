using System;
using UnityEngine;
using HarmonyLib;

namespace ImprovedSkills
{
    public class XPPatch
    {
        [HarmonyPatch]
        static class ImprovedSkillsPatch
        {
            [HarmonyPrefix] 
            [HarmonyPatch(typeof(Skills), "RaiseSkill")]
            private static void multiply(ref Skills __instance, ref Skills.SkillType skillType, ref float factor)
            {
                if (ImprovedSkillsPlugin.IsEnabled())
                {
                    bool isServer = false;
                    foreach (ZNet.PlayerInfo pi in ZNet.instance.GetPlayerList())
                    {
                        isServer |= pi.m_host != "";
                    }
                
                    if (isServer && !ImprovedSkillsPlugin.IsMultiplayerActive()) return;
                    factor *= ImprovedSkillsPlugin.GetMultiplierForSkillBySkillType(skillType);
                }
            }
        }
    }
}