using UnityEngine;
using SeaTruckArms.API.Interfaces;
using SeaTruckArmAPITest.Handlers;

namespace SeaTruckArmAPITest.Requesters
{
    internal class ClawArmHandlerRequest_APITEST : ISeaTruckArmHandlerRequest
    {
        public ISeaTruckArmHandler GetHandler(ref GameObject arm)
        {            
            return arm.AddComponent<ClawArmHandler_APITEST>();
        }
    }
}
