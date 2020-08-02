using UnityEngine;
using UWE;

namespace CheatManagerZero
{
    public class SeaglideOverDrive : MonoBehaviour
    {
        public SeaglideOverDrive Instance { get; private set; }       
              
        public void Awake()
        {
            if (Instance == null)
                Instance = this;
            else
                Destroy(this);
        }
        
        public void Start()
        {
            Main.Instance.isSeaglideFast.changedEvent.AddHandler(this, new Event<Utils.MonitoredValue<bool>>.HandleFunction(IsSeaglideFast));
            Main.Instance.onPlayerMotorModeChanged.AddHandler(this, new Event<Player.MotorMode>.HandleFunction(OnPlayerMotorModeChanged));            
        }

        public void OnDestroy()
        {
            Main.Instance.isSeaglideFast.changedEvent.RemoveHandler(this, IsSeaglideFast);
            Main.Instance.onPlayerMotorModeChanged.RemoveHandler(this, OnPlayerMotorModeChanged);
            Destroy(this);
        }

        private void IsSeaglideFast(Utils.MonitoredValue<bool> SeaglideFastSpeedEnable)
        {
            SetSeaglideSpeed(Player.main.motorMode);
        }

        private void OnPlayerMotorModeChanged(Player.MotorMode playerMotorMode)
        {            
            SetSeaglideSpeed(playerMotorMode);            
        }
        

        private void SetSeaglideSpeed(Player.MotorMode activeMotorMode)
        {         
            if (Main.Instance.isSeaglideFast.value && activeMotorMode == Player.MotorMode.Seaglide && Player.main.IsUnderwater())
            {
                Player.main.playerController.underWaterController.acceleration = 60f;
                Player.main.playerController.underWaterController.verticalMaxSpeed = 75f;
            }
            else
            {
                Player.main.playerController.underWaterController.acceleration = 20;
                Player.main.playerController.underWaterController.verticalMaxSpeed = 5f;
            }
        }        
    }
}
