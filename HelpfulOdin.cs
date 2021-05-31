using HarmonyLib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Runestones
{
    [HarmonyPatch(typeof(Odin), "Awake")]
    public static class HelpfulOdinPatch
    {
        public static void Postfix(Odin __instance)
        {
            __instance.gameObject.AddComponent<HelpfulOdin>();
        }
    }

    public class HelpfulOdin : MonoBehaviour
    {
        public void OnDestroy()
        {
            var loc = gameObject.transform.position + Vector3.up;
            var rand = new System.Random();
            for (int i = 0; i < 3; i++)
            {
                var allRunes = RuneDB.Instance.AllRunes;
                var rune = allRunes[rand.Next(allRunes.Count)];
                GameObject.Instantiate(rune.prefab, loc, Quaternion.identity);
            }
        }
    }
}
