using UnityEngine;
using ModdedArmsHelperBZ.API;
using ModdedArmsHelperBZ.API.Interfaces;
using ModdedArmsHelperBZ.API.ArmHandlers;

namespace SeaTruckArms.InternalArmHandlers
{
    internal class InternalPropulsionArmHandler : PropulsionArmHandler, ISeatruckArm
    {
        public override void Start()
        {
        }

        GameObject ISeatruckArm.GetGameObject()
        {
            return gameObject;
        }

        GameObject ISeatruckArm.GetInteractableRoot(GameObject target)
        {
            if (propulsionCannon.ValidateObject(target))
            {
                return target;
            }
            return null;
        }

        void ISeatruckArm.SetSide(SeatruckArm arm)
        {
            if (arm == SeatruckArm.Right)
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

        bool ISeatruckArm.OnUseDown(out float cooldownDuration)
        {
            usingTool = true;
            cooldownDuration = 1f;
            return propulsionCannon.OnShoot();
        }

        bool ISeatruckArm.OnUseHeld(out float cooldownDuration)
        {
            cooldownDuration = 0f;
            return false;
        }

        bool ISeatruckArm.OnUseUp(out float cooldownDuration)
        {
            usingTool = false;
            cooldownDuration = 0f;
            return true;
        }

        bool ISeatruckArm.OnAltDown()
        {
            if (propulsionCannon.IsGrabbingObject())
            {
                propulsionCannon.ReleaseGrabbedObject();
            }

            return true;
        }

        void ISeatruckArm.Update(ref Quaternion aimDirection)
        {
            propulsionCannon.usingCannon = usingTool;
            propulsionCannon.UpdateActive();
        }

        void ISeatruckArm.ResetArm()
        {
            propulsionCannon.usingCannon = (usingTool = false);
            propulsionCannon.ReleaseGrabbedObject();
        }

        bool ISeatruckArm.HasClaw()
        {
            return false;
        }

        bool ISeatruckArm.HasDrill()
        {
            return false;
        }

        bool ISeatruckArm.HasPropCannon()
        {
            return true;
        }

        float ISeatruckArm.GetEnergyCost()
        {
            return 0f;
        }

        void ISeatruckArm.SetRotation(SeatruckArm arm, bool isDocked)
        {
            if (isDocked && !seatruck.seatruckHelper.TruckDockable.isInTransition)
            {
                BaseRoot baseRoot = seatruck.mainCab.GetComponentInParent<BaseRoot>();

                if (baseRoot.isBase)
                {
                    if (arm == SeatruckArm.Right)
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
                if (arm == SeatruckArm.Right)
                {
                    transform.localRotation = Quaternion.Euler(0, 0, 0);
                }
                else
                {
                    transform.localRotation = Quaternion.Euler(0, 0, 0);
                }
            }
        }

        bool ISeatruckArm.GetCustomUseText(out string customText)
        {
            customText = string.Empty;
            return false;
        }
    }
}
