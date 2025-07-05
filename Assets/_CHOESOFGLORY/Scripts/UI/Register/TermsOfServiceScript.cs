using UnityEngine;
using UnityEngine.EventSystems;

public class TermsOfServiceScript : MonoBehaviour, IPointerClickHandler
{
    public GameObject termsPanel; // Gán TermsPanel qua Inspector

    public void OnPointerClick(PointerEventData eventData)
    {
        if (termsPanel != null)
        {
            termsPanel.SetActive(true); // Hiển thị bảng khi nhấp
        }
    }
}