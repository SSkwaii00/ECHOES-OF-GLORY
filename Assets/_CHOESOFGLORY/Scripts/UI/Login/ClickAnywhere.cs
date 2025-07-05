using UnityEngine;
using UnityEngine.SceneManagement;

public class ClickAnywhere : MonoBehaviour
{
    void Update()
    {
        // Phát hiện nhấp chuột trái
        if (Input.GetMouseButtonDown(0))
        {
            // Chuyển sang Scene đăng nhập
            SceneManager.LoadScene("Login");
        }
    }
}