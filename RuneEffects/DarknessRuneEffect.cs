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
        public const float size = 10;
        public const float ttl = 30;

        public override void DoMagicAttack(Attack baseAttack)
        {
            var player = baseAttack.GetCharacter();
            GameObject gameObject = GameObject.CreatePrimitive(PrimitiveType.Sphere);
            gameObject.transform.position = player.GetCenterPoint();
            gameObject.transform.localScale = new Vector3(size, size, size);
            Collider collider = gameObject.GetComponent<Collider>();
            collider.isTrigger = true;
            var material = gameObject.GetComponent<Renderer>().material;
            material.SetTransparent();
            material.SetColor("_Color", new Color(0, 0, 0, 0.7f));
            var darkZone = gameObject.AddComponent<EnvZone>();
            darkZone.m_environment = "Darkness";
            var darkAoe = gameObject.AddComponent<AoeDarknessStealth>();
            darkAoe.Invoke("OnStop", ttl);
            var timeout = gameObject.AddComponent<TimedDestruction>();
            timeout.m_timeout = ttl;
            timeout.Trigger();
        }

        public static EnvSetup darkEnvSetup = new EnvSetup()
        {
            m_name = "Darkness",
            m_alwaysDark = true,
            m_ambColorDay = Color.black,
            m_ambColorNight = Color.black,
            m_fogColorDay = Color.black,
            m_fogColorNight = Color.black,
            m_fogColorEvening = Color.black,
            m_fogColorMorning = Color.black,
            m_fogColorSunDay = Color.black,
            m_fogColorSunNight = Color.black,
            m_fogColorSunEvening = Color.black,
            m_fogColorSunMorning = Color.black,
            m_lightIntensityDay = 0,
            m_lightIntensityNight = 0,
            m_fogDensityDay = 0.25f,
            m_fogDensityNight = 0.25f,
            m_fogDensityEvening = 0.25f,
            m_fogDensityMorning = 0.25f,
            
            m_psystems = new GameObject[0]
        };

        public class AoeDarknessStealth : Aoe
        {
            public AoeDarknessStealth()
            {
                m_ttl = DarknessRuneEffect.ttl;
                m_statusEffect = "SE_DarknessStealth";
                m_radius = DarknessRuneEffect.size / 2;
                m_useTriggers = true;
            }

            public void OnStop()
            {
                EnvMan.instance.SetForceEnvironment("");
            }
        }

        public class SE_DarknessStealth : SE_Stats
        {
            float m_hiddenTtl = 1;
            public SE_DarknessStealth()
            {
                name = "SE_DarknessStealth";
                m_name = "Darkness";
                m_tooltip = "+25% Stealth";
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
    }

    [HarmonyPatch(typeof(EnvMan), "Awake")]
    public static class EnvRegistrar
    {
        public static void Prefix(EnvMan __instance)
        {
            __instance.m_environments.Add(DarknessRuneEffect.darkEnvSetup);
        }
    }

    public static class MatTransExt
    {
        public static void SetTransparent(this Material material)
        {
            material.SetFloat("_Mode", 3);
            material.SetOverrideTag("RenderType", "Transparent");
            material.SetInt("_SrcBlend", (int)BlendMode.One);
            material.SetInt("_DstBlend", (int)BlendMode.OneMinusSrcAlpha);
            material.SetInt("_ZWrite", 0);
            material.DisableKeyword("_ALPHATEST_ON");
            material.DisableKeyword("_ALPHABLEND_ON");
            material.EnableKeyword("_ALPHAPREMULTIPLY_ON");
            material.DisableKeyword("_SPECULARHIGHLIGHTS_OFF");
            material.DisableKeyword("_GLOSSYREFLECTIONS_OFF");
            material.SetFloat("_SpecularHighlights", 1f);
            material.renderQueue = (int)RenderQueue.Transparent;
        }
    }
}
