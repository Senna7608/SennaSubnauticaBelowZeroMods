using ModdedArmsHelperBZ.API;
using UnityEngine;

namespace ModdedArmsHelperBZ
{
    internal sealed partial class SeatruckArmManager
    {
        private bool leftButtonDownProcessed;
        private bool rightButtonDownProcessed;
        
        //private float[] quickSlotTimeUsed;
        //private float[] quickSlotCooldown;
        //private bool[] quickSlotToggled;
        //private float[] quickSlotCharge;

        public void SlotArmDown()
        {
#if TESTOUTSIDE
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
            
            if (currentSelectedArm == SeatruckArm.Left)
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

                    helper.TruckUpgrades.quickSlotTimeUsed[LeftArmSlotID] = Time.time;
                    helper.TruckUpgrades.quickSlotCooldown[LeftArmSlotID] = coolDown;

                    return;
                }
            }
            else if (currentSelectedArm == SeatruckArm.Right)
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

                    helper.TruckUpgrades.quickSlotTimeUsed[RightArmSlotID] = Time.time;
                    helper.TruckUpgrades.quickSlotCooldown[RightArmSlotID] = coolDown;
                }
            }
            
        }
                
        public void SlotArmHeld()
        {
#if TESTOUTSIDE
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

            if (currentSelectedArm == SeatruckArm.Left)
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

                    helper.TruckUpgrades.quickSlotTimeUsed[LeftArmSlotID] = Time.time;
                    helper.TruckUpgrades.quickSlotCooldown[LeftArmSlotID] = coolDown;
                }
            }
            else if (currentSelectedArm == SeatruckArm.Right)
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
                        helper.TruckPowerRelay.ConsumeEnergy(rightArm.GetEnergyCost(), out float amount);
                    }

                    helper.TruckUpgrades.quickSlotTimeUsed[RightArmSlotID] = Time.time;
                    helper.TruckUpgrades.quickSlotCooldown[RightArmSlotID] = coolDown;
                }
            }
        }
                

        public void SlotArmUp()
        {

#if TESTOUTSIDE
#else
            if (!Player.main.IsPilotingSeatruck() || !AvatarInputHandler.main.IsEnabled())
            {
                return;
            }
#endif
            if (currentSelectedArm == SeatruckArm.Left)
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
                        helper.TruckUpgrades.quickSlotTimeUsed[LeftArmSlotID] = Time.time;
                        helper.TruckUpgrades.quickSlotCooldown[LeftArmSlotID] = coolDown;
                    }

                    helper.TruckUpgrades.quickSlotCharge[LeftArmSlotID] = 0f;
                }
            }
            else if (currentSelectedArm == SeatruckArm.Right)
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
                        helper.TruckUpgrades.quickSlotTimeUsed[RightArmSlotID] = Time.time;
                        helper.TruckUpgrades.quickSlotCooldown[RightArmSlotID] = coolDown;
                    }

                    helper.TruckUpgrades.quickSlotCharge[RightArmSlotID] = 0f;
                }
            }
        }        
    }
}
