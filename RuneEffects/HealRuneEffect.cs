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
        public void DoMagicAttack(Attack baseAttack)
        {
            var project = new MagicProjectile
            {
                m_spawnOnHit = (from GameObject prefab in Resources.FindObjectsOfTypeAll<GameObject>() where prefab.name == projectileName select prefab).FirstOrDefault(),
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
