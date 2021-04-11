using HarmonyLib;
using System;
using System.Reflection;
using UnityEngine;

namespace Runestones
{
    public static class StaffAttack
    {
        public static ItemDrop.ItemData GetAmmoItem(this Attack baseAttack)
        {
            return (ItemDrop.ItemData)typeof(Attack).GetField("m_ammoItem", BindingFlags.NonPublic | BindingFlags.Instance).GetValue(baseAttack);
        }

        public static bool GetInAttack(this Attack baseAttack)
        {
            return (bool)typeof(Attack).GetField("m_wasInAttack", BindingFlags.NonPublic | BindingFlags.Instance).GetValue(baseAttack);
        }

        public static void SetInAttack(this Attack baseAttack, bool inAttack)
        {
            typeof(Attack).GetField("m_wasInAttack", BindingFlags.NonPublic | BindingFlags.Instance).SetValue(baseAttack, inAttack);
        }

        public static Humanoid GetCharacter(this Attack baseAttack)
        {
            return (Humanoid)typeof(Attack).GetField("m_character", BindingFlags.NonPublic | BindingFlags.Instance).GetValue(baseAttack);
        }

        public static bool UseAmmo(this Attack baseAttack)
        {
            return (bool)typeof(Attack).GetMethod("UseAmmo", BindingFlags.NonPublic | BindingFlags.Instance).Invoke(baseAttack, null);
        }

        public static void DoMeleeAttack(this Attack baseAttack)
        {
            typeof(Attack).GetMethod("DoMeleeAttack", BindingFlags.NonPublic | BindingFlags.Instance).Invoke(baseAttack, null);
        }

        public static void DoAreaAttack(this Attack baseAttack)
        {
            typeof(Attack).GetMethod("DoAreaAttack", BindingFlags.NonPublic | BindingFlags.Instance).Invoke(baseAttack, null);
        }

        public static void DoProjectileAttack(this Attack baseAttack)
        {
            typeof(Attack).GetMethod("ProjectileAttackTriggered", BindingFlags.NonPublic | BindingFlags.Instance).Invoke(baseAttack, null);
        }

        public static Vector3 BetterAttackDir(this Attack baseAttack)
        {
            Transform origin = baseAttack.GetAttackOrigin();
            Vector3 result = origin.forward;
            result.y = baseAttack.GetCharacter().GetAimDir(origin.position).y;
            return result.normalized;
        }

        public static Transform GetAttackOrigin(this Attack baseAttack)
        {
            return (Transform)typeof(Attack).GetMethod("GetAttackOrigin", BindingFlags.NonPublic | BindingFlags.Instance).Invoke(baseAttack, null);
        }

        public static void DoMagicAttack(Attack baseAttack)
        {
            Debug.Log("Staff attack triggered");
            if (baseAttack.UseAmmo())
            {
                Debug.Log("Reflected UseAmmo method returned true");
                var runeName = baseAttack.GetAmmoItem()?.m_shared?.m_name;
                if (runeName != null)
                {
                    RuneEffect runeAttack = RuneDB.Instance.GetRune(runeName)?.Effect;
                    if (runeAttack != null)
                    {
                        Debug.Log("Found rune match");
                        runeAttack.DoMagicAttack(baseAttack);
                    }
                }
            }
        }

        [HarmonyPatch(typeof(Attack), "Start")]
        public static class AttackOverride
        {
            public static void Postfix(Attack __instance)
            {
                if (__instance.GetWeapon().m_shared.m_ammoType == "rune")
                {
                    DoMagicAttack(__instance);
                }
            }
        }

        [HarmonyPatch(typeof(Attack), "OnAttackTrigger")]
        public static class AttackTriggerOverride
        {
            public static bool Prefix(Attack __instance)
            {
                if(__instance.GetWeapon().m_shared.m_ammoType == "rune")
                {
                    return false;
                }
                return true;
            }
        }
    }
}
