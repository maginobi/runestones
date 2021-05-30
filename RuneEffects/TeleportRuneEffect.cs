using HarmonyLib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using static HitData;

namespace Runestones.RuneEffects
{
    public class TeleportRuneEffect : RuneEffect
    {
        private const string vfxName = "vfx_odin_despawn";
        private Player caster;
        public const float baseRange = 15;
        public override CastingAnimations.CastSpeed speed { get {
                if (_Quality == RuneQuality.Dark)
                    return CastingAnimations.CastSpeed.Instant;
                else
                    return CastingAnimations.CastSpeed.Medium;
            } set => base.speed = value; }
        public TeleportRuneEffect()
        {
            _FlavorText = "Heimdallr guards the Bifrost, but you may pass";
            _EffectText = new List<string> { "Short-range teleport" };
            _QualityEffectText[RuneQuality.Ancient] = new List<string> { "+300% Range" };
            _QualityEffectText[RuneQuality.Dark] = new List<string> { "Instant cast speed" };
            _RelativeStats = new Dictionary<string, Func<string>> { { "Range", () => $"{baseRange * _Effectiveness * (_Quality==RuneQuality.Ancient ? 4 : 1):F1}m" } };
            targetLock = true;
        }

        public override void Precast(Attack baseAttack)
        {
            var project = new MagicProjectile
            {
                m_actionOnHit = hitLoc => targetLocation = hitLoc,
                m_range = baseRange * _Effectiveness * (_Quality == RuneQuality.Ancient ? 4 : 1),
                m_launchAngle = 0,
                m_attackSpread = 0
            };
            project.Cast(baseAttack.GetAttackOrigin(), baseAttack.BetterAttackDir());
        }

        public override void DoMagicAttack(Attack baseAttack)
        {
            var vfxPrefab = ZNetScene.instance.GetPrefab(vfxName);
            caster = baseAttack.GetCharacter() as Player;

            GameObject.Instantiate(vfxPrefab, caster.transform.position + Vector3.up, Quaternion.identity);
            GameObject.Instantiate(vfxPrefab, targetLocation + Vector3.up, Quaternion.identity);
            TeleportTo(targetLocation);
        }

        public void TeleportTo(Vector3 dest)
        {
            caster.TeleportTo(dest, caster.transform.rotation, false);
            typeof(Player).GetMethod("UpdateTeleport", BindingFlags.NonPublic | BindingFlags.Instance).Invoke(caster, new object[] { 4f });
        }
    }
}
