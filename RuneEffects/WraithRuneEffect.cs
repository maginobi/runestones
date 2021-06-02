using HarmonyLib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Rendering;

namespace Runestones.RuneEffects
{
    class WraithRuneEffect : RuneEffect
    {
        const string vfxName = "vfx_odin_despawn";
        const string effectName = "SE_Wraith";
        const float baseDuration = 30;
        const int ghostLayer = 17;
        const int characterLayer = 9;
        static int[] collisionLayers = {0, 9, 10, 16, 17, 18, 20, 26, 27, 28 };

        public WraithRuneEffect()
        {
            _FlavorText = "Wraith";
            _EffectText = new List<string> { "Allows you to pass through solid objects", "+20% Movement speed" };
            _QualityEffectText[RuneQuality.Ancient] = new List<string> { "Adds 12 spirit damage to all your attacks" };
            _QualityEffectText[RuneQuality.Dark] = new List<string> { "Gain resistance to physical damage", "Adds 5 poison damage to all your attacks", "+10% More movement speed" };
            _RelativeStats = new Dictionary<string, Func<string>> { { "Duration", () => $"{baseDuration * _Effectiveness :F1} sec" } };
        }

        public override void DoMagicAttack(Attack baseAttack)
        {
            SE_Wraith effect = (SE_Wraith)baseAttack.GetCharacter().GetSEMan().AddStatusEffect(effectName, true);
            if (effect == null)
                effect = (SE_Wraith)baseAttack.GetCharacter().GetSEMan().GetStatusEffect(effectName);
            effect.m_ttl = baseDuration * _Effectiveness;
            effect.SetQuality(_Quality);
        }

        public class SE_Wraith : SE_Stats
        {
            RuneQuality Quality = RuneQuality.Common;
            Action ResetMaterials = () => { };
            public SE_Wraith() : base()
            {
                name = effectName;
                m_name = "Wraith";
                m_time = 0;
                m_ttl = baseDuration;
                m_icon = (from Sprite s in Resources.FindObjectsOfTypeAll<Sprite>() where s.name == "TrophyWraith" select s).FirstOrDefault();

                m_startEffects = new EffectList();
                var vfxPrefab = ZNetScene.instance.GetPrefab(vfxName);
                m_startEffects.m_effectPrefabs = new EffectList.EffectData[] { new EffectList.EffectData { m_prefab = vfxPrefab, m_enabled = true, m_attach = true, m_scale = true } };
            }

            public void SetQuality(RuneQuality quality)
            {
                Quality = quality;
                m_mods = new List<HitData.DamageModPair>();
                switch (quality)
                {
                    case RuneQuality.Common:
                        m_tooltip = "Pass through non-terrain objects\n+20% movement speed";
                        break;
                    case RuneQuality.Ancient:
                        m_tooltip = "Pass through non-terrain objects\n+20% movement speed\nDeal 12 additional spirit damage on every attack";
                        break;
                    case RuneQuality.Dark:
                        m_tooltip = "Pass through non-terrain objects\n+30% movement speed\nDeal 5 additional poison damage on every attack";
                        m_mods = new List<HitData.DamageModPair>
                        {
                            new HitData.DamageModPair{m_type = HitData.DamageType.Blunt, m_modifier = HitData.DamageModifier.Resistant},
                            new HitData.DamageModPair{m_type = HitData.DamageType.Pierce, m_modifier = HitData.DamageModifier.Resistant},
                            new HitData.DamageModPair{m_type = HitData.DamageType.Slash, m_modifier = HitData.DamageModifier.Resistant}
                        };
                        break;
                }
            }

            public override void ModifyAttack(Skills.SkillType skill, ref HitData hitData)
            {
                base.ModifyAttack(skill, ref hitData);
                if (Quality == RuneQuality.Ancient)
                    hitData.m_damage.m_spirit += 12;
                else if (Quality == RuneQuality.Dark)
                    hitData.m_damage.m_poison += 5;
            }

            override public void ModifySpeed(ref float speed)
            {
                speed *= Quality==RuneQuality.Dark ? 1.3f : 1.2f;
            }

            public override void Setup(Character character)
            {
                base.Setup(character);
                character.gameObject.layer = ghostLayer;
                foreach (var layer in collisionLayers)
                {
                    Physics.IgnoreLayerCollision(ghostLayer, layer);
                }
                foreach (var renderer in character.gameObject.GetComponentsInChildren<Renderer>())
                {
                    var oldMaterial = renderer.material;
                    var newMaterial = new Material(Shader.Find("Standard"));
                    newMaterial.SetTransparent();
                    newMaterial.SetColor("_Color", new Color(0.1f, 0.1f, 0.1f, 0.75f));
                    renderer.material = newMaterial;
                    ResetMaterials += () =>
                    {
                        try
                        {
                            if (renderer != null)
                                renderer.material = oldMaterial;
                        }
                        catch (Exception ex)
                        {
                            Debug.LogWarning(ex);
                        }
                    };
                }
            }

            public override void Stop()
            {
                base.Stop();
                m_character.gameObject.layer = characterLayer;
                ResetMaterials.Invoke();
            }
        }
    }

    public static class MatTransExt
    {
        public static void SetTransparent(this Material material)
        {
            material.SetFloat("_Mode", 3);
            material.SetOverrideTag("RenderType", "Transparent");
            material.SetInt("_SrcBlend", (int)BlendMode.One);
            material.SetInt("_DstBlend", (int)BlendMode.OneMinusSrcAlpha);
            material.SetInt("_ZWrite", 0);
            material.DisableKeyword("_ALPHATEST_ON");
            material.DisableKeyword("_ALPHABLEND_ON");
            material.EnableKeyword("_ALPHAPREMULTIPLY_ON");
            material.DisableKeyword("_SPECULARHIGHLIGHTS_OFF");
            material.DisableKeyword("_GLOSSYREFLECTIONS_OFF");
            material.SetFloat("_SpecularHighlights", 1f);
            material.renderQueue = (int)RenderQueue.Transparent;
        }
    }
}
