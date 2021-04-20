using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Runestones.RuneEffects
{
    public class CurseRuneEffect : RuneEffect
    {
        const string hitVfxName = "vfx_blob_hit";
        public const string curseVfxName = "vfx_Wet";

        public override void DoMagicAttack(Attack baseAttack)
        {
            var vfxPrefab = ZNetScene.instance.GetPrefab(hitVfxName);
            var gameObject = GameObject.Instantiate(vfxPrefab);
            var aoe = gameObject.AddComponent<Aoe>();

            aoe.m_statusEffect = "SE_Curse";
            aoe.m_ttl = 1;
            aoe.m_radius = 1;

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

        public class SE_Curse : SE_Stats
        {
            public SE_Curse() : base()
            {
                name = "SE_Curse";
                m_name = "Cursed";
                m_tooltip = "-90% Damage dealt";
                m_startMessage = "You have been cursed";
                m_time = 0;
                m_repeatInterval = 60;
                m_ttl = 120;
                m_icon = (from Sprite s in Resources.FindObjectsOfTypeAll<Sprite>() where s.name == "jackoturnip" select s).FirstOrDefault();
                m_modifyAttackSkill = Skills.SkillType.All;
                m_damageModifier = 0.1f;
                m_startEffects = new EffectList();

                var vfxPrefab = (from GameObject prefab in Resources.FindObjectsOfTypeAll<GameObject>() where prefab.name == curseVfxName select prefab).FirstOrDefault();
                m_startEffects.m_effectPrefabs = new EffectList.EffectData[] { new EffectList.EffectData { m_prefab = vfxPrefab, m_enabled = true, m_attach = true, m_scale = true } };
            }

            override public void ModifySpeed(ref float speed)
            {
                speed = 1f * speed;
            }
        }
    }
}
