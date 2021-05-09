using HarmonyLib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UnityEngine;

namespace Runestones.RuneEffects
{
    public class ForceRuneEffect : RuneEffect
    {
        public const float baseForce = 50;
        public const float baseAngle = 25;
        public const float baseRange = 5;
        public const string vfxName = "vfx_bow_fire";
        private float force = baseForce;
        private Vector3 castDir;
        public ForceRuneEffect()
        {
            _FlavorText = "May the force be with you";
            _EffectText = new List<string> { "Knocks back enemies", "Cone: 5m, 25 degrees" };
            _QualityEffectText[RuneQuality.Ancient] = new List<string> { "+100% Range", "+100% Spread angle" };
            _QualityEffectText[RuneQuality.Dark] = new List<string> { "+200% Force" };
            _RelativeStats = new Dictionary<string, Func<string>> { { "Force", () => $"{baseForce * _Effectiveness * (_Quality==RuneQuality.Dark ? 3 : 1)}" } };
        }
        public override void DoMagicAttack(Attack baseAttack)
        {
            castDir = baseAttack.BetterAttackDir();
            force = baseForce * _Effectiveness * (_Quality == RuneQuality.Dark ? 3 : 1);

            var vfx = (from GameObject prefab in Resources.FindObjectsOfTypeAll<GameObject>() where prefab.name == vfxName select prefab).FirstOrDefault();
            GameObject.Instantiate(vfx, baseAttack.GetAttackOrigin().position, Quaternion.LookRotation(castDir));
            
            var project = new MagicProjectile
            {
                m_range = baseRange * (_Quality==RuneQuality.Ancient ? 2 : 1),
                m_actionOnHitCollider = DoPushback,
                m_attackSpread = baseAngle * (_Quality == RuneQuality.Ancient ? 2 : 1)
            };
            project.Cast(baseAttack.GetAttackOrigin(), castDir);
        }
        public void DoPushback(Collider collider)
        {
            var destructible = collider.gameObject.GetComponent<IDestructible>();
            if (destructible != null && destructible is Character character)
            {
                // stagger calculations from Valheim::Character.AddStaggerDamage
                var staggerDamageField = typeof(Character).GetField("m_staggerDamage", BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.FlattenHierarchy);
                var currentStagger = (float)staggerDamageField.GetValue(character);
                currentStagger += force;
                float maxHealth = character.GetMaxHealth();
                float threshold = (character.IsPlayer() ? (maxHealth / 2f) : (maxHealth * character.m_staggerDamageFactor));
                if (currentStagger >= threshold)
                {
                    staggerDamageField.SetValue(character, 0f);
                    character.Stagger(castDir);
                    // adjusted force calculation from Valheim::Character.ApplyPushback
                    float adjustedForce = force * Mathf.Clamp01(1f + character.GetEquipmentMovementModifier() * 1.5f) / character.GetMass() * 5f;
                    Vector3 vectorForce = castDir * adjustedForce;
                    vectorForce.y = 0f;
                    var charPushForce = typeof(Character).GetField("m_pushForce", BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.FlattenHierarchy);
                    if (((Vector3)charPushForce.GetValue(character)).magnitude < vectorForce.magnitude)
                    {
                        charPushForce.SetValue(character, vectorForce);
                    }
                }
                else
                {
                    staggerDamageField.SetValue(character, currentStagger);
                }
            }
        }
        
    }
}
