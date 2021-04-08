using UnityEngine;
using SeaTruckArms.API;
using SeaTruckArms.API.ArmHandlers;
using SeaTruckArms.API.Interfaces;

namespace SeaTruckArms.InternalArmHandlers
{
    internal class InternalPropulsionArmHandler : PropulsionArmHandler, ISeaTruckArmHandler
    {
        GameObject ISeaTruckArmHandler.GetGameObject()
        {
            return gameObject;
        }

        void ISeaTruckArmHandler.SetSide(SeaTruckArm arm)
        {
            if (arm == SeaTruckArm.Right)
            {
                transform.localScale = new Vector3(-0.8f, 0.8f, 0.8f);
                propulsionCannon.localObjectOffset = new Vector3(0.75f, 0f, 0f);
            }
            else
            {
                transform.localScale = new Vector3(0.8f, 0.8f, 0.8f);
                propulsionCannon.localObjectOffset = new Vector3(-0.75f, 0f, 0f);
            }
        }

        bool ISeaTruckArmHandler.OnUseDown(out float cooldownDuration)
        {
            usingTool = true;
            cooldownDuration = 1f;
            return propulsionCannon.OnShoot();
        }

        bool ISeaTruckArmHandler.OnUseHeld(out float cooldownDuration)
        {
            cooldownDuration = 0f;
            return false;
        }

        bool ISeaTruckArmHandler.OnUseUp(out float cooldownDuration)
        {
            usingTool = false;
            cooldownDuration = 0f;
            return true;
        }

        bool ISeaTruckArmHandler.OnAltDown()
        {
            if (propulsionCannon.IsGrabbingObject())
            {
                propulsionCannon.ReleaseGrabbedObject();
            }

            return true;
        }

        void ISeaTruckArmHandler.Update(ref Quaternion aimDirection)
        {
            propulsionCannon.usingCannon = usingTool;
            propulsionCannon.UpdateActive();
        }

        void ISeaTruckArmHandler.Reset()
        {
            propulsionCannon.usingCannon = (usingTool = false);
            propulsionCannon.ReleaseGrabbedObject();
        }

        bool ISeaTruckArmHandler.HasClaw()
        {
            return false;
        }

        bool ISeaTruckArmHandler.HasDrill()
        {
            return false;
        }

        void ISeaTruckArmHandler.SetRotation(SeaTruckArm arm, bool isDocked)
        {
            if (isDocked)
            {
                BaseRoot baseRoot = TruckHelper.MainCab.GetComponentInParent<BaseRoot>();

                if (baseRoot.isBase)
                {
                    if (arm == SeaTruckArm.Right)
                    {
                        transform.localRotation = Quaternion.Euler(20, 6, 0);
                    }
                    else
                    {
                        transform.localRotation = Quaternion.Euler(20, 354, 0);
                    }
                }
            }
            else
            {
                if (arm == SeaTruckArm.Right)
                {
                    transform.localRotation = Quaternion.Euler(0, 0, 0);
                }
                else
                {
                    transform.localRotation = Quaternion.Euler(0, 0, 0);
                }
            }
        }

        float ISeaTruckArmHandler.GetEnergyCost()
        {
            return 0;
        }
    }
}
