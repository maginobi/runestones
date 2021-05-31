using HarmonyLib;
using System;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;

namespace Runestones
{
    class ItemHelper
    {
        public static ItemHelper instance;
        public static ItemHelper Instance
        {
            get
            {
                if (instance == null)
                    instance = new ItemHelper();

                return instance;
            }
        }

        public List<GameObject> ItemPrefabs = new List<GameObject>();
        public Dictionary<string, string> TokenMap = new Dictionary<string, string>();
        public List<Action> Triggers = new List<Action>();

        private bool ZNetSceneLoaded = false;

        private void AfterItemsLoaded()
        {
            if (ObjectDB.instance != null && ObjectDB.instance.m_items.Count > 0 && ZNetScene.instance != null)
            {
                foreach (Action trigger in Triggers)
                {
                    trigger.Invoke();
                }
            }
        }

        public void OnAllItemsLoaded(Action callback)
        {
            Triggers.Add(callback);
        }

        public void AddItem(GameObject item)
        {
            ItemPrefabs.Add(item);
        }

        private void AddAllItemsToObjectDB()
        {
            if (ObjectDB.instance == null || ObjectDB.instance.m_items.Count == 0)
                return;

            foreach (GameObject prefab in ItemPrefabs)
            {
                AddItemToObjectDB(prefab);
            }
        }

        public static void AddItemToObjectDB(GameObject prefab)
        {
            if (prefab == null)
                throw new NullReferenceException("prefab is null");
            if (ObjectDB.instance == null)
                throw new NullReferenceException("ObjectDB is null");
            var itemDrop = prefab.GetComponent<ItemDrop>();
            if (itemDrop != null && ObjectDB.instance.GetItemPrefab(prefab.name.GetStableHashCode()) == null)
            {
                ObjectDB.instance.m_items.Add(prefab);
            }
            ObjectDB.instance.GetType().GetMethod("UpdateItemHashes", BindingFlags.NonPublic | BindingFlags.Instance).Invoke(ObjectDB.instance, null);
        }

        public static void AddItemToZNetScene(GameObject prefab)
        {
            var itemDrop = prefab.GetComponent<ItemDrop>();
            if (itemDrop != null)
                AddPrefabToZNetScene(prefab);
        }

        public static void AddPrefabToZNetScene(GameObject prefab)
        {
            if (prefab == null)
                throw new NullReferenceException("prefab is null");
            if (ZNetScene.instance == null)
                throw new NullReferenceException("ZNetScene is null");
            var namedPrefabsInfo = ZNetScene.instance.GetType().GetField("m_namedPrefabs", BindingFlags.NonPublic | BindingFlags.Instance);
            Dictionary<int, GameObject> namedPrefabs = (Dictionary<int, GameObject>)namedPrefabsInfo.GetValue(ZNetScene.instance);
            if (namedPrefabs == null)
                throw new NullReferenceException("ZNetScene named prefab dictionary is null");
            if (!namedPrefabs.ContainsKey(prefab.name.GetStableHashCode()))
            {
                ZNetScene.instance.m_prefabs.Add(prefab);
                namedPrefabs.Add(prefab.name.GetStableHashCode(), prefab);
            }
        }

        [HarmonyPatch(typeof(ZNetScene), "Awake")]
        public static class ZNetScene_Awake_Patch
        {
            public static void Postfix(ZNetScene __instance)
            {
                if (__instance == null)
                    return;
                
                foreach (GameObject prefab in Instance.ItemPrefabs)
                {
                    AddItemToZNetScene(prefab);
                }

                Instance.ZNetSceneLoaded = true;
            }
        }

        [HarmonyPatch(typeof(ObjectDB), "Awake")]
        public static class ObjectDB_Awake_Patch
        {
            public static void Postfix()
            {
                Instance.AddAllItemsToObjectDB();
                if(Instance.ZNetSceneLoaded)
                    Instance.AfterItemsLoaded();
            }
        }

        [HarmonyPatch(typeof(ObjectDB), "CopyOtherDB")]
        public static class ObjectDB_CopyOtherDB_Patch
        {
            public static void Postfix()
            {
                Instance.AddAllItemsToObjectDB();
                if (Instance.ZNetSceneLoaded)
                    Instance.AfterItemsLoaded();
            }
        }

        public void AddToken(string key, string value)
        {
            if (key.StartsWith("$"))
                TokenMap.Add(key.Substring(1), value);
            else
                TokenMap.Add(key, value);
        }

        [HarmonyPatch(typeof(Localization), "SetupLanguage")]
        public static class Language_Patch
        {
            public static void Postfix(ref Dictionary<string, string> ___m_translations)
            {
                foreach (KeyValuePair<string, string> mapping in Instance.TokenMap)
                {
                    if (!___m_translations.ContainsKey(mapping.Key))
                       ___m_translations.Add(mapping.Key, mapping.Value);
                }
            }
        }

    }
}
