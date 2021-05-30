using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Runestones.RuneEffects
{
    class IndexRuneEffect : RuneEffect
    {
        public const float baseDuration = 120;
        public const string preVfxPrefabName = "vfx_prespawn";
        public IndexRuneEffect()
        {
            _FlavorText = "Knowledge is power";
            _EffectText = new List<string> { "Rearranges your hotkeys for quick and easy spellcasting" };
            _QualityEffectText[RuneQuality.Ancient] = new List<string> { "+25% Magic xp gain" };
            _QualityEffectText[RuneQuality.Dark] = new List<string> { "+25 Spell Effectiveness" };
            speed = CastingAnimations.CastSpeed.Medium;
        }

        public override string GetDescription()
        {
            if (_Quality == RuneQuality.Common)
                _RelativeStats = new Dictionary<string, Func<string>>();
            else
                _RelativeStats = new Dictionary<string, Func<string>> { { "Duration", () => $"{baseDuration * _Effectiveness:F1} sec" } };
            return base.GetDescription();
        }

        public override void Precast(Attack baseAttack)
        {
            base.Precast(baseAttack);
            var vfxPrefab = ZNetScene.instance.GetPrefab(preVfxPrefabName);
            var go = GameObject.Instantiate(vfxPrefab, baseAttack.GetCharacter().GetCenterPoint(), Quaternion.identity, baseAttack.GetCharacter().transform);
            var particles = go.GetComponentInChildren<ParticleSystem>();
            var mainSettings = particles.main;
            var shapeSettings = particles.shape;
            mainSettings.duration = 2;
            mainSettings.startLifetime = 2;
            shapeSettings.radius = 10;
        }

        public override void DoMagicAttack(Attack baseAttack)
        {
            var inventory = baseAttack.GetCharacter().GetInventory();
            for (int i=0; i<Math.Min(inventory.GetWidth(), inventory.GetEmptySlots()); i++)
            {
                var currentItem = inventory.GetItemAt(i, 0);
                if (currentItem != null)
                {
                    var destination = inventory.FindEmptySlot(false);
                    currentItem.m_gridPos = destination;
                }
            }

            List<ItemDrop.ItemData> runePrefs = (from ItemDrop.ItemData item in inventory.GetAllItems()
                                                where item.m_shared.m_ammoType == "rune"
                                                select item).OrderByDescending(item => item.m_variant).ToList();

            for (int i=0; i < (new int[] { inventory.GetWidth(), inventory.GetEmptySlots(), runePrefs.Count }).Min(); i++)
            {
                if(inventory.GetItemAt(i, 0) == null)
                {
                    runePrefs[i].m_gridPos = new Vector2i(i, 0);
                }
            }

            if(_Quality == RuneQuality.Ancient)
            {
                baseAttack.GetCharacter().GetSEMan().AddStatusEffect("SE_Study", true);
                baseAttack.GetCharacter().GetSEMan().GetStatusEffect("SE_Study").m_ttl = baseDuration * _Effectiveness;
            }
            else if(_Quality == RuneQuality.Dark)
            {
                baseAttack.GetCharacter().GetSEMan().AddStatusEffect("SE_Scholar", true);
                baseAttack.GetCharacter().GetSEMan().GetStatusEffect("SE_Scholar").m_ttl = baseDuration * _Effectiveness;
            }

            baseAttack.GetCharacter().Message(MessageHud.MessageType.TopLeft, "Indexed");
        }

        public class SE_Study : SE_Stats
        {
            public SE_Study() : base()
            {
                name = "SE_Study";
                m_name = "Studious";
                m_tooltip = "+25% Magic xp gain";
                m_time = 0;
                m_ttl = baseDuration;
                m_icon = ObjectDB.instance.GetItemPrefab("$AncientIndexRune").GetComponent<ItemDrop>().m_itemData.GetIcon();

                m_raiseSkill = MagicSkill.MagicSkillDef.m_skill;
                m_raiseSkillModifier = 0.25f;
            }
        }

        public interface ISE_MagicBuff
        {
            void ModifyMagic(ref float magic);
        }

        public class SE_Scholar : StatusEffect, ISE_MagicBuff
        {
            public float m_magicModifier = 0.25f;

            public SE_Scholar() : base()
            {
                name = "SE_Scholar";
                m_name = "Scholarly";
                m_tooltip = "+25 Spell Effectiveness";
                m_time = 0;
                m_ttl = baseDuration;
                m_icon = ObjectDB.instance.GetItemPrefab("$DarkIndexRune").GetComponent<ItemDrop>().m_itemData.GetIcon();
            }

            public void ModifyMagic(ref float magic)
            {
                magic += m_magicModifier;
            }
        }

    }
}
