using HarmonyLib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Rendering;

namespace Runestones.RuneEffects
{
    public class DarknessRuneEffect : RuneEffect
    {
        public const float radius = 7.5f;
        public const float baseDuration = 30;
        public const float baseStealth = -0.25f;
        private const string baseVfxPrefabName = "vfx_darkland_groundfog";
        private GameObject gameObject;
        public DarknessRuneEffect()
        {
            _FlavorText = "\u266AHello darkness my old friend\u266A";
            _EffectText = new List<string> { "Darkens an area, making stealth easier", $"{radius}m radius" };
            _QualityEffectText[RuneQuality.Ancient] = new List<string> { "+100% Radius", "+100% Duration" };
            _QualityEffectText[RuneQuality.Dark] = new List<string> { "Your footsteps now make the sound of silence" };
            _RelativeStats = new Dictionary<string, Func<string>> { { "Stealth Buff", () => $"+{-(baseStealth * _Effectiveness) :P1}"},
                                                                    { "Duration", () => $"{baseDuration * _Effectiveness * (_Quality==RuneQuality.Ancient ? 2 : 1) :F1} sec" } };
            speed = CastingAnimations.CastSpeed.Medium;
        }

        public override void Precast(Attack baseAttack)
        {
            base.Precast(baseAttack);
            var player = baseAttack.GetCharacter();
            var adjRad = radius * (_Quality == RuneQuality.Ancient ? 2 : 1);

            var preVfx = GameObject.Instantiate(ZNetScene.instance.GetPrefab(baseVfxPrefabName));
            var particles = preVfx.GetComponentInChildren<ParticleSystem>();
            var mainSettings = particles.main;
            mainSettings.maxParticles = 15;
            var shapeSettings = particles.shape;
            shapeSettings.shapeType = ParticleSystemShapeType.Circle;
            shapeSettings.scale = new Vector3(adjRad / 2, adjRad / 2, 0.5f);
            var velocitySettings = particles.velocityOverLifetime;
            velocitySettings.z = 0;
            velocitySettings.radial = 1;
            var material = preVfx.GetComponentInChildren<Renderer>().material;
            material.color = Color.black;
            gameObject = GameObject.Instantiate(preVfx, player.transform.position, Quaternion.identity);

            var collider = gameObject.AddComponent<SphereCollider>();
            collider.radius = adjRad;
            collider.isTrigger = true;
        }

        public override void DoMagicAttack(Attack baseAttack)
        {
            var ttl = baseDuration * _Effectiveness * (_Quality == RuneQuality.Ancient ? 2 : 1);

            var darkZone = gameObject.AddComponent<EnvZone>();
            darkZone.m_environment = "Darkness";

            var statusEffect = ScriptableObject.CreateInstance<SE_DarknessStealth>();
            statusEffect.m_stealthModifier = baseStealth * _Effectiveness;
            if (_Quality == RuneQuality.Dark)
                statusEffect.m_noiseModifier = -1;
            statusEffect.name = Guid.NewGuid().ToString();
            ObjectDB.instance.m_StatusEffects.Add(statusEffect);

            var darkAoe = gameObject.AddComponent<AoeDarknessStealth>();
            darkAoe.m_ttl = ttl;
            darkAoe.m_radius = radius * (_Quality == RuneQuality.Ancient ? 2 : 1);
            darkAoe.m_statusEffect = statusEffect.name;
            darkAoe.m_hitOwner = true;
            darkAoe.m_hitSame = true;
            darkAoe.Invoke("OnStop", ttl);
            
            var timeout = gameObject.AddComponent<TimedDestruction>();
            timeout.m_timeout = ttl;
            timeout.Trigger();
        }

        public static EnvSetup darkEnvSetup = new EnvSetup()
        {
            m_name = "Darkness",
            m_alwaysDark = true,
            m_ambColorDay = new Color(0.3f, 0.4f, 0.55f),
            m_ambColorNight = new Color(0.15f, 0.2f, 0.25f),
            m_fogColorDay = Color.black,
            m_fogColorNight = Color.black,
            m_fogColorEvening = Color.black,
            m_fogColorMorning = Color.black,
            m_fogColorSunDay = Color.black,
            m_fogColorSunNight = Color.black,
            m_fogColorSunEvening = Color.black,
            m_fogColorSunMorning = Color.black,
            m_lightIntensityDay = 1,
            m_lightIntensityNight = 0.5f,
            m_fogDensityDay = 0.1f,
            m_fogDensityNight = 0.1f,
            m_fogDensityEvening = 0.1f,
            m_fogDensityMorning = 0.1f,
            
            m_psystems = new GameObject[0]
        };

        public class AoeDarknessStealth : PersistentAoe
        {
            public AoeDarknessStealth()
            {
                m_statusEffect = "SE_DarknessStealth";
                m_radius = DarknessRuneEffect.radius;
                m_useTriggers = true;
            }

            public override void OnHit(GameObject gameObject)
            {
                var character = gameObject.GetComponent<Character>();
                if (character != null)
                {
                    character.GetSEMan().AddStatusEffect(m_statusEffect, true);
                }
            }

            public void OnStop()
            {
                EnvMan.instance.SetForceEnvironment("");
            }
        }

        public class SE_DarknessStealth : RuneStatusEffect
        {
            float m_hiddenTtl = 1;
            public SE_DarknessStealth()
            {
                name = "SE_DarknessStealth";
                m_name = "Darkness";
                //m_tooltip = "+25% Stealth";
                m_icon = Sprite.Create((from Texture2D s in Resources.FindObjectsOfTypeAll<Texture2D>() where s.name == "space" select s).FirstOrDefault(), new Rect(256, 256, 256, 256), new Vector2());
                m_stealthModifier = -0.25f;
            }

            public override bool IsDone()
            {
                return m_time > m_hiddenTtl;
            }

            public override void Stop()
            {
                base.Stop();
                EnvMan.instance.SetForceEnvironment("");
            }
        }

        public class SE_DarknessStealthQuiet : SE_DarknessStealth
        {
            public SE_DarknessStealthQuiet() : base()
            {
                name = "SE_DarknessStealthQuiet";
                //m_tooltip = "+25% Stealth; Footsteps silenced";
                m_noiseModifier = -1;
            }
        }
    }

    [HarmonyPatch(typeof(EnvMan), "Awake")]
    public static class EnvRegistrar
    {
        public static void Prefix(EnvMan __instance)
        {
            __instance.m_environments.Add(DarknessRuneEffect.darkEnvSetup);
        }
    }
}
