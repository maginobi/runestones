using System.Collections.Generic;
using System.Reflection;
using UnityEngine;

namespace Runestones
{
    public static class InventoryExt
    {
        public static void RemoveItems(this Inventory inventory, string itemName, int amount)
        {
            for (int i = 0; i < inventory.GetAllItems().Count; i++)
            {
                if (inventory.GetItem(i).m_shared.m_name == itemName && amount > 0)
                {
                    var stackSize = inventory.GetItem(i).m_stack;
                    if (amount - stackSize >= 0)
                    {
                        amount -= stackSize;
                        inventory.RemoveItem(i);
                        i--;
                    }
                    else
                    {
                        inventory.GetItem(i).m_stack -= amount;
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

        public static Vector2i FindEmptySlot(this Inventory inventory, bool topFirst)
        {
            return (Vector2i)typeof(Inventory).GetMethod("FindEmptySlot", BindingFlags.NonPublic | BindingFlags.Instance).Invoke(inventory, new object[] { topFirst });
        }
    }
}
