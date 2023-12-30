using System.Collections;
using UnityEngine;
using SeaTruckArms.InternalArmHandlers;
using ModdedArmsHelperBZ.API;
using ModdedArmsHelperBZ.API.Interfaces;

namespace SeaTruckArms.ArmHandlerRequesters
{
    internal class PropulsionArmModdingRequest : IArmModdingRequest
    {
        public IExosuitModdedArm GetExosuitArmHandler(GameObject clonedArm)
        {
            return null;
        }

        public ISeatruckArm GetSeatruckArmHandler(GameObject clonedArm)
        {
            return clonedArm.AddComponent<InternalPropulsionArmHandler>();
        }

        public IEnumerator SetUpArmAsync(GameObject clonedArm, LowerArmHelper graphicsHelper, IOut<bool> success)
        {
            success.Set(true);
            yield break;
        }
    }
}
