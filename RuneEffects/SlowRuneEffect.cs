using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Runestones.RuneEffects
{
    public class SlowRuneEffect : RuneEffect
    {
        private const string vfxName = "vfx_blob_hit";
        public override void DoMagicAttack(Attack baseAttack)
        {
            var vfxPrefab = ZNetScene.instance.GetPrefab(vfxName);
            Debug.Log($"fetched prefab {vfxPrefab.name}");
            var gameObject = GameObject.Instantiate(vfxPrefab);
            Debug.Log("vfx instantiated");
            gameObject.AddComponent<SlowAoe>();
            Debug.Log($"Added aoe component {gameObject.GetComponent<SlowAoe>()}");
            var propertyInfo = typeof(Aoe).GetField("m_owner", BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.FlattenHierarchy);
            if (propertyInfo != null)
            {
                propertyInfo.SetValue(gameObject.GetComponent<SlowAoe>(), baseAttack.GetCharacter());
                Debug.Log($"Found field, new value: {propertyInfo.GetValue(gameObject.GetComponent<SlowAoe>())}");
                Debug.Log($"Flags: {gameObject.GetComponent<SlowAoe>().m_hitOwner}, {gameObject.GetComponent<SlowAoe>().m_hitSame}, {gameObject.GetComponent<SlowAoe>().m_hitFriendly}");
            }
            else
                Debug.Log("did not find owner property");
            var project = new MagicProjectile
            {
                m_spawnOnHit = gameObject,
                m_range = 10,
                m_launchAngle = 0,
                m_attackSpread = 10,
                m_hitType = Attack.HitPointType.Average
            };

            project.Cast(baseAttack.GetAttackOrigin(), baseAttack.BetterAttackDir());
        }


        public class SlowAoe : Aoe
        {
            public SlowAoe() : base()
            {
                m_useAttackSettings = false;
                m_dodgeable = true;
                m_blockable = false;
                m_statusEffect = "SE_Slow";
                m_hitOwner = false;
                m_hitSame = false;
                m_hitFriendly = false;
                m_hitEnemy = true;
                m_skill = Skills.SkillType.None;
                m_hitInterval = -1;
                m_ttl = 1;
            }
        };

        public class SE_Slow : StatusEffect
        {
            public SE_Slow() : base()
            {
                name = "SE_Slow";
                m_name = "Slow";
                m_tooltip = "-90% Speed";
                m_startMessage = "Slowed";
                m_time = 0;
                m_ttl = 30;
                m_icon = (from Sprite s in Resources.FindObjectsOfTypeAll<Sprite>() where s.name == "CorpseRun" select s).FirstOrDefault();
            }

            override public void ModifySpeed(ref float speed)
            {
                speed *= 0.1f;
            }
        }
    }
}
