using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using TMPro;
using SMLHelper.V2.Utility;

namespace SeaTruckScannerModule
{
    public class uGUI_SeaTruckScanner : MonoBehaviour, IEventSystemHandler
    {        
        public SeaTruckScannerModuleManager manager;
        public SeaTruckSegmentListener segmentListener;

        public uGUI_GraphicRaycaster raycaster;

        private Transform scanner_cullable;
        public GameObject resourceListRoot;        
        public GameObject scanningRoot;
        public GameObject nextPageButton;
        public GameObject prevPageButton;

        public TextMeshProUGUI activeTechTypeLabel;        
        public TextMeshProUGUI scanningText;
        public TextMeshProUGUI navText;

        public uGUI_Icon scanningIcon;
        public List<uGUI_SeaTruckResourceNode> resourceList = new List<uGUI_SeaTruckResourceNode>();

        public FMODAsset startScanningSound;
        public FMODAsset cancelScanningSound;
        public FMODAsset hoverSound;
        public FMODAsset pageChangeSound;        

        private TechType lastActiveTechType;

        private readonly HashSet<TechType> availableTechTypes = new HashSet<TechType>();

        private readonly List<TechType> sortedTechTypes = new List<TechType>();

        private int currentPage;

        private readonly Color disabledColor = new Color(0.5f, 0.5f, 0.5f, 0.3f);

        private readonly Color defaultColor = new Color(0.7f, 0.7f, 0.7f, 1f);

        private readonly Color hoverColor = Color.white;

        [AssertLocalization]
        private const string scanningMessage = "MapRoomScanningText";

        [AssertLocalization(2)]
        private const string pagesFormatText = "MapRoomPagesFormat";

        private int Pages
        {
            get
            {
                float total = sortedTechTypes.Count;
                float perPage = resourceList.Count;
                return Mathf.CeilToInt(total / perPage);
            }
        }

        public void Awake()
        {
            manager = GetComponentInParent<SeaTruckScannerModuleManager>();
            raycaster = GetComponent<uGUI_GraphicRaycaster>();
            scanner_cullable = transform.Find("scanner_cullable");

            List<uGUI_MapRoomResourceNode> mapRoomNodes = new List<uGUI_MapRoomResourceNode>();
            scanner_cullable.Find("list").gameObject.GetComponentsInChildren(true, mapRoomNodes);

            List<uGUI_SeaTruckResourceNode> seatruckNodes = new List<uGUI_SeaTruckResourceNode>();

            foreach (uGUI_MapRoomResourceNode mapRoomNode in mapRoomNodes)
            {
                uGUI_SeaTruckResourceNode seatruckNode = mapRoomNode.gameObject.AddComponent<uGUI_SeaTruckResourceNode>();
                resourceList.Add(seatruckNode);
            }

            

            resourceListRoot = scanner_cullable.Find("list").gameObject;
            scanningRoot = scanner_cullable.Find("scanning").gameObject;
            nextPageButton = scanner_cullable.Find("list/nav/scroll_down").gameObject;
            SetNextPageButtonEventTriggers();
            prevPageButton = scanner_cullable.Find("list/nav/scroll_up").gameObject;
            SetPrevPageButtonEventTriggers();

            scanner_cullable.Find("scanning/cancelButton").gameObject.AddComponent<uGUI_SeaTruckScannerCancel>();

            activeTechTypeLabel = scanner_cullable.Find("scanning/activeTechType").GetComponent<TextMeshProUGUI>();
            scanningText = scanner_cullable.Find("scanning/scanningText").GetComponent<TextMeshProUGUI>();
            navText = scanner_cullable.Find("list/nav/pagenum").GetComponent<TextMeshProUGUI>();

            scanningIcon = scanner_cullable.Find("scanning/icon").GetComponent<uGUI_Icon>();            

            startScanningSound = ScriptableObject.CreateInstance<FMODAsset>();
            startScanningSound.name = "scan";
            startScanningSound.path = "event:/sub/base/map room/scan";

            cancelScanningSound = ScriptableObject.CreateInstance<FMODAsset>();
            cancelScanningSound.name = "option_tweek";
            cancelScanningSound.path = "event:/interface/option_tweek";

            hoverSound = ScriptableObject.CreateInstance<FMODAsset>();
            hoverSound.name = "select";
            hoverSound.path = "event:/sub/base/map room/select";

            pageChangeSound = ScriptableObject.CreateInstance<FMODAsset>();
            pageChangeSound.name = "text_type";
            pageChangeSound.path = "event:/interface/text_type";

            segmentListener = transform.parent.GetComponentInParent<SeaTruckSegmentListener>();

            segmentListener.onPlayerExited += OnPlayerExit;

            Sprite scanning_background = ImageUtils.LoadSpriteFromFile($"{Main.modFolder}/Assets/scanning_background_02.png");
            Sprite scanning_foreground = ImageUtils.LoadSpriteFromFile($"{Main.modFolder}/Assets/scanning_foreground_02.png");
            
            GameObject animation = scanningRoot.transform.Find("animation").gameObject;

            GameObject scanningBackground = new GameObject("scanningBackground", new Type[] { typeof(RectTransform) });
            scanningBackground.transform.SetParent(scanningRoot.transform, false);
            scanningBackground.transform.SetSiblingIndex(0);
            scanningBackground.transform.localScale = new Vector3(0.62f, 0.62f, 0.62f);
            scanningBackground.transform.localPosition = new Vector3(0, -12f, 0);
            Image newScanningBackground = scanningBackground.AddComponent<Image>();
            newScanningBackground.sprite = scanning_background;
            
            Image animationImage = animation.GetComponent<Image>();
            animationImage.sprite = scanning_foreground;

            uGUI_RotateImage rotateImage = animation.GetComponent<uGUI_RotateImage>();
            rotateImage.rotationTime = -4;
           
        }
               
