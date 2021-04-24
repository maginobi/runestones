using HarmonyLib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static ItemDrop;

namespace Runestones
{
    [HarmonyPatch(typeof(ItemData), "GetTooltip", new Type[] { typeof(ItemData), typeof(int), typeof(bool) })]
    public static class RuneDescriptions
    {
        // This patch enables dynamic descriptions for runes
        public static void Postfix(ItemData item, ref string __result)
        {
            if (item.m_shared.m_itemType == ItemData.ItemType.Ammo && item.m_shared.m_ammoType == "rune")
            {
                var runeEffect = RuneDB.Instance.GetRune(item.m_shared.m_name).Effect;
                runeEffect._Effectiveness = 1 + Player.m_localPlayer.GetSkillFactor(MagicSkill.MagicSkillDef.m_skill);
                StringBuilder builder = new StringBuilder(runeEffect.GetDescription());
                builder.Append("\n");
                if (item.m_crafterID != 0L)
                {
                    builder.AppendFormat("\n$item_crafter: <color=orange>{0}</color>", item.m_crafterName);
                }
                builder.AppendFormat("\n$item_weight: <color=orange>{0}</color>", item.GetWeight().ToString("0.0"));
                __result = builder.ToString();
            }
        }
    }
}
