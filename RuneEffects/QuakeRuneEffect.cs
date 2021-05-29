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
    public class QuakeRuneEffect : RuneEffect
    {
        private const string vfxName = "vfx_lox_groundslam";
        private const float baseStaggerDamage = 100;
        public QuakeRuneEffect()
        {
            _FlavorText = "The earth shook whenever the venom fell on Loki's face";
            _EffectText = new List<string> { "Stagger enemies at range", "10m range", "5m radius", "Duration: 10 sec" };
            _QualityEffectText[RuneQuality.Ancient] = new List<string> { "+300% Stagger Power" };
            _QualityEffectText[RuneQuality.Dark] = new List<string> { "5 Bludgeoning damage per second" };
            _RelativeStats = new Dictionary<string, Func<string>> { { "Stagger Power", () => $"{baseStaggerDamage * _Effectiveness * (_Quality==RuneQuality.Ancient ? 4 : 1) :F0}" } };
            targetLock = true;
            speed = CastingAnimations.CastSpeed.Medium;
        }
        public override void DoMagicAttack(Attack baseAttack)
        {
            var vfxPrefab = ZNetScene.instance.GetPrefab(vfxName);
            var gameObject = GameObject.Instantiate(vfxPrefab);
            var aoe = (QuakeAoe)gameObject.AddComponent<QuakeAoe>();
            aoe.staggerDamage = baseStaggerDamage * _Effectiveness * (_Quality == RuneQuality.Ancient ? 4 : 1);
            if (_Quality == RuneQuality.Dark)
                aoe.m_damage.m_blunt = 5 * aoe.m_hitInterval;
            gameObject.GetComponent<TimedDestruction>().m_timeout = 10;
            var particles = gameObject.GetComponent<ParticleSystem>().main;
            particles.loop = true;
            particles.duration = 1.5f;

            var propertyInfo = typeof(Aoe).GetField("m_owner", BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.FlattenHierarchy);
            if (propertyInfo != null)
            {
                propertyInfo.SetValue(gameObject.GetComponent<QuakeAoe>(), baseAttack.GetCharacter());
                Debug.Log($"Found field, new value: {propertyInfo.GetValue(gameObject.GetComponent<QuakeAoe>())}");
                Debug.Log($"Flags: {gameObject.GetComponent<QuakeAoe>().m_hitOwner}, {gameObject.GetComponent<QuakeAoe>().m_hitSame}, {gameObject.GetComponent<QuakeAoe>().m_hitFriendly}");
            }
            else
                Debug.Log("did not find owner property");

            GameObject.Instantiate(gameObject, targetLocation, Quaternion.identity);
        }

        public class QuakeAoe : PersistentAoe
        {
            public float staggerDamage = baseStaggerDamage;
            public QuakeAoe() : base()
            {
                m_useAttackSettings = false;
                m_dodgeable = true;
                m_blockable = false;
                m_hitOwner = true;
                m_hitSame = true;
                m_hitFriendly = true;
                m_hitEnemy = true;
                m_skill = Skills.SkillType.None;
                m_hitInterval = 1.5f;
                m_ttl = 10;
            }

            override public void OnHit(GameObject gameObject)
            {
                IDestructible component = gameObject.GetComponent<IDestructible>();
                if (component != null && component is Character character)
                {
                    var randAngle = UnityEngine.Random.Range(0, (float)(2 * Math.PI));
                    Vector3 randDir = new Vector3((float)Math.Cos(randAngle), 0, (float)Math.Sin(randAngle));
                    typeof(Character).GetMethod("AddStaggerDamage", BindingFlags.NonPublic | BindingFlags.Instance).Invoke(character, new object[] { staggerDamage, randDir });
                }
            }
        };

        //[HarmonyPatch(typeof(Aoe), "CheckHits")]
        //public static class QuakeOnHitMod
        //{
        //    public static void Postfix(Aoe __instance, List<GameObject> ___m_hitList)
        //    {
        //        if (__instance is QuakeAoe _)
        //        {
        //            Debug.Log("Checking quake hits");
        //            foreach (GameObject gameObject in ___m_hitList)
        //            {
        //                IDestructible component = gameObject.GetComponent<IDestructible>();
        //                Character character = component as Character;
        //                if (character != null)
        //                {
        //                    var randAngle = UnityEngine.Random.Range(0, (float)(2 * Math.PI));
        //                    Vector3 randDir = new Vector3((float)Math.Cos(randAngle), 0, (float)Math.Sin(randAngle));
        //                    typeof(Character).GetMethod("AddStaggerDamage", BindingFlags.NonPublic | BindingFlags.Instance).Invoke(character, new object[] { staggerDamage, randDir });
        //                }
        //            }
        //        }
        //    }
        //}
    }
}
