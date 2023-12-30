using UnityEngine;
using ModdedArmsHelperBZ.API;
using ModdedArmsHelperBZ.API.Interfaces;
using ModdedArmsHelperBZ.API.ArmHandlers;
using System.Collections;
using BZHelper;

namespace SeaTruckArms.InternalArmHandlers
{
    internal class InternalTorpedoArmHandler : TorpedoArmHandler, ISeatruckArm
    {
        public override void Start()
        {
        }

        public override void Awake()
        {
            base.Awake();

            handTarget.onHandHover.AddListener(OnHoverTorpedoStorage);
            handTarget.onHandClick.AddListener(OnOpenTorpedoStorage);
        }

        GameObject ISeatruckArm.GetGameObject()
        {
            return gameObject;
        }

        GameObject ISeatruckArm.GetInteractableRoot(GameObject target)
        {
            return null;
        }

        private IEnumerator GetItemsContainer(string slotName)
        {
            BZLogger.Debug($"GetItemsContainer coroutine started for this Seatruck: [{seatruck.seatruckHelper.TruckName}] with slotname: [{slotName}]  armTag.techType: [{armTag.techType}]");

            while (container == null)
            {                
                container = seatruck.seatruckHelper.GetSeamothStorageInSlot(seatruck.seatruckHelper.GetSlotIndex(slotName), armTag.techType);

                BZLogger.Warn($"ItemsContainer is not ready for this Seatruck: [{seatruck.seatruckHelper.TruckName}] slotname: {slotName}  armTag.techType: {armTag.techType}");
                yield return null;
            }

            BZLogger.Log($"ItemsContainer is ready for this SeaTruck: {seatruck.seatruckHelper.TruckName} container type: [{container.containerType}]");
            BZLogger.Log($"GetItemsContainer coroutine stopped for this SeaTruck: [{seatruck.seatruckHelper.TruckName}]");

            if (container != null)
            {
                container.SetAllowedTechTypes(GetTorpedoTypes());
                container.onAddItem += OnAddItem;
                container.onRemoveItem += OnRemoveItem;
                UpdateVisuals();
            }

            yield break;
        }

        void ISeatruckArm.SetSide(SeatruckArm arm)
        {
            if (container != null)
            {
                container.onAddItem -= OnAddItem;
                container.onRemoveItem -= OnRemoveItem;
            }

            if (arm == SeatruckArm.Right)
            {
                transform.localScale = new Vector3(-0.8f, 0.8f, 0.8f);                
                StartCoroutine(GetItemsContainer("SeaTruckArmRight"));

            }
            else
            {
                transform.localScale = new Vector3(0.8f, 0.8f, 0.8f);
                StartCoroutine(GetItemsContainer("SeaTruckArmLeft"));
            }
        }

        bool ISeatruckArm.OnUseDown(out float cooldownDuration)
        {
            return TryShoot(out cooldownDuration, true);
        }

        bool ISeatruckArm.OnUseHeld(out float cooldownDuration)
        {
            return TryShoot(out cooldownDuration, false);
        }

        bool ISeatruckArm.OnUseUp(out float cooldownDuration)
        {
            animator.SetBool("use_tool", false);
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
            animator.SetBool("use_tool", false);
        }

        private bool TryShoot(out float cooldownDuration, bool verbose)
        {
            TorpedoType[] torpedoTypes = TorpedoTypes;
            TorpedoType torpedoType = null;

            for (int i = 0; i < torpedoTypes.Length; i++)
            {
                if (container.Contains(torpedoTypes[i].techType))
                {
                    torpedoType = torpedoTypes[i];
                    break;
                }
            }

            float num = Mathf.Clamp(Time.time - timeFirstShot, 0f, 5f);
            float num2 = Mathf.Clamp(Time.time - timeSecondShot, 0f, 5f);
            float b = 5f - num;
            float b2 = 5f - num2;
            float num3 = Mathf.Min(num, num2);

            if (num3 < 1f)
            {
                cooldownDuration = 0f;
                return false;
            }
            if (num >= 5f)
            {
                if (Shoot(torpedoType, siloFirst, verbose))
                {
                    timeFirstShot = Time.time;
                    cooldownDuration = Mathf.Max(1f, b2);
                    return true;
                }
            }
            else
            {
                if (num2 < 5f)
                {
                    cooldownDuration = 0f;
                    return false;
                }
                if (Shoot(torpedoType, siloSecond, verbose))
                {
                    timeSecondShot = Time.time;
                    cooldownDuration = Mathf.Max(1f, b);
                    return true;
                }
            }

            animator.SetBool("use_tool", false);
            cooldownDuration = 0f;
            return false;
        }


