using System;
using System.Collections.Generic;
using System.Text;

namespace Runestones
{
    public abstract class RuneEffect
    {
        public RuneQuality _Quality;
        public float _Effectiveness;
        public string _FlavorText = "";
        public List<string> _EffectText = new List<string>();
        public Dictionary<RuneQuality, List<string>> _QualityEffectText = new Dictionary<RuneQuality, List<string>>();
        public Dictionary<string, Func<string>> _RelativeStats;
        public virtual string GetDescription()
        {
            StringBuilder builder = new StringBuilder();
            builder.Append($"<color=grey><i>{_FlavorText}</i></color>");
            foreach (string effect in _EffectText)
            {
                builder.Append("\n"+effect);
            }
            if (_Quality == RuneQuality.Common)
            {
                if (_QualityEffectText.TryGetValue(RuneQuality.Common, out List<string> effectList))
                {
                    foreach (string effect in effectList)
                    {
                        builder.Append("\n" + effect);
                    }
                }
            }
            else
            {
                builder.Append("<size=6>\n</size>");
                var color = _Quality == RuneQuality.Ancient ? "green" : (_Quality == RuneQuality.Dark ? "purple" : "white" );
                builder.Append($"\n<color={color}><b>{Enum.GetName(typeof(RuneQuality), _Quality)}:</b></color>");
                if (_QualityEffectText.TryGetValue(_Quality, out List<string> effectList))
                {
                    foreach (string effect in effectList)
                    {
                        builder.Append("\n" + effect);
                    }
                }
            }
            builder.Append("<size=6>\n</size>");
            if (_RelativeStats != null && _RelativeStats.Count > 0)
            {
                foreach (KeyValuePair<string, Func<string>> statItem in _RelativeStats)
                {
                    builder.Append($"\n<color=orange><b>{statItem.Key}: {statItem.Value()}</b></color>");
                }
            }
            else
            {
                builder.Append("\n<color=grey>Spell effectiveness does not apply to this rune.</color>");
            }
            return builder.ToString();
        }
        public abstract void DoMagicAttack(Attack baseAttack);
    }
}
