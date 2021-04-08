using System.Collections;
using UnityEngine;
using UWE;
using SlotExtenderZero.Configuration;
using SlotExtenderZero.API;
using BZCommon;

namespace SlotExtenderZero
{
    internal class SlotExtenderZero : MonoBehaviour
    {
        public SlotExtenderZero Instance { get; private set; }

        private Vehicle ThisVehicle;
        public SeaTruckHelper helper;

        private Player PlayerMain;
        private PDA PdaMain;
        public bool isActive = false;

        private void Awake()
        {
            // set this SlotExtenderZero instance
            Instance = this;

            if (gameObject.GetComponent<Exosuit>())
            {
                // this Vehicle type is Exosuit
                ThisVehicle = Instance.GetComponent<Exosuit>();
            }
            else if (gameObject.GetComponent<SeaTruckUpgrades>() != null)
            {
                // this Vehicle type is Seatruck
                //helper = new SeaTruckHelper(gameObject, false, false, false);

                helper = SeatruckServices.Main.GetSeaTruckHelper(gameObject);
            }
        }

        private void Start()
        {
            //get player instance
            PlayerMain = Player.main;
            PdaMain = PlayerMain.GetPDA();
            //forced triggering the Awake method in uGUI_Equipment for patching
            PdaMain.Open();
            PdaMain.Close();
            //add and start a handler to check the player mode if changed
            PlayerMain.playerModeChanged.AddHandler(gameObject, new Event<Player.Mode>.HandleFunction(OnPlayerModeChanged));

            string vehicleName = string.Empty;            

            if (ThisVehicle)
            {
                isActive = (PlayerMain.GetVehicle() == ThisVehicle);
                vehicleName = ThisVehicle.vehicleName;                
            }
            else if (helper.isReady)
            {
                isActive = (PlayerMain.GetComponentInParent<SeaTruckUpgrades>() == helper.TruckUpgrades);
                vehicleName = helper.TruckName;                
            }

            BZLogger.Log($"Broadcasting message: 'WakeUp', Name: {vehicleName}, Instance ID: {gameObject.GetInstanceID()}");
            gameObject.BroadcastMessage("WakeUp");
        }


        private void OnPlayerModeChanged(Player.Mode playerMode)
        {
            if (playerMode == Player.Mode.LockedPiloting)
            {
                StartCoroutine(WaitForPlayerModeChangeFinished());
                return;
            }
            isActive = false;
        }

        private IEnumerator WaitForPlayerModeChangeFinished()
        {
            while (Player.main.currentMountedVehicle == null && !Player.main.IsPilotingSeatruck())
            {
                yield return null;
            }

            if (ThisVehicle)
            {
                if (Player.main.currentMountedVehicle == ThisVehicle)
                {
                    isActive = true;
                }
                else
                {
                    isActive = false;
                }
            }
            else if (helper.isReady)
            {
                if (Player.main.GetComponentInParent<SeaTruckUpgrades>() == helper.TruckUpgrades)
                {
                    isActive = true;
                }
                else
                {
                    isActive = false;
                }
            }
            yield break;
        }

