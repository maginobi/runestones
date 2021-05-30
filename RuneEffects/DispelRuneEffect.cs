using HarmonyLib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Runestones.RuneEffects
{
    public class DispelRuneEffect : RuneEffect
    {
        public const float baseDuration = 5;
        public const float baseProbability = 0.5f;
        public const string vfxName = "vfx_Potion_stamina_medium";
        public override CastingAnimations.CastSpeed speed { get => _Quality == RuneQuality.Common ? CastingAnimations.CastSpeed.Medium : CastingAnimations.CastSpeed.Instant; set => base.speed = value; }
        public DispelRuneEffect()
        {
            _FlavorText = "Counterspell";
            _EffectText = new List<string> { "Chance to remove each active status effect on you" };
            _QualityEffectText[RuneQuality.Ancient] = new List<string> { "Faster casting", "Briefly makes you very resistant to elemental damage" };
            _QualityEffectText[RuneQuality.Dark] = new List<string> { "Faster casting", "Removes status effects from targeted enemy instead", "100% dispel chance" };
            _RelativeStats = new Dictionary<string, Func<string>> { { "Dispel chance", () => $"{(_Quality == RuneQuality.Dark ? 1 : Math.Min(1, baseProbability * _Effectiveness)) :P0}" } };
            speed = CastingAnimations.CastSpeed.Medium;
        }
        public override void DoMagicAttack(Attack baseAttack)
        {
            SEMan seman = null;
            if (_Quality == RuneQuality.Dark)
            {
                Character hitCharacter = null;
                var project = new MagicProjectile
                {
                    m_actionOnHitCollider = collider =>
                    {
                        var destructible = collider.gameObject.GetComponent<IDestructible>();
                        if (destructible is Character character)
                            hitCharacter = character;
                    },
                    m_range = 10,
                    m_launchAngle = 0,
                    m_attackSpread = 6,
                    degInterval = 1
                };
                project.Cast(baseAttack.GetAttackOrigin(), baseAttack.BetterAttackDir());
                seman = hitCharacter.GetSEMan();
                GameObject.Instantiate(ZNetScene.instance.GetPrefab(vfxName), hitCharacter.GetCenterPoint(), Quaternion.identity, hitCharacter.gameObject.transform);
            }
            else
            {
                seman = baseAttack.GetCharacter().GetSEMan();
                GameObject.Instantiate(ZNetScene.instance.GetPrefab(vfxName), baseAttack.GetCharacter().GetCenterPoint(), Quaternion.identity, baseAttack.GetCharacter().gameObject.transform);
            }
            System.Random rand = new System.Random();
            var toRemove = new List<StatusEffect>();
            foreach(var effect in seman.GetStatusEffects())
            {
                if (_Quality == RuneQuality.Dark || rand.NextDouble() < baseProbability * _Effectiveness)
                    toRemove.Add(effect);
            }
            foreach(var effect in toRemove)
            {
                seman.RemoveStatusEffect(effect);
            }
            if (_Quality == RuneQuality.Ancient)
                seman.AddStatusEffect("SE_DispelResist");
        }

        public class SE_DispelResist : SE_Stats
        {
            public SE_DispelResist() : base()
            {
                name = "SE_DispelResist";
                m_name = "Dispel";
                m_tooltip = "Very resistant to elemental damage";
                m_time = 0;
                m_ttl = baseDuration;
                m_icon = (from Sprite s in Resources.FindObjectsOfTypeAll<Sprite>() where s.name == "ac_bkg_large" select s).FirstOrDefault();

                // Add resistances here
                m_mods = new List<HitData.DamageModPair>
                {
                    new HitData.DamageModPair {m_type = HitData.DamageType.Fire, m_modifier = HitData.DamageModifier.VeryResistant},
                    new HitData.DamageModPair {m_type = HitData.DamageType.Frost, m_modifier = HitData.DamageModifier.VeryResistant},
                    new HitData.DamageModPair {m_type = HitData.DamageType.Lightning, m_modifier = HitData.DamageModifier.VeryResistant},
                    new HitData.DamageModPair {m_type = HitData.DamageType.Poison, m_modifier = HitData.DamageModifier.VeryResistant},
                    new HitData.DamageModPair {m_type = HitData.DamageType.Spirit, m_modifier = HitData.DamageModifier.VeryResistant}
                };
            }
        }
    }
}
