using System.Collections.Generic;
using UnityEngine;
using SeaTruckArms.API;
using BZCommon;
using SeaTruckArms.API.Interfaces;

namespace SeaTruckArms
{
    internal class ArmRegistrationListener : MonoBehaviour
    {
        private void Awake()
        {
            BZLogger.Log("API message: Arm registration listener started.");
        }

        private void Update()
        {
            if (uGUI.isInitialized && !uGUI.main.loading.IsLoading)
            {
                enabled = false;
            }

            if (ArmServices.main.isWaitForRegistration)
            {
                RegisterNewArms();
            }
        }

        private void RegisterNewArms()
        {
            foreach (KeyValuePair<CraftableSeaTruckArm, ISeaTruckArmHandlerRequest> kvp in ArmServices.main.waitForRegistration)
            {
                if (Main.graphics.ArmTechTypes.ContainsKey(kvp.Key.TechType))
                {
                    continue;
                }

                Main.graphics.RegisterNewArm(kvp.Key, kvp.Value);
            }            

            ArmServices.main.isWaitForRegistration = false;
        }
    }
}
