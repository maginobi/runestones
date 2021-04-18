using HarmonyLib;
using System.Reflection;
using UnityEngine;

namespace Runestones.RuneEffects
{
    public class ForceRuneEffect : RuneEffect
    {
        public override void DoMagicAttack(Attack baseAttack)
        {
            Debug.Log("Force rune attack begins");
            baseAttack.m_attackType = Attack.AttackType.Horizontal;
            baseAttack.m_forceMultiplier = 1;
            baseAttack.m_attackRange = 7.5f;
            //baseAttack.m_staggerMultiplier = 10; //use this to adjust amount of stagger
            baseAttack.GetWeapon().m_shared.m_attackForce = 1000;
            baseAttack.DoMeleeAttack();
            baseAttack.GetWeapon().m_shared.m_attackForce = 10;
            Debug.Log("Force rune attack complete");
        }
    }
}
