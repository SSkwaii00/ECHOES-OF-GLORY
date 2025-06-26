using UnityEngine;

public class CloseButtonScript : MonoBehaviour
{
    public GameObject termsPanel; // Gán TermsPanel qua Inspector

    public void OnClose()
    {
        if (termsPanel != null)
        {
            termsPanel.SetActive(false); // Ẩn bảng khi nhấp Close
        }
    }
}