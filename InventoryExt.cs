using System.Collections.Generic;
using UnityEngine;

namespace Runestones
{
    public static class InventoryExt
    {
        public static void RemoveItems(this Inventory inventory, string itemName, int amount)
        {
            var foundIndices = new List<int>();
            for (int i = 0; i < inventory.GetAllItems().Count; i++)
            {
                if (inventory.GetItem(i).m_shared.m_name == itemName)
                    foundIndices.Add(i);
            }
            foreach (var index in foundIndices)
            {
                if (amount > 0)
                {
                    var stackSize = inventory.GetItem(index).m_stack;
                    if (amount - stackSize >= 0)
                    {
                        amount -= stackSize;
                        inventory.RemoveItem(index);
                    }
                    else
                    {
                        inventory.GetItem(index).m_stack -= amount;
                        amount = 0;
                    }
                }
                else
                {
                    return;
                }
            }
        }

        public static void AddItems(this Inventory inventory, string itemName, string prefabName, int amount)
        {
            while (amount > 0)
            {
                var tempObj = GameObject.Instantiate(ObjectDB.instance.GetItemPrefab(prefabName));
                var itemData = tempObj.GetComponent<ItemDrop>().m_itemData;
                if (amount - itemData.m_shared.m_maxStackSize > 0)
                {
                    itemData.m_stack = itemData.m_shared.m_maxStackSize;
                    inventory.AddItem(itemData);
                    amount -= itemData.m_shared.m_maxStackSize;
                }
                else
                {
                    itemData.m_stack = amount;
                    inventory.AddItem(itemData);
                    amount = 0;
                }
                ZNetScene.instance.Destroy(tempObj);
            }
        }
    }
}
