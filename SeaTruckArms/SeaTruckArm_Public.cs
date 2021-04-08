using BZCommon.Helpers;
using SeaTruckArms.API;
using SeaTruckArms.API.Interfaces;
using UnityEngine;

namespace SeaTruckArms
{
    internal partial class SeaTruckArmManager
    {
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

        public ISeaTruckArmHandler LeftArm
        {
            get
            {
                return leftArm;
            }
        }

        public ISeaTruckArmHandler RightArm
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
                return helper.GetSlotIndex("SeaTruckArmLeft");
            }
        }

        public int RightArmSlotID
        {
            get
            {
                return helper.GetSlotIndex("SeaTruckArmRight");
            }
        }

        public bool IsArmSlotSelected
        {
            get
            {
                return currentSelectedArm == SeaTruckArm.None ? false : true;
            }
        }

        public enum ObjectType
        {
            None,
            Pickupable,
            Drillable
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
            if (currentSelectedArm == SeaTruckArm.Left)
            {
                return LeftArmType;
            }

            if (currentSelectedArm == SeaTruckArm.Right)
            {
                return RightArmType;
            }

            return TechType.None;
        }   

        public GameObject GetActiveTarget()
        {
            return activeTarget;
        }

        public bool HasClaw()
        {
            bool resultLeft = false;
            bool resultRight = false;

            if (leftArm != null)
                resultLeft = leftArm.HasClaw();

            if (rightArm != null)
                resultRight = rightArm.HasClaw();

            return resultLeft || resultRight;
        }

        public bool HasDrill()
        {
            bool resultLeft = false;
            bool resultRight = false;

            if (leftArm != null)
                resultLeft = leftArm.HasDrill();

            if (rightArm != null)
                resultRight = rightArm.HasDrill();

            return resultLeft || resultRight;
        }
        
        public void RemoveArm(SeaTruckArm arm)
        {
            armsDirty = true;

            if (arm == SeaTruckArm.Left)
            {
                GameObject leftModel = objectHelper.FindDeepChild(leftArm.GetGameObject(), "ArmModel");
                helper.UnregisterRendererToSkyApplier(helper.TruckOuterApplier, leftModel);
                Destroy(leftArm.GetGameObject());
                leftArm = null;
                currentLeftArmType = TechType.None;                
            }
            else
            {
                GameObject rightModel = objectHelper.FindDeepChild(rightArm.GetGameObject(), "ArmModel");
                helper.UnregisterRendererToSkyApplier(helper.TruckOuterApplier, rightModel);
                Destroy(rightArm.GetGameObject());
                rightArm = null;
                currentRightArmType = TechType.None;
            }

            SetIllum();

            armsDirty = false;
        }

        public void AddArm(SeaTruckArm arm, TechType techType)
        {
            armsDirty = true;

            if (arm == SeaTruckArm.Left)
            {
                if (leftArm != null)
                {
                    Destroy(leftArm.GetGameObject());
                    leftArm = null;
                }

                leftArm = Main.graphics.SpawnArm(techType, leftArmAttach);
                leftArm.SetSide(Left);
                leftArm.SetRotation(Left, helper.IsDocked);
                currentLeftArmType = techType;

                GameObject leftModel = objectHelper.FindDeepChild(leftArm.GetGameObject(), "ArmModel");
                helper.RegisterRendererToSkyApplier(helper.TruckOuterApplier, leftModel);
            }
            else
            {
                if (rightArm != null)
                {
                    Destroy(rightArm.GetGameObject());
                    rightArm = null;
                }

                rightArm = Main.graphics.SpawnArm(techType, rightArmAttach);
                rightArm.SetSide(Right);
                rightArm.SetRotation(Right, helper.IsDocked);
                currentRightArmType = techType;

                GameObject rightModel = objectHelper.FindDeepChild(rightArm.GetGameObject(), "ArmModel");
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

        public void ResetArms()
        {
            if (leftArm != null)
            {
                leftArm.Reset();
            }

            if (rightArm != null)
            {
                rightArm.Reset();
            }
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
