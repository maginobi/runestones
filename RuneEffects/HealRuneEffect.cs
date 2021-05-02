using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Runestones.RuneEffects
{
    class HealRuneEffect : RuneEffect
    {
        const string projectileName = "shaman_heal_aoe";
        const float baseHeal = 30;
        public HealRuneEffect()
        {
            _FlavorText = "True strength is helping others";
            _EffectText = new List<string> { "Heals others", "5m radius" };
            _QualityEffectText[RuneQuality.Ancient] = new List<string> { "+100% Heal" };
            _QualityEffectText[RuneQuality.Dark] = new List<string> { "+200% Heal" };
            _RelativeStats = new Dictionary<string, Func<string>> { { "Heal", () => $"{baseHeal * _Effectiveness * (int)_Quality :F1}" } };
        }
        public override void DoMagicAttack(Attack baseAttack)
        {
            var aoePrefab = GameObject.Instantiate((from GameObject prefab in Resources.FindObjectsOfTypeAll<GameObject>() where prefab.name == projectileName select prefab).FirstOrDefault());
            var aoe = aoePrefab.GetComponent<Aoe>();
            aoe.m_hitOwner = false;
            aoe.m_damage.m_damage = -baseHeal*_Effectiveness*(int)_Quality;
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
    }
}
