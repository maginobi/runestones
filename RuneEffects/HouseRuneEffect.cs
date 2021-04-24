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
    class HouseRuneEffect : RuneEffect
    {
        private const string bedPieceName = "$piece_bed";
        private const string houseLocationName = "WoodHouse3";
        private const int spawnSeed = 32979;
        private const float bedDisplacement = 4;
        private const string magicBedDesc = "magic_bed";
        private const float magicBedExposureAllowed = 0.65f;
        public HouseRuneEffect()
        {
            _FlavorText = "Home is where ya put yer feet up";
            _EffectText = new List<string> { "Conjures a small lean-to" };
        }

        public override void DoMagicAttack(Attack baseAttack)
        {
            var player = baseAttack.GetCharacter() as Player;
            bool origPlaceCost = false;
            if (player == null)
            {
                Debug.LogError("Character with home rune not a player");
                return;
            }
            try
            {
                //Spawn location structure
                var pos = player.transform.position;
                var rot = player.GetLookYaw() * Quaternion.Euler(0, 90, 0);
                var getLocationMethods = typeof(ZoneSystem).GetMethods(BindingFlags.NonPublic | BindingFlags.Instance);
                var method = (from MethodInfo m in getLocationMethods where m.Name == "GetLocation" && m.GetParameters().Any(param => param.ParameterType == typeof(string)) select m).FirstOrDefault();
                if (method == null)
                    throw new NullReferenceException("Could not find ZoneSystem.GetLocation method");
                ZoneSystem.ZoneLocation location = (ZoneSystem.ZoneLocation)method.Invoke(ZoneSystem.instance, new object[] { houseLocationName });
                //m_didZoneTest = true;
                List<GameObject> spawnedGhostObjects = new List<GameObject>();
                typeof(ZoneSystem).GetMethod("SpawnLocation", BindingFlags.NonPublic | BindingFlags.Instance).Invoke(ZoneSystem.instance, new object[] { location, spawnSeed, pos, rot, ZoneSystem.SpawnMode.Full, spawnedGhostObjects });

                //Set up for bed spawning
                var bedPos = player.transform.position + player.transform.forward * bedDisplacement;
                var bedRot = rot;

                //Get piece table
                var pieceTableName = "_HammerPieceTable";
                var pieceTable = (from PieceTable table in Resources.FindObjectsOfTypeAll<PieceTable>() where table.gameObject.name == pieceTableName select table).FirstOrDefault();
                if (pieceTable == null || pieceTable.m_pieces.Count <= 0)
                    throw new NullReferenceException("Could not find hammer piece table");
                pieceTable.UpdateAvailable(null, null, false, true); //noPlacementCost set to true - other fields don't matter

                //Select piece
                int category = -1;
                Vector2Int pieceIndex = pieceTable.GetPieceIndexVec(bedPieceName, ref category);
                pieceTable.SetCategory(category);
                pieceTable.SetSelected(pieceIndex);

                //Place piece
                var piece = pieceTable.GetSelectedPiece();
                piece.m_description = magicBedDesc;
                TerrainModifier.SetTriggerOnPlaced(trigger: true);
                GameObject gameObject = UnityEngine.Object.Instantiate(piece.gameObject, bedPos, bedRot);
                TerrainModifier.SetTriggerOnPlaced(trigger: false);
                WearNTear wear = gameObject.GetComponent<WearNTear>();
                if ((bool)wear)
                {
                    wear.OnPlaced();
                }
                piece.m_description = "";
            }
            catch (Exception e)
            {
                Debug.LogError("Home rune effect failed:\n"+e.ToString());
                player.PickupPrefab(RuneDB.Instance.GetRune("$HomeRune").prefab);
            }
            finally
            {
                typeof(Player).GetMethod("SetPlaceMode", BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.FlattenHierarchy).Invoke(player, new object[] { null });
                typeof(Player).GetField("m_noPlacementCost", BindingFlags.NonPublic | BindingFlags.Instance).SetValue(player, origPlaceCost);
            }
        }

        [HarmonyPatch(typeof(Bed), "CheckExposure")]
        public static class MagicBedMod
        {
            public static bool Prefix(Bed __instance, ref bool __result, Player human)
            {
                if(__instance.gameObject.GetComponent<Piece>().m_description != magicBedDesc)
                    return true;

                __result = false;
                Cover.GetCoverForPoint(__instance.GetSpawnPoint(), out float num, out bool flag);
                if (!flag)
                {
                    human.Message(MessageHud.MessageType.Center, "$msg_bedneedroof");
                    return false;
                }
                if (num < magicBedExposureAllowed)
                {
                    human.Message(MessageHud.MessageType.Center, "$msg_bedtooexposed");
                    return false;
                }
                ZLog.Log((object)("exporeusre check " + num + "  " + flag.ToString()));
                __result = true;
                return false;
            }
        }
    }
}
