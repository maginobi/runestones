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
            _QualityEffectText[RuneQuality.Ancient] = new List<string> { "+100% Capacity" };
            _QualityEffectText[RuneQuality.Dark] = new List<string> { "+200% Capacity" };
            _RelativeStats = new Dictionary<string, Func<string>> { { "Slots", () => $"{Math.Floor(baseWidth * _Effectiveness) * ((int)_Quality+1)}" } };
            speed = CastingAnimations.CastSpeed.Instant;
        }
        public override void DoMagicAttack(Attack baseAttack)
        {
            var player = baseAttack.GetCharacter() as Player;
            if (player == Player.m_localPlayer)
                Debug.Log("Pocket rune caster is local player");
            else
                throw new Exception("Pocket rune caster not local player");
            MagicInventory.OpenMagicInventory(InventoryGui.instance, (int)(baseWidth * _Effectiveness), (int)_Quality+1);
        }

    }
}
