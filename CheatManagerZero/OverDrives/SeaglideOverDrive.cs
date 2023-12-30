using BZHelper;
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

        private void OnPlayerMotorModeChanged(Player.MotorMode newMotorMode)
        {
            UnderwaterMotor underwaterMotor = Player.main.GetComponent<UnderwaterMotor>();

            if (Main.Instance.isSeaglideFast.value && newMotorMode == Player.MotorMode.Seaglide)
            {
                underwaterMotor.playerSpeedModifier.SetValue(3f);
                BZLogger.UnityLog($"playerSpeedModifier: motormode: {newMotorMode} value: {underwaterMotor.playerSpeedModifier.Value}");                
            }
            else
            {
                underwaterMotor.playerSpeedModifier.SetValue(1f);
                BZLogger.UnityLog($"playerSpeedModifier: motormode: {newMotorMode} value: {underwaterMotor.playerSpeedModifier.Value}");                
            }
        }

        private void IsSeaglideFast(Utils.MonitoredValue<bool> SeaglideFastSpeedEnable)
        {
            OnPlayerMotorModeChanged(Player.main.motorMode);
        }
    }
}
