using UnityEngine;
using TMPro;

public class DamagePopup : MonoBehaviour
{
    private TextMeshPro textMesh;
    private float disappearTimer = 1f;
    private Vector3 moveVector;

    public static void Create(Vector3 position, int damage, bool isCritical)
    {
        GameObject popupObj = new GameObject("DamagePopup");
        popupObj.transform.position = position;
        var popup = popupObj.AddComponent<DamagePopup>();
        popup.Setup(damage, isCritical);
    }

    private void Awake()
    {
        textMesh = gameObject.AddComponent<TextMeshPro>();
        textMesh.fontSize = 36;
        textMesh.alignment = TextAlignmentOptions.Center;
        moveVector = new Vector3(0, 1f, 0);
    }

    private void Setup(int damage, bool isCritical)
    {
        textMesh.text = damage.ToString();
        textMesh.color = isCritical ? Color.red : Color.white;
    }

    private void Update()
    {
        transform.position += moveVector * Time.deltaTime;
        disappearTimer -= Time.deltaTime;
        if (disappearTimer <= 0)
        {
            Destroy(gameObject);
        }
    }
}