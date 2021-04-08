using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

namespace SeaTruckScannerModule
{
    public class uGUI_SeaTruckScannerCancel : MonoBehaviour, IPointerEnterHandler, IEventSystemHandler, IPointerExitHandler, IPointerClickHandler
    {
        public uGUI_SeaTruckScanner mainUI;

        public Image image;

        private void Awake()
        {
            DestroyImmediate(GetComponent<uGUI_MapRoomCancel>());

            mainUI = transform.GetComponentInParent<uGUI_SeaTruckScanner>();
            image = GetComponent<Image>();
        }

        private void Start()
        {
            image.color = Color.grey;
        }

        public void OnPointerEnter(PointerEventData eventData)
        {
            image.color = Color.yellow;
        }

        public void OnPointerExit(PointerEventData eventData)
        {
            image.color = Color.grey;
        }

        public void OnPointerClick(PointerEventData eventData)
        {
            mainUI.OnCancelScan();
            image.color = Color.grey;
        }        
    }
}
