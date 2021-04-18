namespace Runestones.RuneEffects
{
    class AlchemyRuneEffect : RuneEffect
    {
        public string itemAName = null;
        public string itemBName = null;
        public string itemAPrefabName = null;
        public string itemBPrefabName = null;

        public string alertMessage = null;
        public int ratio = 2;
        public bool reversible = false;
        public override void DoMagicAttack(Attack baseAttack)
        {
            var inventory = baseAttack.GetCharacter().GetInventory();
            var itemACount = 0;
            var itemBCount = 0;
            foreach (var item in inventory.GetAllItems())
            {
                if (item.m_shared.m_name == itemAName)
                    itemACount += item.m_stack;
                else if (item.m_shared.m_name == itemBName && reversible)
                    itemBCount += item.m_stack;
            }
            itemACount -= itemACount % ratio;
            inventory.RemoveItems(itemAName, itemACount);
            inventory.RemoveItems(itemBName, itemBCount);
            inventory.AddItems(itemBName, itemBPrefabName, itemACount / ratio);
            inventory.AddItems(itemAName, itemAPrefabName, itemBCount * ratio);
            baseAttack.GetCharacter().Message(MessageHud.MessageType.Center, alertMessage);
        }

    }
}
