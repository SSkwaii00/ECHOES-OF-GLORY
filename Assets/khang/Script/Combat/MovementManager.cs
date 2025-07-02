using UnityEngine;
using System.Collections;

public class MovementManager : MonoBehaviour
{
    [SerializeField] private float moveSpeed = 5f;
    private CombatManager combatManager;

    public void SetCombatManager(CombatManager manager)
    {
        combatManager = manager;
    }

    public IEnumerator MoveForAction(Transform mover, Transform target, AttackType attackType, float moveDistance, bool returnToStart = false, Vector3? originalPosition = null)
    {
        Vector3 startPosition = mover.position;
        Vector3 moveTarget = startPosition;

        // Xác định vị trí mục tiêu dựa trên AttackType
        if (attackType == AttackType.Melee)
        {
            moveTarget = target.position; // Di chuyển đến vị trí chính xác của mục tiêu
        }
        else if (attackType == AttackType.Ranged && moveDistance <= 0f)
        {
            moveTarget = startPosition; // Không di chuyển cho tầm xa
        }

        // Di chuyển đến mục tiêu
        if (Vector3.Distance(mover.position, moveTarget) > 0.1f)
        {
            Debug.Log("Moving " + mover.name + " to target: " + target.name + " at " + moveTarget);
            while (Vector3.Distance(mover.position, moveTarget) > 0.1f)
            {
                mover.position = Vector3.MoveTowards(mover.position, moveTarget, Time.deltaTime * moveSpeed);
                yield return null;
            }
            mover.position = moveTarget; // Đảm bảo vị trí chính xác
        }

        // Quay về vị trí ban đầu nếu yêu cầu
        if (returnToStart && originalPosition.HasValue)
        {
            Debug.Log("Returning " + mover.name + " to original position: " + originalPosition.Value);
            while (Vector3.Distance(mover.position, originalPosition.Value) > 0.1f)
            {
                mover.position = Vector3.MoveTowards(mover.position, originalPosition.Value, Time.deltaTime * moveSpeed);
                yield return null;
            }
            mover.position = originalPosition.Value; // Đảm bảo vị trí chính xác
        }
    }
}