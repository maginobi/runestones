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
            MagicSkillDef.m_icon = (from Sprite s in Resources.FindObjectsOfTypeAll<Sprite>() where s.name == "jackoturnip" select s).FirstOrDefault();
            __instance.m_skills.Add(MagicSkillDef);
            var translationField = typeof(Localization).GetField("m_translations", BindingFlags.NonPublic | BindingFlags.Instance);
            Dictionary<string, string> translations = (Dictionary<string, string>)translationField.GetValue(Localization.instance);
            var token = "skill_" + MagicSkillDef.m_skill.ToString();
            Debug.Log($"token {token}");
            if (!translations.ContainsKey(token))
            {
                translations.Add(token, skillName);
                //translationField.SetValue(Localization.instance, translations);
                //Debug.Log("set translations success");
            }
            Debug.Log($"localized skill token: {Localization.instance.Localize("$" + token)}");
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
}
