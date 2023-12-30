using BZHelper;
using UnityEngine;

namespace RuntimeHelperZero.Command
{
    public class RHZCommand : MonoBehaviour
    {
        public static RHZCommand Instance;

        public void Awake()
        {                       
            DevConsole.RegisterConsoleCommand(this, "rhzero", false, false);
            
        }        

        public void OnConsoleCommand_rhzero(NotificationCenter.Notification n)
        {
            RuntimeHelperZero rhz = new RuntimeHelperZero();
        }

        
        public RHZCommand()
        {
            if (!Instance)
            {
                Instance = FindObjectOfType(typeof(RHZCommand)) as RHZCommand;

                if (!Instance)
                {
                    GameObject rhz_command_go = new GameObject("rhz_command_go");
                    Instance = rhz_command_go.AddComponent<RHZCommand>();
                    rhz_command_go.AddComponent<Indestructible>();
                }
            }            
        }
        
    }
}
