using UnityEngine;
using SeaTruckArms.API.Interfaces;
using SeaTruckArms.InternalArmHandlers;

namespace SeaTruckArms.ArmHandlerRequesters
{
    internal class PropulsionArmHandlerRequest : ISeaTruckArmHandlerRequest
    {
        public ISeaTruckArmHandler GetHandler(ref GameObject arm)
        {
            return arm.AddComponent<InternalPropulsionArmHandler>();
        }
    }
}
