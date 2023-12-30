using UnityEngine;

namespace BZHelper
{
#pragma warning disable CS1591 // Missing XML documentation

    public class Indestructible : MonoBehaviour
    {
        public void Awake()
        {
            SceneCleanerPreserve scp = gameObject.AddComponent<SceneCleanerPreserve>();
            scp.enabled = true;
            DontDestroyOnLoad(gameObject);
        }
    }
}
