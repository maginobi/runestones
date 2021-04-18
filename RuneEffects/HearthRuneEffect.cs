using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Runestones.RuneEffects
{
    class HearthRuneEffect : RuneEffect
    {
        private readonly Vector2Int campfireIndex = new Vector2Int(1, 0); //The piece table index for the campfire

        public override void DoMagicAttack(Attack baseAttack)
        {
            var player = baseAttack.GetCharacter() as Player;
            if (player == null)
            {
                Debug.LogError("Character with hearth rune not a player");
                return;
            }
            try
            {
                var pieceTableName = "_HammerPieceTable";
                var pieceTable = (from PieceTable table in Resources.FindObjectsOfTypeAll<PieceTable>() where table.gameObject.name == pieceTableName select table).FirstOrDefault();
                if (pieceTable == null || pieceTable.m_pieces.Count <= 0)
                    throw new NullReferenceException("Could not find hammer piece table");
                pieceTable.UpdateAvailable(null, null, false, true); //noPlacementCost set to true - other fields don't matter
                pieceTable.SetCategory((int)Piece.PieceCategory.Misc);
                pieceTable.SetSelected(campfireIndex);
                typeof(Player).GetMethod("SetPlaceMode", BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.FlattenHierarchy).Invoke(player, new object[] { pieceTable });
                bool noCost = (bool)typeof(Player).GetField("m_noPlacementCost", BindingFlags.NonPublic | BindingFlags.Instance).GetValue(player);
                noCost = true;
                bool success = (bool)typeof(Player).GetMethod("PlacePiece", BindingFlags.NonPublic | BindingFlags.Instance).Invoke(player, new object[] { pieceTable.GetSelectedPiece() });
                if (!success)
                    throw new Exception("campfire placement failed");
            }
            catch (Exception e)
            {
                Debug.LogError("Hearth rune effect failed:\n"+e.ToString());
                player.PickupPrefab(RuneDB.Instance.GetRune("$HearthRune").prefab);
            }
            finally
            {
                typeof(Player).GetMethod("SetPlaceMode", BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.FlattenHierarchy).Invoke(player, new object[] { null });
                bool noCost = (bool)typeof(Player).GetField("m_noPlacementCost", BindingFlags.NonPublic | BindingFlags.Instance).GetValue(player);
                noCost = false;
            }
        }
    }
}
