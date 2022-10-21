using Fusion;
using UnityEngine;
using System;

namespace GDT.Data
{
    [Flags]
    public enum InputButton
    {
        Left = 1 << 0,
        Right = 1 << 1,
        Jump = 1 << 2
    }

    public struct NetworkInputData : INetworkInput
    {
        public NetworkButtons Buttons;
        public NetworkBool JumpButtonPressed;
        
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