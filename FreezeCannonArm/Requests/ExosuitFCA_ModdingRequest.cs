using System.Collections;
using UnityEngine;
using ModdedArmsHelperBZ.API;
using ModdedArmsHelperBZ.API.Interfaces;
using BZHelper;

namespace FreezeCannonArm.Handlers
{
    internal class ExosuitFCA_ModdingRequest : IArmModdingRequest
    {
        public IExosuitModdedArm GetExosuitArmHandler(GameObject clonedArm)
        {
            return clonedArm.AddComponent<ExosuitFCA_ArmHandler>();
        }

        public ISeatruckArm GetSeatruckArmHandler(GameObject clonedArm)
        {
            return null;
        }

        public IEnumerator SetUpArmAsync(GameObject clonedArm, LowerArmHelper graphicsHelper, IOut<bool> success)
        {
            GameObject ArmRig = UnityHelper.FindDeepChild(clonedArm, "ArmRig");
            GameObject model = ArmRig.FindChild("ArmModel");

            GraphicsHelper.ChangeObjectTexture(model, 2, illumTex: Main.illumTex);

            success.Set(true);
            yield break;
        }
    }
}
