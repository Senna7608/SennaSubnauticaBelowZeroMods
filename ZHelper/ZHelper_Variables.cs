using System.Collections.Generic;
using UnityEngine;

namespace ZHelper
{
    public partial class ZHelper // Variables
    {
        private Rect mainWindowRect = new Rect(0, 30, 300, 700);        

        public List<GameObject> GameObject_Blacklist = new List<GameObject>();

        private List<Transform> TRANSFORMS = new List<Transform>();
        
        private Vector2 scrollpos_transforms = Vector2.zero;
        
        //private int current_transform_index = 0;

        //private int addedChildObjectCount = 0;

        private GameObject baseObject, selectedObject, tempObject;

        private string OBJECTINFO = string.Empty;
        private string COLLIDERINFO = string.Empty;

        //private Vector3 lPos, lScale, lRot;
        //private Vector2 rtPos, rtSize, rtPivot;
        public bool isDirty = false;

        private string sizeText = string.Empty;
        //private bool showCollider = true;

        //private bool isRootList = false;

        //private bool showLocal = true;

        private GameObject RHZ_VISUAL_BASE;
    }
}
