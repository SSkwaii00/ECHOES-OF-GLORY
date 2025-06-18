using UnityEngine;

public class EnemyCombatantBase : MonoBehaviour
{
    private Combatant combatant;

    void Start()
    {
        combatant = GetComponent<Combatant>();
        // Thay ElementType bằng Element
        if (combatant != null && combatant.Element == "Fire")
        {
            // Logic cho element Fire
        }
    }
}