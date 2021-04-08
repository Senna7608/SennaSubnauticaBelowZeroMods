using UnityEngine;

namespace SeaTruckArms.API.Interfaces
{
    public interface ISeaTruckArmHandlerRequest
    {
        ISeaTruckArmHandler GetHandler(ref GameObject arm);
    }
}
