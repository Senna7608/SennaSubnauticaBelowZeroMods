extern alias SEZero;

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SEZero::SlotExtenderZero.API;
using BZCommon;
using System.Reflection;

namespace SeaTruckArms.API
{
    public class SeaTruckDrillable : MonoBehaviour, IManagedUpdateBehaviour, IManagedBehaviour
    {
        private Drillable drillable;

        public event Drillable.OnDrilled onDrilled;

        public int managedUpdateIndex { get; set; }

        private MeshRenderer[] renderers;
        private const float drillDamage = 10f;
        private const float maxHealth = 200f;
        private float timeLastDrilled;
        private List<GameObject> lootPinataObjects = new List<GameObject>();

        private SeaTruckHelper helper;

        public string GetProfileTag()
        {
            return "SeaTruckDrillable";
        }

        private void OnEnable()
        {
            BehaviourUpdateUtils.RegisterForUpdate(this);
        }

        private void OnDisable()
        {
            BehaviourUpdateUtils.Deregister(this);
        }

        private void OnDestroy()
        {
            BehaviourUpdateUtils.Deregister(this);
        }

        public void ManagedFixedUpdate()
        {
        }

        public void ManagedLateUpdate()
        {
        }

        private void Awake()
        {
            drillable = GetComponent<Drillable>();
            renderers = GetComponentsInChildren<MeshRenderer>();
        }

        public void HoverDrillable()
        {
            SeaTruckArmManager control = Player.main.GetComponentInParent<SeaTruckArmManager>();

            if (control && control.HasDrill())
            {
                HandReticle.main.SetText(HandReticle.TextType.Hand, Language.main.GetFormat("DrillResource", Language.main.Get(drillable.primaryTooltip)), false, GameInput.Button.LeftHand);
                HandReticle.main.SetText(HandReticle.TextType.HandSubscript, drillable.secondaryTooltip, true, GameInput.Button.None);
                HandReticle.main.SetIcon(HandReticle.IconType.Drill, 1f);
            }
            else
            {
                HandReticle.main.SetText(HandReticle.TextType.Hand, drillable.primaryTooltip, true, GameInput.Button.None);
                HandReticle.main.SetText(HandReticle.TextType.HandSubscript, "NeedExoToMine", true, GameInput.Button.None);
            }
        }

        public void OnDrill(Vector3 position, SeaTruckHelper helper, out GameObject hitObject)
        {
            float num = 0f;

            for (int i = 0; i < drillable.health.Length; i++)
            {
                num += drillable.health[i];
            }

            this.helper = helper;

            Vector3 zero = Vector3.zero;

            int num2 = FindClosestMesh(position, out zero);

            hitObject = renderers[num2].gameObject;

            timeLastDrilled = Time.time;

            if (num > 0f)
            {
                float num3 = drillable.health[num2];

                drillable.health[num2] = Mathf.Max(0f, drillable.health[num2] - 5f);

                num -= num3 - drillable.health[num2];

                if (num3 > 0f && drillable.health[num2] <= 0f)
                {
                    renderers[num2].gameObject.SetActive(false);

                    SpawnFX(drillable.breakFX, zero);

                    if (drillable.resources.Length != 0)
                    {
                        StartCoroutine(SpawnLootAsync(zero));
                    }
                }

                if (num <= 0f)
                {
                    SpawnFX(drillable.breakAllFX, zero);

                    onDrilled?.Invoke(drillable);

                    if (drillable.deleteWhenDrilled)
                    {
                        ResourceTracker component = GetComponent<ResourceTracker>();

                        if (component)
                        {
                            component.OnBreakResource();
                        }

                        float time = (!drillable.lootPinataOnSpawn) ? 0f : 6f;
                        drillable.Invoke("DestroySelf", time);
                    }
                }
            }

            BehaviourUpdateUtils.Register(this);
        }
        
        private void ClipWithTerrain(ref Vector3 position)
        {
            Vector3 origin = position;

            origin.y = transform.position.y + 5f;

            Ray ray = new Ray(origin, Vector3.down);

            int num = UWE.Utils.RaycastIntoSharedBuffer(ray, 10f, -5, QueryTriggerInteraction.UseGlobal);

            for (int i = 0; i < num; i++)
            {
                RaycastHit raycastHit = UWE.Utils.sharedHitBuffer[i];

                if (raycastHit.collider.gameObject.GetComponentInParent<VoxelandChunk>() != null)
                {
                    position.y = Mathf.Max(position.y, raycastHit.point.y + 0.3f);
                    break;
                }
            }
        }        

