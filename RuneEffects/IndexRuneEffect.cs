using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Runestones.RuneEffects
{
    class IndexRuneEffect : RuneEffect
    {

        public void DoMagicAttack(Attack baseAttack)
        {
            var inventory = baseAttack.GetCharacter().GetInventory();
            for (int i=0; i<Math.Min(inventory.GetWidth(), inventory.GetEmptySlots()); i++)
            {
                var currentItem = inventory.GetItemAt(i, 0);
                if (currentItem != null)
                {
                    var destination = inventory.FindEmptySlot(false);
                    currentItem.m_gridPos = destination;
                }
            }

            List<ItemDrop.ItemData> runePrefs = (from ItemDrop.ItemData item in inventory.GetAllItems()
                                                where item.m_shared.m_ammoType == "rune"
                                                select item).OrderByDescending(item => 10*item.m_quality + item.m_variant).ToList();

            for (int i=0; i<Math.Min(inventory.GetWidth(), inventory.GetEmptySlots()); i++)
            {
                if(inventory.GetItemAt(i, 0) == null)
                {
                    runePrefs[i].m_gridPos = new Vector2i(i, 0);
                }
            }

            baseAttack.GetCharacter().Message(MessageHud.MessageType.TopLeft, "Indexed");
        }

    }
}
