using System.Collections;
using UnityEngine;
using BZCommon;
using SeaTruckArms.ArmPrefabs;

namespace SeaTruckArms.ArmControls
{
    public class SeaTruckTorpedoArmControl : MonoBehaviour, ISeaTruckArm
    {
        private SeaTruckArmManager manager;
        private SeaTruckHelper helper;

        private GenericHandTarget handTarget;
        public Animator animator;

        private const float cooldownTime = 5f;
        private const float cooldownInterval = 1f;

        public Transform siloFirst;
        public Transform siloSecond;
        public GameObject visualTorpedoFirst;
        public GameObject visualTorpedoSecond;
        public GameObject visualTorpedoReload;
        public FMODAsset fireSound;
        public FMODAsset torpedoDisarmed;

        public ItemsContainer container;
        private float timeFirstShot = float.NegativeInfinity;
        private float timeSecondShot = float.NegativeInfinity;

        private void Awake()
        {
            manager = GetComponentInParent<SeaTruckArmManager>();
            helper = manager.helper;

            animator = GetComponent<Animator>();

            siloFirst = manager.objectHelper.FindDeepChild(gameObject, "TorpedoSiloFirst").transform;
            siloSecond = manager.objectHelper.FindDeepChild(gameObject, "TorpedoSiloSecond").transform;

            visualTorpedoFirst = manager.objectHelper.FindDeepChild(gameObject, "TorpedoFirst");
            visualTorpedoSecond = manager.objectHelper.FindDeepChild(gameObject, "TorpedoSecond");
            visualTorpedoReload = manager.objectHelper.FindDeepChild(gameObject, "TorpedoReload");

            handTarget = gameObject.GetComponentInChildren<GenericHandTarget>(true);
            handTarget.onHandHover.AddListener(OnHoverTorpedoStorage);
            handTarget.onHandClick.AddListener(OnOpenTorpedoStorage);

            fireSound = ScriptableObject.CreateInstance<FMODAsset>();
            fireSound.path = "event:/sub/seamoth/torpedo_fire";
            fireSound.name = "torpedo_fire";

            torpedoDisarmed = ScriptableObject.CreateInstance<FMODAsset>();
            torpedoDisarmed.path = "event:/sub/seamoth/torpedo_disarmed";
            torpedoDisarmed.name = "torpedo_disarmed";
        }

        GameObject ISeaTruckArm.GetGameObject()
        {
            return gameObject;
        }

        private IEnumerator GetItemsContainer(string slotName)
        {
            //BZLogger.Log($"[SeamothArms] GetItemsContainer coroutine started for this SeaTruck: {helper.GetInstanceID()}");

            while (container == null)
            {
                container = helper.GetSeamothStorageInSlot(helper.GetSlotIndex(slotName), SeaTruckTorpedoArmPrefab.TechTypeID);

                //BZLogger.Log($"[SeamothArms] ItemsContainer is not ready for this SeaTruck: {helper.GetInstanceID()}");
                yield return null;
            }

            //BZLogger.Log($"[SeamothArms] ItemsContainer is ready for this SeaTruck: {helper.GetInstanceID()}");
            //BZLogger.Log($"[SeamothArms] GetItemsContainer coroutine stopped for this SeaTruck: {helper.GetInstanceID()}");

            if (container != null)
            {
                container.SetAllowedTechTypes(GetTorpedoTypes());
                container.onAddItem += OnAddItem;
                container.onRemoveItem += OnRemoveItem;
                UpdateVisuals();
            }

            yield break;
        }

        void ISeaTruckArm.SetSide(Arm arm)
        {
            if (container != null)
            {
                container.onAddItem -= OnAddItem;
                container.onRemoveItem -= OnRemoveItem;
            }

            if (arm == Arm.Right)
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

        bool ISeaTruckArm.OnUseDown(out float cooldownDuration)
        {
            return TryShoot(out cooldownDuration, true);
        }

        bool ISeaTruckArm.OnUseHeld(out float cooldownDuration)
        {
            return TryShoot(out cooldownDuration, false);
        }

        bool ISeaTruckArm.OnUseUp(out float cooldownDuration)
        {
            animator.SetBool("use_tool", false);
            cooldownDuration = 0f;
            return true;
        }

        bool ISeaTruckArm.OnAltDown()
        {
            return false;
        }

        void ISeaTruckArm.Update(ref Quaternion aimDirection)
        {
        }

        void ISeaTruckArm.Reset()
        {
            animator.SetBool("use_tool", false);
        }

        private bool TryShoot(out float cooldownDuration, bool verbose)
        {
            TorpedoType[] torpedoTypes = Main.graphics.TorpedoTypes;
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
            TorpedoType[] torpedoTypes = Main.graphics.TorpedoTypes;

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
            Player.main.GetPDA().Open(PDATab.Inventory, useTransform, null, -1f);
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

        bool ISeaTruckArm.HasClaw()
        {
            return false;
        }

        bool ISeaTruckArm.HasDrill()
        {
            return false;
        }

        void ISeaTruckArm.SetRotation(Arm arm, bool isDocked)
        {
            if (isDocked)
            {
                BaseRoot baseRoot = helper.MainCab.GetComponentInParent<BaseRoot>();

                if (baseRoot.isBase)
                {
                    if (arm == Arm.Right)
                    {
                        transform.localRotation = Quaternion.Euler(32, 6, 9);
                    }
                    else
                    {
                        transform.localRotation = Quaternion.Euler(32, 354, 351);
                    }
                }
            }
            else
            {
                if (arm == Arm.Right)
                {
                    transform.localRotation = Quaternion.Euler(0, 0, 0);
                }
                else
                {
                    transform.localRotation = Quaternion.Euler(0, 0, 0);
                }
            }
        }

        float ISeaTruckArm.GetEnergyCost()
        {
            return 0;
        }

        private TechType[] GetTorpedoTypes()
        {
            TechType[] techTypes = new TechType[Main.graphics.TorpedoTypes.Length];

            for (int i = 0; i < Main.graphics.TorpedoTypes.Length; i++)
            {
                techTypes[i] = Main.graphics.TorpedoTypes[i].techType;
            }

            return techTypes;
        }
    }
}