        private IEnumerator SpawnLootAsync(Vector3 position)
        {
            int numResources = Random.Range(drillable.minResourcesToSpawn, drillable.maxResourcesToSpawn);
            TaskResult<GameObject> prefabResult = new TaskResult<GameObject>();

            int num2;

            for (int i = 0; i < numResources; i = num2 + 1)
            {
                yield return ChooseRandomResourceAsync(prefabResult);
                GameObject gameObject = prefabResult.Get();

                if (gameObject)
                {
                    GameObject gameObject2 = Instantiate(gameObject);
                    Vector3 vector = position;
                    float num = 1f;
                    vector.x += Random.Range(-num, num);
                    vector.z += Random.Range(-num, num);
                    vector.y += Random.Range(-num, num);
                   
                    ClipWithTerrain(ref vector);
                    gameObject2.transform.position = vector;
                    Vector3 vector2 = Random.onUnitSphere;
                    vector2.y = 0f;
                    vector2 = Vector3.Normalize(vector2);
                    vector2.y = 1f;
                    gameObject2.GetComponent<Rigidbody>().isKinematic = false;
                    gameObject2.GetComponent<Rigidbody>().AddForce(vector2);
                    gameObject2.GetComponent<Rigidbody>().AddTorque(Vector3.right * UnityEngine.Random.Range(3f, 6f));

                    if (drillable.lootPinataOnSpawn)
                    {
                        StartCoroutine(AddResourceToPinata(gameObject2));
                    }
                }
                num2 = i;
            }
            yield break;
        }

        private IEnumerator AddResourceToPinata(GameObject resource)
        {
            yield return new WaitForSeconds(1.5f);
            lootPinataObjects.Add(resource);
            yield break;
        }
        
        private int FindClosestMesh(Vector3 position, out Vector3 center)
        {
            int result = 0;
            float num = float.PositiveInfinity;
            center = Vector3.zero;

            for (int i = 0; i < renderers.Length; i++)
            {
                if (renderers[i].gameObject.activeInHierarchy)
                {
                    Bounds encapsulatedAABB = UWE.Utils.GetEncapsulatedAABB(renderers[i].gameObject, -1);
                    float sqrMagnitude = (encapsulatedAABB.center - position).sqrMagnitude;
                    if (sqrMagnitude < num)
                    {
                        num = sqrMagnitude;
                        result = i;
                        center = encapsulatedAABB.center;
                        if (sqrMagnitude <= 0.5f)
                        {
                            break;
                        }
                    }
                }
            }
            return result;
        }        

        private IEnumerator ChooseRandomResourceAsync(IOut<GameObject> result)
        {
            for (int i = 0; i < drillable.resources.Length; i++)
            {
                Drillable.ResourceType resourceType = drillable.resources[i];

                if (resourceType.chance >= 1f)
                {
                    CoroutineTask<GameObject> task = CraftData.GetPrefabForTechTypeAsync(resourceType.techType, true);
                    yield return task;
                    result.Set(task.GetResult());
                    break;
                }
                if (Player.main.gameObject.GetComponent<PlayerEntropy>().CheckChance(resourceType.techType, resourceType.chance))
                {
                    CoroutineTask<GameObject> task = CraftData.GetPrefabForTechTypeAsync(resourceType.techType, true);
                    yield return task;
                    result.Set(task.GetResult());
                    break;
                }
            }
            yield break;
        }        

        private void SpawnFX(GameObject fx, Vector3 position)
        {
            GameObject gameObject = Instantiate(fx);
            gameObject.transform.position = position;
        }

        public void ManagedUpdate()
        {
            if (timeLastDrilled + 0.5f > Time.time)
            {
                drillable.modelRoot.transform.position = transform.position + new Vector3(Mathf.Sin(Time.time * 60f), Mathf.Cos(Time.time * 58f + 0.5f), Mathf.Cos(Time.time * 64f + 2f)) * 0.011f;
            }
            if (lootPinataObjects.Count > 0 && helper != null)
            {
                List<GameObject> list = new List<GameObject>();

                foreach (GameObject gameObject in lootPinataObjects)
                {
                    if (gameObject == null)
                    {
                        list.Add(gameObject);
                    }
                    else
                    {
                        Vector3 b = helper.MainCab.transform.position + new Vector3(0f, 0.8f, 0f);

                        gameObject.transform.position = Vector3.Lerp(gameObject.transform.position, b, Time.deltaTime * 5f);

                        float num = Vector3.Distance(gameObject.transform.position, b);

                        if (num < 3f)
                        {
                            Pickupable pickupable = gameObject.GetComponentInChildren<Pickupable>();
                                                                                   
                            if (pickupable)
                            {
                                if (!helper.HasRoomForItem(pickupable))
                                {
                                    if (helper.IsPiloted())
                                    {
                                        ErrorMessage.AddMessage(Language.main.Get("ContainerCantFit"));
                                    }
                                }
                                else
                                {
                                    string arg = Language.main.Get(pickupable.GetTechName());
                                    ErrorMessage.AddMessage(Language.main.GetFormat("VehicleAddedToStorage", arg));
                                    uGUI_IconNotifier.main.Play(pickupable.GetTechType(), uGUI_IconNotifier.AnimationType.From, null);
                                    pickupable.Initialize();
                                    InventoryItem item = new InventoryItem(pickupable);
                                    helper.GetRoomForItem(pickupable).UnsafeAdd(item);
                                    pickupable.PlayPickupSound();
                                }
                                list.Add(gameObject);
                            }
                        }
                    }
                }

                if (list.Count > 0)
                {
                    foreach (GameObject item2 in list)
                    {
                        lootPinataObjects.Remove(item2);
                    }
                }
            }
        }        
    }

}

