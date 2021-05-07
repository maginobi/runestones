using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Runestones.RuneEffects
{
    class IceRuneEffect : RuneEffect
    {
        const string projectileName = "hatchling_cold_projectile";
        const float baseAccuracy = 15;
        const float baseDamage = 30;
        public IceRuneEffect()
        {
            _FlavorText = "The breath of the dragon";
            _EffectText = new List<string> { "Launches a shard of ice" };
            _QualityEffectText[RuneQuality.Ancient] = new List<string> { "Apply up to 90% slow for up to 10 seconds, based on damage and enemy health" };
            _QualityEffectText[RuneQuality.Dark] = new List<string> { "Apply up to 90% slow for up to 10 seconds, based on damage and enemy health", "Launch 3 Projectiles", "10 degrees less accurate" };
            _RelativeStats = new Dictionary<string, Func<string>> { { "Ice Damage", () => $"{baseDamage * _Effectiveness:F1}" },
                                                                    { "Accuracy", () => $"{Math.Max(baseAccuracy * (2 - _Effectiveness) + (_Quality==RuneQuality.Dark ? 10 : 0), 0) :F0} degrees" } };
        }
        public override void DoMagicAttack(Attack baseAttack)
        {
            baseAttack.m_attackType = Attack.AttackType.Projectile;
            baseAttack.m_attackProjectile = GameObject.Instantiate((from GameObject prefab in Resources.FindObjectsOfTypeAll<GameObject>() where prefab.name == projectileName select prefab).FirstOrDefault());
            if (_Quality == RuneQuality.Ancient || _Quality == RuneQuality.Dark)
                baseAttack.m_attackProjectile.GetComponent<Projectile>().m_statusEffect = "Frost"; //"$se_frost_name";
            if (_Quality == RuneQuality.Dark)
                baseAttack.m_projectiles = 3;
            baseAttack.m_projectileAccuracy = Math.Max(baseAccuracy * (2 - _Effectiveness) + (_Quality == RuneQuality.Dark ? 10 : 0), 0);
            baseAttack.m_useCharacterFacing = true;
            baseAttack.m_useCharacterFacingYAim = true;
            baseAttack.m_launchAngle = -baseAttack.m_projectileAccuracy/2;
            baseAttack.m_attackHeight = 1.5f;
            baseAttack.m_attackRange = 1;
            baseAttack.GetWeapon().m_shared.m_damages.m_frost = baseDamage * _Effectiveness;
            baseAttack.DoProjectileAttack();
            baseAttack.GetWeapon().m_shared.m_damages.m_frost = 0;
        }
    }
}
