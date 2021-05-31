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

        public void Load()
        {
            GameObject forcefieldPrefab = (from GameObject prefab in Resources.FindObjectsOfTypeAll<GameObject>() where prefab.name == "ForceField" select prefab).FirstOrDefault();
            if (forcefieldPrefab != null)
            {
                Debug.Log("ForceField loaded");
                forcefieldPrefab.AddComponent<ZNetView>();
                Prefabs.Add(forcefieldPrefab);
            }
            else
            {
                Debug.Log("Failed to find ForceField prefab");
            }
        }

        //[HarmonyPatch(typeof())]

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
