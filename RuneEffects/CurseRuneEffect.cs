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
        public const float baseDuration = 60;
        public const float baseDamageMod = 0.75f;

        public CurseRuneEffect()
        {
            _FlavorText = "Revenge is a dish best served with raspberries";
            _EffectText = new List<string> { "Reduces enemy damage dealt", "1m radius" };
            _RelativeStats = new Dictionary<string, Func<string>> { { "Damage", () => $"-{1-(baseDamageMod / _Effectiveness) :P1}"},
                                                                    { "Duration", () => $"{baseDuration * _Effectiveness :F1} sec" } };
        }

        public override void DoMagicAttack(Attack baseAttack)
        {
            var vfxPrefab = ZNetScene.instance.GetPrefab(hitVfxName);
            var gameObject = GameObject.Instantiate(vfxPrefab);
            var aoe = gameObject.AddComponent<Aoe>();

            aoe.m_statusEffect = "SE_Curse";
            aoe.m_ttl = 1;
            aoe.m_radius = 1;
            typeof(Aoe).GetField("m_owner", BindingFlags.NonPublic | BindingFlags.Instance).SetValue(aoe, baseAttack.GetCharacter());

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
                m_ttl = baseDuration;
                m_icon = (from Sprite s in Resources.FindObjectsOfTypeAll<Sprite>() where s.name == "jackoturnip" select s).FirstOrDefault();
                m_modifyAttackSkill = Skills.SkillType.All;
                m_damageModifier = 0.75f;
                m_startEffects = new EffectList();

                var vfxPrefab = (from GameObject prefab in Resources.FindObjectsOfTypeAll<GameObject>() where prefab.name == curseVfxName select prefab).FirstOrDefault();
                m_startEffects.m_effectPrefabs = new EffectList.EffectData[] { new EffectList.EffectData { m_prefab = vfxPrefab, m_enabled = true, m_attach = true, m_scale = true } };
            }
            public override void SetAttacker(Character attacker)
            {
                base.SetAttacker(attacker);
                float effectiveness = (1 + attacker.GetSkillFactor(MagicSkill.MagicSkillDef.m_skill));
                m_ttl = baseDuration * effectiveness;
                m_damageModifier = baseDamageMod / effectiveness;
            }
        }
    }
}
