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
        private const string vfxName = "vfx_spawn";
        private const float baseSize = 5;
        private const float darkSize = 30;
        private const float baseDuration = 15;
        public WardRuneEffect()
        {
            _FlavorText = "With \u00C6gishjalmur, the staff of protection, no power will be a match for your own";
            _EffectText = new List<string> { "Creates a powerful ward", "Bars all monsters from entering", "2.5m radius" };
            _QualityEffectText[RuneQuality.Ancient] = new List<string> { "+300% Duration" };
            _QualityEffectText[RuneQuality.Dark] = new List<string> { "15m radius", "<i>Permanent</i>" };
            _RelativeStats = new Dictionary<string, Func<string>> { { "Duration", () => _Quality==RuneQuality.Dark ? "Permanent" : $"{baseDuration * _Effectiveness * (_Quality==RuneQuality.Ancient ? 4 : 1) :F1} sec" } };
            speed = CastingAnimations.CastSpeed.Medium;
        }

        public override void DoMagicAttack(Attack baseAttack)
        {
            var player = baseAttack.GetCharacter();
            GameObject forcefieldPrefab = ZNetScene.instance.GetPrefab(forcefieldName);
            var sphere = GameObject.Instantiate(forcefieldPrefab);
            var size = _Quality == RuneQuality.Dark ? darkSize : baseSize;
            sphere.transform.localScale = new Vector3(size, size, size);
            if (_Quality != RuneQuality.Dark)
            {
                var zView = sphere.GetComponent<ZNetView>();
                zView.m_persistent = false;
                var timeout = sphere.AddComponent<TimedDestruction>();
                timeout.m_timeout = baseDuration * _Effectiveness * (_Quality == RuneQuality.Ancient ? 4 : 1);
                timeout.m_triggerOnAwake = true;
            }
            else
            {
                var zView = sphere.GetComponent<ZNetView>();
                zView.m_persistent = true;
                GameObject vfxPrefab = (from GameObject prefab in Resources.FindObjectsOfTypeAll<GameObject>() where prefab.name == vfxName select prefab).FirstOrDefault();
                GameObject.Instantiate(vfxPrefab, player.GetCenterPoint(), Quaternion.identity);
            }
            GameObject.Instantiate(sphere, player.GetCenterPoint(), Quaternion.identity);
        }

    }
}
