using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class SliderPercentage : MonoBehaviour
{
    public Slider slider;       // Gán Slider vào đây trong Inspector
    public TMP_Text percentageText; // Gán TextMeshPro vào đây trong Inspector

    void Start()
    {
        // Đảm bảo slider và text được gán
        if (slider != null && percentageText != null)
        {
            UpdatePercentageText(); // Cập nhật text ban đầu
            slider.onValueChanged.AddListener(delegate { UpdatePercentageText(); }); // Lắng nghe thay đổi giá trị
        }
    }

    void UpdatePercentageText()
    {
        if (slider != null && percentageText != null)
        {
            float percentage = slider.value * 100; // Chuyển giá trị slider (0-1) thành phần trăm (0-100)
            percentageText.text = Mathf.RoundToInt(percentage).ToString() + "%"; // Hiển thị phần trăm
        }
    }
}