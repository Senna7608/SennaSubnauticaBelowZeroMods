extern alias SEZero;
using System.Text;
using UnityEngine;
using UWE;
using ModdedArmsHelperBZ.API;
using ModdedArmsHelperBZ.API.Interfaces;
using SEZero::SlotExtenderZero.API;
using BZHelper;

namespace ModdedArmsHelperBZ
{
    internal sealed partial class SeatruckArmManager : MonoBehaviour
    {
        public SeatruckHelper helper;
        private Seatruck seatruck;
        public SeatruckArmManager Instance => this;

        private const SeatruckArm Left = SeatruckArm.Left;
        private const SeatruckArm Right = SeatruckArm.Right;
        private SeatruckArm currentSelectedArm;

        private VFXConstructing vfxConstructing;

        private ISeatruckArm leftArm;
        private ISeatruckArm rightArm;

        public TechType currentLeftArmType;
        public TechType currentRightArmType;
        
        public bool armsDirty;

        public GameObject leftArmAttachPoint;
        public Transform leftArmAttach;

        public GameObject rightArmAttachPoint;
        public Transform rightArmAttach;

        public Transform aimTargetLeft;
        public Transform aimTargetRight;

        private GameObject activeTarget;

        private bool hasInitStrings;
        private bool lastHasPropCannon;
        private StringBuilder sb = new StringBuilder();
        private string uiStringPrimary;

        private bool isSeatruckArmSlotsReady = false;

        private Material LeftSocketMat;
        private Material RightSocketMat;

        private void Awake()
        {
            helper = SeatruckServices.Main.GetSeaTruckHelper(gameObject);
            seatruck = gameObject.AddComponent<Seatruck>();

            vfxConstructing = GetComponent<VFXConstructing>();

            leftArmAttachPoint = UnityHelper.CreateGameObject("leftArmAttachPoint", transform, new Vector3(-1.06f, -0.52f, 1.82f), new Vector3(350, 7, 0));
            leftArmAttach = leftArmAttachPoint.transform;

            GameObject armSocketLeft = Instantiate(ModdedArmsHelperBZ_Main.armsCacheManager.ArmSocket, leftArmAttach);
            armSocketLeft.SetActive(true);
            armSocketLeft.name = "armSocketLeft";
            ColorizationHelper.AddRendererToColorCustomizer(gameObject, armSocketLeft, false, new int[] { 0 });
            ColorizationHelper.AddRendererToSkyApplier(gameObject, armSocketLeft, Skies.Auto);

            LeftSocketMat = armSocketLeft.GetComponent<Renderer>().material;

            rightArmAttachPoint = UnityHelper.CreateGameObject("rightArmAttachPoint", transform, new Vector3(1.06f, -0.52f, 1.82f), new Vector3(350, 353, 0));
            rightArmAttach = rightArmAttachPoint.transform;

            GameObject armSocketRight = armSocketLeft.GetPrefabClone(rightArmAttach, true, "armSocketRight");

            ColorizationHelper.AddRendererToColorCustomizer(gameObject, armSocketRight, false, new int[] { 0 });
            ColorizationHelper.AddRendererToSkyApplier(gameObject, armSocketRight, Skies.Auto);

            armSocketLeft.transform.localPosition = new Vector3(0f, 0.01f, -0.01f);
            armSocketLeft.transform.localScale = new Vector3(1.17f, 1.50f, 1.50f);
            armSocketLeft.transform.localRotation = Quaternion.Euler(90, 0, 0);

            RightSocketMat = armSocketRight.GetComponent<Renderer>().material;

            armSocketRight.transform.localPosition = new Vector3(0f, 0.01f, -0.01f);
            armSocketRight.transform.localScale = new Vector3(-1.17f, 1.50f, 1.50f);
            armSocketRight.transform.localRotation = Quaternion.Euler(90, 0, 0);

            GameObject leftAimForward = UnityHelper.CreateGameObject("leftAimForward", leftArmAttachPoint.transform);
            aimTargetLeft = leftAimForward.transform;

            GameObject rightAimForward = UnityHelper.CreateGameObject("rightAimForward", rightArmAttachPoint.transform);
            aimTargetRight = rightAimForward.transform;
        }

