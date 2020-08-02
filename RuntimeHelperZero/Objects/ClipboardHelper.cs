using RuntimeHelperZero.VisualHelpers;
using System;
using UnityEngine;

namespace RuntimeHelperZero.Objects
{
    public static class ClipboardHelper
    {
        public static int pasteCount = 0;

        public static void CopyObject(this GameObject gameObject, out GameObject tempObject)
        {
            gameObject.SetActive(false);

            tempObject = UnityEngine.Object.Instantiate(gameObject, null, false);

            tempObject.SetActive(false);

            var dob = tempObject.GetComponentInChildren<DrawObjectBounds>(true);
            dob.gameObject.SetActive(false);

            UnityEngine.Object.Destroy(dob.gameObject);

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
