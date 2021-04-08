using UnityEngine;
using BZCommon;

namespace CheatManagerZero.Configuration
{
    public class CmZConfigCommand : MonoBehaviour
    {
        CmZConfigCommand Instance;

        public void Awake()
        {            
            DevConsole.RegisterConsoleCommand(this, "cmzconfig", false, false);
            BZLogger.Log($"[{CmZConfig.PROGRAM_NAME}] Information: Enter 'cmzconfig' command for configuration window.");
        }

        private void OnConsoleCommand_cmzconfig(NotificationCenter.Notification n)
        {
            new CmZConfigUI();
        }

        public CmZConfigCommand()
        {
            if (Instance == null)
            {
                Instance = FindObjectOfType(typeof(CmZConfigCommand)) as CmZConfigCommand;

                if (Instance == null)
                {
                    GameObject cmzconfig_command = new GameObject("cmzconfig_command");
                    Instance = cmzconfig_command.AddComponent<CmZConfigCommand>();
                }
            }
            else
            {
                Instance.Awake();
            }            
        }
    }
}
