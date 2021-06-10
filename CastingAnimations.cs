using HarmonyLib;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Runestones
{
    public class CastingAnimations : MonoBehaviour
    {
        public enum CastSpeed
        {
            Instant,
            Fast,
            Medium,
            Slow,
            Super
        }

        public static Dictionary<CastSpeed, string> SpeedAnimations = new Dictionary<CastSpeed, string>
        {
            { CastSpeed.Instant, "Zombie_Attack2" }, // "knife_slash0" },
            { CastSpeed.Fast, "axe_swing" }, //"Standing 1H Magic Attack 01" }, //1H, directional; also note CastFireball for longer 1H directional
            { CastSpeed.Medium, "Standing 1H Magic Attack 03" }, //1H, vertical; also note Standing 1H Cast Spell 01 for less dramatic 1H vertical
            { CastSpeed.Slow, "ProtectiveSpell" }, //2H, long; 'CastFireball' similar length. //Check out 'Standing 2H Magic Attack 04' for this as well
            { CastSpeed.Super, "CastFireball" }
        };
        /*
        public static Dictionary<CastSpeed, float> FrameRateAdjustments = new Dictionary<CastSpeed, float>
        {
            { CastSpeed.Instant, 1 },
            { CastSpeed.Fast, 1 },
            { CastSpeed.Medium, 1 },
            { CastSpeed.Slow, 1 },
            { CastSpeed.Super, 1 }
        };
        */

        public const string overrideAnimName = "Cheer";
        public const string overrideAnimTrigger = "emote_cheer";
        public const string overrideAnimTriggerStop = "emote_stop";

        public const float instantSpellDelay = 0.05f;
        public float lastCastTime = -1;

        public bool isCasting = false;

        public System.Action OnComplete { get; set; }
        public bool IsLocked { get => Time.time - lastCastTime <= instantSpellDelay; }

        public void Play(CastSpeed speed)
        {
            // Lock this component on a delay
            lastCastTime = Time.time;

            // Get animator reference
            var animator = gameObject.GetComponentInChildren<Animator>();

            // Get proper animation
            var anim_name = SpeedAnimations[speed];
            var animation = (from AnimationClip motion in Resources.FindObjectsOfTypeAll<AnimationClip>() where motion.name == anim_name select motion).FirstOrDefault();

            // Take control of animation controller
            var originalController = animator.runtimeAnimatorController;
            var overrideController = new AnimatorOverrideController(originalController);

            // Set up for cleanup
            var originalAnimation = overrideController[overrideAnimName];
            OnComplete += () =>
            {
                animator.ResetTrigger(overrideAnimTrigger);
                animator.SetTrigger(overrideAnimTriggerStop);
                overrideController[overrideAnimName] = originalAnimation;
            };

            // Set up and play animation
            overrideController[overrideAnimName] = animation;
            animator.runtimeAnimatorController = overrideController;
            animator.SetTrigger(overrideAnimTrigger);

            isCasting = true;
            Invoke("FinishAnim", Mathf.Max(animation?.length ?? 0, instantSpellDelay));
        }

        public void FinishAnim()
        {
            isCasting = false;
            OnComplete.Invoke();
        }
    }

    [HarmonyPatch(typeof(Player), "InAttack")]
    public class PlayerInAttackPatch
    {
        public static void Postfix(Player __instance, ref bool __result)
        {
            if (!__result)
            {
                var castingAnim = __instance.gameObject.GetComponent<CastingAnimations>();
                if (castingAnim != null)
                {
                    __result = castingAnim.isCasting;
                }
            }
        }
    }

    [HarmonyPatch(typeof(Player), "Awake")]
    public class AddCastingAnimationsPatch
    {
        public static void Postfix(Player __instance)
        {
            if (__instance.gameObject.GetComponent<CastingAnimations>() == null)
                __instance.gameObject.AddComponent<CastingAnimations>();
        }
    }
}
