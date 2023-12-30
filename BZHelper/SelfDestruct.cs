using UnityEngine;

namespace BZHelper
{
#pragma warning disable CS1591 // Missing XML documentation

    public class SelfDestruct : MonoBehaviour
    {
        private bool isRoot = false;

        private void Start()
        {
            isRoot = transform.parent == null ? true : false;
        }

        private void Update()
        {
            if (isRoot && Vector3.Distance(transform.position, Vector3.zero) > 8000)
            {
                Destroy(gameObject);
            }
        }
    }
}