        public void WakeUp()
        {
            BZLogger.Log("Received SlotExtenderZero 'WakeUp' message.");

            isSeatruckArmSlotsReady = true;

            helper.onActiveSlotChanged += OnActiveSlotChanged;
            helper.onDockedChanged += OnDockedChanged;

            Player.main.playerModeChanged.AddHandler(this, new Event<Player.Mode>.HandleFunction(OnPlayerModeChanged));

            helper.TruckQuickSlots.onToggle += OnToggleSlot;
            helper.TruckEquipment.onEquip += OnEquip;
            helper.TruckEquipment.onUnequip += OnUnequip;

            CheckArmSlots();
        }

        private void OnDestroy()
        {
            helper.onActiveSlotChanged -= OnActiveSlotChanged;
            helper.onDockedChanged -= OnDockedChanged;
            Player.main.playerModeChanged.RemoveHandler(this, OnPlayerModeChanged);
            helper.TruckEquipment.onEquip -= OnEquip;
            helper.TruckEquipment.onUnequip -= OnUnequip;
        }

        private void OnEquip(string slot, InventoryItem item)
        {
            if (ArmServices.main.IsSeatruckArm(item.item.GetTechType(), out TechType techType))
            {
                int slotID = helper.GetSlotIndex(slot);

                if (slotID == LeftArmSlotID)
                {
                    AddArm(SeatruckArm.Left, techType);
                    return;
                }
                else if (slotID == RightArmSlotID)
                {
                    AddArm(SeatruckArm.Right, techType);
                    return;
                }
            }
        }

        private void OnUnequip(string slot, InventoryItem item)
        {
            if (ArmServices.main.IsSeatruckArm(item.item.GetTechType(), out TechType techType))
            {
                int slotID = helper.GetSlotIndex(slot);

                if (slotID == LeftArmSlotID)
                {
                    RemoveArm(SeatruckArm.Left);
                    return;
                }
                else if (slotID == RightArmSlotID)
                {
                    RemoveArm(SeatruckArm.Right);
                    return;
                }
            }
        }

        private void OnToggleSlot(int slotID, bool state)
        {
            if (state)
            {
                if (slotID == LeftArmSlotID)
                {
                    currentSelectedArm = SeatruckArm.Left;
                }
                else if (slotID == RightArmSlotID)
                {
                    currentSelectedArm = SeatruckArm.Right;
                }
            }
            else
            {
                currentSelectedArm = SeatruckArm.None;
                ResetArms();
            }
        }

        private void OnActiveSlotChanged(int newValue)
        {
            int slotID = newValue;
            hasInitStrings = false;

            if (slotID == LeftArmSlotID)
            {
                currentSelectedArm = SeatruckArm.Left;
                return;
            }
            else if (slotID == RightArmSlotID)
            {
                currentSelectedArm = SeatruckArm.Right;
                return;
            }

            currentSelectedArm = SeatruckArm.None;

            ResetArms();
        }

        private void OnPlayerModeChanged(Player.Mode newMode)
        {
            if (newMode != Player.Mode.LockedPiloting)
            {
                ResetArms();
            }
        }

        public bool CanPilot()
        {
            return !FPSInputModule.current.lockMovement;
        }

