using HarmonyLib;
using System;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;

namespace Runestones.RuneEffects
{
    public class ForceRuneEffect : RuneEffect
    {
        public const float baseKnockback = 1000;
        public ForceRuneEffect()
        {
            _FlavorText = "May the force be with you";
            _EffectText = new List<string> { "Knocks back enemies", "8m range" };
            _RelativeStats = new Dictionary<string, Func<string>> { { "$item_knockback", () => $"{baseKnockback * _Effectiveness}" } };
        }
        public override void DoMagicAttack(Attack baseAttack)
        {
            Debug.Log("Force rune attack begins");
            baseAttack.m_attackType = Attack.AttackType.Horizontal;
            baseAttack.m_forceMultiplier = 1;
            baseAttack.m_attackRange = 8f;
            //baseAttack.m_staggerMultiplier = 10; //use this to adjust amount of stagger
            baseAttack.GetWeapon().m_shared.m_attackForce = baseKnockback * _Effectiveness;
            baseAttack.DoMeleeAttack();
            baseAttack.GetWeapon().m_shared.m_attackForce = 10;
            Debug.Log("Force rune attack complete");
        }
    }
}
