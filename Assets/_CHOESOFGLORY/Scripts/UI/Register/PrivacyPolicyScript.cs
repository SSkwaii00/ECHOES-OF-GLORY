using UnityEngine;
using UnityEngine.EventSystems;

public class PrivacyPolicyScript : MonoBehaviour, IPointerClickHandler
{
    public GameObject privacyPanel; // Gán PrivacyPanel qua Inspector

    public void OnPointerClick(PointerEventData eventData)
    {
        if (privacyPanel != null)
        {
            privacyPanel.SetActive(true); // Hiển thị bảng khi nhấp
        }
    }
}