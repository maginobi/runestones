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
        private const string vfxName = "vfx_blob_hit";
        private Player caster;
        public override void DoMagicAttack(Attack baseAttack)
        {
            var vfxPrefab = ZNetScene.instance.GetPrefab(vfxName);
            caster = baseAttack.GetCharacter() as Player;

            var project = new MagicProjectile
            {
                m_spawnOnHit = vfxPrefab,
                m_actionOnHit = this.TeleportTo,
                m_range = 10,
                m_launchAngle = 0,
                m_attackSpread = 10,
                m_hitType = Attack.HitPointType.Average
            };
            project.Cast(baseAttack.GetAttackOrigin(), baseAttack.BetterAttackDir());
        }

        public void TeleportTo(Vector3 dest)
        {
            caster.TeleportTo(dest, caster.transform.rotation, false);
            typeof(Player).GetMethod("UpdateTeleport", BindingFlags.NonPublic | BindingFlags.Instance).Invoke(caster, new object[] { 4f });
        }
    }
}
