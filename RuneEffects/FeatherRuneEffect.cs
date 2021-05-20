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
        const float glideMaxFallSpeed = -2;
        const float glideMaxForwardSpeed = 20;
        const float glideAcceleration = 1;
        const float baseDuration = 30;
        const float flySpeedFactor = 0.2f;

        public FeatherRuneEffect()
        {
            _FlavorText = "I'm sure Hugin can lend you some";
            _EffectText = new List<string> { "Prevents fall damage", "Limits fall speed" };
            _QualityEffectText[RuneQuality.Ancient] = new List<string> { "Gliding", "+100% Duration" };
            _QualityEffectText[RuneQuality.Dark] = new List<string> { "True flight", "+200% Duration" };
            _RelativeStats = new Dictionary<string, Func<string>> { { "Duration", () => $"{baseDuration * _Effectiveness * (1 + (int)_Quality):F1} sec" } };
        }

        public override void DoMagicAttack(Attack baseAttack)
        {
            StatusEffect effect;
            string effectName;
            if (_Quality == RuneQuality.Dark)
                effectName = "SE_Flight";
            else
                effectName = "SE_Feather";
            effect = baseAttack.GetCharacter().GetSEMan().AddStatusEffect(effectName, true);
            if (effect == null)
                effect = baseAttack.GetCharacter().GetSEMan().GetStatusEffect(effectName);
            effect.m_ttl = baseDuration * _Effectiveness * (1 + (int)_Quality);
            if (_Quality == RuneQuality.Ancient)
                ((SE_Feather)effect).glide = true;
        }

        public class SE_Feather : StatusEffect
        {
            public bool glide = false;
            public SE_Feather() : base()
            {
                name = "SE_Feather";
                m_name = "Feather Falling";
                m_tooltip = "Fall slowly and avoid fall damage";
                m_startMessage = "You feel light as a feather";
                m_time = 0;
                m_ttl = baseDuration;
                m_icon = (from Sprite s in Resources.FindObjectsOfTypeAll<Sprite>() where s.name == "feather" select s).FirstOrDefault();

                m_startEffects = new EffectList();
                var vfxPrefab = (from GameObject prefab in Resources.FindObjectsOfTypeAll<GameObject>() where prefab.name == featherVfxName select prefab).FirstOrDefault();
                m_startEffects.m_effectPrefabs = new EffectList.EffectData[] { new EffectList.EffectData { m_prefab = vfxPrefab, m_enabled = true, m_attach = true, m_scale = true } };
            }

            public void ModifyFall(ref Vector3 velocity, float dt)
            {
                if (!glide && velocity.y < maxFallSpeed)
                {
                    velocity.y = maxFallSpeed;
                }
                else if (glide && velocity.y < glideMaxFallSpeed)
                {
                    velocity.y = glideMaxFallSpeed;
                    var moveDir = new Vector3(velocity.x, 0, velocity.z);
                    if (moveDir.magnitude < glideMaxForwardSpeed)
                        velocity = velocity + moveDir.normalized * glideAcceleration * dt;
                }
            }

            public override void Stop()
            {
                base.Stop();
                typeof(Character).GetField("m_maxAirAltitude", BindingFlags.NonPublic | BindingFlags.Instance).SetValue(m_character, m_character.transform.position.y);
            }
        }

        public class SE_Flight : StatusEffect
        {
            private bool origFlySetting = false;
            public SE_Flight() : base()
            {
                name = "SE_Flight";
                m_name = "Flight";
                m_tooltip = "Flight";
                m_startMessage = "You feel light as a feather";
                m_time = 0;
                m_ttl = baseDuration * 3;
                m_icon = (from Sprite s in Resources.FindObjectsOfTypeAll<Sprite>() where s.name == "feather" select s).FirstOrDefault();

                m_startEffects = new EffectList();
                var vfxPrefab = (from GameObject prefab in Resources.FindObjectsOfTypeAll<GameObject>() where prefab.name == featherVfxName select prefab).FirstOrDefault();
                m_startEffects.m_effectPrefabs = new EffectList.EffectData[] { new EffectList.EffectData { m_prefab = vfxPrefab, m_enabled = true, m_attach = true, m_scale = true } };
            }

            public override void Setup(Character character)
            {
                base.Setup(character);
                if(character is Player player)
                {
                    origFlySetting = player.IsDebugFlying();
                    typeof(Player).GetField("m_debugFly", BindingFlags.NonPublic | BindingFlags.Instance).SetValue(player, true);
                    ((ZNetView)typeof(Player).GetField("m_nview", BindingFlags.NonPublic | BindingFlags.Instance).GetValue(player)).GetZDO().Set("DebugFly", true);
                }
            }

            public override void Stop()
            {
                base.Stop();
                if (m_character is Player player)
                {
                    typeof(Player).GetField("m_debugFly", BindingFlags.NonPublic | BindingFlags.Instance).SetValue(player, origFlySetting);
                    ((ZNetView)typeof(Player).GetField("m_nview", BindingFlags.NonPublic | BindingFlags.Instance).GetValue(player)).GetZDO().Set("DebugFly", origFlySetting);
                    typeof(Character).GetField("m_maxAirAltitude", BindingFlags.NonPublic | BindingFlags.Instance).SetValue(m_character, m_character.transform.position.y);
                }
            }
        }

        [HarmonyPatch(typeof(Character), "UpdateWalking")]
        public static class FallSpeedMod
        {
            public static void Postfix(Rigidbody ___m_body, SEMan ___m_seman, float dt)
            {
                if (___m_seman.HaveStatusEffect("SE_Feather"))
                {
                    var vel = ___m_body.velocity;
                    ((SE_Feather)___m_seman.GetStatusEffect("SE_Feather")).ModifyFall(ref vel, dt);
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

        [HarmonyPatch(typeof(Character), "UpdateDebugFly")]
        public static class FlySpeedMod
        {
            public static void Prefix(ref Vector3 ___m_moveDir, SEMan ___m_seman)
            {
                if (___m_seman.HaveStatusEffect("SE_Flight"))
                    ___m_moveDir = ___m_moveDir * flySpeedFactor;
            }
        }
    }
}
