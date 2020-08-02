using UnityEngine;
using BZCommon;

namespace SeaTruckArms.ArmControls
{
    public class SeaTruckClawArmControl : MonoBehaviour, ISeaTruckArm
    {
        private SeaTruckArmManager manager;
        private SeaTruckHelper helper;

        public Animator animator;
        public const float kGrabDistance = 4.8f;
        public FMODAsset hitTerrainSound;
        public FMODAsset hitFishSound;
        public FMODAsset pickupSound;
        public Transform front;
        public VFXEventTypes vfxEventType;
        public VFXController fxControl;
        public float cooldownPunch = 1f;
        public float cooldownPickup = 1.533f;
        private const float attackDist = 5.2f;
        private const float damage = 50f;
        private const DamageType damageType = DamageType.Normal;
        private float timeUsed = float.NegativeInfinity;
        private float cooldownTime;
        private bool shownNoRoomNotification = true;
        private const float energyCost = 0.5f;

        private void Awake()
        {
            manager = GetComponentInParent<SeaTruckArmManager>();
            helper = manager.helper;

            animator = GetComponent<Animator>();
            fxControl = GetComponent<VFXController>();
            vfxEventType = VFXEventTypes.impact;

            foreach (FMODAsset asset in GetComponents<FMODAsset>())
            {
                if (asset.name == "claw_hit_terrain")
                    hitTerrainSound = asset;

                if (asset.name == "claw_hit_fish")
                    hitFishSound = asset;

                if (asset.name == "claw_pickup")
                    pickupSound = asset;
            }


            front = manager.objectHelper.FindDeepChild(gameObject,"wrist").transform;
        }

        GameObject ISeaTruckArm.GetGameObject()
        {
            return gameObject;
        }

        void ISeaTruckArm.SetSide(Arm arm)
        {
            if (arm == Arm.Right)
            {
                transform.localScale = new Vector3(-0.8f, 0.8f, 0.8f);
            }
            else
            {
                transform.localScale = new Vector3(0.8f, 0.8f, 0.8f);
            }
        }

        bool ISeaTruckArm.OnUseDown(out float cooldownDuration)
        {
            return TryUse(out cooldownDuration);
        }

        bool ISeaTruckArm.OnUseHeld(out float cooldownDuration)
        {
            return TryUse(out cooldownDuration);
        }

        bool ISeaTruckArm.OnUseUp(out float cooldownDuration)
        {
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
        }

        private bool TryUse(out float cooldownDuration)
        {
            if (Time.time - timeUsed >= cooldownTime)
            {   
                Pickupable pickupable = null;
                PickPrefab x = null;

                GameObject activeTarget = manager.GetActiveTarget();

                if (activeTarget)
                {
                    pickupable = activeTarget.GetComponent<Pickupable>();
                    x = activeTarget.GetComponent<PickPrefab>();
                }

                if (pickupable != null && pickupable.isPickupable)
                {
                    if (helper.HasRoomForItem(pickupable))
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
            if (helper.IsPiloted())
            {
                Vector3 position = default(Vector3);
                GameObject targetObject = null;

                UWE.Utils.TraceFPSTargetPosition(helper.MainCab, 6.5f, ref targetObject, ref position, out Vector3 normal, true);

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

                    VFXSurfaceTypeManager.main.Play(component2, vfxEventType, position, Quaternion.Euler(euler), helper.MainCab.transform);

                    targetObject.SendMessage("BashHit", this, SendMessageOptions.DontRequireReceiver);
                }
            }
        }

        
        public void OnPickup()
        {            
            GameObject activeTarget = manager.GetActiveTarget();

            if (activeTarget)
            {
                Pickupable pickupable = activeTarget.GetComponent<Pickupable>();
                PickPrefab component = activeTarget.GetComponent<PickPrefab>();

                bool isCanFit = Player.main.HasInventoryRoom(pickupable);

                ItemsContainer container = helper.GetRoomForItem(pickupable);

                if (pickupable != null && pickupable.isPickupable && container != null)
                {
                    pickupable.Initialize();
                    InventoryItem item = new InventoryItem(pickupable);
                    container.UnsafeAdd(item);
                    Utils.PlayFMODAsset(pickupSound, front, 20f);
                }
                else if (component != null && component.AddToContainer(container))
                {
                    component.SetPickedUp();
                }

            }
        }
        

        bool ISeaTruckArm.HasClaw()
        {
            return true;
        }

        bool ISeaTruckArm.HasDrill()
        {
            return false;
        }

        float ISeaTruckArm.GetEnergyCost()
        {
            return energyCost;
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
    }

}