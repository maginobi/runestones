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
        public Type EffectClass;
        public RuneEffect _FixedEffect = null;
        public RuneEffect Effect
        {
            get
            {
                if (_FixedEffect != null)
                    return _FixedEffect;
                else
                {
                    RuneEffect result = (RuneEffect)EffectClass.GetConstructor(Type.EmptyTypes).Invoke(Type.EmptyTypes);
                    result._Quality = Quality;
                    return result;
                }
            }
        }

        public Dictionary<string, int> SimpleRecipe;
        public RuneQuality Quality;
        public GameObject prefab;
        public Recipe Recipe;
        public string DiscoveryToken;

        public string GetName()
        {
            return $"{(Quality==RuneQuality.Common ? "" : Quality.ToString())} \"{Name}\" Rune".Trim();
        }

        public string GetToken()
        {
            return $"${(Quality == RuneQuality.Common ? "" : Quality.ToString())}{Name}Rune";
        }
    }
}
