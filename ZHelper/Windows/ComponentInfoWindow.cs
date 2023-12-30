using BZCommon.Helpers.RuntimeGUI;
using System.Collections.Generic;
using UnityEngine;
using static Zhelper.Objects.FieldHelper;
using static Zhelper.Objects.PropertyHelper;

namespace ZHelper
{
    public partial class ZHelper // Component Info Window
    {
        private List<string> objectInfoText = new List<string>();
                
        //private bool showSetButton = false;
        private UnityEngine.Object selected;
        private ObjectProperties objectProperties;
        private ObjectFields objectFields;

        private Rect compInfoWindowRect = new Rect(604, 30, 600, 400);        

        private List<GUI_content> compInfoScrollContents = new List<GUI_content>();         

        void CompInfoWindow_Awake()
        {
            GuiBase.gUIEventControl += ComponentInfoWindow_EventControl;            
        }

        void CompInfoWindow_Start(UnityEngine.Object _object)
        {
            selected = _object;

            //showSetButton = false;

            objectInfoText.Clear();                

            objectProperties = new ObjectProperties(_object);

            objectInfoText.Add("#Green#Properties:");

            foreach (ObjectProperty objectProperty in objectProperties)
            {
                objectInfoText.Add(objectProperty.ToString());
            }

            objectFields = new ObjectFields(_object);

            if (objectFields.GetFieldCount() > 0)
            {
                objectInfoText.Add("#Green#Fields:");

                foreach (ObjectField objectField in objectFields)
                {
                    objectInfoText.Add(objectField.ToString());
                }
            }

            FillGroupContents(ref objectInfoText, ref compInfoScrollContents);

            GuiBase.RefreshGroup(4, 6);

        }

        private void ComponentInfoWindow_EventControl(GUI_event guiEvent)
        {
            
        }
    }
}
