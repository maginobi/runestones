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
        const float baseDuration = 300;
        const float baseRadius = 4.3f;
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
            var aoePrefab = GameObject.Instantiate(ZNetScene.instance.GetPrefab(aoeName));
            //vfx scaling not being applied properly
            if (_Quality == RuneQuality.Dark)
                aoePrefab.transform.localScale = 2 * aoePrefab.transform.localScale;
            GameObject.Destroy(aoePrefab.GetComponent<Aoe>());
            
            var aoe = aoePrefab.AddComponent<FarmAoe>();
            aoe.m_ttl = baseDuration * _Effectiveness * (_Quality == RuneQuality.Dark ? 2 : 1);
            aoe.m_accelerationFactor = _Quality == RuneQuality.Ancient ? 2 : 1;
            aoe.m_radius = baseRadius * (_Quality == RuneQuality.Dark ? 2 : 1);
            var particles = aoePrefab.GetComponentInChildren<ParticleSystem>().main;
            particles.duration = 10/3f;
            particles.simulationSpeed = 1/3f;
            particles.loop = true;

            aoePrefab.GetComponent<ZNetView>().SetPersistent(true);

            GameObject.Instantiate(aoePrefab, targetLocation, Quaternion.identity);
            
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
                m_skill = Skills.SkillType.None;
                m_hitInterval = 10f;
                m_ttl = baseDuration;
            }

            public override void OnHit(GameObject gameObject)
            {
                var plant = gameObject.GetComponent<Plant>();
                if (plant != null)
                {
                    ZNetView plantView = (ZNetView)typeof(Plant).GetField("m_nview", BindingFlags.NonPublic | BindingFlags.Instance).GetValue(plant);
                    if (plantView.GetZDO() != null && plantView.IsOwner())
                    {
                        var currentPlantTime = new DateTime(plantView.GetZDO().GetLong("plantTime", ZNet.instance.GetTime().Ticks));
                        var newPlantTime = currentPlantTime.AddSeconds(-m_hitInterval*m_accelerationFactor);
                        plantView.GetZDO().Set("plantTime", newPlantTime.Ticks);
                    }
                }
            }
        }
    }
}
