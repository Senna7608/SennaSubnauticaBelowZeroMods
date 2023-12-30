using UnityEngine;
using BZHelper;
using ModdedArmsHelperBZ.API.ArmHandlers;
using ModdedArmsHelperBZ.API.Interfaces;

namespace FreezeCannonArm.Handlers
{
    internal class ExosuitFCA_ArmHandler : PropulsionArmHandler, IExosuitModdedArm
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
            base.Start();
        }

        GameObject IExosuitArm.GetGameObject()
        {
            return gameObject;
        }

        GameObject IExosuitArm.GetInteractableRoot(GameObject target)
        {
            if (freezeCannon.ValidateObject(target))
            {
                return target;
            }
            return null;
        }

        void IExosuitArm.SetSide(Exosuit.Arm arm)
        {
            if (arm == Exosuit.Arm.Right)
            {
                transform.localScale = new Vector3(-1f, 1f, 1f);
                freezeCannon.localObjectOffset = new Vector3(0.75f, 0f, 0f);
            }
            else
            {
                transform.localScale = new Vector3(1f, 1f, 1f);
                freezeCannon.localObjectOffset = new Vector3(-0.75f, 0f, 0f);
            }
        }

        bool IExosuitArm.OnUseDown(out float cooldownDuration)
        {
            usingTool = true;
            cooldownDuration = 1f;
            return freezeCannon.OnShoot();
        }

        bool IExosuitArm.OnUseHeld(out float cooldownDuration)
        {
            cooldownDuration = 0f;
            return false;
        }

        bool IExosuitArm.OnUseUp(out float cooldownDuration)
        {
            usingTool = false;
            cooldownDuration = 0f;
            return true;
        }

        bool IExosuitArm.OnAltDown()
        {
            if (freezeCannon.IsLockedObject())
            {
                freezeCannon.ReleaseLockedObject();
            }

            return true;
        }

        void IExosuitArm.Update(ref Quaternion aimDirection)
        {
            freezeCannon.usingCannon = usingTool;
            freezeCannon.UpdateActive();
        }

        void IExosuitArm.ResetArm()
        {
            freezeCannon.usingCannon = (usingTool = false);
            freezeCannon.ReleaseLockedObject();
        }

        void IExosuitArm.SetArmEffects(Exosuit.Arm arm)
        {
        }

        bool IExosuitModdedArm.GetCustomUseText(out string customText)
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
        
        float IExosuitModdedArm.GetEnergyCost()
        {
            return 0;
        }
    }
}
