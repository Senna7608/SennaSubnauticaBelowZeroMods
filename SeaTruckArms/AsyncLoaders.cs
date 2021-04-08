using BZCommon;
using System.Collections;
using UnityEngine;
using UWE;

namespace SeaTruckArms
{
    internal partial class SeaTruckArms_Graphics //AsyncLoaders
    {
        private IEnumerator LoadPipeResourcesAsync()
        {
            IPrefabRequest request = PrefabDatabase.GetPrefabForFilenameAsync("WorldEntities/Tools/Pipe.prefab");

            yield return request;

            if (!request.TryGetPrefab(out GameObject prefab))
            {
                BZLogger.Warn("API message: Cannot load [Pipe] prefab!");
                yield break;
            }

            pipeResource = UWE.Utils.InstantiateDeactivated(prefab, GraphicsRoot.transform, Vector3.zero, Quaternion.identity);            

            pipeResource.GetComponent<Rigidbody>().isKinematic = true;

            pipeResource.GetComponent<WorldForces>().enabled = false;

            Utils.ZeroTransform(pipeResource.transform);

            yield break;
        }

        private IEnumerator LoadExosuitResourcesAsync()
        {
            IPrefabRequest request = PrefabDatabase.GetPrefabForFilenameAsync("WorldEntities/Tools/Exosuit.prefab");

            yield return request;

            if (!request.TryGetPrefab(out GameObject prefab))
            {
                BZLogger.Warn("API message: Cannot load [Exosuit] prefab!");
                yield break;
            }

            exosuitResource = UWE.Utils.InstantiateDeactivated(prefab, GraphicsRoot.transform, Vector3.zero, Quaternion.identity);

            exosuitResource.GetComponent<Exosuit>().enabled = false;
            exosuitResource.GetComponent<Rigidbody>().isKinematic = true;
            exosuitResource.GetComponent<WorldForces>().enabled = false;
            UWE.Utils.ZeroTransform(exosuitResource.transform);

            yield break;
        }
    }
}
