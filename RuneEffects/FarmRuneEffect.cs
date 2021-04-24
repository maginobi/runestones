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
        public FarmRuneEffect()
        {
            _FlavorText = "Agriculture is the foundation of civilization";
            _EffectText = new List<string> { "+100% Crop growth speed", "5m radius" };
            _RelativeStats = new Dictionary<string, Func<string>> { { "Duration", () => $"{baseDuration * _Effectiveness :F0} sec" } };
        }
        public override void DoMagicAttack(Attack baseAttack)
        {
            var aoePrefab = GameObject.Instantiate(ZNetScene.instance.GetPrefab(aoeName));
            GameObject.Destroy(aoePrefab.GetComponent<Aoe>());
            
            var aoe = aoePrefab.AddComponent<FarmAoe>();
            aoe.m_ttl = baseDuration * _Effectiveness;
            var particles = aoePrefab.GetComponentInChildren<ParticleSystem>().main;
            particles.duration = 10/3f;
            particles.simulationSpeed = 1/3f;
            particles.loop = true;

            aoePrefab.GetComponent<ZNetView>().SetPersistent(true);

            var project = new MagicProjectile
            {
                m_spawnOnHit = aoePrefab,
                m_range = 10,
                m_launchAngle = 0,
                m_attackSpread = 10,
                m_hitType = Attack.HitPointType.Closest
            };
            var origin = baseAttack.GetAttackOrigin();
            Debug.Log($"origin direction: {origin.forward} attack direction: {baseAttack.GetCharacter().GetAimDir(origin.position)} better attack dir: {baseAttack.BetterAttackDir()}");
            project.Cast(origin, baseAttack.BetterAttackDir());
        }
        public class FarmAoe : PersistentAoe
        {
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
                        var newPlantTime = currentPlantTime.AddSeconds(-m_hitInterval);
                        plantView.GetZDO().Set("plantTime", newPlantTime.Ticks);
                    }
                }
            }
        }
    }
}
