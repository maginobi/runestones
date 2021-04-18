using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Runestones.RuneEffects
{
    class LightRuneEffect : RuneEffect
    {
        
        public override void DoMagicAttack(Attack baseAttack)
        {
            baseAttack.GetCharacter().GetSEMan().AddStatusEffect("SE_Runestones_Light");
        }

        public static GameObject ConstructGameObject()
        {
            GameObject torchPrefab = ZNetScene.instance.GetPrefab("Torch");
            GameObject effectPrefab = torchPrefab.transform.Find("attach/equiped")?.gameObject;
            GameObject modifiedEffectPrefab = GameObject.Instantiate(effectPrefab);
            GameObject result = new GameObject();

            result.AddComponent<ZNetView>();
            result.AddComponent<LightHover>();

            modifiedEffectPrefab.SetActive(true);
            var light = modifiedEffectPrefab.GetComponentInChildren<Light>();
            if (light != null)
            {
                Debug.Log("Found light component");
                light.range = 10;
                light.intensity = 2;
            }
            else
            {
                Debug.Log("Unable to find light component");
            }

            var numLights = 3f;
            for (int i = 0; i < numLights; i++)
            {
                var angle = (i / numLights) * 2*Math.PI;
                var go = GameObject.Instantiate(modifiedEffectPrefab);
                GameObject.Destroy(go.transform.Find("flame/smoke (1)").gameObject);
                go.transform.SetParent(result.transform);
                go.transform.localPosition = new Vector3((float)(1.5 * Math.Cos(angle)), 0, (float)(1.5 * Math.Sin(angle)));
            }
            GameObject.Destroy(modifiedEffectPrefab);
            
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
                m_ttl = 60;
                m_icon = Sprite.Create((from Texture2D s in Resources.FindObjectsOfTypeAll<Texture2D>() where s.name == "bam2" select s).FirstOrDefault(), new Rect(0,0,256,256), new Vector2());
                Debug.Log($"light status effect icon: {m_icon}");
                m_startEffects = new EffectList();

                m_startEffects.m_effectPrefabs = new EffectList.EffectData[] { new EffectList.EffectData { m_prefab = ConstructGameObject(), m_enabled = true, m_attach = true } };

                m_stealthModifier = 1f;
            }
        }

        public class LightHover : MonoBehaviour
        {
            private const float loopTime = 15;
            private const float heightRange = 0.2f;
            private const float heightLoopTimeRatio = 0.3f;
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

    }
}