        private void Update()
        {
            if (!isActive)
                return; // SlotExtenderZero not active. Exit method.

            if (Main.isConsoleActive)
                return; // Input console active. Exit method.

            if (!PlayerMain.inExosuit && !PlayerMain.IsPilotingSeatruck())
                return; // Player not in any vehicle. Exit method.

            if (Input.GetKeyDown(SEzConfig.KEYBINDINGS["Upgrade"]))
            {
                if (PdaMain.isOpen)
                {
                    PdaMain.Close();
                    return;
                }
                else // Is Closed
                {
                    if (ThisVehicle)
                    {
                        ThisVehicle.upgradesInput.OpenFromExternal();
                    }
                    else if (helper.isReady)
                    {
                        helper.TruckUpgrades.upgradesInput.OpenFromExternal();
                    }

                    return;
                }
            }
            else if (Input.GetKeyDown(SEzConfig.KEYBINDINGS["Storage"]))
            {
                if (!ThisVehicle)
                {
                    return;
                }
                if (ThisVehicle.GetType() != typeof(Exosuit))
                {
                    return;
                }
                if (PdaMain.isOpen)
                {
                    PdaMain.Close();
                    return;
                }

                StorageContainer storageContainer = ThisVehicle.GetComponent<Exosuit>().storageContainer;
                storageContainer.Open(storageContainer.gameObject.transform);
                return;
            }
            else if (GameInput.GetButtonDown(GameInput.Button.Slot1))
            {
                TryUseSlotItem(0);
                return;
            }
            else if (GameInput.GetButtonDown(GameInput.Button.Slot2))
            {
                TryUseSlotItem(1);
                return;
            }
            else if (GameInput.GetButtonDown(GameInput.Button.Slot3))
            {
                TryUseSlotItem(2);
                return;
            }
            else if (GameInput.GetButtonDown(GameInput.Button.Slot4))
            {
                TryUseSlotItem(3);
                return;
            }
            else if (GameInput.GetButtonDown(GameInput.Button.Slot5))
            {
                TryUseSlotItem(4);
                return;
            }
            else if (Input.GetKeyDown(SEzConfig.KEYBINDINGS["Slot_6"]))
            {
                TryUseSlotItem(5);
                return;
            }
            else if (Input.GetKeyDown(SEzConfig.KEYBINDINGS["Slot_7"]))
            {
                TryUseSlotItem(6);
                return;
            }
            else if (Input.GetKeyDown(SEzConfig.KEYBINDINGS["Slot_8"]))
            {
                TryUseSlotItem(7);
                return;
            }
            else if (Input.GetKeyDown(SEzConfig.KEYBINDINGS["Slot_9"]))
            {
                TryUseSlotItem(8);
                return;
            }
            else if (Input.GetKeyDown(SEzConfig.KEYBINDINGS["Slot_10"]))
            {
                TryUseSlotItem(9);
                return;
            }
            else if (Input.GetKeyDown(SEzConfig.KEYBINDINGS["Slot_11"]))
            {
                TryUseSlotItem(10);
                return;
            }
            else if (Input.GetKeyDown(SEzConfig.KEYBINDINGS["Slot_12"]))
            {
                TryUseSlotItem(11);
                return;
            }
            else if (Input.GetKeyDown(SEzConfig.KEYBINDINGS[SlotHelper.slotStringCache[SlotName.SeaTruckArmLeft]]))
            {
                TryUseSeatruckArm(SlotHelper.slotStringCache[SlotName.SeaTruckArmLeft]);
                return;
            }
            else if (Input.GetKeyDown(SEzConfig.KEYBINDINGS[SlotHelper.slotStringCache[SlotName.SeaTruckArmRight]]))
            {
                TryUseSeatruckArm(SlotHelper.slotStringCache[SlotName.SeaTruckArmRight]);
                return;
            }
        }

        private void TryUseSlotItem(int slotID)
        {
            if (PdaMain.isOpen)
            {
                PdaMain.Close();
                return;
            }

            if (helper.isReady)
            {
                if (helper.IsValidSeaTruckStorageContainer(slotID))
                {
                    if (helper.TryOpenSeaTruckStorageContainer(slotID))
                        return;
                }
                else if (slotID > 5)
                {
                    helper.TruckQuickSlots.SlotKeyDown(slotID);
                    return;
                }
            }
            
            if (slotID > 5)
            {
                ThisVehicle.SlotKeyDown(slotID);
            }
           
        }

        private void TryUseSeatruckArm(string seatruckArmID)
        {
            if (helper.isReady)
            {
                helper.TruckQuickSlots.SlotKeyDown(helper.GetSlotIndex(seatruckArmID));
            }
        }
               
        private void OnDestroy()
        {
            // removing unused handler from memory
            PlayerMain.playerModeChanged.RemoveHandler(gameObject, OnPlayerModeChanged);
            Destroy(Instance);
        }
    }
}
