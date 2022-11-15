using Fusion;

namespace GDT.Data
{
    public enum InputButton
    {
        Left = 1,
        Right = 2,
        Jump = 3,
        Shoot = 4,
        StandardArrow = 5,
        IceArrow = 6,
        InvertedArrow = 7,
        Ready = 8
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