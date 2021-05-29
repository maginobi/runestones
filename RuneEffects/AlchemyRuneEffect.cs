using System.Collections.Generic;
using System.Text;
using UnityEngine;

namespace Runestones.RuneEffects
{
    class AlchemyRuneEffect : RuneEffect
    {
        public const string vfxPrefabName = "vfx_bonfire_AddFuel";

        public struct Conversion
        {
            public string itemAName;
            public string itemBName;
            public string itemAPrefabName;
            public string itemBPrefabName;
            public int ratio;
            public bool reversible;
        };

        public List<Conversion> conversionList;

        public AlchemyRuneEffect()
        {
            _FlavorText = "Is this really an equivalent exchange?";
            speed = CastingAnimations.CastSpeed.Slow;
        }

        public override string GetDescription()
        {
            var conv = conversionList[(int)_Quality];
            _QualityEffectText[_Quality] = new List<string> { $"Converts {conv.itemAPrefabName} {(conv.reversible ? "<" : "" + "")}=> {conv.itemBPrefabName} (ratio {conv.ratio}:1)" };
            return base.GetDescription();
        }

        public override void DoMagicAttack(Attack baseAttack)
        {
            var conv = conversionList[(int)_Quality];
            var inventory = baseAttack.GetCharacter().GetInventory();
            var itemACount = 0;
            var itemBCount = 0;
            foreach (var item in inventory.GetAllItems())
            {
                if (item.m_shared.m_name == conv.itemAName)
                    itemACount += item.m_stack;
                else if (item.m_shared.m_name == conv.itemBName && conv.reversible)
                    itemBCount += item.m_stack;
            }
            itemACount -= itemACount % conv.ratio;
            inventory.RemoveItem(conv.itemAName, itemACount);
            inventory.RemoveItem(conv.itemBName, itemBCount);
            inventory.AddItems(conv.itemBName, conv.itemBPrefabName, itemACount / conv.ratio);
            inventory.AddItems(conv.itemAName, conv.itemAPrefabName, itemBCount * conv.ratio);
            var vfxPrefab = ZNetScene.instance.GetPrefab(vfxPrefabName);
            GameObject.Instantiate(vfxPrefab, baseAttack.GetCharacter().transform.position, Quaternion.identity);
            baseAttack.GetCharacter().Message(MessageHud.MessageType.Center, $"Converted {conv.itemAPrefabName} {(conv.reversible ? "<" : "" + "")}=> {conv.itemBPrefabName} (ratio {conv.ratio}:1)");
        }

    }
}
