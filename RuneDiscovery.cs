using HarmonyLib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using UnityEngine;

namespace Runestones
{
    [HarmonyPatch(typeof(RuneStone), "Interact")]
    public static class RuneDiscovery
    {
        public static Regex runeTokenRegex = new Regex(@"^\$lore_(.+)_random(\d+)");
        public static void Postfix(RuneStone __instance, Humanoid character)
        {
            Debug.Log("Runestone interaction");
            Player player = character as Player;
            if (__instance == null || player == null || RuneDB.Instance.AllRunes == null)
                return;
            MethodInfo methodInfo = typeof(RuneStone).GetMethod("GetRandomText", BindingFlags.NonPublic | BindingFlags.Instance);
            RuneStone.RandomRuneText runeText = (RuneStone.RandomRuneText)methodInfo.Invoke(__instance, null);
            if (runeText != null && runeTokenRegex.IsMatch(runeText.m_text))
            {
                var matchInfo = runeTokenRegex.Match(runeText.m_text);
                runeText.m_label = $"Rune Lore: {char.ToUpper(matchInfo.Groups[1].Value[0])}{matchInfo.Groups[1].Value.Substring(1)} {Int32.Parse(matchInfo.Groups[2].Value)}";
                player.AddKnownText(runeText.m_label, runeText.m_text);
                typeof(Player).GetMethod("UpdateKnownRecipesList", BindingFlags.NonPublic | BindingFlags.Instance).Invoke(player, null);
            }
        }
    }

    [HarmonyPatch(typeof(Player), "HaveRequirements", new Type[] { typeof(Recipe), typeof(bool), typeof(int) })]
    public static class SecretRecipes
    {
        public static bool Prefix(HashSet<string> ___m_knownMaterial, Dictionary<string, string> ___m_knownTexts, ref bool __result, Recipe recipe, bool discover)
        {
            var itemName = recipe?.m_item?.GetComponent<ItemDrop>()?.m_itemData?.m_shared?.m_name;
            if (discover && itemName != null)
            {
                var rune = RuneDB.Instance.GetRune(itemName);
                if (rune != null)
                {
                    __result = ___m_knownTexts.Values.Contains(rune.DiscoveryToken);
                    var baseMaterials = RuneDB.Instance.RuneBases[(int)rune.Quality];

                    foreach(var materialName in baseMaterials.Keys)
                    {
                        var materialItem = ObjectDB.instance.GetItemPrefab(materialName).GetComponent<ItemDrop>();
                        __result = __result && ___m_knownMaterial.Contains(materialItem.m_itemData.m_shared.m_name);
                    }

                    return false;
                }
            }
            return true;
        }
    }
}