        private void Update()
        {
            if (!isSeatruckArmSlotsReady || armsDirty)
            {                
                return;
            }

            if (helper.TruckMotor.IsPiloted())
            {
                if (AvatarInputHandler.main != null && AvatarInputHandler.main.IsEnabled())
                {
                    Player playerMain = Player.main;

                    Vector3 eulerAngles = transform.eulerAngles;
                    eulerAngles.x = MainCamera.camera.transform.eulerAngles.x;
                    Quaternion quaternion = Quaternion.Euler(eulerAngles.x, eulerAngles.y, eulerAngles.z);
                    Quaternion rotation = quaternion;                    
                                        
                    aimTargetLeft.transform.position = MainCamera.camera.transform.position + quaternion * Vector3.forward * 100f;
                    aimTargetRight.transform.position = MainCamera.camera.transform.position + rotation * Vector3.forward * 100f;
                    
                    bool hasPropCannon = false;

                    if (currentSelectedArm == SeatruckArm.Left && leftArm != null)
                    {
                        leftArm.Update(ref quaternion);
                        hasPropCannon = leftArm.HasPropCannon();
                    }
                    
                    if (currentSelectedArm == SeatruckArm.Right && rightArm != null)
                    {
                        rightArm.Update(ref rotation);
                        hasPropCannon = rightArm.HasPropCannon();
                    }                     
                                        
                    UpdateUIText(hasPropCannon);
                    
                    if (GameInput.GetButtonDown(GameInput.Button.AltTool))
                    {
                        if (currentSelectedArm == SeatruckArm.Left && leftArm != null)
                        {
                            leftArm.OnAltDown();                            
                        }
                        else if (currentSelectedArm == SeatruckArm.Right && rightArm != null)
                        {
                            rightArm.OnAltDown();
                        }
                    }

                    if (currentSelectedArm != SeatruckArm.None)
                    {
                        if (playerMain.GetLeftHandDown())
                        {
                            SlotArmDown();
                        }
                        else if (playerMain.GetLeftHandHeld())
                        {
                            SlotArmHeld();
                        }
                        if (playerMain.GetLeftHandUp())
                        {
                            SlotArmUp();
                        }
                    }
                }

                //UpdateActiveTarget(HasClaw(), HasDrill());
                UpdateActiveTarget();
            }            
        }

        private void UpdateUIText(bool hasPropCannon)
        {
            if (!hasInitStrings || lastHasPropCannon != hasPropCannon)
            {
                sb.Length = 0;
                sb.AppendLine(LanguageCache.GetButtonFormat("PressToExit", GameInput.Button.Exit));

                if (hasPropCannon)
                {
                    sb.AppendLine(LanguageCache.GetButtonFormat("PropulsionCannonToRelease", GameInput.Button.AltTool));
                }
                
                lastHasPropCannon = hasPropCannon;
                uiStringPrimary = sb.ToString();                
            }

            if (currentSelectedArm != SeatruckArm.None)
            {
                if (currentSelectedArm == SeatruckArm.Left && leftArm != null && leftArm.GetCustomUseText(out string useLeftText))
                {
                    uiStringPrimary = useLeftText;
                }
                else if (currentSelectedArm == SeatruckArm.Right && rightArm != null && rightArm.GetCustomUseText(out string useRightText))
                {
                    uiStringPrimary = useRightText;
                }
            }

            HandReticle.main.SetTextRaw(HandReticle.TextType.Use, uiStringPrimary);
            HandReticle.main.SetTextRaw(HandReticle.TextType.UseSubscript, string.Empty);
            hasInitStrings = true;
        }

        private void OnDockedChanged(bool isDocked, bool isInTransition)
        {
            UpdateColliders();

            if (leftArm != null)
            {
                leftArm.SetRotation(SeatruckArm.Left, isDocked);
            }

            if (rightArm != null)
            {
                rightArm.SetRotation(SeatruckArm.Right, isDocked);
            }
        }

        private void UpdateColliders()
        {            
            if (leftArm != null)
            {
                Collider[] collidersLeft = leftArm.GetGameObject().GetComponentsInChildren<Collider>(true);

                for (int j = 0; j < collidersLeft.Length; j++)
                {
                    if (collidersLeft[j].name.Equals("Hook(Clone)"))
                        continue;

                    collidersLeft[j].enabled = !helper.IsDocked;
                }                
            }

            if (rightArm != null)
            {
                Collider[] collidersRight = rightArm.GetGameObject().GetComponentsInChildren<Collider>(true);

                for (int k = 0; k < collidersRight.Length; k++)
                {
                    if (collidersRight[k].name.Equals("Hook(Clone)"))
                        continue;

                    collidersRight[k].enabled = !helper.IsDocked;
                }
            }
        }

