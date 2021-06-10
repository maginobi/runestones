using HarmonyLib;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Runestones
{
    public class MagicInventory : Inventory
    {
        public static Dictionary<long, MagicInventory> AllContainers = new Dictionary<long, MagicInventory>();
        public long PlayerID;
        public const string PocketContainerPath = "";

        public int Width
        {
            get
            {
                return (int)typeof(Inventory).GetField("m_width", BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.FlattenHierarchy).GetValue(this);
            }
            set
            {
                typeof(Inventory).GetField("m_width", BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.FlattenHierarchy).SetValue(this, value);
            }
        }

        public int Height
        {
            get
            {
                return (int)typeof(Inventory).GetField("m_height", BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.FlattenHierarchy).GetValue(this);
            }
            set
            {
                typeof(Inventory).GetField("m_height", BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.FlattenHierarchy).SetValue(this, value);
            }
        }

        MagicInventory() : base("Pocket", (from Sprite s in Resources.FindObjectsOfTypeAll<Sprite>() where s.name == "chest_bkg" select s).FirstOrDefault(), 6, 3) { }

        MagicInventory(string arg1, Sprite arg2, int arg3, int arg4) : base(arg1, arg2, arg3, arg4) { }

        public static MagicInventory Create(long playerID)
        {
            var newInv = new MagicInventory() { PlayerID = playerID };
            AllContainers[playerID] = newInv;
            return newInv;
        }

        public static void OpenMagicInventory(InventoryGui inventoryGui, int width, int height)
        {
            Player player = Player.m_localPlayer;
            if ((bool)player)
            {
                ZNetView nview = (ZNetView)typeof(Player).GetField("m_nview", BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.FlattenHierarchy).GetValue(player);
                
                long playerID = nview.GetZDO().GetLong("playerID");

                MagicContainer magicContainer = player.gameObject.GetComponent<MagicInventory.MagicContainer>();
                bool newContainer = magicContainer == null;
                if (newContainer)
                {
                    magicContainer = player.gameObject.AddComponent<MagicInventory.MagicContainer>();
                }

                MagicInventory.AllContainers.TryGetValue(playerID, out var magicInventory);

                if (magicInventory == null)
                {
                    magicInventory = MagicInventory.Create(playerID);
                }
                magicInventory.Width = width;
                magicInventory.Height = height;
                typeof(Container).GetField("m_inventory", BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.FlattenHierarchy).SetValue(magicContainer, magicInventory);

                if (newContainer)
                {
                    Action onContainerChanged = () => typeof(Container).GetMethod("OnContainerChanged", BindingFlags.NonPublic | BindingFlags.Instance).Invoke(magicContainer, null);
                    magicInventory.m_onChanged = (Action)Delegate.Combine(magicInventory.m_onChanged, onContainerChanged);
                }

                inventoryGui.Show(magicContainer);
            }
        }

        public List<ItemDrop.ItemData> GetAllItemsOverride()
        {
            List<ItemDrop.ItemData> result = new List<ItemDrop.ItemData>();
            List<ItemDrop.ItemData> orig_inv = (List<ItemDrop.ItemData>)typeof(Inventory).GetField("m_inventory", BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.FlattenHierarchy).GetValue(this);
            foreach (ItemDrop.ItemData item in orig_inv)
            {
                if (item.m_gridPos.x < Width && item.m_gridPos.y < Height)
                    result.Add(item);
            }
            return result;
        }

        public void LoadOverride()
        {
            ZPackage zPackage;
            using (StreamReader saveFile = new StreamReader(Path.Combine(PocketContainerPath, PlayerID.ToString() + "_pocket.txt")))
            {
                zPackage = new ZPackage(saveFile.ReadToEnd());
            }
            Load(zPackage);
        }

        public void SaveOverride(ZPackage zPackage)
        {
            using (StreamWriter saveFile = new StreamWriter(Path.Combine(PocketContainerPath, PlayerID.ToString() + "_pocket.txt")))
            {
                saveFile.Write(zPackage.GetBase64());
            }
        }

        public class MagicContainer : Container { }
    }

    [HarmonyPatch(typeof(Inventory), "GetAllItems", new Type[] { })]
    public static class GetAllItemsOverride
    {
        // This patch allows item hiding without exceptions
        public static bool Prefix(Inventory __instance, ref List<ItemDrop.ItemData> __result)
        {
            if (__instance is MagicInventory magicInventory)
            {
                __result = magicInventory.GetAllItemsOverride();
                return false;
            }
            return true;
        }
    }

    [HarmonyPatch(typeof(Inventory), "Save")]
    public static class MagicInventorySaveOverride
    {
        public static void Postfix(Inventory __instance, ZPackage pkg)
        {
            if (__instance is MagicInventory magicInventory)
                magicInventory.SaveOverride(pkg);
        }
    }

    [HarmonyPatch(typeof(Player), "Load")]
    public static class MagicInventoryLoadOverride
    {
        public static void Postfix()
        {
            if (Player.m_localPlayer == null)
                return;
            ZNetView nview = (ZNetView)typeof(Player).GetField("m_nview", BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.FlattenHierarchy).GetValue(Player.m_localPlayer);
            
            long playerID = nview.GetZDO().GetLong("playerID");

            if (File.Exists(Path.Combine(MagicInventory.PocketContainerPath, playerID.ToString() + "_pocket.txt")))
            {
                var inv = MagicInventory.Create(playerID);
                inv.LoadOverride();
            }
        }
    }
}
