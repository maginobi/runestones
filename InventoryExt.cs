using System.Collections.Generic;
using System.Reflection;
using UnityEngine;

namespace Runestones
{
    public static class InventoryExt
    {
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
