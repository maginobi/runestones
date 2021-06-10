using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Runestones.RuneEffects
{
    public class HealRuneEffect : RuneEffect
    {
        const string projectileName = "shaman_heal_aoe";
        const float baseHeal = 10;
        public HealRuneEffect()
        {
            _FlavorText = "True strength is helping others";
            _EffectText = new List<string> { "Heals allies", "5m radius" };
            _QualityEffectText[RuneQuality.Ancient] = new List<string> { "+100% Heal" };
            _QualityEffectText[RuneQuality.Dark] = new List<string> { "+200% Heal", "Spell effectiveness twice as effective" };
            _RelativeStats = new Dictionary<string, Func<string>> { { "Heal", () => $"{baseHeal * _Effectiveness * ((int)_Quality+1) * (_Quality==RuneQuality.Dark ? _Effectiveness : 1) :F1}" } };
            targetLock = true;
            speed = CastingAnimations.CastSpeed.Medium;
        }
        public override void DoMagicAttack(Attack baseAttack)
        {
            var aoePrefab = GameObject.Instantiate((from GameObject prefab in Resources.FindObjectsOfTypeAll<GameObject>() where prefab.name == projectileName select prefab).FirstOrDefault());
            var aoe = aoePrefab.GetComponent<Aoe>();
            aoe.m_hitEnemy = false;
            aoe.m_hitCharacters = true;
            aoe.m_hitProps = false;
            aoe.m_hitOwner = true;
            aoe.m_hitFriendly = true;
            aoe.m_damage.m_damage = -baseHeal * _Effectiveness * ((int)_Quality + 1) * (_Quality == RuneQuality.Dark ? _Effectiveness : 1);
            aoe.Setup(baseAttack.GetCharacter(), Vector3.zero, 0, null, null);

            var go = GameObject.Instantiate(aoePrefab, targetLocation, Quaternion.identity);
        }
    }
}
