using UnityEngine;

public class ClosePrivacyButtonScript : MonoBehaviour
{
    public GameObject privacyPanel; // Gán PrivacyPanel qua Inspector

    public void OnClose()
    {
        if (privacyPanel != null)
        {
            privacyPanel.SetActive(false); // Ẩn bảng khi nhấp Close
        }
    }
}