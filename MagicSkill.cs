using HarmonyLib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Runestones
{
    [HarmonyPatch(typeof(Skills), "Awake")]
    public static class MagicSkill
    {
        public const string skillName = "Magic";
        public static Skills.SkillDef MagicSkillDef = new Skills.SkillDef
        {
            m_skill = (Skills.SkillType)Math.Abs(skillName.GetStableHashCode()),
            m_description = "Improves spell effects in a variety of ways by a percentage equal to skill level"
        };

        public static void Postfix(Skills __instance)
        {
            MagicSkillDef.m_icon = (from Sprite s in Resources.FindObjectsOfTypeAll<Sprite>() where s.name == "walknut_bw" select s).FirstOrDefault();
            __instance.m_skills.Add(MagicSkillDef);
            var translationField = typeof(Localization).GetField("m_translations", BindingFlags.NonPublic | BindingFlags.Instance);
            Dictionary<string, string> translations = (Dictionary<string, string>)translationField.GetValue(Localization.instance);
            var token = "skill_" + MagicSkillDef.m_skill.ToString();

            if (!translations.ContainsKey(token))
            {
                translations.Add(token, skillName);
            }
        }
    }

    [HarmonyPatch(typeof(Skills), "GetRandomSkillFactor")]
    public static class RandomSkillFactorPatch
    {
        public static void Postfix(Skills.SkillType skillType, ref float __result)
        {
            if (skillType == MagicSkill.MagicSkillDef.m_skill)
                __result = 1;
        }
    }

    [HarmonyPatch(typeof(Skills), "GetSkillFactor")]
    public static class NormalSkillFactorPatch
    {
        public static void Postfix(Skills.SkillType skillType, ref float __result, Player ___m_player)
        {
            if (skillType == MagicSkill.MagicSkillDef.m_skill && ___m_player != null)
            {
                __result += ___m_player.GetEquipmentMovementModifier();
            }
        }
    }

    [HarmonyPatch(typeof(Skills), "IsSkillValid")]
    public static class MagicSkillValidityPatch
    {
        public static void Postfix(Skills.SkillType type, ref bool __result)
        {
            if (type == MagicSkill.MagicSkillDef.m_skill)
                __result = true;
        }
    }

    [HarmonyPatch(typeof(Skills), "CheatRaiseSkill")]
    public static class MagicSkillCheatRaisePatch
    {
        public static void Postfix(Skills __instance, string name, float value)
        {
            if (name == MagicSkill.skillName)
            {
                Skills.Skill skill = (Skills.Skill)typeof(Skills).GetMethod("GetSkill", BindingFlags.Instance | BindingFlags.NonPublic).Invoke(__instance, new object[] { MagicSkill.MagicSkillDef.m_skill });
                skill.m_level += value;
            }
        }
    }

    [HarmonyPatch(typeof(Skills), "CheatResetSkill")]
    public static class MagicSkillCheatResetPatch
    {
        public static void Postfix(Skills __instance, string name)
        {
            if (name == MagicSkill.skillName)
            {
                __instance.ResetSkill(MagicSkill.MagicSkillDef.m_skill);
            }
        }
    }
}
