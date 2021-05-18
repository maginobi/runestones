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
        private const float baseSize = 5;
        private const float darkSize = 30;
        private const float baseDuration = 30;
        public WardRuneEffect()
        {
            _FlavorText = "With \u00C6gishjalmur, the staff of protection, no power will be a match for your own";
            _EffectText = new List<string> { "Creates a powerful ward", "Bars all monsters from entering", "2.5m radius" };
            _QualityEffectText[RuneQuality.Ancient] = new List<string> { "+300% Duration" };
            _QualityEffectText[RuneQuality.Dark] = new List<string> { "15m radius" };
            _RelativeStats = new Dictionary<string, Func<string>> { { "Duration", () => $"{baseDuration * _Effectiveness * (_Quality==RuneQuality.Ancient ? 4 : 1) :F1} sec" } };
        }

        public override void DoMagicAttack(Attack baseAttack)
        {
            var player = baseAttack.GetCharacter();
            GameObject forcefieldPrefab = (from GameObject prefab in Resources.FindObjectsOfTypeAll<GameObject>() where prefab.name == forcefieldName select prefab).FirstOrDefault();
            var sphere = GameObject.Instantiate(forcefieldPrefab, player.GetCenterPoint(), Quaternion.identity);
            var size = _Quality == RuneQuality.Dark ? darkSize : baseSize;
            sphere.transform.localScale = new Vector3(size, size, size);
            var timeout = sphere.AddComponent<TimedDestruction>();
            timeout.m_timeout = baseDuration * _Effectiveness * (_Quality == RuneQuality.Ancient ? 4 : 1);
            timeout.Trigger();
        }

    }
}
