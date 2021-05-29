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
            Slow
        }

        public static Dictionary<CastSpeed, string> SpeedAnimations = new Dictionary<CastSpeed, string>
        {
            { CastSpeed.Instant, "bow_fire" },
            { CastSpeed.Fast, "Standing 1H Magic Attack 01" }, //1H, directional; also note CastFireball for longer 1H directional
            { CastSpeed.Medium, "Standing 1H Magic Attack 03" }, //1H, vertical; also note Standing 1H Cast Spell 01 for less dramatic 1H vertical
            { CastSpeed.Slow, "Standing 2H Magic Attack 04" } //2H, directional, long; CastFireball similar length
        };

        public static Dictionary<CastSpeed, float> FrameRateAdjustments = new Dictionary<CastSpeed, float>
        {
            { CastSpeed.Instant, 1 },
            { CastSpeed.Fast, 1 },
            { CastSpeed.Medium, 4 },
            { CastSpeed.Slow, 1 }
        };

        public const string overrideAnimName = "Cheer";
        public const string overrideAnimTrigger = "emote_cheer";
        public const string overrideAnimTriggerStop = "emote_stop";

        public System.Action OnComplete { get; set; }

        public void Play(CastSpeed speed)
        {
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
            Invoke("FinishAnim", animation.length);
            
            // Reset the animator to original settings (perhaps after a delay)
            //animator.runtimeAnimatorController = originalController;
            //overrideController[overrideAnimName] = originalAnimation;
            //GameObject.Destroy(overrideController);
        }

        public void FinishAnim()
        {
            OnComplete.Invoke();
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
