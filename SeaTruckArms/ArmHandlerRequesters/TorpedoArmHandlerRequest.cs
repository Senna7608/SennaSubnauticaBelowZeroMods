using UnityEngine;
using SeaTruckArms.API.Interfaces;
using SeaTruckArms.InternalArmHandlers;

namespace SeaTruckArms.ArmHandlerRequesters
{
    internal class TorpedoArmHandlerRequest : ISeaTruckArmHandlerRequest
    {
        public ISeaTruckArmHandler GetHandler(ref GameObject arm)
        {
            return arm.AddComponent<InternalTorpedoArmHandler>();
        }
    }
}
