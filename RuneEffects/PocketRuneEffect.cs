using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Runestones.RuneEffects
{
    class PocketRuneEffect : RuneEffect
    {

        public override void DoMagicAttack(Attack baseAttack)
        {
            var player = baseAttack.GetCharacter() as Player;
            if (player == Player.m_localPlayer)
                Debug.Log("Caster is local player");
            else
                Debug.Log("Caster not local player");
            MagicInventory.OpenMagicInventory(InventoryGui.instance);
        }

    }
}