        private bool Shoot(TorpedoType torpedoType, Transform siloTransform, bool verbose)
        {
            if (Vehicle.TorpedoShot(container, torpedoType, siloTransform))
            {
                Utils.PlayFMODAsset(fireSound, siloTransform, 20f);
                animator.SetBool("use_tool", true);

                if (container.count == 0)
                {
                    Utils.PlayFMODAsset(torpedoDisarmed, transform, 1f);
                }
                return true;
            }
            if (verbose)
            {
                ErrorMessage.AddError(Language.main.Get("VehicleTorpedoNoAmmo"));
            }
            return false;
        }

        private void OnAddItem(InventoryItem item)
        {
            UpdateVisuals();
        }

        private void OnRemoveItem(InventoryItem item)
        {
            UpdateVisuals();
        }

        private void UpdateVisuals()
        {
            int num = 0;
            TorpedoType[] torpedoTypes = TorpedoTypes;

            for (int i = 0; i < torpedoTypes.Length; i++)
            {
                num += container.GetCount(torpedoTypes[i].techType);
            }

            visualTorpedoReload.SetActive(num >= 3);
            visualTorpedoSecond.SetActive(num >= 2);
            visualTorpedoFirst.SetActive(num >= 1);
        }

        public void OnHoverTorpedoStorage(HandTargetEventData eventData)
        {
            if (container == null)
            {
                return;
            }
            HandReticle.main.SetText(HandReticle.TextType.Hand, "VehicleTorpedoStorageLabel", true, GameInput.Button.LeftHand);
            HandReticle.main.SetText(HandReticle.TextType.HandSubscript, string.Empty, false, GameInput.Button.None);
            HandReticle.main.SetIcon(HandReticle.IconType.Hand, 1f);
        }

        public void OnOpenTorpedoStorage(HandTargetEventData eventData)
        {
            OpenTorpedoStorageExternal(eventData.transform);
        }

        public void OpenTorpedoStorageExternal(Transform useTransform)
        {
            if (container == null)
            {
                return;
            }

            Inventory.main.SetUsedStorage(container, false);
            Player.main.GetPDA().Open(PDATab.Inventory, useTransform, null);
        }

        private void OnDestroy()
        {
            if (container != null)
            {
                container.onAddItem -= OnAddItem;
                container.onRemoveItem -= OnRemoveItem;
            }

            handTarget.onHandHover.RemoveListener(OnHoverTorpedoStorage);
            handTarget.onHandClick.RemoveListener(OnOpenTorpedoStorage);
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

        void ISeatruckArm.SetRotation(SeatruckArm arm, bool isDocked)
        {
            if (isDocked && !seatruck.seatruckHelper.TruckDockable.isInTransition)
            {
                BaseRoot baseRoot = seatruck.seatruckHelper.MainCab.GetComponentInParent<BaseRoot>();

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

        float ISeatruckArm.GetEnergyCost()
        {
            return 0;
        }

        bool ISeatruckArm.GetCustomUseText(out string customText)
        {
            customText = string.Empty;
            return false;
        }

        /*
        private TechType[] GetTorpedoTypes()
        {
            TechType[] techTypes = new TechType[Main.graphics.TorpedoTypes.Length];

            for (int i = 0; i < Main.graphics.TorpedoTypes.Length; i++)
            {
                techTypes[i] = Main.graphics.TorpedoTypes[i].techType;
            }

            return techTypes;
        }
        */
    }
}
