using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using UnityEngine;

namespace Runestones.RuneEffects
{
    class WeatherRuneEffect : RuneEffect
    {
        static CancellationTokenSource _cancellationTokenSource = null;
        static CancellationTokenSource CancellationTokenSource
        {
            get
            {
                if (_cancellationTokenSource == null)
                    _cancellationTokenSource = new CancellationTokenSource();
                return _cancellationTokenSource;
            }
        }

        public const float baseDuration = 180;

        public WeatherRuneEffect()
        {
            _FlavorText = "So, you want to harness lightning. Best start with a stiff breeze";
            _EffectText = new List<string> { "Changes wind direction, for a time" };
            _RelativeStats = new Dictionary<string, Func<string>> { { "Duration", () => $"{baseDuration * _Effectiveness :F0} sec" } };
        }

        public override void DoMagicAttack(Attack baseAttack)
        {
            var player = baseAttack.GetCharacter();
            var lookDir = (Quaternion)typeof(Character).GetField("m_lookYaw", BindingFlags.NonPublic | BindingFlags.Instance).GetValue(player);
            Debug.Log($"Player facing: {lookDir.eulerAngles}");
            CancellationTokenSource.Cancel();
            EnvMan.instance.ResetDebugWind();
            EnvMan.instance.SetDebugWind(lookDir.eulerAngles.y, 0.5f);
            Task.Run(() => ResetWind(CancellationTokenSource.Token, (int)(baseDuration * _Effectiveness * 1000)), CancellationTokenSource.Token);
        }

        public static async void ResetWind(CancellationToken cancellationToken, int delay)
        {
            await Task.Delay(delay, cancellationToken); //3 minutes before the wind resets; 1 stack of these will give 15 min
            if (cancellationToken.IsCancellationRequested)
                return;
            EnvMan.instance.ResetDebugWind();
        }
    }
}
