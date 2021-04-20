using HarmonyLib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Runestones.RuneEffects
{
    class WardRuneEffect : RuneEffect
    {
        private const string forcefieldName = "ForceField";
        private const float size = 5;
        private const float ttl = 30;

        public override void DoMagicAttack(Attack baseAttack)
        {
            var player = baseAttack.GetCharacter();
            GameObject forcefieldPrefab = (from GameObject prefab in Resources.FindObjectsOfTypeAll<GameObject>() where prefab.name == forcefieldName select prefab).FirstOrDefault();
            var sphere = GameObject.Instantiate(forcefieldPrefab, player.GetCenterPoint(), Quaternion.identity);
            sphere.transform.localScale = new Vector3(size, size, size);
            var timeout = sphere.AddComponent<TimedDestruction>();
            timeout.m_timeout = ttl;
            timeout.Trigger();
        }

    }
}
