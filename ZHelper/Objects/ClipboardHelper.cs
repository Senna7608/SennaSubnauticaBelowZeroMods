using BZCommon;
using BZHelper;
using UnityEngine;
using ZHelper.VisualHelpers;

namespace ZHelper.Objects
{
    public static class ClipboardHelper
    {
        public static int pasteCount = 0;

        public static void CopyObject(this GameObject gameObject, out GameObject tempObject)
        {
            gameObject.SetActive(false);

            tempObject = UnityEngine.Object.Instantiate(gameObject, null, false);

            tempObject.SetActive(false);

            if (tempObject.TryGetComponentInChildren(out DrawObjectBounds dob))
            {
                dob.transform.parent.gameObject.SetActive(false);
                UnityEngine.Object.Destroy(dob.transform.parent.gameObject);
            }           

            gameObject.SetActive(true);
        }

        public static GameObject PasteObject(this GameObject tempObject, Transform parent)
        {
            GameObject newObject = UnityEngine.Object.Instantiate(tempObject, parent, false);
            newObject.name = $"newPastedObject_{pasteCount}";
           
            newObject.transform.localPosition = new Vector3(0, tempObject.transform.localPosition.y, 0);
            newObject.transform.localRotation = tempObject.transform.localRotation;

            newObject.SetActive(true);

            UnityEngine.Object.Destroy(tempObject);

            pasteCount++;

            return newObject;
        }

    }
}