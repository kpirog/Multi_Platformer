using Fusion;
using System;

namespace GDT.Data
{
    [Flags]
    public enum InputButton
    {
        Left = 1 << 0,
        Right = 1 << 1,
        Jump = 1 << 2,
        Shoot = 1 << 3,
        StandardArrow = 1 << 4,
        IceArrow = 1 << 5,
        Ready = 1 << 6
    }

    public struct NetworkInputData : INetworkInput
    {
        public NetworkButtons Buttons;
        public float ShootingAngle;

        public bool GetButton(InputButton button) 
        {
            return Buttons.IsSet(button);
        }
        
        public bool AxisPressed()
        {
            return GetButton(InputButton.Left) || GetButton(InputButton.Right);
        }
    }
}