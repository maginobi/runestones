using BepInEx;
using HarmonyLib;
using System;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;

namespace Runestones
{
    class RuneLoader
    {
        private const string PrefabPath = "Assets/PrefabInstance/Rune.prefab";
        private static RuneLoader _instance;
        public static RuneLoader Instance
        {
            get
            {
                if (_instance == null)
                    _instance = new RuneLoader();
                return _instance;
            }
        }

        private static GameObject RuneContainer;

        public GameObject BaseRunePrefab;

        private RuneLoader()
        {
            BaseRunePrefab = RunestonesMod.GetLoadedAssets().LoadAsset<GameObject>(PrefabPath);
            RuneContainer = new GameObject("RuneContainer");
            GameObject.DontDestroyOnLoad(RuneContainer);
            RuneContainer.SetActive(false);
            foreach (Rune rune in RuneDB.Instance.AllRunes)
            {
                ItemHelper.Instance.AddToken(rune.GetToken(), rune.GetName());
                ItemHelper.Instance.AddToken(rune.GetToken()+"_Desc", rune.Desc);
            }
            ItemHelper.Instance.OnAllItemsLoaded(AfterLoad);
        }

        public static void AfterLoad()
        {
            CreateRunes();
            CreateRecipes(); //recipes must be added on the ObjectDB.Awake/Copy after the ZNetScene.Awake, or they will be deleted.
        }

        private static void CreateRunes()
        {
            if (Instance.BaseRunePrefab == null)
            {
                Debug.LogError("Rune prefab is null");
                return;
            }

            foreach (Rune rune in RuneDB.Instance.AllRunes)
            {
                try
                {
                    rune.prefab = UnityEngine.Object.Instantiate(Instance.BaseRunePrefab, RuneContainer.transform);
                    rune.prefab.name = rune.GetToken();
                    rune.prefab.GetComponent<ItemDrop>().m_itemData.m_shared.m_name = rune.GetToken();
                    rune.prefab.GetComponent<ItemDrop>().m_itemData.m_shared.m_description = rune.GetToken()+"_Desc"; //rune.Desc ?? "";
                    rune.prefab.GetComponent<ItemDrop>().m_itemData.m_variant = rune.AssetIndex;
                    if (ObjectDB.instance.GetItemPrefab(rune.prefab.name.GetStableHashCode()) == null)
                    {
                        ItemHelper.AddItemToObjectDB(rune.prefab);
                        ItemHelper.AddItemToZNetScene(rune.prefab);
                    }
                }
                catch (Exception e)
                {
                    Debug.LogError($"Failed to create rune prefab: {rune.GetName()}");
                    Debug.LogException(e);
                }
            }
        }

        private static void CreateRecipes()
        {
            var namedPrefabsInfo = ZNetScene.instance.GetType().GetField("m_namedPrefabs", BindingFlags.NonPublic | BindingFlags.Instance);
            Dictionary<int, GameObject> namedPrefabs = (Dictionary<int, GameObject>)namedPrefabsInfo.GetValue(ZNetScene.instance);

            foreach (Rune rune in RuneDB.Instance.AllRunes)
            {
                try
                {
                    rune.Recipe = ScriptableObject.CreateInstance<Recipe>();
                    rune.Recipe.name = rune.GetToken();
                    rune.Recipe.m_item = rune.prefab.GetComponent<ItemDrop>();
                    rune.Recipe.m_amount = 5;

                    var resources = new List<Piece.Requirement>();
                    foreach (KeyValuePair<string, int> requirement in rune.SimpleRecipe)
                    {
                        var resourceItemDrop = ObjectDB.instance.GetItemPrefab(requirement.Key).GetComponent<ItemDrop>();
                        resources.Add(
                            new Piece.Requirement()
                            {
                                m_resItem = resourceItemDrop,
                                m_amount = requirement.Value
                            }
                        );
                    }
                    rune.Recipe.m_resources = resources.ToArray();

                    rune.Recipe.m_minStationLevel = 1;
                    rune.Recipe.m_enabled = true;

                    rune.Recipe.m_craftingStation = namedPrefabs["piece_workbench".GetStableHashCode()].GetComponent<CraftingStation>();
                                        
                    ObjectDB.instance.m_recipes.Add(rune.Recipe);

                    Debug.Log($"Created Recipe for Rune {rune.GetToken()}");
                }
                catch (Exception e)
                {
                    Debug.LogError($"Failed to create recipe for rune: {rune.GetName()}");
                    Debug.LogException(e);
                }
            }
            Debug.Log($"Loaded recipes for {RuneDB.Instance.AllRunes.FindAll(r => r.Recipe != null && r.Recipe.m_item != null).Count} / {RuneDB.Instance.AllRunes.Count} runes");
        }

    }

}
