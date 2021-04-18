using HarmonyLib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using static HitData;

namespace Runestones.RuneEffects
{
    public class FearRuneEffect : RuneEffect
    {
        private const string vfxName = "vfx_blob_hit";
        public override void DoMagicAttack(Attack baseAttack)
        {
            var vfxPrefab = ZNetScene.instance.GetPrefab(vfxName);
            var gameObject = GameObject.Instantiate(vfxPrefab);
            gameObject.AddComponent<FearAoe>();

            var propertyInfo = typeof(Aoe).GetField("m_owner", BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.FlattenHierarchy);
            if (propertyInfo != null)
            {
                propertyInfo.SetValue(gameObject.GetComponent<FearAoe>(), baseAttack.GetCharacter());
                Debug.Log($"Found field, new value: {propertyInfo.GetValue(gameObject.GetComponent<FearAoe>())}");
                Debug.Log($"Flags: {gameObject.GetComponent<FearAoe>().m_hitOwner}, {gameObject.GetComponent<FearAoe>().m_hitSame}, {gameObject.GetComponent<FearAoe>().m_hitFriendly}");
            }
            else
                Debug.Log("did not find owner property");

            var project = new MagicProjectile
            {
                m_spawnOnHit = gameObject,
                m_range = 10,
                m_launchAngle = 0,
                m_attackSpread = 10,
                m_hitType = Attack.HitPointType.Average
            };
            project.Cast(baseAttack.GetAttackOrigin(), baseAttack.BetterAttackDir());
        }


        public class FearAoe : Aoe
        {
            public FearAoe() : base()
            {
                m_useAttackSettings = false;
                m_dodgeable = true;
                m_blockable = false;
                m_statusEffect = "SE_Fear";
                m_hitOwner = false;
                m_hitSame = false;
                m_hitFriendly = false;
                m_hitEnemy = true;
                m_skill = Skills.SkillType.None;
                m_hitInterval = -1;
                m_ttl = 1;
                m_radius = 1;
            }
        };

        public class SE_Fear : StatusEffect
        {
            public SE_Fear() : base()
            {
                name = "SE_Fear";
                m_name = "Fear";
                m_tooltip = "Fleeing at +30% move speed";
                m_startMessage = "Fear overpowers you";
                m_time = 0;
                m_ttl = 30;
                m_icon = (from Sprite s in Resources.FindObjectsOfTypeAll<Sprite>() where s.name == "CorpseRun" select s).FirstOrDefault();
                
                var vfxPrefab = (from GameObject prefab in Resources.FindObjectsOfTypeAll<GameObject>() where prefab.name == CurseRuneEffect.curseVfxName select prefab).FirstOrDefault();
                m_startEffects.m_effectPrefabs = new EffectList.EffectData[] { new EffectList.EffectData { m_prefab = vfxPrefab, m_enabled = true, m_attach = true, m_scale = true } };
            }

            override public void ModifySpeed(ref float speed)
            {
                speed *= 1.3f;
            }
        }

        [HarmonyPatch(typeof(MonsterAI), "UpdateAI")]
        public static class MonsterFearMod
        {
            public static bool failed = false;
            public static bool Prefix(MonsterAI __instance, Character ___m_character, Character ___m_targetCreature, float dt)
            {
                if (___m_character.GetSEMan().HaveStatusEffect("SE_Fear"))
                {
                    __instance.UpdateTakeoffLanding(dt);
                    typeof(BaseAI).GetMethod("UpdateRegeneration", BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.FlattenHierarchy).Invoke(__instance, new object[] { dt });
                    Vector3 fleeFrom = ___m_targetCreature?.transform?.position ?? ___m_character.transform.position;
                    var methodinfo = typeof(BaseAI).GetMethod("Flee", BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.FlattenHierarchy);
                    methodinfo.Invoke(__instance, new object[] { dt, fleeFrom });
                    return false;
                }
                return true;
            }
        }

        [HarmonyPatch(typeof(AnimalAI), "UpdateAI")]
        public static class AnimalFearMod
        {
            public static bool Prefix(AnimalAI __instance, Character ___m_character, Character ___m_target, float dt)
            {
                if (___m_character.GetSEMan().HaveStatusEffect("SE_Fear"))
                {
                    __instance.UpdateTakeoffLanding(dt);
                    typeof(BaseAI).GetMethod("UpdateRegeneration", BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.FlattenHierarchy).Invoke(__instance, new object[] { dt });
                    Vector3 fleeFrom = ___m_target?.transform?.position ?? ___m_character.transform.position;
                    var methodinfo = typeof(BaseAI).GetMethod("Flee", BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.FlattenHierarchy);
                    methodinfo.Invoke(__instance, new object[] { dt, fleeFrom });
                    return false;
                }
                return true;
            }
        }
    }
}
