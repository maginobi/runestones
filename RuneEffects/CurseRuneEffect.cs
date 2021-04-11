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
        const string hitVfxName = "vfx_lox_groundslam";
        public const string curseVfxName = "vfx_Wet";

        public void DoMagicAttack(Attack baseAttack)
        {
            baseAttack.m_attackType = Attack.AttackType.Area;
            baseAttack.m_attackRange = 7.5f;
            baseAttack.m_hitTerrain = true;
            baseAttack.m_attackRayWidth = 2.5f;

            //This vfx is not getting applied properly
            var vfxPrefab = (from GameObject prefab in Resources.FindObjectsOfTypeAll<GameObject>() where prefab.name == hitVfxName select prefab).FirstOrDefault();
            //vfxPrefab.transform.localScale = vfxPrefab.transform.localScale * 5;
            var vfx = new EffectList.EffectData { m_prefab = vfxPrefab, m_enabled = true };

            baseAttack.m_hitEffect.m_effectPrefabs  = new EffectList.EffectData[] { vfx };
            baseAttack.GetWeapon().m_shared.m_attackStatusEffect = ObjectDB.instance.GetStatusEffect("SE_Curse");
            baseAttack.DoAreaAttack();
            baseAttack.GetWeapon().m_shared.m_attackStatusEffect = null;

            //baseAttack.GetCharacter().GetSEMan().AddStatusEffect("SE_Curse"); //Use code like this to add status effects to the player
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
