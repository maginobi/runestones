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
    public class SunderRuneEffect : RuneEffect
    {
        private const string vfxName = "vfx_blob_hit";
        public const float baseDuration = 30;
        public SunderRuneEffect()
        {
            _FlavorText = "No armor is without cracks";
            _EffectText = new List<string> { "Removes enemy resistances to physical damage", "1m radius" };
            _RelativeStats = new Dictionary<string, Func<string>> { { "Duration", () => $"{baseDuration * _Effectiveness :F1} sec" } };
        }
        public override void DoMagicAttack(Attack baseAttack)
        {
            var vfxPrefab = ZNetScene.instance.GetPrefab(vfxName);
            var gameObject = GameObject.Instantiate(vfxPrefab);
            var aoe = gameObject.AddComponent<SunderAoe>();

            var propertyInfo = typeof(Aoe).GetField("m_owner", BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.FlattenHierarchy);
            if (propertyInfo != null)
            {
                propertyInfo.SetValue(aoe, baseAttack.GetCharacter());
                Debug.Log($"Found field, new value: {propertyInfo.GetValue(aoe)}");
                Debug.Log($"Flags: {aoe.m_hitOwner}, {aoe.m_hitSame}, {aoe.m_hitFriendly}");
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


        public class SunderAoe : Aoe
        {
            public SunderAoe() : base()
            {
                m_useAttackSettings = false;
                m_dodgeable = true;
                m_blockable = false;
                m_statusEffect = "SE_Sunder";
                m_hitOwner = false;
                m_hitSame = false;
                m_hitFriendly = false;
                m_hitEnemy = true;
                m_skill = Skills.SkillType.None;
                m_hitInterval = -1;
                m_ttl = 1;
            }
        };

        public class SE_Sunder : SE_Stats
        {
            public SE_Sunder() : base()
            {
                name = "SE_Sunder";
                m_name = "Sunder";
                m_tooltip = "Weak to physical damage";
                m_startMessage = "Defenses reduced";
                m_time = 0;
                m_ttl = baseDuration;
                m_icon = (from Sprite s in Resources.FindObjectsOfTypeAll<Sprite>() where s.name == "CorpseRun" select s).FirstOrDefault();
                
                var vfxPrefab = (from GameObject prefab in Resources.FindObjectsOfTypeAll<GameObject>() where prefab.name == CurseRuneEffect.curseVfxName select prefab).FirstOrDefault();
                m_startEffects.m_effectPrefabs = new EffectList.EffectData[] { new EffectList.EffectData { m_prefab = vfxPrefab, m_enabled = true, m_attach = true, m_scale = true } };
            }
            public override void SetAttacker(Character attacker)
            {
                base.SetAttacker(attacker);
                float effectiveness = (1 + attacker.GetSkillFactor(MagicSkill.MagicSkillDef.m_skill));
                m_ttl = baseDuration * effectiveness;
            }

            public override void ModifyDamageMods(ref DamageModifiers modifiers)
            {
                if (modifiers.m_blunt != DamageModifier.Ignore && modifiers.m_blunt != DamageModifier.Immune && modifiers.m_blunt != DamageModifier.VeryWeak)
                    modifiers.m_blunt = DamageModifier.Weak;
                if (modifiers.m_slash != DamageModifier.Ignore && modifiers.m_slash != DamageModifier.Immune && modifiers.m_slash != DamageModifier.VeryWeak)
                    modifiers.m_slash = DamageModifier.Weak;
                if (modifiers.m_pierce != DamageModifier.Ignore && modifiers.m_pierce != DamageModifier.Immune && modifiers.m_pierce != DamageModifier.VeryWeak)
                    modifiers.m_pierce = DamageModifier.Weak;
            }
        }
    }
}
