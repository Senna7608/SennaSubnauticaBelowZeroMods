using UnityEngine;
using UWE;

namespace CheatManagerZero
{
    public class SeaTruckOverDrive : MonoBehaviour
    {
        public SeaTruckOverDrive Instance { get; private set; }
        public SeaTruckMotor thisMotor;        
        private readonly float[] SpeedModuleBoost = { 15f, 30f, 45f };
        private const float MaxAcceleration = 50f;

        public void Awake()
        {
            Instance = this;
            thisMotor = Instance.GetComponent<SeaTruckMotor>();
        }        

        public void Start()
        {            
            //Main.Instance.isSeamothCanFly.changedEvent.AddHandler(this, new Event<Utils.MonitoredValue<bool>>.HandleFunction(OnSeamothCanFlyChanged));
            Main.Instance.onSeatruckSpeedValueChanged.AddHandler(this, new Event<object>.HandleFunction(OnSeaTruckSpeedValueChanged));
            Main.Instance.onPlayerMotorModeChanged.AddHandler(this, new Event<Player.MotorMode>.HandleFunction(OnPlayerMotorModeChanged));
        }

        private void OnSeaTruckSpeedValueChanged(object newValue)
        {
            Main.Instance.seatruckSpeedMultiplier = (float)newValue;
            SetSeaTruckOverDrive((float)newValue);
        }                

        public void OnDestroy()
        {            
            Main.Instance.onSeatruckSpeedValueChanged.RemoveHandler(this, OnSeaTruckSpeedValueChanged);
            Main.Instance.onPlayerMotorModeChanged.RemoveHandler(this, OnPlayerMotorModeChanged);
            Destroy(Instance);
        }

        private void OnPlayerMotorModeChanged(Player.MotorMode newMotorMode)
        {
            if (newMotorMode == Player.MotorMode.Vehicle)
            {
                if (Player.main.GetComponentInParent<SeaTruckMotor>() == thisMotor)
                {
                    SetSeaTruckOverDrive(Main.Instance.seatruckSpeedMultiplier);
                }
            }
        }
                

        internal void SetSeaTruckOverDrive(float multiplier)
        {
            float boost = SpeedModuleBoost[0];

            if (multiplier == 1)
            {
                thisMotor.acceleration = 15f + boost;                
            }
            else
            {
                float overDrive = MaxAcceleration * (((float)SpeedModuleBoost[0] + 10) / 10);
                thisMotor.acceleration = CalcForce(17.5f, boost, overDrive, multiplier);                
            }
        }

        private float CalcForce(float defaultForce, float boost, float overDrive, float multiplier)
        {
            return defaultForce + boost + (multiplier * ((overDrive - (defaultForce + boost)) / 5));
        }
    }
}
