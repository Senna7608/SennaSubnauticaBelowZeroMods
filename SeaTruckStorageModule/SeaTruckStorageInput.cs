extern alias SEZero;
using SEZero::SlotExtenderZero.API;

using SMLHelper.V2.Handlers;
using UnityEngine;

namespace SeaTruckStorage
{
    public class SeaTruckStorageInput : MonoBehaviour, IHandTarget
    {
        public SeaTruckHelper helper;

        public GameObject model;

        public Transform flap;
        public Collider collider;
        public FMODAsset openSound;
        public FMODAsset closeSound;

        public float timeOpen = 0.5f;
        public float timeClose = 0.25f;
        public Vector3 anglesClosed = new Vector3(0f, 0f, 0f);
        public Vector3 anglesOpened = new Vector3(60f, 0f, 0f);

        private Transform tr;
        private Sequence sequence;

        public int slotID = -1;

        private bool state;

        public const string storageModuleString = "SeaTruckStorage";

        public void Awake()
        {
            helper = GetComponentInParent<SeaTruckStorageManager>().helper;

            tr = GetComponent<Transform>();
            sequence = new Sequence();

            collider = GetComponent<Collider>();
            model = gameObject.FindChild("model");
            flap = model.transform;

            openSound = ScriptableObject.CreateInstance<FMODAsset>();
            openSound.name = "storage_open";
            openSound.path = "event:/sub/seamoth/storage_open";

            closeSound = ScriptableObject.CreateInstance<FMODAsset>();
            closeSound.name = "storage_close";
            closeSound.path = "event:/sub/seamoth/storage_close";

            UpdateColliderState();
        }

        private void Update()
        {
            sequence.Update();

            if (sequence.active)
            {
                Quaternion a = Quaternion.Euler(anglesClosed);
                Quaternion b = Quaternion.Euler(anglesOpened);
                flap.localRotation = Quaternion.Lerp(a, b, sequence.t);
            }
        }

        private void OnDisable()
        {
            sequence.Reset();
            flap.localRotation = Quaternion.Euler(anglesClosed);
        }

        private void ChangeFlapState(bool open, bool pda = false)
        {
            float time = (!open) ? timeClose : timeOpen;

            if (pda)
            {
                sequence.Set(time, open, new SequenceCallback(OpenPDA));
            }
            else
            {
                sequence.Set(time, open, null);
            }

            FMODAsset asset = (!open) ? closeSound : openSound;

            Utils.PlayFMODAsset(asset, transform, 1f);
        }

        private void OpenPDA()
        {
            bool isStorageTypeExists = TechTypeHandler.TryGetModdedTechType(storageModuleString, out TechType techType);

            if (!isStorageTypeExists)
                return;

            ItemsContainer storageInSlot = helper.GetSeamothStorageInSlot(slotID, techType);

            if (storageInSlot != null)
            {
                PDA pda = Player.main.GetPDA();

                Inventory.main.SetUsedStorage(storageInSlot, false);

                if (!pda.Open(PDATab.Inventory, tr, new PDA.OnClose(OnClosePDA)))
                {
                    OnClosePDA(pda);
                }
            }
            else
            {
                OnClosePDA(null);
            }
        }

        private void OnClosePDA(PDA pda)
        {
            sequence.Set(timeClose, false, null);
            Utils.PlayFMODAsset(closeSound, transform, 1f);
        }

        private void UpdateColliderState()
        {
            if (collider != null)
            {
                collider.enabled = state;
            }
        }

        public void SetEnabled(bool state)
        {
            if (this.state == state)
            {
                return;
            }

            this.state = state;

            UpdateColliderState();

            if (model != null)
            {
                model.SetActive(state);                
            }
        }

        public void OpenFromExternal()
        {
            bool isStorageTypeExists = TechTypeHandler.TryGetModdedTechType(storageModuleString, out TechType techType);

            if (!isStorageTypeExists)
                return;

            ItemsContainer storageInSlot = helper.GetSeamothStorageInSlot(slotID, techType);

            if (storageInSlot != null)
            {
                PDA pda = Player.main.GetPDA();
                Inventory.main.SetUsedStorage(storageInSlot, false);
                pda.Open(PDATab.Inventory, null, null);
            }
        }

        public void OnHandHover(GUIHand hand)
        {
            

            HandReticle main = HandReticle.main;
            main.SetText(HandReticle.TextType.Hand, "OpenStorage", true, GameInput.Button.LeftHand);
            //main.SetText(HandReticle.TextType.HandSubscript, string.Empty, false, GameInput.Button.None);
            main.SetText(HandReticle.TextType.HandSubscript, IsEmpty() ? "Empty" : string.Empty, true, GameInput.Button.None);
            HandReticle.main.SetIcon(HandReticle.IconType.Hand, 1f);
        }

        public bool IsEmpty()
        {
            bool isStorageTypeExists = TechTypeHandler.TryGetModdedTechType(storageModuleString, out TechType techType);

            if (!isStorageTypeExists)
                return false;

            ItemsContainer container = helper.GetSeamothStorageInSlot(slotID, techType);

            return container.count <= 0;
        }

        public void OnHandClick(GUIHand hand)
        {
            bool isStorageTypeExists = TechTypeHandler.TryGetModdedTechType(storageModuleString, out TechType techType);

            if (!isStorageTypeExists)
                return;

            if (helper.GetSeamothStorageInSlot(slotID, techType) == null)
            {
                return;
            }

            ChangeFlapState(true, true);
        }
    }
}

