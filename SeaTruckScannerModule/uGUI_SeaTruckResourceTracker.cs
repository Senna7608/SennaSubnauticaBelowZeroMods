using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

namespace SeaTruckScannerModule
{
    public class uGUI_SeaTruckResourceTracker : MonoBehaviour
    {                       
        public GameObject mainCanvas;        
        public GameObject blip;
        private bool showGUI;
        private bool showDistance;
        private bool gatherNextTick;
        private RectTransform blipRect;
        private RectTransform canvasRect;
        private const ManagedUpdate.Queue updateQueue = ManagedUpdate.Queue.Ping;
        private readonly List<Blip> blips = new List<Blip>();
        private readonly List<TechType> techTypes = new List<TechType>();
        private readonly List<SeaTruckScannerModuleManager> scannerModules = new List<SeaTruckScannerModuleManager>();
        private readonly HashSet<ResourceTrackerDatabase.ResourceInfo> nodes = new HashSet<ResourceTrackerDatabase.ResourceInfo>();

        public void GetSeaTruckScannerModulesInRange(Vector3 position, float range, ICollection<SeaTruckScannerModuleManager> outlist)
        {
            for (int i = 0; i < SeaTruckScannerModule_Main.scannerModules.Count; i++)
            {
                SeaTruckScannerModuleManager manager = SeaTruckScannerModule_Main.scannerModules[i];

                if ((manager.transform.position - position).sqrMagnitude <= range * range)
                {
                    outlist.Add(manager);
                }
            }
        }

        private void Awake()
        {
            GameObject resourceTracker = uGUI.main.gameObject.transform.Find("ScreenCanvas/ResourceTracker").gameObject;

            uGUI_ResourceTracker uGUI_ResourceTracker = resourceTracker.GetComponent<uGUI_ResourceTracker>();

            blip = UWE.Utils.InstantiateDeactivated(resourceTracker.transform.Find("blip").gameObject);
            blip.transform.SetParent(transform, false);
            blip.name = "blip";

            GameObject distanceText = Instantiate(blip.transform.Find("text").gameObject, blip.transform);
            distanceText.name = "distanceText";
            distanceText.SetActive(false);

            Color color = new Color(1, 0, 0, 1);

            TextMeshProUGUI tmproDist = distanceText.GetComponent<TextMeshProUGUI>();
            tmproDist.color = color;

            TextMeshProUGUI tmproText = blip.transform.Find("text").GetComponent<TextMeshProUGUI>();
            tmproText.color = color;

            RectTransformExtensions.CenterRect(distanceText.transform as RectTransform, Vector2.zero);

            Image image = blip.GetComponent<Image>();
            image.color = color;
            
            mainCanvas = UWE.Utils.InstantiateDeactivated(resourceTracker.transform.Find("mainCanvas").gameObject);
            mainCanvas.transform.SetParent(transform, false);
            mainCanvas.name = "mainCanvas";
        }
        
        private void Start()
        {
            blipRect = blip.GetComponent<RectTransform>();
            canvasRect = mainCanvas.GetComponent<RectTransform>();
            InvokeRepeating("UpdateVisibility", Random.value, 0.1f);            
            InvokeRepeating("GatherNodes", Random.value, 10f);
            ResourceTrackerDatabase.onResourceRemoved += OnResourceRemoved;            
        }

        private void OnEnable()
        {
            ManagedUpdate.Subscribe(ManagedUpdate.Queue.Ping, new ManagedUpdate.OnUpdate(UpdateBlips));
        }

        private void OnDisable()
        {
            ManagedUpdate.Unsubscribe(ManagedUpdate.Queue.Ping, new ManagedUpdate.OnUpdate(UpdateBlips));
        }

        private void OnDestroy()
        {
            ResourceTrackerDatabase.onResourceRemoved -= OnResourceRemoved;            
        }

        private void OnResourceRemoved(ResourceTrackerDatabase.ResourceInfo info)
        {
            gatherNextTick = true;
        }        

        private void UpdateVisibility()
        {
            bool HUDChipCount = Inventory.main != null && Inventory.main.equipment.GetCount(SeaTruckScannerHUDChip_Prefab.TechTypeID) > 0;
            showDistance = Inventory.main != null && Inventory.main.equipment.GetCount(SeaTruckScannerHUDChipUpgrade_Prefab.TechTypeID) > 0;

            showGUI = HUDChipCount || showDistance;            

            if (showGUI)
            {
                showGUI = Player.main != null && !Player.main.cinematicModeActive;
            }

            mainCanvas.SetActive(showGUI);
        }

        private void GatherNodes()
        {            
            if (showGUI)
            {
                GatherScanned();
            }
        }               

        private void GatherScanned()
        {
            nodes.Clear();
            scannerModules.Clear();

            GetSeaTruckScannerModulesInRange(MainCamera.camera.transform.position, 150f, scannerModules);
            
            for (int i = 0; i < scannerModules.Count; i++)
            {
                if (scannerModules[i].GetActiveTechType() != TechType.None)
                {
                    scannerModules[i].GetDiscoveredNodes(nodes);
                }
            }
        }

        private void UpdateBlips()
        {
            Camera camera = MainCamera.camera;
            Vector3 position = camera.transform.position;
            Vector3 forward = camera.transform.forward;
            int num = 0;

            foreach (ResourceTrackerDatabase.ResourceInfo resourceInfo in nodes)
            {
                if (Vector3.Dot(resourceInfo.position - position, forward) > 0f)
                {
                    if (num >= blips.Count)
                    {
                        GameObject blipClone = Instantiate(blip, Vector3.zero, Quaternion.identity);
                        RectTransform rt = blipClone.GetComponent<RectTransform>();
                        rt.SetParent(canvasRect, false);
                        rt.localScale = blipRect.localScale;

                        Blip newBlip = new Blip
                        {
                            gameObject = blipClone,
                            rect = rt,
                            text = blipClone.transform.Find("text").GetComponent<TextMeshProUGUI>(),
                            distanceText = blipClone.transform.Find("distanceText").GetComponent<TextMeshProUGUI>(),
                            techType = TechType.None
                        };

                        blips.Add(newBlip);
                    }

                    Blip blip2 = blips[num];
                    blip2.gameObject.SetActive(true);
                    Vector2 vector = camera.WorldToViewportPoint(resourceInfo.position);
                    blip2.rect.anchorMin = vector;
                    blip2.rect.anchorMax = vector;

                    if (blip2.techType != resourceInfo.techType)
                    {
                        blip2.text.text = Language.main.Get(resourceInfo.techType.AsString(false));
                        blip2.techType = resourceInfo.techType;
                    }

                    if (showDistance)
                    {
                        float distance = Vector3.Distance(camera.transform.position, resourceInfo.position);
                        blip2.distanceText.text = $"{IntStringCache.GetStringForInt((int)distance)} m";
                        
                        if (!blip2.distanceText.gameObject.activeSelf)
                        {
                            blip2.distanceText.gameObject.SetActive(true);
                        }
                    }
                    else if (blip2.distanceText.gameObject.activeSelf)
                    {
                        blip2.distanceText.gameObject.SetActive(false);
                    }

                    num++;
                }
            }

            for (int i = num; i < blips.Count; i++)
            {
                blips[i].gameObject.SetActive(false);
            }
        }

        private void LateUpdate()
        {
            if (gatherNextTick)
            {
                GatherScanned();
                gatherNextTick = false;
            }
        }

        private class Blip
        {
            public GameObject gameObject;

            public RectTransform rect;

            public TextMeshProUGUI text;

            public TextMeshProUGUI distanceText;

            public TechType techType;
        }        
    }
}