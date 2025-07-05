using UnityEngine;
using UnityEngine.UI;

public class ShowImageOnClick : MonoBehaviour
{
    public GameObject hiddenImage; // Gán image đã ẩn vào đây trong Inspector

    public void OnButtonClick()
    {
        if (hiddenImage != null)
        {
            hiddenImage.SetActive(true); // Hiển thị image
            Invoke("HideImage", 1f);    // Ẩn image sau 1 giây
        }
    }

    void HideImage()
    {
        if (hiddenImage != null)
        {
            hiddenImage.SetActive(false); // Ẩn image
        }
    }
}