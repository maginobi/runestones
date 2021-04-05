using HarmonyLib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Runestones.RuneEffects
{
    public class ReviveRuneEffect : RuneEffect
    {
        static Dictionary<Skills.SkillType, float> localLastDeathSkills = new Dictionary<Skills.SkillType, float>();
        public void DoMagicAttack(Attack baseAttack)
        {
            var player = (Player)baseAttack.GetCharacter();
            if (player == Player.m_localPlayer)
                Debug.Log("Caster is local player");
            else
                Debug.Log("Caster not local player");
            Debug.Log("Revive rune triggered");
            Debug.Log($"HardDeath: {typeof(Player).GetMethod("HardDeath", BindingFlags.NonPublic | BindingFlags.Instance).Invoke(player, null)}");
            Debug.Log($"lastDeathDict: {localLastDeathSkills}");
            if (!(bool)typeof(Player).GetMethod("HardDeath", BindingFlags.NonPublic | BindingFlags.Instance).Invoke(player, null) && player == Player.m_localPlayer)
            {
                var playerSkills = (Dictionary<Skills.SkillType, Skills.Skill>)typeof(Skills).GetField("m_skillData", BindingFlags.NonPublic | BindingFlags.Instance).GetValue(player.GetSkills());
                foreach (var keyValue in localLastDeathSkills)
                {
                    Debug.Log($"difference between {keyValue.Value} and {playerSkills[keyValue.Key].m_level}");
                    float difference = keyValue.Value - playerSkills[keyValue.Key].m_level;
                    playerSkills[keyValue.Key].m_level += difference / 2;
                    if (((int)difference) % 2 != 0)
                        playerSkills[keyValue.Key].m_accumulator = (float)typeof(Skills).GetMethod("GetNextLevelRequirement", BindingFlags.NonPublic | BindingFlags.Instance).Invoke(player.GetSkills(), null) / 2;
                }
                typeof(Player).GetField("m_timeSinceDeath", BindingFlags.NonPublic | BindingFlags.Instance).SetValue(player, 999999f);
                Debug.Log($"time since death: {typeof(Player).GetField("m_timeSinceDeath", BindingFlags.NonPublic | BindingFlags.Instance).GetValue(player)}");
                player.GetSEMan().AddStatusEffect("SE_Revivify");
            }
            else
            {
                baseAttack.GetCharacter().Message(MessageHud.MessageType.Center, "Revive failed");
                player.PickupPrefab(RuneDB.Instance.GetRune("$ReviveRune").prefab);
            }
        }

        [HarmonyPatch(typeof(Skills), "OnDeath")]
        public static class SkillDeathMod
        {
            public static void Prefix(Player ___m_player, Dictionary<Skills.SkillType, Skills.Skill> ___m_skillData)
            {
                Debug.Log("Skill death mod triggered");
                if (___m_player == Player.m_localPlayer)
                {
                    Debug.Log("player is local");
                    localLastDeathSkills = ___m_skillData.ToDictionary<KeyValuePair<Skills.SkillType, Skills.Skill>, Skills.SkillType, float>(pair => pair.Key, pair => pair.Value.m_level);
                }
            }
        }

        public class SE_Revivify : SE_Stats
        {
            public SE_Revivify() : base()
            {
                name = "SE_Revivify";
                m_name = "Revivify";
                m_tooltip = "+75% Speed";
                m_startMessage = "Revivified";
                m_time = 0;
                m_ttl = 120;
                m_icon = (from Sprite s in Resources.FindObjectsOfTypeAll<Sprite>() where s.name == "CorpseRun" select s).FirstOrDefault();
            }

            override public void ModifySpeed(ref float speed)
            {
                speed *= 1.75f;
            }
        }
    }
}
