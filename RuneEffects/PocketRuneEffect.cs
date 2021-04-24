using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Runestones.RuneEffects
{
    class PocketRuneEffect : RuneEffect
    {
        const int baseWidth = 3;
        public PocketRuneEffect()
        {
            _FlavorText = "The magician's coat is filled with hidden pockets";
            _EffectText = new List<string> { "Access extradimensional storage" };
            _RelativeStats = new Dictionary<string, Func<string>> { { "Slots", () => $"{Math.Floor(baseWidth * _Effectiveness)}" } };
        }
        public override void DoMagicAttack(Attack baseAttack)
        {
            var player = baseAttack.GetCharacter() as Player;
            if (player == Player.m_localPlayer)
                Debug.Log("Caster is local player");
            else
                Debug.Log("Caster not local player");
            MagicInventory.OpenMagicInventory(InventoryGui.instance, (int)(baseWidth * _Effectiveness), (int)_Quality+1);
        }

    }
}
