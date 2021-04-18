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

        public override void DoMagicAttack(Attack baseAttack)
        {
            var player = baseAttack.GetCharacter();
            var lookDir = (Quaternion)typeof(Character).GetField("m_lookYaw", BindingFlags.NonPublic | BindingFlags.Instance).GetValue(player);
            Debug.Log($"Player facing: {lookDir.eulerAngles}");
            CancellationTokenSource.Cancel();
            EnvMan.instance.ResetDebugWind();
            EnvMan.instance.SetDebugWind(lookDir.eulerAngles.y, 0.65f);
            Task.Run(() => ResetWind(CancellationTokenSource.Token), CancellationTokenSource.Token);
        }

        public static async void ResetWind(CancellationToken cancellationToken)
        {
            await Task.Delay(180000, cancellationToken); //3 minutes before the wind resets; 1 stack of these will give 15 min
            if (cancellationToken.IsCancellationRequested)
                return;
            EnvMan.instance.ResetDebugWind();
        }
    }
}
