/*
using System;
using System.Collections;
using ProtoBuf;
using UnityEngine;
using UWE;

namespace SeaTruckStorage
{
    [ProtoContract]
    public class SeaTruckStorageContainer : MonoBehaviour, IProtoEventListenerAsync, IProtoTreeEventListener, ICraftTarget
    {
        [AssertLocalization]
        public string storageLabel = "VehicleStorageLabel";

        public int width = 8;

        public int height = 8;

        public TechType[] allowedTech = new TechType[0];

        //[AssertNotNull]
        public ChildObjectIdentifier storageRoot;

        private const int currentVersion = 3;

        [ProtoMember(1)]
        [NonSerialized]
        public int version = 3;

        [ProtoMember(2, OverwriteList = true)]
        [NonSerialized]
        [Obsolete("Obsolete since v2")]
        public byte[] serializedStorage;

        public ItemsContainer container { get; private set; }

        private void Awake()
        {
            Init();
        }

        private void Init()
        {
            if (container != null)
            {
                return;
            }

            storageRoot = GetComponentInChildren<ChildObjectIdentifier>();

            container = new ItemsContainer(width, height, storageRoot.transform, storageLabel, null);

            container.SetAllowedTechTypes(allowedTech);
        }

        public IEnumerator OnProtoDeserializeAsync(ProtobufSerializer serializer)
        {
            Init();

            container.Clear(false);

            if (serializedStorage != null)
            {
                yield return StorageHelper.RestoreItemsAsync(serializer, serializedStorage, container);

                serializedStorage = null;
            }
            yield break;
        }

        public void OnProtoSerializeObjectTree(ProtobufSerializer serializer)
        {
        }

        public void OnProtoDeserializeObjectTree(ProtobufSerializer serializer)
        {
            if (version < 2)
            {
                foreach (StoreInformationIdentifier storeInformationIdentifier in gameObject.GetComponentsInChildren<StoreInformationIdentifier>(true))
                {
                    if (storeInformationIdentifier && storeInformationIdentifier.transform.parent == base.transform)
                    {
                        Destroy(storeInformationIdentifier.gameObject);
                    }
                }

                version = 2;
            }
            else
            {
                StorageHelper.TransferItems(storageRoot.gameObject, container);
            }
            if (version < 3)
            {
                CoroutineHost.StartCoroutine(CleanUpDuplicatedStorage());
            }
        }

        private IEnumerator CleanUpDuplicatedStorage()
        {
            yield return StorageHelper.DestroyDuplicatedItems(gameObject);
            version = Mathf.Max(version, 3);
            yield break;
        }

        public void OnCraftEnd(TechType techType)
        {
            Init();

            if (techType == TechType.SeamothTorpedoModule || techType == TechType.ExosuitTorpedoArmModule)
            {
                StartCoroutine(OnCraftEndAsync());
            }
        }

        private IEnumerator OnCraftEndAsync()
        {
            TaskResult<GameObject> taskResult = new TaskResult<GameObject>();

            TaskResult<Pickupable> pickupableResult = new TaskResult<Pickupable>();

            int num;

            for (int i = 0; i < 2; i = num + 1)
            {
                IEnumerator enumerator = CraftData.InstantiateFromPrefabAsync(TechType.WhirlpoolTorpedo, taskResult, false);

                yield return enumerator;

                GameObject gameObject = taskResult.Get();

                if (gameObject != null)
                {
                    Pickupable component = gameObject.GetComponent<Pickupable>();

                    if (component != null)
                    {
                        pickupableResult.Set(null);

                        component.Pickup(false);

                        if (container.AddItem(component) == null)
                        {
                            Destroy(component.gameObject);
                        }
                    }
                }
                num = i;
            }
            yield break;
        }        
    }
}
*/