        public void Start()
        {
            UpdateAvailableTechTypes();

            for (int i = 0; i < resourceList.Count; i++)
            {                
                resourceList[i].index = i;
                resourceList[i].mainUI = this;
                resourceList[i].hoverSound = hoverSound;
            }

            ResourceTrackerDatabase.onResourceDiscovered += OnResourceDiscovered;

            scanningText.text = Language.main.Get("MapRoomScanningText");            
        }
        
        public void HoverNextPageEnter()
        {
            if (currentPage < Pages - 1)
            {
                nextPageButton.GetComponent<Image>().color = hoverColor;
            }
        }

        public void HoverNextPageExit()
        {
            if (currentPage < Pages - 1)
            {
                nextPageButton.GetComponent<Image>().color = defaultColor;
            }
        }

        public void HoverPrevPageEnter()
        {
            if (currentPage > 0)
            {
                prevPageButton.GetComponent<Image>().color = hoverColor;
            }
        }

        public void HoverPrevPageExit()
        {
            if (currentPage > 0)
            {
                prevPageButton.GetComponent<Image>().color = defaultColor;
            }
        }

        public void NextPage()
        {
            if (currentPage < Pages - 1)
            {
                currentPage++;
                RebuildResourceList();
                Utils.PlayFMODAsset(pageChangeSound);
            }
        }

        public void PreviousPage()
        {
            if (currentPage > 0)
            {
                currentPage--;
                RebuildResourceList();
                Utils.PlayFMODAsset(pageChangeSound);
            }
        }

        public void OnPlayerExit()
        {           
            raycaster.enabled = false;
        }


        private void OnTriggerEnter(Collider c)
        {
            if (c.GetComponent<Player>())
            {
                raycaster.enabled = true;
            }
        }

        private void OnTriggerExit(Collider c)
        {
            if (c.GetComponent<Player>())
            {
                raycaster.enabled = false;
            }
        }

        private void OnDisable()
        {
            if (manager.GetActiveTechType() != TechType.None)
            {
                OnCancelScan();
            }
        }

        public void OnResourceDiscovered(ResourceTrackerDatabase.ResourceInfo info)
        {
            if (!availableTechTypes.Contains(info.techType))
            {
                Vector3 vector = manager.transform.position - info.position;
                float scanRange = manager.GetScanRange();
                float num = scanRange * scanRange;
                if (vector.sqrMagnitude <= num)
                {
                    availableTechTypes.Add(info.techType);
                    RebuildResourceList();                    
                }
            }
        }

        public void OnCancelScan()
        {
            manager.OnScanning = false;
            manager.StartScanning(TechType.None);
            UpdateGUIState();           
        }

        public void OnStartScan(int index)
        {
            CancelInvoke("UpdateAvailableTechTypes");
            index = Mathf.Clamp(index + currentPage * resourceList.Count, 0, sortedTechTypes.Count - 1);
            manager.StartScanning(sortedTechTypes[index]);
            UpdateGUIState();            
            manager.OnScanning = true;
        }

        public void UpdateAvailableTechTypes()
        {            
            availableTechTypes.Clear();
            ResourceTrackerDatabase.GetTechTypesInRange(manager.transform.position, manager.GetScanRange(), availableTechTypes);
            RebuildResourceList();
        }

