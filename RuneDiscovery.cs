using HarmonyLib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Runestones
{
    [HarmonyPatch(typeof(RuneStone), "Interact")]
    public static class RuneDiscovery
    {
        public static void Postfix(RuneStone __instance, Humanoid character)
        {
            Debug.Log("Runestone interaction");
            Player player = character as Player;
            if (__instance == null || player == null || RuneDB.Instance.AllRunes == null)
                return;
            MethodInfo methodInfo = typeof(RuneStone).GetMethod("GetRandomText", BindingFlags.NonPublic | BindingFlags.Instance);
            RuneStone.RandomRuneText runeText = (RuneStone.RandomRuneText)methodInfo.Invoke(__instance, null);
            if (runeText != null)
            {
                Debug.Log($"Rune Token: {runeText.m_text}");
                foreach (Rune r in RuneDB.Instance.AllRunes)
                {
                    if (r.Recipe != null && r.DiscoveryToken != null && r.DiscoveryToken == runeText.m_text)
                    {
                        player.AddKnownRecipe(r.Recipe);
                        Debug.Log($"Added known recipe for {r.GetToken()} on runestone interaction");
                    }
                }
            }
        }
    }

    [HarmonyPatch(typeof(Player), "HaveRequirements", new Type[] { typeof(Recipe), typeof(bool), typeof(int) })]
    public static class SecretRecipes
    {
        public static bool Prefix(ref bool __result, Recipe recipe, bool discover)
        {
            var itemName = recipe?.m_item?.GetComponent<ItemDrop>()?.m_itemData?.m_shared?.m_name;
            if (discover && itemName != null && RuneDB.Instance.GetRune(itemName) != null)
            {
                __result = false;
                return false;
            }
            return true;
        }
    }
}
