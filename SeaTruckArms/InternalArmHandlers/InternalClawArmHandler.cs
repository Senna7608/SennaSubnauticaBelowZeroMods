using UnityEngine;
using System.Collections;
using ModdedArmsHelperBZ.API;
using ModdedArmsHelperBZ.API.Interfaces;
using ModdedArmsHelperBZ.API.ArmHandlers;

namespace SeaTruckArms.InternalArmHandlers
{
    internal class InternalClawArmHandler : ClawArmHandler, ISeatruckArm
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
            Pickupable pickupable = target.GetComponentInParent<Pickupable>();

            if (pickupable != null && pickupable.isPickupable)
            {
                return pickupable.gameObject;
            }

            PickPrefab pickPrefab = target.GetComponentProfiled<PickPrefab>();

            if (pickPrefab != null)
            {
                return pickPrefab.gameObject;
            }

            BreakableResource breakableResource = target.GetComponentInParent<BreakableResource>();

            if (breakableResource != null)
            {
                return breakableResource.gameObject;
            }

            return null;
        }

        void ISeatruckArm.SetSide(SeatruckArm arm)
        {
            if (arm == SeatruckArm.Right)
            {
                transform.localScale = new Vector3(-0.8f, 0.8f, 0.8f);
            }
            else
            {
                transform.localScale = new Vector3(0.8f, 0.8f, 0.8f);
            }
        }

        bool ISeatruckArm.OnUseDown(out float cooldownDuration)
        {
            return TryUse(out cooldownDuration);
        }

        bool ISeatruckArm.OnUseHeld(out float cooldownDuration)
        {
            return TryUse(out cooldownDuration);
        }

        bool ISeatruckArm.OnUseUp(out float cooldownDuration)
        {
            cooldownDuration = 0f;
            return true;
        }

        bool ISeatruckArm.OnAltDown()
        {
            return false;
        }

        void ISeatruckArm.Update(ref Quaternion aimDirection)
        {
        }

        void ISeatruckArm.ResetArm()
        {
        }

        private bool TryUse(out float cooldownDuration)
        {
            if (Time.time - timeUsed >= cooldownTime)
            {   
                Pickupable pickupable = null;
                PickPrefab x = null;

                GameObject activeTarget = ArmServices.main.GetActiveTarget(seatruck);

                if (activeTarget)
                {
                    pickupable = activeTarget.GetComponent<Pickupable>();
                    x = activeTarget.GetComponent<PickPrefab>();
                }

                if (pickupable != null && pickupable.isPickupable)
                {
                    if (seatruck.seatruckHelper.HasRoomForItem(pickupable))
                    {
                        animator.SetTrigger("use_tool");
                        cooldownTime = (cooldownDuration = cooldownPickup);
                        shownNoRoomNotification = false;
                        return true;
                    }
                    if (!shownNoRoomNotification)
                    {
                        ErrorMessage.AddDebug(Language.main.Get("ContainerCantFit"));
                        shownNoRoomNotification = true;
                    }

                }
                else
                {
                    if (x != null)
                    {
                        animator.SetTrigger("use_tool");
                        cooldownTime = (cooldownDuration = cooldownPickup);
                        return true;
                    }
                    
                    animator.SetTrigger("bash");
                    cooldownTime = (cooldownDuration = cooldownPunch);
                    fxControl.Play(0);
                    return true;
                }
            }

            cooldownDuration = 0f;
            return false;
        }

        public void OnHit()
        {
            if (seatruck.seatruckHelper.IsPiloted())
            {
                Vector3 position = default(Vector3);
                GameObject targetObject = null;

                UWE.Utils.TraceFPSTargetPosition(seatruck.mainCab, 6.5f, ref targetObject, ref position, out Vector3 normal, true);

                if (targetObject == null)
                {
                    InteractionVolumeUser component = Player.main.gameObject.GetComponent<InteractionVolumeUser>();

                    if (component != null && component.GetMostRecent() != null)
                    {
                        targetObject = component.GetMostRecent().gameObject;
                    }
                }
                if (targetObject)
                {
                    LiveMixin liveMixin = targetObject.FindAncestor<LiveMixin>();

                    if (liveMixin)
                    {
                        bool flag = liveMixin.IsAlive();
                        liveMixin.TakeDamage(50f, position, DamageType.Normal, null);
                        Utils.PlayFMODAsset(hitFishSound, front, 50f);
                    }
                    else
                    {
                        Utils.PlayFMODAsset(hitTerrainSound, front, 50f);
                    }

                    VFXSurface component2 = targetObject.GetComponent<VFXSurface>();

                    Vector3 euler = MainCameraControl.main.transform.eulerAngles + new Vector3(300f, 90f, 0f);

                    VFXSurfaceTypeManager.main.Play(component2, vfxEventType, position, Quaternion.Euler(euler), seatruck.mainCab.transform);

                    targetObject.SendMessage("BashHit", this, SendMessageOptions.DontRequireReceiver);
                }
            }
        }

        public void OnPickup()
        {
            GameObject activeTarget = ArmServices.main.GetActiveTarget(seatruck);

            if (activeTarget)
            {
                Pickupable pickupable = activeTarget.GetComponent<Pickupable>();
                PickPrefab pickPrefab = activeTarget.GetComponent<PickPrefab>();

                StartCoroutine(OnPickupAsync(pickupable, pickPrefab));
            }
        }

        private IEnumerator OnPickupAsync(Pickupable pickupable, PickPrefab pickPrefab)
        {
            ItemsContainer container = seatruck.seatruckHelper.GetRoomForItem(pickupable);

            if (pickupable != null && pickupable.isPickupable && container != null)
            {
                pickupable.Initialize();
                InventoryItem item = new InventoryItem(pickupable);                
                container.UnsafeAdd(item);
                Utils.PlayFMODAsset(pickupSound, front, 5f);
            }
            else if (pickPrefab != null)
            {
                TaskResult<bool> result = new TaskResult<bool>();
                yield return pickPrefab.AddToContainerAsync(container, result);
                if (result.Get())
                {
                    pickPrefab.SetPickedUp();
                }
                result = null;
            }
            yield break;
        }        

        bool ISeatruckArm.HasClaw()
        {
            return true;
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
            return energyCost;
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