        private void RebuildResourceList()
        {
            sortedTechTypes.Clear();
            sortedTechTypes.AddRange(availableTechTypes);
            sortedTechTypes.Sort(new Comparison<TechType>(CompareByName));
            int num = currentPage * resourceList.Count;
            int num2 = 0;
            int num3 = Mathf.Min(sortedTechTypes.Count, num + resourceList.Count);

            for (int i = num; i < num3; i++)
            {
                TechType techType = sortedTechTypes[i];
                uGUI_SeaTruckResourceNode uGUI_SeaTruckResourceNode = resourceList[num2];
                uGUI_SeaTruckResourceNode.gameObject.SetActive(true);
                uGUI_SeaTruckResourceNode.SetTechType(techType);
                num2++;
            }

            for (int j = num2; j < resourceList.Count; j++)
            {
                resourceList[j].gameObject.SetActive(false);
            }

            navText.text = Language.main.GetFormat("MapRoomPagesFormat", currentPage + 1, Pages);
            prevPageButton.GetComponent<Image>().color = ((currentPage != 0) ? defaultColor : disabledColor);
            nextPageButton.GetComponent<Image>().color = ((currentPage + 1 < Pages) ? defaultColor : disabledColor);
        }

        private static int CompareByName(TechType a, TechType b)
        {
            string strA = Language.main.Get(a.AsString(false));
            string strB = Language.main.Get(b.AsString(false));
            return string.Compare(strA, strB, StringComparison.OrdinalIgnoreCase);
        }

        private void UpdateGUIState()
        {
            TechType activeTechType = manager.GetActiveTechType();
            resourceListRoot.SetActive(activeTechType == TechType.None);
            scanningRoot.SetActive(activeTechType > TechType.None);

            if (lastActiveTechType != activeTechType)
            {
                if (activeTechType != TechType.None)
                {
                    Sprite sprite = SpriteManager.Get(activeTechType, null);

                    if (sprite != null)
                    {
                        scanningIcon.sprite = sprite;
                        scanningIcon.enabled = true;                        
                    }
                    else
                    {                        
                        scanningIcon.enabled = false;
                    }

                    activeTechTypeLabel.text = Language.main.Get(activeTechType.AsString(false));

                    Utils.PlayFMODAsset(startScanningSound, transform, 20f);
                }
                else
                {
                    Utils.PlayFMODAsset(cancelScanningSound, transform, 20f);
                }
            }

            lastActiveTechType = activeTechType;
        }        

        private void OnDestroy()
        {
            ResourceTrackerDatabase.onResourceDiscovered -= OnResourceDiscovered;
            segmentListener.onPlayerExited -= OnPlayerExit;
        }        

        private void SetNextPageButtonEventTriggers()
        {
            EventTrigger nextPageButton_Events = nextPageButton.GetComponent<EventTrigger>();

            foreach (EventTrigger.Entry entry in nextPageButton_Events.triggers)
            {
                entry.callback.RemoveAllListeners();
            }

            nextPageButton_Events.triggers.Clear();

            EventTrigger.Entry nextPageButton_PointerDown = new EventTrigger.Entry
            {
                eventID = EventTriggerType.PointerDown
            };
            nextPageButton_PointerDown.callback.AddListener((data) => { NextPage(); });
            nextPageButton_Events.triggers.Add(nextPageButton_PointerDown);
            
            EventTrigger.Entry nextPageButton_PointerEnter = new EventTrigger.Entry
            {
                eventID = EventTriggerType.PointerEnter
            };
            nextPageButton_PointerEnter.callback.AddListener((data) => { HoverNextPageEnter (); });
            nextPageButton_Events.triggers.Add(nextPageButton_PointerEnter);
            
            EventTrigger.Entry nextPageButton_PointerExit = new EventTrigger.Entry
            {
                eventID = EventTriggerType.PointerExit
            };
            nextPageButton_PointerExit.callback.AddListener((data) => { HoverNextPageExit (); });
            nextPageButton_Events.triggers.Add(nextPageButton_PointerExit);
            
        }

        private void SetPrevPageButtonEventTriggers()
        {
            EventTrigger prevPageButton_Events = prevPageButton.GetComponent<EventTrigger>();

            foreach (EventTrigger.Entry entry in prevPageButton_Events.triggers)
            {
                entry.callback.RemoveAllListeners();
            }

            prevPageButton_Events.triggers.Clear();

            EventTrigger.Entry prevPageButton_PointerDown = new EventTrigger.Entry
            {
                eventID = EventTriggerType.PointerDown
            };
            prevPageButton_PointerDown.callback.AddListener((data) => { PreviousPage(); });
            prevPageButton_Events.triggers.Add(prevPageButton_PointerDown);
            
            EventTrigger.Entry prevPageButton_PointerEnter = new EventTrigger.Entry
            {
                eventID = EventTriggerType.PointerEnter
            };
            prevPageButton_PointerEnter.callback.AddListener((data) => { HoverPrevPageEnter (); });
            prevPageButton_Events.triggers.Add(prevPageButton_PointerEnter);
            
            EventTrigger.Entry prevPageButton_PointerExit = new EventTrigger.Entry
            {
                eventID = EventTriggerType.PointerExit
            };
            prevPageButton_PointerExit.callback.AddListener((data) => { HoverPrevPageExit (); });
            prevPageButton_Events.triggers.Add(prevPageButton_PointerExit);            
        }
    }
}
