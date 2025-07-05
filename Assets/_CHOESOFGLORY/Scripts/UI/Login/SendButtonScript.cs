using UnityEngine;
using UnityEngine.SceneManagement; // Thêm để sử dụng SceneManager

public class SendButtonScript : MonoBehaviour
{
    public void OnSendClick()
    {
        SceneManager.LoadScene("SendCode"); // Chuyển sang Scene SendCode
    }
}