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

        MagicInventory() : base("Pocket", (from Sprite s in Resources.FindObjectsOfTypeAll<Sprite>() where s.name == "chest_bkg" select s).FirstOrDefault(), 3, 2) { }

        MagicInventory(string arg1, Sprite arg2, int arg3, int arg4) : base(arg1, arg2, arg3, arg4) { }

        public static MagicInventory Create(long playerID)
        {
            var newInv = new MagicInventory() { PlayerID = playerID };
            AllContainers[playerID] = newInv;
            return newInv;
        }

        public static void OpenMagicInventory(InventoryGui inventoryGui)
        {
            Player player = Player.m_localPlayer;
            if ((bool)player)
            {
                if (player.gameObject.GetComponent<MagicInventory.MagicContainer>() is MagicInventory.MagicContainer magicContainer)
                {
                    inventoryGui.Show(magicContainer);
                }
                else
                {
                    ZNetView nview = (ZNetView)typeof(Player).GetField("m_nview", BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.FlattenHierarchy).GetValue(player);
                    Debug.Log($"ZNetView: {nview}");
                    long playerID = nview.GetZDO().GetLong("playerID");
                    Debug.Log($"Player id: {playerID}");
                    MagicInventory.AllContainers.TryGetValue(playerID, out var magicInventory);
                    Debug.Log($"Container: {magicInventory}");
                    if (magicInventory == null)
                    {
                        magicInventory = MagicInventory.Create(playerID);
                        Debug.Log($"Container: {magicInventory}");
                    }
                    magicContainer = player.gameObject.AddComponent<MagicInventory.MagicContainer>();
                    typeof(Container).GetField("m_inventory", BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.FlattenHierarchy).SetValue(magicContainer, magicInventory);
                    Action onContainerChanged = () => typeof(Container).GetMethod("OnContainerChanged", BindingFlags.NonPublic | BindingFlags.Instance).Invoke(magicContainer, null);
                    magicInventory.m_onChanged = (Action)Delegate.Combine(magicInventory.m_onChanged, onContainerChanged);
                    inventoryGui.Show(magicContainer);
                }
            }
        }

        public void LoadOverride()
        {
            Debug.Log("Magic load triggered");
            ZPackage zPackage;
            using (StreamReader saveFile = new StreamReader(Path.Combine(PocketContainerPath, PlayerID.ToString() + "_pocket.txt")))
            {
                zPackage = new ZPackage(saveFile.ReadToEnd());
            }
            Load(zPackage);
        }

        public void SaveOverride(ZPackage zPackage)
        {
            Debug.Log("Magic save triggered");
            using (StreamWriter saveFile = new StreamWriter(Path.Combine(PocketContainerPath, PlayerID.ToString() + "_pocket.txt")))
            {
                saveFile.Write(zPackage.GetBase64());
            }
        }

        public class MagicContainer : Container { }
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
            Debug.Log(Player.m_localPlayer);
            if (Player.m_localPlayer == null)
                return;
            ZNetView nview = (ZNetView)typeof(Player).GetField("m_nview", BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.FlattenHierarchy).GetValue(Player.m_localPlayer);
            Debug.Log($"ZNetView: {nview}");
            long playerID = nview.GetZDO().GetLong("playerID");
            Debug.Log($"player id: {playerID}");
            if (File.Exists(Path.Combine(MagicInventory.PocketContainerPath, playerID.ToString() + "_pocket.txt")))
            {
                var inv = MagicInventory.Create(playerID);
                inv.LoadOverride();
            }
        }
    }
}
