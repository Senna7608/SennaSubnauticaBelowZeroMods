using UnityEngine;

namespace SeaTruckArms
{
    public enum ArmTemplate
    {
        DrillArm,
        ClawArm,
        //PropulsionArm,
        GrapplingArm,
        TorpedoArm
    };

    //for future use
    public struct NewSeaTruckArm
    {
        public ArmTemplate armBase;
        public TechType techType;
        public Mesh newMesh;
        public Material[] newMeshMaterials;
        public ISeaTruckArm armControl;
    }

    public enum Arm
    {
        None,
        Left,
        Right
    }

    public partial class SeaTruckArmManager
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

        public ISeaTruckArm LeftArm
        {
            get
            {
                return leftArm;
            }
        }

        public ISeaTruckArm RightArm
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
                return currentSelectedArm == Arm.None ? false : true;
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
            if (currentSelectedArm == Arm.Left)
            {
                return LeftArmType;
            }

            if (currentSelectedArm == Arm.Right)
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
        
        public void RemoveArm(Arm arm)
        {
            armsDirty = true;

            if (arm == Arm.Left)
            {
                Destroy(leftArm.GetGameObject());
                leftArm = null;
                currentLeftArmType = TechType.None;                
            }
            else
            {
                Destroy(rightArm.GetGameObject());
                rightArm = null;
                currentRightArmType = TechType.None;
            }

            SetIllum();

            armsDirty = false;
        }

        public void AddArm(Arm arm, TechType techType)
        {
            armsDirty = true;

            if (arm == Arm.Left)
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
            }

            SetIllum();

            vfxConstructing.Regenerate();
            helper.thisColorNameControl.ApplyColors();

            armsDirty = false;
            
            UpdateColliders();
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
    }
}
