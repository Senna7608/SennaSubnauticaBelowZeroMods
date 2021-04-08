using UnityEngine;
using UnityEngine.EventSystems;
using TMPro;

namespace SeaTruckScannerModule
{
    public class uGUI_SeaTruckResourceNode : MonoBehaviour, IPointerEnterHandler, IEventSystemHandler, IPointerExitHandler, IPointerClickHandler
    {        
        public uGUI_SeaTruckScanner mainUI;        
        public int index;
        public TextMeshProUGUI text;
        public GameObject hover;
        public GameObject background;
        public uGUI_Icon icon;        
        public FMODAsset hoverSound;

        public void Awake()
        {
            DestroyImmediate(GetComponent<uGUI_MapRoomResourceNode>());            

            text = transform.Find("text").GetComponent<TextMeshProUGUI>();
            hover = transform.Find("hover").gameObject;
            background = transform.Find("bg").gameObject;
            icon = transform.Find("icon").GetComponent<uGUI_Icon>();
        }

        private void Start()
        {
            hover.SetActive(false);
            background.SetActive(true);
        }

        public void SetTechType(TechType techType)
        {
            text.text = Language.main.Get(techType.AsString(false));

            Sprite sprite = SpriteManager.Get(techType, null);

            if (sprite != null)
            {
                icon.sprite = sprite;
                icon.enabled = true;
                return;
            }

            icon.enabled = false;
        }

        public void OnPointerEnter(PointerEventData eventData)
        {
            hover.SetActive(true);
            background.SetActive(false);
            Utils.PlayFMODAsset(hoverSound, transform, 20f);
        }

        public void OnPointerExit(PointerEventData eventData)
        {
            hover.SetActive(false);
            background.SetActive(true);
        }

        public void OnPointerClick(PointerEventData eventData)
        {
            mainUI.OnStartScan(index);
            hover.SetActive(false);
            background.SetActive(true);
        }        
    }
}
