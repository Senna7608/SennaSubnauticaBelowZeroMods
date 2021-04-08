/*
namespace SlotExtenderZero.Configuration
{
    public class SEzCommand : ConsoleCommand
    {
        public override bool AvailableInStartScreen => true;
        public override bool AvailableInGame => false;

        public void Awake()
        {
            BZLogger.Log("SlotExtenderZero", "Information: Enter 'sezconfig' command in main menu for configuration window.");
        }

        public override void RegisterCommand()
        {
            DevConsole.RegisterConsoleCommand(this, "sezconfig", false, false);
        }

        public void OnConsoleCommand_sezconfig(NotificationCenter.Notification n)
        {
            SEzConfigUI configUI = new SEzConfigUI();
        }
    }
}
*/