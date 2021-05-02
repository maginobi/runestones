using HarmonyLib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Runestones
{
    public abstract class PersistentAoe : Aoe
    {
        public abstract void OnHit(GameObject gameObject);
    }
    
    [HarmonyPatch(typeof(Aoe), "OnHit", new Type[] { typeof(Collider), typeof(Vector3) })]
    public static class AoeOnHitMod
    {
        public static void Postfix(Aoe __instance, Collider collider)
        {
            if (__instance is PersistentAoe aoe)
            {
                aoe.OnHit(collider.gameObject);
            }
        }
    }
}
