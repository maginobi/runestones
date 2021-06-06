using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Runestones.RuneEffects
{
    public class FarmRuneEffect : RuneEffect
    {
        const string aoeName = "shaman_heal_aoe";
        const string customName = "CultivateEffect";
        const float baseDuration = 300;
        const float baseRadius = 5;
        public FarmRuneEffect()
        {
            _FlavorText = "Agriculture is the foundation of civilization";
            _EffectText = new List<string> { "+100% Crop growth speed", "5m radius" };
            _QualityEffectText[RuneQuality.Ancient] = new List<string> { "+100% more growth speed (stacks additively)" };
            _QualityEffectText[RuneQuality.Dark] = new List<string> { "+100% radius", "+100% Duration" };
            _RelativeStats = new Dictionary<string, Func<string>> { { "Duration", () => $"{baseDuration * _Effectiveness * (_Quality == RuneQuality.Dark ? 2 : 1) :F0} sec" } };
            targetLock = true;
            speed = CastingAnimations.CastSpeed.Slow;
        }
        public override void DoMagicAttack(Attack baseAttack)
        {
            var preVfx = ZNetScene.instance.GetPrefab(customName);

            var aoePrefab = GameObject.Instantiate(preVfx);
            var zView = aoePrefab.AddComponent<ZNetView>();
            zView.m_persistent = true;
            var adjRad = baseRadius * (_Quality == RuneQuality.Dark ? 2 : 1);

            var particles = aoePrefab.GetComponent<ParticleSystem>();
            var emissionSettings = particles.emission;
            var shapeSettings = particles.shape;
            emissionSettings.rateOverTime = 5 * (_Quality == RuneQuality.Dark ? 4 : 1);
            shapeSettings.radius = adjRad;
            
            var aoe = aoePrefab.GetComponent<FarmAoe>();
            aoe.m_ttl = baseDuration * _Effectiveness * (_Quality == RuneQuality.Dark ? 2 : 1);
            aoe.m_accelerationFactor = _Quality == RuneQuality.Ancient ? 2 : 1;
            aoe.m_radius = adjRad;
            aoe.enabled = true;

            GameObject.Instantiate(aoePrefab, targetLocation, Quaternion.identity);
        }

        public static GameObject ConstructGameObject()
        {
            var aoePrefab = (from GameObject prefab in Resources.FindObjectsOfTypeAll<GameObject>() where prefab.name == aoeName select prefab).FirstOrDefault();
            var preVfx = GameObject.Instantiate(aoePrefab.transform.Find("flames_world").gameObject);
            var adjRad = baseRadius;
            
            var particles = preVfx.GetComponent<ParticleSystem>();
            var mainSettings = particles.main;
            var emissionSettings = particles.emission;
            var shapeSettings = particles.shape;
            mainSettings.loop = true;
            mainSettings.gravityModifier = -0.1f;
            mainSettings.startSpeed = 0;
            emissionSettings.SetBursts(new ParticleSystem.Burst[0]);
            emissionSettings.rateOverTime = 5;
            shapeSettings.shapeType = ParticleSystemShapeType.Circle;
            shapeSettings.radius = adjRad;
            
            var aoe = preVfx.AddComponent<FarmAoe>();
            aoe.m_ttl = baseDuration;
            aoe.m_accelerationFactor = 1;
            aoe.m_radius = adjRad;

            preVfx.name = customName;
            return preVfx;
        }

        public class FarmAoe : PersistentAoe
        {
            public float m_accelerationFactor = 1;
            public FarmAoe() : base()
            {
                m_useAttackSettings = false;
                m_dodgeable = false;
                m_blockable = false;
                m_hitOwner = false;
                m_hitSame = false;
                m_hitFriendly = false;
                m_hitEnemy = false;
                m_hitProps = true;
                m_skill = Skills.SkillType.None;
                m_hitInterval = 10f;
                m_ttl = baseDuration;
                m_useTriggers = true;
            }

            public override void OnHit(GameObject gameObject)
            {
                Debug.Log("Farm Rune OnHit");
                var plant = gameObject.GetComponent<Plant>();
                if (plant != null)
                {
                    ZNetView plantView = (ZNetView)typeof(Plant).GetField("m_nview", BindingFlags.NonPublic | BindingFlags.Instance).GetValue(plant);
                    if (plantView.GetZDO() != null && plantView.IsOwner())
                    {
                        var currentPlantTime = new DateTime(plantView.GetZDO().GetLong("plantTime", ZNet.instance.GetTime().Ticks));
                        var newPlantTime = currentPlantTime.AddSeconds(-m_hitInterval*m_accelerationFactor);
                        plantView.GetZDO().Set("plantTime", newPlantTime.Ticks);
                        Debug.Log($"Accelerated plant growth from {currentPlantTime} to {newPlantTime}");
                    }
                }
            }
        }
    }
}
