using UnityEngine;

namespace CheatManagerZero.NewCommands
{
    public class ResistColdConsoleCommand : MonoBehaviour
    {
        public static ResistColdConsoleCommand main;
        private bool ResistCold;

        public void Awake()
        {
            main = this;
            DontDestroyOnLoad(this);
            DevConsole.RegisterConsoleCommand(this, "resistcold", false, false);
        }

        private void OnConsoleCommand_resistcold(NotificationCenter.Notification n)
        {
            ResistCold = !ResistCold;
            ToggleResistColdCheat();
            ErrorMessage.AddMessage($"resistcold cheat is now {ResistCold}");
        }

        public bool GetResistColdCheat()
        {
            return ResistCold;
        }

        public void SetResistColdCheat(bool value)
        {
            ResistCold = value;
            var component = Player.main.GetComponent<BodyTemperature>();
            component.enabled = !ResistCold;        
        }

        public void ToggleResistColdCheat()
        {
            var component = Player.main.GetComponent<BodyTemperature>();

            if (ResistCold)
            {
                component.enabled = false;
            }
            else
            {
                component.enabled = true;
            }
        }
    }
}
