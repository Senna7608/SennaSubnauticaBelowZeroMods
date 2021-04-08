extern alias SEZero;

using System.Text;
using UnityEngine;
using UWE;
using BZCommon;
using BZCommon.Helpers;
using SeaTruckArms.ArmPrefabs;
using SeaTruckArms.API;
using SeaTruckArms.API.Interfaces;
using SEZero::SlotExtenderZero.API;

namespace SeaTruckArms
{
    internal partial class SeaTruckArmManager : MonoBehaviour
    {
        public SeaTruckHelper helper;

        public SeaTruckArmManager manager => this;
        
        private const SeaTruckArm Left = SeaTruckArm.Left;
        private const SeaTruckArm Right = SeaTruckArm.Right;
        private SeaTruckArm currentSelectedArm;

        private VFXConstructing vfxConstructing;

        private ISeaTruckArmHandler leftArm;
        private ISeaTruckArmHandler rightArm;

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

        private bool isSeaTruckArmSlotsReady = false;

        private Material LeftSocketMat;
        private Material RightSocketMat;

        public ObjectHelper objectHelper { get; private set; }

        private void Awake()
        {
            helper = SeatruckServices.Main.GetSeaTruckHelper(gameObject);
            
            objectHelper = Main.graphics.objectHelper;

            vfxConstructing = GetComponent<VFXConstructing>();                    

            leftArmAttachPoint = objectHelper.CreateGameObject("leftArmAttachPoint", transform, new Vector3(-1.06f, -0.52f, 1.82f), new Vector3(350, 7, 0));
            leftArmAttach = leftArmAttachPoint.transform;

            GameObject armSocketLeft = Instantiate(Main.graphics.ArmSocket, leftArmAttach);
            armSocketLeft.SetActive(true);
            armSocketLeft.name = "armSocketLeft";
            ColorizationHelper.AddRendererToColorCustomizer(gameObject, armSocketLeft, false, new int[] { 0 });
            ColorizationHelper.AddRendererToSkyApplier(gameObject, armSocketLeft, Skies.Auto);            

            LeftSocketMat = armSocketLeft.GetComponent<Renderer>().material;            

            rightArmAttachPoint = objectHelper.CreateGameObject("rightArmAttachPoint", transform, new Vector3(1.06f, -0.52f, 1.82f), new Vector3(350, 353, 0));
            rightArmAttach = rightArmAttachPoint.transform;

            objectHelper.GetPrefabClone(ref armSocketLeft, rightArmAttach, true, "armSocketRight", out GameObject armSocketRight);

            ColorizationHelper.AddRendererToColorCustomizer(gameObject, armSocketRight, false, new int[] { 0 });
            ColorizationHelper.AddRendererToSkyApplier(gameObject, armSocketRight, Skies.Auto);

            armSocketLeft.transform.localPosition = new Vector3(0f, 0.01f, -0.01f);
            armSocketLeft.transform.localScale = new Vector3(1.17f, 1.50f, 1.50f);
            armSocketLeft.transform.localRotation = Quaternion.Euler(90, 0, 0);

            RightSocketMat = armSocketRight.GetComponent<Renderer>().material;
            
            armSocketRight.transform.localPosition = new Vector3(0f, 0.01f, -0.01f);
            armSocketRight.transform.localScale = new Vector3(-1.17f, 1.50f, 1.50f);
            armSocketRight.transform.localRotation = Quaternion.Euler(90, 0, 0);            

            GameObject leftAimForward = objectHelper.CreateGameObject("leftAimForward", transform);
            aimTargetLeft = leftAimForward.transform;

            GameObject rightAimForward = objectHelper.CreateGameObject("rightAimForward", transform);
            aimTargetRight = rightAimForward.transform;                              
        }

        public void WakeUp()
        {
            BZLogger.Debug("Received SlotExtenderZero 'WakeUp' message.");

            isSeaTruckArmSlotsReady = true;
            
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
            if (ArmServices.main.IsSeaTruckArm(item.item.GetTechType(), out TechType techType))
            {
                int slotID = helper.GetSlotIndex(slot);

                if (slotID == LeftArmSlotID)
                {
                    AddArm(SeaTruckArm.Left, techType);
                    return;
                }
                else if (slotID == RightArmSlotID)
                {
                    AddArm(SeaTruckArm.Right, techType);
                    return;
                }
            }
        }

        private void OnUnequip(string slot, InventoryItem item)
        {
            if (ArmServices.main.IsSeaTruckArm(item.item.GetTechType(), out TechType techType))
            {
                int slotID = helper.GetSlotIndex(slot);                

                if (slotID == LeftArmSlotID)
                {
                    RemoveArm(SeaTruckArm.Left);
                    return;
                }
                else if (slotID == RightArmSlotID)
                {
                    RemoveArm(SeaTruckArm.Right);
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
                    currentSelectedArm = SeaTruckArm.Left;
                }
                else if (slotID == RightArmSlotID)
                {
                    currentSelectedArm = SeaTruckArm.Right;
                }
            }
            else
            {
                currentSelectedArm = SeaTruckArm.None;
                ResetArms();
            }
        }        

        private void OnActiveSlotChanged(int newValue)
        {
            int slotID = newValue;

            if (slotID == LeftArmSlotID)
            {
                currentSelectedArm = SeaTruckArm.Left;
                return;
            }
            else if (slotID == RightArmSlotID)
            {
                currentSelectedArm = SeaTruckArm.Right;
                return;
            }
            
            currentSelectedArm = SeaTruckArm.None;

            ResetArms();
        }

        private void OnPlayerModeChanged(Player.Mode newMode)
        {
            if (newMode != Player.Mode.LockedPiloting)
            {
                ResetArms();
            }
        }

