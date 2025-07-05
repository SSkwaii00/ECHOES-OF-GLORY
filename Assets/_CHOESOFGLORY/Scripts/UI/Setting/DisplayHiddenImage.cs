using UnityEngine;
using UnityEngine.EventSystems;
using TMPro;

public class DisplayHiddenImage : MonoBehaviour, IPointerClickHandler
{
    [SerializeField] private GameObject targetImage; // Gán image đã ẩn qua Inspector

    public void OnPointerClick(PointerEventData eventData)
    {
        if (IsImageValid())
        {
            ShowTargetImage();
        }
    }

    private bool IsImageValid()
    {
        return targetImage != null;
    }

    private void ShowTargetImage()
    {
        targetImage.SetActive(true);
    }
}