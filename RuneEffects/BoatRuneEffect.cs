using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Runestones.RuneEffects
{
    class BoatRuneEffect : RuneEffect
    {
        private List<string> pieceList = new List<string> { "$ship_raft", "$ship_karve", "$ship_longship" };
        public BoatRuneEffect()
        {
            _FlavorText = "You'll need one of these if you want to make it to Vinland";
            _EffectText = new List<string> { "Conjures a raft" };
            _QualityEffectText[RuneQuality.Ancient] = new List<string> { "Conjures a Karve instead" };
            _QualityEffectText[RuneQuality.Dark] = new List<string> { "Conjures a Longship instead" };
            speed = CastingAnimations.CastSpeed.Medium;
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
                Vector2Int pieceIndex = pieceTable.GetPieceIndexVec(pieceList[(int)_Quality], ref category);
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
                    throw new Exception("boat placement failed");
            }
            catch (Exception e)
            {
                Debug.LogError("Boat rune effect failed:\n"+e.ToString());
                player.PickupPrefab(RuneDB.Instance.GetRune("$BoatRune").prefab);
            }
            finally
            {
                typeof(Player).GetMethod("SetPlaceMode", BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.FlattenHierarchy).Invoke(player, new object[] { null });
                typeof(Player).GetField("m_noPlacementCost", BindingFlags.NonPublic | BindingFlags.Instance).SetValue(player, origPlaceCost);
            }
        }
    }
}
