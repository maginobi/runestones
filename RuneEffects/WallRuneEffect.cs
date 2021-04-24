using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Runestones.RuneEffects
{
    class WallRuneEffect : RuneEffect
    {
        private const string wallPieceName = "$piece_stakewall";
        public WallRuneEffect()
        {
            _FlavorText = "Keeps out your enemies";
            _EffectText = new List<string> { "Conjures a stakewall" };
        }

        public override void DoMagicAttack(Attack baseAttack)
        {
            var player = baseAttack.GetCharacter() as Player;
            bool origPlaceCost = false;
            if (player == null)
            {
                Debug.LogError("Character with wall rune not a player");
                return;
            }
            try
            {
                //Get piece table
                var pieceTableName = "_HammerPieceTable";
                var pieceTable = (from PieceTable table in Resources.FindObjectsOfTypeAll<PieceTable>() where table.gameObject.name == pieceTableName select table).FirstOrDefault();
                if (pieceTable == null || pieceTable.m_pieces.Count <= 0)
                    throw new NullReferenceException("Could not find hammer piece table");
                pieceTable.UpdateAvailable(null, null, false, true); //noPlacementCost set to true - other fields don't matter

                //Select piece
                int category = -1;
                Vector2Int pieceIndex = pieceTable.GetPieceIndexVec(wallPieceName, ref category);
                pieceTable.SetCategory(category);
                pieceTable.SetSelected(pieceIndex);

                //Set rotation
                int rotation = (int)Math.Floor(player.GetLookYaw().eulerAngles.y/22.5);
                typeof(Player).GetField("m_placeRotation", BindingFlags.NonPublic | BindingFlags.Instance).SetValue(player, rotation);

                //Place piece
                origPlaceCost = (bool)typeof(Player).GetField("m_noPlacementCost", BindingFlags.NonPublic | BindingFlags.Instance).GetValue(player);
                typeof(Player).GetField("m_noPlacementCost", BindingFlags.NonPublic | BindingFlags.Instance).SetValue(player, true);
                typeof(Player).GetMethod("SetPlaceMode", BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.FlattenHierarchy).Invoke(player, new object[] { pieceTable });
                bool success = (bool)typeof(Player).GetMethod("PlacePiece", BindingFlags.NonPublic | BindingFlags.Instance).Invoke(player, new object[] { pieceTable.GetSelectedPiece() });
                if (!success)
                    throw new Exception("wall placement failed");
            }
            catch (Exception e)
            {
                Debug.LogError("Wall rune effect failed:\n"+e.ToString());
                player.PickupPrefab(RuneDB.Instance.GetRune("$WallRune").prefab);
            }
            finally
            {
                typeof(Player).GetMethod("SetPlaceMode", BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.FlattenHierarchy).Invoke(player, new object[] { null });
                typeof(Player).GetField("m_noPlacementCost", BindingFlags.NonPublic | BindingFlags.Instance).SetValue(player, origPlaceCost);
            }
        }
    }
}
