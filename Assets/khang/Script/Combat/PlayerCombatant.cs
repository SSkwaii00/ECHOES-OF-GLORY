using UnityEngine;

public class PlayerCombatant : MonoBehaviour
{
    private Combatant combatant;

    void Start()
    {
        combatant = GetComponent<Combatant>();
        if (combatant != null)
        {
            combatant.GainEnergy(10); // Thêm năng lượng
        }
    }

    void Update()
    {
        if (combatant != null)
        {
            Debug.Log("Potential Energy: " + combatant.PotentialEnergy);
        }
    }
}