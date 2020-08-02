using UnityEngine;

namespace SeaTruckArms
{
    public partial class SeaTruckArmManager
    {
        private bool leftButtonDownProcessed;
        private bool rightButtonDownProcessed;

        public void SlotArmDown()
        {
#if DEBUG
#else  
            if (!Player.main.IsPilotingSeatruck() || !AvatarInputHandler.main.IsEnabled())
            {                
                return;
            }
#endif           
            if (!helper.IsPowered())
            {                
                return;
            }

            if (currentSelectedArm == Arm.Left)
            {
                leftButtonDownProcessed = true;                

                if (helper.GetSlotProgress(LeftArmSlotID) != 1f)
                {                    
                    return;
                }

                QuickSlotType quickSlotType = TechData.GetSlotType(currentLeftArmType);

                if (quickSlotType == QuickSlotType.Selectable && leftArm.OnUseDown(out float coolDown))
                {
                    if (helper.powerRelay)
                    {
                        helper.powerRelay.ConsumeEnergy(leftArm.GetEnergyCost(), out float amount);
                    }

                    helper.quickSlotTimeUsed[LeftArmSlotID] = Time.time;
                    helper.quickSlotCooldown[LeftArmSlotID] = coolDown;

                    return;
                }
            }
            else if (currentSelectedArm == Arm.Right)
            {
                rightButtonDownProcessed = true;

                if (helper.GetSlotProgress(RightArmSlotID) != 1f)
                {
                    return;
                }

                QuickSlotType quickSlotType = TechData.GetSlotType(currentRightArmType);

                if (quickSlotType == QuickSlotType.Selectable && rightArm.OnUseDown(out float coolDown))
                {
                    if (helper.powerRelay)
                    {
                        helper.powerRelay.ConsumeEnergy(rightArm.GetEnergyCost(), out float amount);
                    }

                    helper.quickSlotTimeUsed[RightArmSlotID] = Time.time;
                    helper.quickSlotCooldown[RightArmSlotID] = coolDown;
                }
            }            
        }

        public void SlotArmHeld()
        {
#if DEBUG
#else
            if (!Player.main.IsPilotingSeatruck() || !AvatarInputHandler.main.IsEnabled())
            {
                return;
            }
#endif                                               
            if (!helper.IsPowered())
            {
                return;
            }

            if (currentSelectedArm == Arm.Left)
            {
                if (!leftButtonDownProcessed)
                {
                    return;
                }

                if (helper.GetSlotProgress(LeftArmSlotID) != 1f)
                {
                    return;
                }

                QuickSlotType quickSlotType = TechData.GetSlotType(currentLeftArmType);

                if (quickSlotType == QuickSlotType.Selectable && leftArm.OnUseHeld(out float coolDown))
                {
                    if (helper.powerRelay)
                    {
                        helper.powerRelay.ConsumeEnergy(leftArm.GetEnergyCost(), out float amount);
                    }

                    helper.quickSlotTimeUsed[LeftArmSlotID] = Time.time;
                    helper.quickSlotCooldown[LeftArmSlotID] = coolDown;
                }
            }
            else if (currentSelectedArm == Arm.Right)
            {
                if (!rightButtonDownProcessed)
                {
                    return;
                }

                if (helper.GetSlotProgress(RightArmSlotID) != 1f)
                {
                    return;
                }

                QuickSlotType quickSlotType = TechData.GetSlotType(currentRightArmType);

                if (quickSlotType == QuickSlotType.Selectable && rightArm.OnUseHeld(out float coolDown))
                {
                    if (helper.powerRelay)
                    {
                        helper.powerRelay.ConsumeEnergy(leftArm.GetEnergyCost(), out float amount);
                    }

                    helper.quickSlotTimeUsed[RightArmSlotID] = Time.time;
                    helper.quickSlotCooldown[RightArmSlotID] = coolDown;
                }
            }
        }


        public void SlotArmUp()
        {

#if DEBUG
#else
            if (!Player.main.IsPilotingSeatruck() || !AvatarInputHandler.main.IsEnabled())
            {
                return;
            }
#endif
            if (currentSelectedArm == Arm.Left)
            {
                leftButtonDownProcessed = false;

                QuickSlotType quickSlotType = TechData.GetSlotType(currentLeftArmType);

                if (quickSlotType == QuickSlotType.Selectable)
                {
                    leftArm.OnUseUp(out float coolDown);
                }
                else if (quickSlotType == QuickSlotType.SelectableChargeable)
                {
                    if (!helper.IsPowered())
                    {
                        return;
                    }

                    if (helper.GetSlotProgress(LeftArmSlotID) != 1f)
                    {
                        return;
                    }

                    if (leftArm.OnUseUp(out float coolDown))
                    {
                        helper.quickSlotTimeUsed[LeftArmSlotID] = Time.time;
                        helper.quickSlotCooldown[LeftArmSlotID] = coolDown;
                    }

                    helper.quickSlotCharge[LeftArmSlotID] = 0f;
                }
            }
            else if (currentSelectedArm == Arm.Right)
            {
                rightButtonDownProcessed = false;

                QuickSlotType quickSlotType = TechData.GetSlotType(currentRightArmType);

                if (quickSlotType == QuickSlotType.Selectable)
                {
                    rightArm.OnUseUp(out float coolDown);
                }
                else if (quickSlotType == QuickSlotType.SelectableChargeable)
                {
                    if (!helper.IsPowered())
                    {
                        return;
                    }

                    if (helper.GetSlotProgress(RightArmSlotID) != 1f)
                    {
                        return;
                    }

                    if (rightArm.OnUseUp(out float coolDown))
                    {
                        helper.quickSlotTimeUsed[RightArmSlotID] = Time.time;
                        helper.quickSlotCooldown[RightArmSlotID] = coolDown;
                    }

                    helper.quickSlotCharge[LeftArmSlotID] = 0f;
                }
            }
        }
    }
}
