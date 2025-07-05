using UnityEngine;
using UnityEngine.UI;

public class clickButtonToImage : MonoBehaviour
{
    public GameObject hiddenImage; // Gán image đã ẩn vào đây trong Inspector

    public void OnButtonClick()
    {
        if (hiddenImage != null)
        {
            hiddenImage.SetActive(true); // Hiển thị image
        }
    }
}