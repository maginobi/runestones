using System.Collections.Generic;
using System.Reflection;
using UnityEngine;

namespace Runestones
{
    class Staff
    {
        private const string StaffPrefabPath = "Assets/PrefabInstance/Staff.prefab";
        public GameObject StaffPrefab;

        private const string TokenName = "$custom_item_staff";
        private const string TokenValue = "Staff";

        private const string TokenDescriptionName = "$custom_item_staff_description";
        private const string TokenDescriptionValue = "A ritual staff used for Seidr";

        public static void Load(AssetBundle assets)
        {
            var staffPrefab = assets.LoadAsset<GameObject>(StaffPrefabPath);
            staffPrefab.GetComponent<ItemDrop>().m_itemData.m_shared.m_attack = new Attack();
            staffPrefab.GetComponent<ItemDrop>().m_itemData.m_shared.m_attack.m_attackType = Attack.AttackType.Horizontal;
            staffPrefab.GetComponent<ItemDrop>().m_itemData.m_shared.m_attack.m_attackAnimation = "bow_fire";
            var staffObj = new Staff() { StaffPrefab = staffPrefab };

            ItemHelper.Instance.AddItem(staffPrefab);
            ItemHelper.Instance.AddToken(TokenName, TokenValue);
            ItemHelper.Instance.AddToken(TokenDescriptionName, TokenDescriptionValue);

            ItemHelper.Instance.OnAllItemsLoaded(staffObj.CreateRecipe);
        }

        private void CreateRecipe()
        {
            if (StaffPrefab == null || ObjectDB.instance.GetRecipe(StaffPrefab.GetComponent<ItemDrop>().m_itemData) != null)
                return;

            var recipe = ScriptableObject.CreateInstance<Recipe>();
            recipe.name = "Staff";
            recipe.m_item = StaffPrefab.GetComponent<ItemDrop>();

            var woodDrop = ObjectDB.instance.GetItemPrefab("Wood").GetComponent<ItemDrop>();

            var materials = new Piece.Requirement[] {
                new Piece.Requirement()
                {
                    m_resItem = woodDrop,
                    m_amount = 4
                }
            };
            recipe.m_resources = materials;

            recipe.m_minStationLevel = 1;
            recipe.m_enabled = true;

            var namedPrefabsInfo = ZNetScene.instance.GetType().GetField("m_namedPrefabs", BindingFlags.NonPublic | BindingFlags.Instance);
            Dictionary<int, GameObject> namedPrefabs = (Dictionary<int, GameObject>)namedPrefabsInfo.GetValue(ZNetScene.instance);
            recipe.m_repairStation = namedPrefabs["piece_workbench".GetStableHashCode()].GetComponent<CraftingStation>();            

            ObjectDB.instance.m_recipes.Add(recipe);
        }
    }
}
