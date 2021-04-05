namespace Runestones.RuneEffects
{
    class TransmuteRuneEffect : RuneEffect
    {
        private const string copperName = "$item_copper";
        private const string tinName = "$item_tin";
        private const string copperPrefabName = "Copper";
        private const string tinPrefabName = "Tin";
        public void DoMagicAttack(Attack baseAttack)
        {
            var inventory = baseAttack.GetCharacter().GetInventory();
            var copperCount = 0;
            var tinCount = 0;
            foreach (var item in inventory.GetAllItems())
            {
                if (item.m_shared.m_name == copperName)
                    copperCount += item.m_stack;
                else if (item.m_shared.m_name == tinName)
                    tinCount += item.m_stack;
            }
            if (tinCount % 2 != 0)
                tinCount--;
            inventory.RemoveItems(copperName, copperCount);
            inventory.RemoveItems(tinName, tinCount);
            inventory.AddItems(copperName, copperPrefabName, tinCount / 2);
            inventory.AddItems(tinName, tinPrefabName, copperCount * 2);
            baseAttack.GetCharacter().Message(MessageHud.MessageType.Center, "Transmuted Copper/Tin");
        }

    }
}
