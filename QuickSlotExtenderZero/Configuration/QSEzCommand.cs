using UnityEngine;
using BZCommon;

namespace QuickSlotExtenderZero.Configuration
{
    public class QSEzCommand : MonoBehaviour
    {
        public QSEzCommand Instance;

        private const string Message = "Information: Enter 'qsezconfig' command in main menu for configuration window.";
                
        public void Awake()
        {                
            DevConsole.RegisterConsoleCommand(this, "qsezconfig", false, false);

            BZLogger.Log("QuickSlotExtenderZero", Message);
        }
        
        private void OnConsoleCommand_qsezconfig(NotificationCenter.Notification n)
        {
            new QSEzConfigUI();
        }

        public QSEzCommand()
        {
            if (Instance == null)
            {
                Instance = FindObjectOfType(typeof(QSEzCommand)) as QSEzCommand;

                if (Instance == null)
                {
                    GameObject qsez_command = new GameObject("QSEzCommand");
                    Instance = qsez_command.AddComponent<QSEzCommand>();
                }
            }
            else
            {
                Instance.Awake();
            }                       
        }
    }
}
