using UnityEngine;
using UnityEngine.EventSystems;

public class ToggleMiniMenu : MonoBehaviour, IPointerClickHandler
{
    public GameObject miniMenu; // Gán GameObject miniMenuResolution vào đây trong Inspector

    public void OnPointerClick(PointerEventData eventData)
    {
        if (miniMenu != null)
        {
            miniMenu.SetActive(!miniMenu.activeSelf); // Chuyển đổi trạng thái hiển thị/ẩn khi nhấp vào image
        }
    }

    void Update()
    {
        if (Input.GetMouseButtonDown(0)) // Phát hiện nhấp chuột trái
        {
            if (miniMenu != null && miniMenu.activeSelf)
            {
                // Kiểm tra xem nhấp chuột có nằm ngoài GameObject hiện tại không
                PointerEventData pointerData = new PointerEventData(EventSystem.current)
                {
                    position = Input.mousePosition
                };
                var raycastResults = new System.Collections.Generic.List<RaycastResult>();
                EventSystem.current.RaycastAll(pointerData, raycastResults);

                bool clickedOutside = true;
                foreach (var result in raycastResults)
                {
                    if (result.gameObject == gameObject)
                    {
                        clickedOutside = false;
                        break;
                    }
                }

                if (clickedOutside)
                {
                    miniMenu.SetActive(false); // Ẩn miniMenu nếu nhấp ngoài
                }
            }
        }
    }
}