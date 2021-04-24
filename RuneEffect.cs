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
        public Dictionary<string, Func<string>> _RelativeStats;
        public virtual string GetDescription()
        {
            StringBuilder builder = new StringBuilder();
            builder.Append($"<color=grey><i>{_FlavorText}</i></color>");
            foreach (string effect in _EffectText)
            {
                builder.Append("\n"+effect);
            }
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