        private void Update()
        {
            if (!isSeaTruckArmSlotsReady || armsDirty)
            {
                return;
            }
            
            if (helper.TruckMotor.IsPiloted())
            {
                if (AvatarInputHandler.main != null && AvatarInputHandler.main.IsEnabled())
                {
                    Player main = Player.main;

                    Vector3 eulerAngles = gameObject.transform.eulerAngles;
                    eulerAngles.x = MainCamera.camera.transform.eulerAngles.x;
                    Quaternion quaternion = Quaternion.Euler(eulerAngles.x, eulerAngles.y, eulerAngles.z);
                    Quaternion rotation = quaternion;

                    aimTargetLeft.transform.position = MainCamera.camera.transform.position + quaternion * Vector3.forward * 100f;
                    aimTargetRight.transform.position = MainCamera.camera.transform.position + rotation * Vector3.forward * 100f;

                    bool hasPropCannon = false;

                    if (currentSelectedArm == SeaTruckArm.Left && leftArm != null)
                    {
                        leftArm.Update(ref quaternion);
                        hasPropCannon = LeftArmType == SeaTruckPropulsionArm_Prefab.TechTypeID;
                    }

                    if (currentSelectedArm == SeaTruckArm.Right && rightArm != null)
                    {
                        rightArm.Update(ref rotation);
                        hasPropCannon = RightArmType == SeaTruckPropulsionArm_Prefab.TechTypeID;
                    }

                    UpdateUIText(hasPropCannon);

                    if (GameInput.GetButtonDown(GameInput.Button.AltTool))
                    {
                        if (rightArm != null && !rightArm.OnAltDown())
                        {
                            if (leftArm != null)
                            {
                                leftArm.OnAltDown();
                            }
                        }
                    }

                    if (currentSelectedArm != SeaTruckArm.None)
                    { 
                        if (main.GetLeftHandDown())
                        {
                            SlotArmDown();
                        }
                        else if (main.GetLeftHandHeld())
                        {
                            SlotArmHeld();
                        }
                        if (main.GetLeftHandUp())
                        {
                            SlotArmUp();
                        }
                    }                    
                }

                UpdateActiveTarget(HasClaw(), HasDrill());
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
                    string[] splittedHint = LanguageCache.GetButtonFormat("PropulsionCannonToRelease", GameInput.Button.AltTool).Split(' ');

                    sb.AppendLine($"{splittedHint[0]} {splittedHint.GetLast()}");
                }
                
                lastHasPropCannon = hasPropCannon;
                uiStringPrimary = sb.ToString();
            }

            HandReticle.main.SetTextRaw(HandReticle.TextType.Use, uiStringPrimary);
            hasInitStrings = true;
        }

        
        private void OnDockedChanged(bool isDocked)
        {
            UpdateColliders();

            if (leftArm != null)
            {
                leftArm.SetRotation(SeaTruckArm.Left, isDocked);
            }

            if (rightArm != null)
            {
                rightArm.SetRotation(SeaTruckArm.Right, isDocked);
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
        

        private void UpdateActiveTarget(bool canPickup, bool canDrill)
        {
            GameObject targetObject = null;

            ObjectType objectType = ObjectType.None;

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
                        objectType = ObjectType.Pickupable;
                    }
                    else
                    {
                        targetObject = null;
                    }
                }
                else if (rootObject.GetComponentProfiled<Drillable>())
                {
                    targetObject = rootObject;
                    objectType = ObjectType.Drillable;
                }
                else
                {
                    targetObject = null;
                }
            }

            activeTarget = targetObject;

            if (activeTarget && currentSelectedArm != SeaTruckArm.None)
            {
                if (canDrill && objectType == ObjectType.Drillable && GetSelectedArmTechType() == SeaTruckDrillArm_Prefab.TechTypeID)
                {
                    GUIHand component = Player.main.GetComponent<GUIHand>();
                    GUIHand.Send(activeTarget, HandTargetEventType.Hover, component);

                }
                else if (canPickup && objectType == ObjectType.Pickupable && GetSelectedArmTechType() == SeaTruckClawArm_Prefab.TechTypeID)
                {
                    Pickupable pickupable = activeTarget.GetComponent<Pickupable>();
                    TechType techType = pickupable.GetTechType();

                    HandReticle.main.SetText(HandReticle.TextType.Hand, LanguageCache.GetPickupText(techType), false, GameInput.Button.LeftHand);
                    HandReticle.main.SetIcon(HandReticle.IconType.Hand, 1f);
                }
            }
        }    
        
        private void CheckArmSlots()
        {
           if (ArmServices.main.IsSeaTruckArm(helper.TruckEquipment.GetTechTypeInSlot("SeaTruckArmLeft"), out TechType leftArmtechType))
           {
                AddArm(SeaTruckArm.Left, leftArmtechType);
           }

            if (ArmServices.main.IsSeaTruckArm(helper.TruckEquipment.GetTechTypeInSlot("SeaTruckArmRight"), out TechType rightArmTechType))
            {
                AddArm(SeaTruckArm.Right, rightArmTechType);
            }
        }        

        private void SetIllum()
        {
            if (leftArm == null)
            {
                LeftSocketMat.SetTexture(Shader.PropertyToID("_Illum"), Main.graphics.ArmSocket_Illum_Green);
            }
            else
            {
                LeftSocketMat.SetTexture(Shader.PropertyToID("_Illum"), Main.graphics.ArmSocket_Illum_Red);
            }

            if (rightArm == null)
            {
                RightSocketMat.SetTexture(Shader.PropertyToID("_Illum"), Main.graphics.ArmSocket_Illum_Green);
            }
            else
            {
                RightSocketMat.SetTexture(Shader.PropertyToID("_Illum"), Main.graphics.ArmSocket_Illum_Red);
            }
        }        
    }
}
