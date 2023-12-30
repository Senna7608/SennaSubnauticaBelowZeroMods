using UnityEngine;
using BZHelper;
using ModdedArmsHelperBZ.API;
using ModdedArmsHelperBZ.API.Interfaces;
using ModdedArmsHelperBZ.API.ArmHandlers;

namespace FreezeCannonArm.Handlers
{
    internal class SeatruckFCA_ArmHandler : PropulsionArmHandler, ISeatruckArm
    {
        public FreezeCannon freezeCannon;
        private string cachedPrimaryUseText = string.Empty;
        private string cachedAltUseText = string.Empty;
        private string cachedCustomUseText = string.Empty;       

        public override void Awake()
        {
            base.Awake();

            freezeCannon = gameObject.AddComponent<FreezeCannon>();
            freezeCannon.fxBeam = propulsionCannon.fxBeam;
            freezeCannon.fxTrailPrefab = propulsionCannon.fxTrailPrefab;
            freezeCannon.muzzle = propulsionCannon.muzzle;
            freezeCannon.fxControl = propulsionCannon.fxControl;
            freezeCannon.connectSound = propulsionCannon.grabbingSound;
            freezeCannon.shootSound = propulsionCannon.shootSound;
            freezeCannon.validTargetSound = propulsionCannon.validTargetSound;
            freezeCannon.energyInterface = energyInterface;
            freezeCannon.animator = animator;

            propulsionCannon.enabled = false;
        }

        public override void Start()
        {
        }

        GameObject ISeatruckArm.GetGameObject()
        {
            return gameObject;
        }

        GameObject ISeatruckArm.GetInteractableRoot(GameObject target)
        {           
            if (freezeCannon.ValidateObject(target))
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
                freezeCannon.localObjectOffset = new Vector3(0.75f, 0f, 0f);
            }
            else
            {
                transform.localScale = new Vector3(0.8f, 0.8f, 0.8f);
                freezeCannon.localObjectOffset = new Vector3(-0.75f, 0f, 0f);
            }
        }

        bool ISeatruckArm.OnUseDown(out float cooldownDuration)
        {
            usingTool = true;
            cooldownDuration = 1f;
            return freezeCannon.OnShoot();
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
            if (freezeCannon.IsLockedObject())
            {
                freezeCannon.ReleaseLockedObject();
            }

            return true;
        }

        void ISeatruckArm.Update(ref Quaternion aimDirection)
        {
            freezeCannon.usingCannon = usingTool;
            freezeCannon.UpdateActive();
        }

        void ISeatruckArm.ResetArm()
        {
            freezeCannon.ReleaseLockedObject();
            freezeCannon.usingCannon = (usingTool = false);
            
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
            return false;
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

        public bool GetCustomUseText(out string customText)
        {
            bool flag = freezeCannon.IsLockedObject();
            bool flag2 = freezeCannon.HasChargeForShot();

            if (!flag && !flag2)
            {
                customText = string.Empty;
                return false;
            }

            string text = string.Empty;
            string text2 = string.Empty;

            if (flag)
            {
                text = $"{Language.main.Get("FreezeCannon_Freeze")} ({uGUI.FormatButton(GameInput.Button.LeftHand, false, " / ", false)})";
                text2 = $"{Language.main.Get("FreezeCannon_Release")} ({uGUI.FormatButton(GameInput.Button.AltTool, false, " / ", false)})";
            }
            else
            {
                if (freezeCannon.CanFreeze)
                {
                    text = $"{Language.main.Get("FreezeCannon_Lock")} ({uGUI.FormatButton(GameInput.Button.LeftHand, false, " / ", false)})";
                    text2 = freezeCannon.TargetTech;
                }
                else
                {
                    text = Language.main.Get("FreezeCannon_Targeting");
                    text2 = freezeCannon.TargetTech;
                }
            }

            if (text != cachedPrimaryUseText || text2 != cachedAltUseText)
            {
                cachedCustomUseText = $"{text}\n{text2}";
                cachedPrimaryUseText = text;
                cachedAltUseText = text2;
            }
            customText = cachedCustomUseText;
            return true;
        }
    }
}
