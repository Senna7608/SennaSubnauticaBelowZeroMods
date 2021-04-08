using SeaTruckArms.API;
using UnityEngine;

namespace SeaTruckArms
{
    internal partial class SeaTruckArmManager
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

            if (currentSelectedArm == SeaTruckArm.Left)
            {
                leftButtonDownProcessed = true;                

                if (helper.GetSlotProgress(LeftArmSlotID) != 1f)
                {                    
                    return;
                }

                QuickSlotType quickSlotType = TechData.GetSlotType(currentLeftArmType);

                if (quickSlotType == QuickSlotType.Selectable && leftArm.OnUseDown(out float coolDown))
                {
                    if (helper.TruckPowerRelay)
                    {
                        helper.TruckPowerRelay.ConsumeEnergy(leftArm.GetEnergyCost(), out float amount);
                    }

                    helper.TruckQuickSlotTimeUsed[LeftArmSlotID] = Time.time;
                    helper.TruckQuickSlotCooldown[LeftArmSlotID] = coolDown;

                    return;
                }
            }
            else if (currentSelectedArm == SeaTruckArm.Right)
            {
                rightButtonDownProcessed = true;

                if (helper.GetSlotProgress(RightArmSlotID) != 1f)
                {
                    return;
                }

                QuickSlotType quickSlotType = TechData.GetSlotType(currentRightArmType);

                if (quickSlotType == QuickSlotType.Selectable && rightArm.OnUseDown(out float coolDown))
                {
                    if (helper.TruckPowerRelay)
                    {
                        helper.TruckPowerRelay.ConsumeEnergy(rightArm.GetEnergyCost(), out float amount);
                    }

                    helper.TruckQuickSlotTimeUsed[RightArmSlotID] = Time.time;
                    helper.TruckQuickSlotCooldown[RightArmSlotID] = coolDown;
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

            if (currentSelectedArm == SeaTruckArm.Left)
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
                    if (helper.TruckPowerRelay)
                    {
                        helper.TruckPowerRelay.ConsumeEnergy(leftArm.GetEnergyCost(), out float amount);
                    }

                    helper.TruckQuickSlotTimeUsed[LeftArmSlotID] = Time.time;
                    helper.TruckQuickSlotCooldown[LeftArmSlotID] = coolDown;
                }
            }
            else if (currentSelectedArm == SeaTruckArm.Right)
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
                    if (helper.TruckPowerRelay)
                    {
                        helper.TruckPowerRelay.ConsumeEnergy(leftArm.GetEnergyCost(), out float amount);
                    }

                    helper.TruckQuickSlotTimeUsed[RightArmSlotID] = Time.time;
                    helper.TruckQuickSlotCooldown[RightArmSlotID] = coolDown;
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
            if (currentSelectedArm == SeaTruckArm.Left)
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
                        helper.TruckQuickSlotTimeUsed[LeftArmSlotID] = Time.time;
                        helper.TruckQuickSlotCooldown[LeftArmSlotID] = coolDown;
                    }

                    helper.TruckQuickSlotCharge[LeftArmSlotID] = 0f;
                }
            }
            else if (currentSelectedArm == SeaTruckArm.Right)
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
                        helper.TruckQuickSlotTimeUsed[RightArmSlotID] = Time.time;
                        helper.TruckQuickSlotCooldown[RightArmSlotID] = coolDown;
                    }

                    helper.TruckQuickSlotCharge[LeftArmSlotID] = 0f;
                }
            }
        }
    }
}
