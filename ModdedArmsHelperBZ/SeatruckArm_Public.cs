using BZHelper;
using ModdedArmsHelperBZ.API;
using ModdedArmsHelperBZ.API.Interfaces;
using UnityEngine;

namespace ModdedArmsHelperBZ
{
    internal sealed partial class SeatruckArmManager
    {
        //public Event<bool> onDockedChanged = new Event<bool>();

        public TechType LeftArmType
        {
            get
            {
                return currentLeftArmType;
            }
        }

        public TechType RightArmType
        {
            get
            {
                return currentRightArmType;
            }
        }

        public ISeatruckArm LeftArm
        {
            get
            {
                return leftArm;
            }
        }

        public ISeatruckArm RightArm
        {
            get
            {
                return rightArm;
            }
        }

        public int LeftArmSlotID
        {
            get
            {
                return seatruck.seatruckHelper.GetSlotIndex("SeaTruckArmLeft");
            }
        }

        public int RightArmSlotID
        {
            get
            {
                return seatruck.seatruckHelper.GetSlotIndex("SeaTruckArmRight");
            }
        }       

        public bool IsArmSlotSelected
        {
            get
            {
                return currentSelectedArm == SeatruckArm.None ? false : true;
            }
        }        

        public bool IsAnyArmAttached
        {
            get
            {
                return (leftArm == null && rightArm == null) ? false : true;
            }
        }

        public TechType GetSelectedArmTechType()
        {
            if (currentSelectedArm == SeatruckArm.Left)
            {
                return LeftArmType;
            }

            if (currentSelectedArm == SeatruckArm.Right)
            {
                return RightArmType;
            }

            return TechType.None;
        }

        public ISeatruckArm GetSelectedArm()
        {
            if (currentSelectedArm == SeatruckArm.Left)
            {
                return leftArm;
            }

            if (currentSelectedArm == SeatruckArm.Right)
            {
                return rightArm;
            }

            return null;
        }
        
        public GameObject GetActiveTarget()
        {
            return activeTarget;
        }

        public GameObject GetInteractableRoot(GameObject targetObject)
        {
            if (targetObject == null)
            {
                return null;
            }

            GameObject objectRoot = UWE.Utils.GetEntityRoot(targetObject);            

            objectRoot = (!(objectRoot != null)) ? targetObject : objectRoot;

            if (currentSelectedArm == SeatruckArm.Right && rightArm != null)
            {
                targetObject = rightArm.GetInteractableRoot(objectRoot);
            }
            else if (currentSelectedArm == SeatruckArm.Left && leftArm != null)
            {
                targetObject = leftArm.GetInteractableRoot(objectRoot);
            }            

            return targetObject;
        }

        internal bool HasClaw()
        {
            bool resultLeft = false;
            bool resultRight = false;

            if (leftArm != null)
                resultLeft = leftArm.HasClaw();

            if (rightArm != null)
                resultRight = rightArm.HasClaw();

            return resultLeft || resultRight;
        }

        internal bool HasDrill()
        {
            bool resultLeft = false;
            bool resultRight = false;

            if (leftArm != null)
                resultLeft = leftArm.HasDrill();

            if (rightArm != null)
                resultRight = rightArm.HasDrill();

            return resultLeft || resultRight;
        }

        public void RemoveArm(SeatruckArm arm)
        {
            armsDirty = true;

            if (arm == SeatruckArm.Left)
            {
                GameObject leftModel = UnityHelper.FindDeepChild(leftArm.GetGameObject(), "ArmModel");
                helper.UnregisterRendererToSkyApplier(helper.TruckOuterApplier, leftModel);
                Destroy(leftArm.GetGameObject());
                leftArm = null;
                currentLeftArmType = TechType.None;
            }
            else
            {
                GameObject rightModel = UnityHelper.FindDeepChild(rightArm.GetGameObject(), "ArmModel");
                helper.UnregisterRendererToSkyApplier(helper.TruckOuterApplier, rightModel);
                Destroy(rightArm.GetGameObject());
                rightArm = null;
                currentRightArmType = TechType.None;
            }

            SetIllum();

            armsDirty = false;
        }

        public void AddArm(SeatruckArm arm, TechType techType)
        {
            armsDirty = true;

            if (arm == SeatruckArm.Left)
            {
                if (leftArm != null)
                {
                    Destroy(leftArm.GetGameObject());
                    leftArm = null;
                }

                leftArm = ModdedArmsHelperBZ_Main.armsCacheManager.SpawnArm(techType, leftArmAttach);
                leftArm.SetSide(Left);
                leftArm.SetRotation(Left, helper.IsDocked);
                currentLeftArmType = techType;

                GameObject leftModel = UnityHelper.FindDeepChild(leftArm.GetGameObject(), "ArmModel");
                helper.RegisterRendererToSkyApplier(helper.TruckOuterApplier, leftModel);
            }
            else
            {
                if (rightArm != null)
                {
                    Destroy(rightArm.GetGameObject());
                    rightArm = null;
                }

                rightArm = ModdedArmsHelperBZ_Main.armsCacheManager.SpawnArm(techType, rightArmAttach);
                rightArm.SetSide(Right);
                rightArm.SetRotation(Right, helper.IsDocked);
                currentRightArmType = techType;

                GameObject rightModel = UnityHelper.FindDeepChild(rightArm.GetGameObject(), "ArmModel");
                helper.RegisterRendererToSkyApplier(helper.TruckOuterApplier, rightModel);
            }

            SetIllum();

            vfxConstructing.Regenerate();

            ApplyColors();
            //helper.TruckColorNameControl.ApplyColors();

            armsDirty = false;

            UpdateColliders();

            //ColorizationHelper.AddColorCustomizerToGameObject(rightArm.GetGameObject());
        }

        internal void ResetArms()
        {
            leftArm?.ResetArm();
            rightArm?.ResetArm();            
        }

        internal void ApplyColors()
        {
            ColorCustomizer colorCustomizer;

            if (leftArm != null)
            {
                colorCustomizer = leftArm.GetGameObject().GetComponent<ColorCustomizer>();

                if (helper.TruckColorNameControl.savedColors.Length != 0)
                {
                    colorCustomizer.SetMainColor(helper.TruckColorNameControl.savedColors[0]);
                }
                if (helper.TruckColorNameControl.savedColors.Length > 1)
                {
                    colorCustomizer.SetStripe1Color(helper.TruckColorNameControl.savedColors[1]);
                }
                if (helper.TruckColorNameControl.savedColors.Length > 2)
                {
                    colorCustomizer.SetStripe2Color(helper.TruckColorNameControl.savedColors[2]);
                }
            }

            if (rightArm != null)
            {
                colorCustomizer = rightArm.GetGameObject().GetComponent<ColorCustomizer>();

                if (helper.TruckColorNameControl.savedColors.Length != 0)
                {
                    colorCustomizer.SetMainColor(helper.TruckColorNameControl.savedColors[0]);
                }
                if (helper.TruckColorNameControl.savedColors.Length > 1)
                {
                    colorCustomizer.SetStripe1Color(helper.TruckColorNameControl.savedColors[1]);
                }
                if (helper.TruckColorNameControl.savedColors.Length > 2)
                {
                    colorCustomizer.SetStripe2Color(helper.TruckColorNameControl.savedColors[2]);
                }
            }
        }
    }
}
