using HarmonyLib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Runestones
{
    public class CustomEffectPrefabs
    {
        private static CustomEffectPrefabs instance;
        public static CustomEffectPrefabs Instance
        {
            get
            {
                if (instance == null)
                    instance = new CustomEffectPrefabs();
                return instance;
            }
        }
        public List<GameObject> Prefabs = new List<GameObject>();
        private static GameObject PrefabContainer;

        private CustomEffectPrefabs()
        {
            PrefabContainer = new GameObject("PrefabContainer");
            PrefabContainer.SetActive(false);
            PrefabContainer.transform.SetParent(null);
            GameObject.DontDestroyOnLoad(PrefabContainer);
        }

        public void AddPrefab(GameObject prefab)
        {
            if (prefab != null)
            {
                var tempGameObject = GameObject.Instantiate(prefab);
                var zView = tempGameObject.AddComponent<ZNetView>();
                zView.m_persistent = true;
                var gameObject = GameObject.Instantiate(tempGameObject, PrefabContainer.transform);
                gameObject.name = prefab.name;
                Prefabs.Add(gameObject);
            }
            else
            {
                Debug.LogWarning("Attempted to register custom effect prefab that is null");
            }
        }

        public void Load()
        {
            GameObject forcefieldPrefab = (from GameObject prefab in Resources.FindObjectsOfTypeAll<GameObject>() where prefab.name == "ForceField" select prefab).FirstOrDefault();
            AddPrefab(forcefieldPrefab);
            GameObject cultivatePrefab = RuneEffects.FarmRuneEffect.ConstructGameObject();
            AddPrefab(cultivatePrefab);
        }

        [HarmonyPatch(typeof(ZNet), "LoadWorld")]
        public static class ZNet_LoadWorld_Patch
        {
            public static void Prefix()
            {
                Instance.Load();
                if (ZNetScene.instance == null)
                    return;
                foreach (GameObject prefab in Instance.Prefabs)
                {
                    ItemHelper.AddPrefabToZNetScene(prefab);
                }
            }
        }
    }
}
