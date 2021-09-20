using BZCommon;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UWE;

namespace SeaTruckFlyModule
{
    public partial class FlyManager //AsyncLoader
    {
        private GameObject robotArmResource;

        private IEnumerator LoadBiodomeRobotArmResourcesAsync()
        {
            IPrefabRequest request = PrefabDatabase.GetPrefabForFilenameAsync("WorldEntities/Alterra/Base/biodome_Robot_Arm.prefab");

            yield return request;

            if (!request.TryGetPrefab(out robotArmResource))
            {
                BZLogger.Warn("Cannot load [biodome_robot_arm] prefab!");
                yield break;
            }

            BZLogger.Debug("Resource loaded: [biodome_robot_arm]");

            Utils.ZeroTransform(robotArmResource.transform);

            yield break;
        }

    }
}
