using HarmonyLib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Runestones.RuneEffects
{
    public class TimberRuneEffect : RuneEffect
    {
        const string baseProjectileName = "gdking_root_projectile";
        const string customProjectileName = "runestones_timber_projectile";
        const float basePierceDamage = 35;
        const float baseChopDamage = 9999;
        const float darkChopDamage = 999999;
        static readonly string[] treelikeCreatures = new string[] { "$enemy_greyling", "$enemy_greydwarf", "$enemy_greydwarfshaman", "$enemy_greydwarfbrute", "$enemy_gdking" };
        public TimberRuneEffect()
        {
            _FlavorText = "Give me six hours to chop down a tree and I will spend the first four sharpening the axe - Abraham Lincoln";
            _EffectText = new List<string> { "Fells trees", "Damages treelike creatures" };
            _QualityEffectText[RuneQuality.Ancient] = new List<string> { "+200% Damage to treelike creatures" };
            _QualityEffectText[RuneQuality.Dark] = new List<string> { "Fell tougher trees" };
            _RelativeStats = new Dictionary<string, Func<string>> { { "Piercing damage", () => $"{basePierceDamage * _Effectiveness * (_Quality == RuneQuality.Ancient ? 3 : 1):F1}" } };
        }

        public override void DoMagicAttack(Attack baseAttack)
        {
            var projectilePrefab = GameObject.Instantiate(ZNetScene.instance.GetPrefab(baseProjectileName));
            var projectile = projectilePrefab.GetComponent<Projectile>();
            projectile.name = customProjectileName;
            projectile.m_damage.m_chop = (_Quality==RuneQuality.Dark ? darkChopDamage : baseChopDamage);
            projectile.m_damage.m_pierce = basePierceDamage * _Effectiveness * (_Quality==RuneQuality.Ancient ? 3 : 1);

            baseAttack.m_attackType = Attack.AttackType.Projectile;
            baseAttack.m_attackProjectile = projectilePrefab;
            baseAttack.m_projectileAccuracy = 0;
            baseAttack.m_useCharacterFacing = true;
            baseAttack.m_useCharacterFacingYAim = true;
            baseAttack.m_attackHeight = 1.5f;
            baseAttack.m_attackRange = 1;
            baseAttack.DoProjectileAttack();
        }

        [HarmonyPatch(typeof(Projectile), "Setup")]
        public static class TimberProjectileSetupPatch
        {
            public static void Prefix(Projectile __instance, ref HitData hitData)
            {
                // Do not modify the timber projectile hit data based on weapon data
                if (__instance.name.Contains(customProjectileName))
                {
                    hitData = null;
                }
            }
        }

        [HarmonyPatch(typeof(Projectile), "OnHit")]
        public static class TimberProjectileHitPatch
        {
            public static bool Prefix(Projectile __instance, ref Collider collider, Vector3 hitPoint)
            {
                if (__instance.name.Contains(customProjectileName))
                {
                    GameObject gameObject = (collider ? Projectile.FindHitObject(collider) : null);
                    IDestructible destructible = (gameObject ? gameObject.GetComponent<IDestructible>() : null);
                    if (destructible != null)
                    {
                        bool hitCharacter = false;
                        bool hitValid = (bool)typeof(Projectile).GetMethod("IsValidTarget", BindingFlags.NonPublic | BindingFlags.Instance).Invoke(__instance, new object[] { destructible, hitCharacter });
                        if (!hitValid)
                            return true;
                        HitData hitData = new HitData();
                        hitData.m_hitCollider = collider;
                        hitData.m_damage = __instance.m_damage.Clone();
                        hitData.m_toolTier = __instance.m_damage.m_chop == darkChopDamage ? 2 : 0;
                        if ((destructible is Character character) && !treelikeCreatures.Contains(character.m_name))
                        {
                            hitData.m_damage.m_pierce = 0;
                        }
                        hitData.m_pushForce = 20;
                        hitData.m_backstabBonus = 3;
                        hitData.m_point = hitPoint;
                        hitData.m_dir = __instance.transform.forward;
                        hitData.m_dodgeable = true;
                        hitData.m_blockable = true;
                        hitData.m_skill = MagicSkill.MagicSkillDef.m_skill;
                        var owner = (Character)typeof(Projectile).GetField("m_owner", BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.FlattenHierarchy).GetValue(__instance);
                        hitData.SetAttacker(owner);
                        destructible.Damage(hitData);

                        __instance.m_hitEffects.Create(hitPoint, Quaternion.identity);
                        if (__instance.m_hitNoise > 0f)
                        {
                            BaseAI.DoProjectileHitNoise(__instance.transform.position, __instance.m_hitNoise, owner);
                        }
                        ((ZNetView)typeof(Projectile).GetField("m_nview", BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.FlattenHierarchy).GetValue(__instance)).InvokeRPC("OnHit");
                        ZNetScene.instance.Destroy(__instance.gameObject);
                        return false;
                    }
                }
                return true;
            }
        }
    }
}