        private void UpdateActiveTarget()
        {
            Targeting.GetTarget(seatruck.mainCab, 6f, out GameObject target, out float num);

            activeTarget = GetInteractableRoot(target);
            
            if (activeTarget != null)
            {
                GUIHand component = Player.main.GetComponent<GUIHand>();
                GUIHand.Send(activeTarget, HandTargetEventType.Hover, component);
            }            
        }


        /*
        private void UpdateActiveTarget(bool canPickup, bool canDrill)
        {
            GameObject targetObject = null;

            TargetObjectType objectType = TargetObjectType.None;

            if (canPickup || canDrill)
            {
                Targeting.GetTarget(gameObject, 4.8f, out targetObject, out float num);
            }

            if (targetObject)
            {
                GameObject rootObject = UWE.Utils.GetEntityRoot(targetObject);

                rootObject = (!(rootObject != null)) ? targetObject : rootObject;

                if (rootObject.GetComponentProfiled<Pickupable>())
                {
                    if (rootObject.GetComponent<Pickupable>().isPickupable)
                    {
                        targetObject = rootObject;
                        objectType = TargetObjectType.Pickupable;
                    }
                    else
                    {
                        targetObject = null;
                    }
                }
                else if (rootObject.GetComponentProfiled<SeatruckDrillable>())
                {
                    targetObject = rootObject;
                    objectType = TargetObjectType.Drillable;
                }
                else
                {
                    targetObject = null;
                }
            }

            activeTarget = targetObject;

            if (activeTarget && currentSelectedArm != SeatruckArm.None)
            {
                if (canDrill && objectType == TargetObjectType.Drillable && GetSelectedArm().HasDrill())
                {
                    GUIHand component = Player.main.GetComponent<GUIHand>();
                    GUIHand.Send(activeTarget, HandTargetEventType.Hover, component);

                }
                else if (canPickup && objectType == TargetObjectType.Pickupable && GetSelectedArm().HasClaw())
                {
                    Pickupable pickupable = activeTarget.GetComponent<Pickupable>();
                    TechType techType = pickupable.GetTechType();                    
                    
                    HandReticle.main.SetText(HandReticle.TextType.Hand, LanguageCache.GetPickupText(techType), false);
                    HandReticle.main.SetIcon(HandReticle.IconType.Hand, 1f);
                }
            }
        }
        */

        private void CheckArmSlots()
        {
            if (ArmServices.main.IsSeatruckArm(helper.TruckEquipment.GetTechTypeInSlot("SeaTruckArmLeft"), out TechType leftArmtechType))
            {
                AddArm(SeatruckArm.Left, leftArmtechType);
            }

            if (ArmServices.main.IsSeatruckArm(helper.TruckEquipment.GetTechTypeInSlot("SeaTruckArmRight"), out TechType rightArmTechType))
            {
                AddArm(SeatruckArm.Right, rightArmTechType);
            }
        }

        private void SetIllum()
        {
            if (leftArm == null)
            {
                LeftSocketMat.SetTexture(Shader.PropertyToID("_Illum"), ModdedArmsHelperBZ_Main.armsCacheManager.ArmSocket_Illum_Green);
            }
            else
            {
                LeftSocketMat.SetTexture(Shader.PropertyToID("_Illum"), ModdedArmsHelperBZ_Main.armsCacheManager.ArmSocket_Illum_Red);
            }

            if (rightArm == null)
            {
                RightSocketMat.SetTexture(Shader.PropertyToID("_Illum"), ModdedArmsHelperBZ_Main.armsCacheManager.ArmSocket_Illum_Green);
            }
            else
            {
                RightSocketMat.SetTexture(Shader.PropertyToID("_Illum"), ModdedArmsHelperBZ_Main.armsCacheManager.ArmSocket_Illum_Red);
            }
        }        
    }
}
