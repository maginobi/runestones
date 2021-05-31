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
        private List<string> pieceList = new List<string> { "$piece_firepit", "$piece_bonfire", "$piece_hearth" };

        public HearthRuneEffect()
        {
            _FlavorText = "A warm fire is magic in and of itself";
            _EffectText = new List<string> { "Conjures a campfire" };
            _QualityEffectText[RuneQuality.Ancient] = new List<string> { "Conjures a Bonfire instead" };
            _QualityEffectText[RuneQuality.Dark] = new List<string> { "Conjures a Hearth instead" };
        }

        public override void DoMagicAttack(Attack baseAttack)
        {
            var player = baseAttack.GetCharacter() as Player;
            var origPlaceCost = false;
            if (player == null)
            {
                Debug.LogError("Character with hearth rune not a player");
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
                Vector2Int pieceIndex = pieceTable.GetPieceIndexVec(pieceList[(int)_Quality], ref category);
                pieceTable.SetCategory(category);
                pieceTable.SetSelected(pieceIndex);

                //Set rotation
                int rotation = (int)Math.Floor(player.GetLookYaw().eulerAngles.y / 22.5);
                typeof(Player).GetField("m_placeRotation", BindingFlags.NonPublic | BindingFlags.Instance).SetValue(player, rotation);

                //Place piece
                origPlaceCost = (bool)typeof(Player).GetField("m_noPlacementCost", BindingFlags.NonPublic | BindingFlags.Instance).GetValue(player);
                typeof(Player).GetField("m_noPlacementCost", BindingFlags.NonPublic | BindingFlags.Instance).SetValue(player, true);
                typeof(Player).GetMethod("SetPlaceMode", BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.FlattenHierarchy).Invoke(player, new object[] { pieceTable });
                var piece = pieceTable.GetSelectedPiece();
                piece.gameObject.GetComponent<Fireplace>().m_startFuel = 1;
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
                typeof(Player).GetField("m_noPlacementCost", BindingFlags.NonPublic | BindingFlags.Instance).SetValue(player, origPlaceCost);
            }
        }
    }
}
