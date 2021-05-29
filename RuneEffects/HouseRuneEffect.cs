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
        private Vector3 originLocation;
        private Vector3 originForward;
        public HouseRuneEffect()
        {
            _FlavorText = "Home is where ya put yer feet up";
            _EffectText = new List<string> { "Conjures a small lean-to" };
            _QualityEffectText[RuneQuality.Ancient] = new List<string> { "Conjures a larger house instead" };
            _QualityEffectText[RuneQuality.Dark] = new List<string> { "Conjures a furnished log cabin instead" };
            targetLock = true;
            speed = CastingAnimations.CastSpeed.Slow;
        }

        public override void Precast(Attack baseAttack)
        {
            originLocation = baseAttack.GetCharacter().transform.position;
            originForward = baseAttack.GetCharacter().transform.forward;
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
                TerrainModifier.SetTriggerOnPlaced(trigger: true);
                switch(_Quality)
                {
                    case RuneQuality.Common:
                        CommonEffect(player);
                        break;
                    case RuneQuality.Ancient:
                        AncientEffect(player);
                        break;
                    case RuneQuality.Dark:
                        DarkEffect(player);
                        break;
                }
                SnapToGround.SnappAll();
                TerrainModifier.SetTriggerOnPlaced(trigger: false);
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

        public void CommonEffect(Player player)
        {
            //Spawn location structure
            var pos = originLocation;
            var rot = Quaternion.LookRotation(originForward) * Quaternion.Euler(0, 90, 0);
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

        public void AncientEffect(Player player)
        {
            var houseLocPrefab = (from GameObject prefab in Resources.FindObjectsOfTypeAll<GameObject>() where prefab.name == "WoodHouse11" select prefab).FirstOrDefault();
            var housePrefab = houseLocPrefab.transform.Find("house").gameObject;
            var flattenPrefab = houseLocPrefab.transform.Find("flatten").gameObject;
            var bedPrefab = (from GameObject prefab in Resources.FindObjectsOfTypeAll<GameObject>() where prefab.name == "bed" select prefab).FirstOrDefault();
            var workbenchPrefab = (from GameObject prefab in Resources.FindObjectsOfTypeAll<GameObject>() where prefab.name == "piece_workbench" select prefab).FirstOrDefault();
            var torchPrefab = (from GameObject prefab in Resources.FindObjectsOfTypeAll<GameObject>() where prefab.name == "piece_groundtorch_wood" select prefab).FirstOrDefault();
            var bedPos = new Vector2(1, 1.75f);
            var benchPos = new Vector2(1, -1.75f);
            var torch1Pos = new Vector2(-4, 1.75f);
            var torch2Pos = new Vector2(-4, -1.75f);
            Debug.Log($"Prefab: {housePrefab}");
            var housePos = originLocation + originForward * 7.5f;
            var rotation = Quaternion.LookRotation(originForward);
            var forwardDir = originForward;
            var rightDir = originForward; //rotate 90 deg
            var quat90 = Quaternion.Euler(0, -90, 0);
            var quat180 = Quaternion.Euler(0, -180, 0);
            GameObject.Instantiate(housePrefab, housePos, rotation * quat180 * quat90);
            GameObject.Instantiate(flattenPrefab, housePos, rotation * quat180 * quat90);

            GameObject.Instantiate(bedPrefab, housePos + forwardDir * bedPos.x + rightDir * bedPos.y, rotation * quat180);
            GameObject.Instantiate(workbenchPrefab, housePos + forwardDir * benchPos.x + rightDir * benchPos.y, rotation * quat180 * quat90);
            GameObject.Instantiate(torchPrefab, housePos + forwardDir * torch1Pos.x + rightDir * torch1Pos.y + player.transform.up, rotation);
            GameObject.Instantiate(torchPrefab, housePos + forwardDir * torch2Pos.x + rightDir * torch2Pos.y + player.transform.up, rotation);
        }

        public void DarkEffect(Player player)
        {
            var cabinPrefab = (from GameObject prefab in Resources.FindObjectsOfTypeAll<GameObject>() where prefab.name == "AbandonedLogCabin01" select prefab).FirstOrDefault();
            var bedPrefab = (from GameObject prefab in Resources.FindObjectsOfTypeAll<GameObject>() where prefab.name == "piece_bed02" select prefab).FirstOrDefault();
            var workbenchPrefab = (from GameObject prefab in Resources.FindObjectsOfTypeAll<GameObject>() where prefab.name == "piece_workbench" select prefab).FirstOrDefault();
            var chestPrefab = (from GameObject prefab in Resources.FindObjectsOfTypeAll<GameObject>() where prefab.name == "piece_chest_wood" select prefab).FirstOrDefault();
            var bedPos = new Vector2(1, 4);
            var benchPos = new Vector2(-3, -4);
            var chestPos = new Vector2(1.9f, 1.75f);
            Debug.Log($"Prefab: {cabinPrefab}");
            var cabinPos = originLocation + originForward * 7.5f;
            var rotation = Quaternion.LookRotation(originForward);
            var forwardDir = originForward;
            var rightDir = originForward; //rotate 90 deg
            var quat90 = Quaternion.Euler(0, -90, 0);
            var quat180 = Quaternion.Euler(0, -180, 0);
            var cabinObj = GameObject.Instantiate(cabinPrefab, cabinPos, rotation * quat90);
            GameObject.Instantiate(bedPrefab, cabinPos + forwardDir * bedPos.x + rightDir * bedPos.y, rotation * quat180);
            GameObject.Instantiate(workbenchPrefab, cabinPos + forwardDir * benchPos.x + rightDir * benchPos.y, rotation * quat180 * quat90);
            GameObject.Instantiate(chestPrefab, cabinPos + forwardDir * chestPos.x + rightDir * chestPos.y, rotation * quat180);
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
