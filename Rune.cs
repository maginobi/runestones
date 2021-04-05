using HarmonyLib;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace Runestones
{
    class Rune
    {
        public string Name;
        public string Desc;
        public int AssetIndex;
        public RuneEffect Effect;
        public Dictionary<string, int> SimpleRecipe;
        public string Quality;
        public GameObject prefab;
        public Recipe Recipe;
        public string DiscoveryToken;

        public string GetName()
        {
            return $"{Quality ?? ""} \"{Name}\" Rune".Trim();
        }

        public string GetToken()
        {
            return $"${Quality ?? ""}{Name}Rune";
        }
    }
}
