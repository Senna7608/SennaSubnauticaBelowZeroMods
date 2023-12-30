using System.Collections;
using UnityEngine;
using ModdedArmsHelperBZ.API;
using ModdedArmsHelperBZ.API.Interfaces;
using BZHelper;
using FreezeCannonArm.Handlers;

namespace FreezeCannonArm.Requests
{
    internal class SeatruckFCA_ModdingRequest : IArmModdingRequest
    {
        public IExosuitModdedArm GetExosuitArmHandler(GameObject clonedArm)
        {
            return null;
        }

        public ISeatruckArm GetSeatruckArmHandler(GameObject clonedArm)
        {
            return clonedArm.AddComponent<SeatruckFCA_ArmHandler>();
        }

        public IEnumerator SetUpArmAsync(GameObject clonedArm, LowerArmHelper graphicsHelper, IOut<bool> success)
        {
            GameObject ArmRig = UnityHelper.FindDeepChild(clonedArm, "ArmRig");
            GameObject exosuit_repulsion_geo = ArmRig.FindChild("ArmModel");

            GraphicsHelper.ChangeObjectTexture(exosuit_repulsion_geo, 2, illumTex: Main.illumTex);

            success.Set(true);
            yield break;
        }
    }
}
