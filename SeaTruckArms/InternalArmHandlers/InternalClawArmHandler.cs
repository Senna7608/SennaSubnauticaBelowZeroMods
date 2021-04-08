using UnityEngine;
using SeaTruckArms.API;
using SeaTruckArms.API.ArmHandlers;
using SeaTruckArms.API.Interfaces;
using System.Collections;

namespace SeaTruckArms.InternalArmHandlers
{
    internal class InternalClawArmHandler : ClawArmHandler, ISeaTruckArmHandler
    {
        GameObject ISeaTruckArmHandler.GetGameObject()
        {
            return gameObject;
        }

        void ISeaTruckArmHandler.SetSide(SeaTruckArm arm)
        {
            if (arm == SeaTruckArm.Right)
            {
                transform.localScale = new Vector3(-0.8f, 0.8f, 0.8f);
            }
            else
            {
                transform.localScale = new Vector3(0.8f, 0.8f, 0.8f);
            }
        }

        bool ISeaTruckArmHandler.OnUseDown(out float cooldownDuration)
        {
            return TryUse(out cooldownDuration);
        }

        bool ISeaTruckArmHandler.OnUseHeld(out float cooldownDuration)
        {
            return TryUse(out cooldownDuration);
        }

        bool ISeaTruckArmHandler.OnUseUp(out float cooldownDuration)
        {
            cooldownDuration = 0f;
            return true;
        }

        bool ISeaTruckArmHandler.OnAltDown()
        {
            return false;
        }

        void ISeaTruckArmHandler.Update(ref Quaternion aimDirection)
        {
        }

        void ISeaTruckArmHandler.Reset()
        {
        }

        private bool TryUse(out float cooldownDuration)
        {
            if (Time.time - timeUsed >= cooldownTime)
            {   
                Pickupable pickupable = null;
                PickPrefab x = null;

                GameObject activeTarget = ArmServices.main.GetActiveTarget(TruckHelper.MainCab);

                if (activeTarget)
                {
                    pickupable = activeTarget.GetComponent<Pickupable>();
                    x = activeTarget.GetComponent<PickPrefab>();
                }

                if (pickupable != null && pickupable.isPickupable)
                {
                    if (TruckHelper.HasRoomForItem(pickupable))
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
            if (TruckHelper.IsPiloted())
            {
                Vector3 position = default(Vector3);
                GameObject targetObject = null;

                UWE.Utils.TraceFPSTargetPosition(TruckHelper.MainCab, 6.5f, ref targetObject, ref position, out Vector3 normal, true);

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

                    VFXSurfaceTypeManager.main.Play(component2, vfxEventType, position, Quaternion.Euler(euler), TruckHelper.MainCab.transform);

                    targetObject.SendMessage("BashHit", this, SendMessageOptions.DontRequireReceiver);
                }
            }
        }

        public void OnPickup()
        {
            GameObject activeTarget = ArmServices.main.GetActiveTarget(TruckHelper.MainCab);

            if (activeTarget)
            {
                Pickupable pickupable = activeTarget.GetComponent<Pickupable>();
                PickPrefab pickPrefab = activeTarget.GetComponent<PickPrefab>();

                StartCoroutine(OnPickupAsync(pickupable, pickPrefab));
            }
        }

        private IEnumerator OnPickupAsync(Pickupable pickupable, PickPrefab pickPrefab)
        {
            ItemsContainer container = TruckHelper.GetRoomForItem(pickupable);

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

        bool ISeaTruckArmHandler.HasClaw()
        {
            return true;
        }

        bool ISeaTruckArmHandler.HasDrill()
        {
            return false;
        }

        float ISeaTruckArmHandler.GetEnergyCost()
        {
            return energyCost;
        }

        void ISeaTruckArmHandler.SetRotation(SeaTruckArm arm, bool isDocked)
        {
            if (isDocked)
            {
                BaseRoot baseRoot = TruckHelper.MainCab.GetComponentInParent<BaseRoot>();

                if (baseRoot.isBase)
                {
                    if (arm == SeaTruckArm.Right)
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
                if (arm == SeaTruckArm.Right)
                {
                    transform.localRotation = Quaternion.Euler(0, 0, 0);
                }
                else
                {
                    transform.localRotation = Quaternion.Euler(0, 0, 0);
                }
            }
        }
    }

}