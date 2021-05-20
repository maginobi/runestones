using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UnityEngine;

namespace Runestones.RuneEffects
{
    class LightRuneEffect : RuneEffect
    {
        const float baseDuration = 60;
        public LightRuneEffect()
        {
            _FlavorText = "Let there be light";
            _EffectText = new List<string> { "Conjures orbs of light", "Doubles stealth difficulty" };
            _QualityEffectText[RuneQuality.Ancient] = new List<string> { "Brighter", "+100% radius" };
            _QualityEffectText[RuneQuality.Dark] = new List<string> { "5 Fire damage per second to nearby enemies" };
            _RelativeStats = new Dictionary<string, Func<string>> { { "Duration", () => $"{baseDuration * _Effectiveness :F1} sec" } };
        }
        
        public override void DoMagicAttack(Attack baseAttack)
        {
            var effect = baseAttack.GetCharacter().GetSEMan().AddStatusEffect("SE_Runestones_Light", true);
            if (effect == null)
                effect = baseAttack.GetCharacter().GetSEMan().GetStatusEffect("SE_Runestones_Light");
            effect.m_ttl = baseDuration * _Effectiveness;
            effect.SetAttacker(baseAttack.GetCharacter());
            ((SE_Light)effect).SetQuality(_Quality);
        }

        public static GameObject ConstructGameObject(RuneQuality quality = RuneQuality.Common)
        {
            GameObject torchPrefab = ZNetScene.instance.GetPrefab("Torch");
            GameObject effectPrefab = torchPrefab.transform.Find("attach/equiped")?.gameObject;
            GameObject modifiedEffectPrefab = GameObject.Instantiate(effectPrefab);
            GameObject result = new GameObject();

            result.AddComponent<ZNetView>();
            LightHover hoverEffect = result.AddComponent<LightHover>();
            if (quality == RuneQuality.Dark)
            {
                hoverEffect.loopTime = 5f;
                hoverEffect.heightLoopTimeRatio = 0.9f;
            }

            modifiedEffectPrefab.SetActive(true);
            var light = modifiedEffectPrefab.GetComponentInChildren<Light>();
            if (light != null)
            {
                Debug.Log("Found light component");
                light.range = (quality == RuneQuality.Ancient ? 20 : 10);
                light.intensity = (quality == RuneQuality.Common ? 1.5f : 2);
            }
            else
            {
                Debug.Log("Unable to find light component");
            }

            var numLights = 3f;
            var radius = (quality == RuneQuality.Ancient ? 3f : 1.5f);
            for (int i = 0; i < numLights; i++)
            {
                var angle = (i / numLights) * 2*Math.PI;
                var go = GameObject.Instantiate(modifiedEffectPrefab);
                // Must destroy smoke here; doesn't successfully destroy if destroyed on modifiedEffectPrefab
                GameObject.Destroy(go.transform.Find("flame/smoke (1)").gameObject);
                go.transform.SetParent(result.transform);
                go.transform.localPosition = new Vector3((float)(radius * Math.Cos(angle)), 0, (float)(radius * Math.Sin(angle)));
            }
            GameObject.Destroy(modifiedEffectPrefab);


            if (quality == RuneQuality.Dark)
            {
                result.AddComponent<FireAura>();
                /*
                fireAura.Setup(baseAttack.GetCharacter(), Vector3.zero, 0, null, null);
                fireAura.m_ttl = effect.m_ttl;
                ((SE_Light)effect).fireAura = fireAura;
                */
            }

            return result;
        }

        public class SE_Light : SE_Stats
        {
            public SE_Light() : base()
            {
                name = "SE_Runestones_Light";
                m_name = "Light";
                m_tooltip = "Stealth difficulty increased 100%";
                m_startMessage = "Let there be light";
                m_time = 0;
                m_ttl = baseDuration;
                m_icon = Sprite.Create((from Texture2D s in Resources.FindObjectsOfTypeAll<Texture2D>() where s.name == "bam2" select s).FirstOrDefault(), new Rect(0,0,256,256), new Vector2());
                Debug.Log($"light status effect icon: {m_icon}");
                m_startEffects = new EffectList();

                m_stealthModifier = 1f;
            }

            public void SetQuality(RuneQuality quality)
            {
                var lightEffect = ConstructGameObject(quality);
                if(quality == RuneQuality.Dark)
                {
                    lightEffect.GetComponent<FireAura>().m_ttl = m_ttl - m_time;
                    typeof(Aoe).GetField("m_owner", BindingFlags.NonPublic | BindingFlags.Instance).SetValue(lightEffect.GetComponent<FireAura>(), m_character);
                    Debug.Log(typeof(Aoe).GetField("m_owner", BindingFlags.NonPublic | BindingFlags.Instance).GetValue(lightEffect.GetComponent<FireAura>()));
                }
                m_startEffects.m_effectPrefabs = new EffectList.EffectData[] { new EffectList.EffectData { m_prefab = lightEffect, m_enabled = true, m_attach = true } };
                TriggerStartEffects();
            }
        }

        public class LightHover : MonoBehaviour
        {
            public float loopTime = 15;
            private const float heightRange = 0.2f;
            public float heightLoopTimeRatio = 0.3f;
            private float elapsed = 0;
            public void Update()
            {
                elapsed = (elapsed + Time.deltaTime) % loopTime;
                var heightTime = ((elapsed / heightLoopTimeRatio) % loopTime) / loopTime * 2 * Math.PI;
                var height = 1 + heightRange * (1 + Math.Sin(heightTime));
                gameObject.transform.localPosition = new Vector3(0, (float)height, 0);
                var angle = (elapsed / loopTime) * 360;
                gameObject.transform.rotation = Quaternion.Euler(0, angle, 0);
            }
        }

        public class FireAura : Aoe
        {
            public FireAura() : base()
            {
                m_attachToCaster = true;
                m_hitOwner = false;
                m_hitSame = false;
                m_hitFriendly = false;
                m_hitProps = false;
                m_hitEnemy = false;
                m_hitCharacters = false;
                m_radius = 1.5f;
                m_damage.m_fire = 5f;
                m_ttl = baseDuration;
                m_hitInterval = 1f;
                m_useTriggers = false;
            }
        }

    }
}
