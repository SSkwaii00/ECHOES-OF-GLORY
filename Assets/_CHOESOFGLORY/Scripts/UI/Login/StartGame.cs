using UnityEngine;
using UnityEngine.SceneManagement;

public class StartGame : MonoBehaviour
{
    public void OnStartGame()
    {
        SceneManager.LoadScene("GameScene"); // Thay b?ng Scene ti?p theo
    }
}