using UnityEngine;
using UnityEngine.EventSystems;

public class CheckBoxImageScript : MonoBehaviour, IPointerClickHandler
{
    public GameObject checkmark; // Gán CheckmarkImage qua Inspector

    private bool isChecked = false;

    void Start()
    {
        if (checkmark != null)
        {
            checkmark.SetActive(isChecked); // Ẩn dấu tích ban đầu
        }
        else
        {
            Debug.LogWarning("Checkmark is not assigned in Inspector!", this);
        }
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        isChecked = !isChecked; // Đảo trạng thái khi nhấp
        if (checkmark != null)
        {
            checkmark.SetActive(isChecked); // Hiển thị hoặc ẩn dấu tích
            Debug.Log("Checkbox state changed to: " + isChecked); // Debug trạng thái
        }
    }
}