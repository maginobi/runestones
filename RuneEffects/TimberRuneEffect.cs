using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Runestones.RuneEffects
{
    class TimberRuneEffect : RuneEffect
    {
        const string projectileName = "gdking_root_projectile";
        public override void DoMagicAttack(Attack baseAttack)
        {
            baseAttack.m_attackType = Attack.AttackType.Projectile;
            baseAttack.m_attackProjectile = ZNetScene.instance.GetPrefab(projectileName);
            baseAttack.m_projectileAccuracy = 0;
            baseAttack.m_useCharacterFacing = true;
            baseAttack.m_useCharacterFacingYAim = true;
            baseAttack.m_attackHeight = 1.5f;
            baseAttack.m_attackRange = 1;
            baseAttack.GetWeapon().m_shared.m_damages.m_chop = 9999;
            baseAttack.DoProjectileAttack();
            baseAttack.GetWeapon().m_shared.m_damages.m_chop = 0;
        }
    }
}
