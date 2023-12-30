using System.Collections.Generic;
using UnityEngine;
using ZHelper.Helpers;

namespace ZHelper.VisualHelpers
{
    public class DrawObjectBounds : MonoBehaviour
    {    
        public IDictionary<int, TransformInfo> transformOriginals = new Dictionary<int, TransformInfo>();

        private GameObject base_2D, base_3D, pointer_base;
        private List<GameObject> lineContainers_2D = new List<GameObject>();
        private List<GameObject> lineContainers_3D = new List<GameObject>();

        public static float LineWidth = 0.004f;       

        private void Awake()
        {
            base_2D = new GameObject("base_2D");
            base_2D.transform.SetParent(transform);
            Utils.ZeroTransform(base_2D.transform);           

            base_3D = new GameObject("base_3D");
            base_3D.transform.SetParent(transform);
            Utils.ZeroTransform(base_3D.transform);                  

            base_2D.CreateLineContainers(ref lineContainers_2D, ContainerType.Rectangle, 0.008f, Color.green, false);           

            base_3D.CreateLineContainers(ref lineContainers_3D, ContainerType.Box, 0.008f, Color.green, false);
            
            pointer_base = new GameObject("pointer_base");
            pointer_base.transform.SetParent(transform);
            Utils.ZeroTransform(pointer_base.transform);            

            pointer_base.AddLineRendererToContainer(LineWidth, Color.green, true);
            pointer_base.EnsureComponent<TracePlayerPos>().PointerType = PointerType.Object;           
        }

        public bool GetTransformInfo(int tInstanceID, out TransformInfo transformInfo)
        {
            return transformOriginals.TryGetValue(tInstanceID, out transformInfo);
        }

        public void IsDraw(bool value)
        {
            int tInstanceID = transform.parent.parent.GetInstanceID();

            if (!transformOriginals.ContainsKey(tInstanceID))
            {
                transformOriginals.Add(tInstanceID, new TransformInfo(transform.parent.parent));
            }

            if (transform.parent.parent.GetType().Equals(typeof(RectTransform)))
            {
                base_2D.SetActive(value);
                base_3D.SetActive(!value);
                lineContainers_2D.DrawRectangle((RectTransform)transform.parent.parent);                
            }
            else
            {
                base_2D.SetActive(!value);
                base_3D.SetActive(value);

                Vector3 newSize = LineHelper.CompensateSizefromScale(new Vector3(0.6f, 0.6f, 0.6f), transform.parent.parent.localScale);
                lineContainers_3D.DrawBox(Vector3.zero, newSize);                
            }            
        }
    }
}