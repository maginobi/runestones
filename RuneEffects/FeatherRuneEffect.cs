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
    class FeatherRuneEffect : RuneEffect
    {
        const string featherVfxName = "fx_raven_despawn";
        const float maxFallSpeed = -5;

        public void DoMagicAttack(Attack baseAttack)
        {
            baseAttack.GetCharacter().GetSEMan().AddStatusEffect("SE_Feather"); //Use code like this to add status effects to the player
        }

        public class SE_Feather : StatusEffect
        {
            public SE_Feather() : base()
            {
                name = "SE_Feather";
                m_name = "Feather Falling";
                m_tooltip = "Fall slowly and avoid fall damage";
                m_startMessage = "You feel light as a feather";
                m_time = 0;
                m_ttl = 30;
                m_icon = (from Sprite s in Resources.FindObjectsOfTypeAll<Sprite>() where s.name == "feather" select s).FirstOrDefault();

                m_startEffects = new EffectList();
                //This vfx is not getting applied properly
                var vfxPrefab = (from GameObject prefab in Resources.FindObjectsOfTypeAll<GameObject>() where prefab.name == featherVfxName select prefab).FirstOrDefault();
                m_startEffects.m_effectPrefabs = new EffectList.EffectData[] { new EffectList.EffectData { m_prefab = vfxPrefab, m_enabled = true, m_attach = true, m_scale = true } };
            }

            public override void Stop()
            {
                base.Stop();
                typeof(Character).GetField("m_maxAirAltitude", BindingFlags.NonPublic | BindingFlags.Instance).SetValue(m_character, m_character.transform.position.y);
            }
        }

        [HarmonyPatch(typeof(Character), "UpdateWalking")]
        public static class FallSpeedMod
        {
            public static void Postfix(Rigidbody ___m_body, SEMan ___m_seman)
            {
                if (___m_seman.HaveStatusEffect("SE_Feather") && ___m_body.velocity.y < maxFallSpeed)
                {
                    var vel = ___m_body.velocity;
                    vel.y = maxFallSpeed;
                    ___m_body.velocity = vel;
                }
            }
        }

        [HarmonyPatch(typeof(Character), "UpdateGroundContact")]
        public static class FallDamageMod
        {
            public static void Prefix(Character __instance, SEMan ___m_seman, ref float ___m_maxAirAltitude)
            {
                if (___m_seman.HaveStatusEffect("SE_Feather"))
                {
                    ___m_maxAirAltitude = __instance.transform.position.y;
                }
            }
        }
    }
}
