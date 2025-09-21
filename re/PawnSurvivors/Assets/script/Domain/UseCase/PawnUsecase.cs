using UnityEngine;

public class PawnUseCase
{
    /// 목표를 향하는 방향을 계산합니다.
    public static void GetFaceDirection(DataContext dataContext)
    {
        if (dataContext == null)
        {
            Debug.LogWarning("GetFaceDirection: dataContext가 null입니다.");
            return;
        }

        if (dataContext.Target == null)
        {
            dataContext.InputDirection = Vector3.zero;
            return;
        }

        if (dataContext.Target.transform == null)
        {
            Debug.LogWarning($"GetFaceDirection: Target '{dataContext.Target.name}'의 transform이 null입니다.");
            dataContext.InputDirection = Vector3.zero;
            return;
        }

        Vector3 directionToTarget = dataContext.Target.transform.position - dataContext.Activator.transform.position;

        if (directionToTarget.magnitude > 0)
        {
            dataContext.InputDirection = directionToTarget.normalized;
            return;
        }

        dataContext.InputDirection = Vector3.zero;
    }

    /// 플레이어 이동 입력을 처리합니다.
    public static void GetPlayerMovementInput(DataContext dataContext)
    {
        Vector3 moveDirection = (Vector3)dataContext.InputDirection?.normalized;
        dataContext.InputDirection = moveDirection;
    }

    /// 오브젝트를 이동시킵니다.
    public static void Move(DataContext dataContext)
    {
        Vector3 movement = dataContext.MoveSpeed * Time.deltaTime * (dataContext.InputDirection ?? Vector3.zero);
        dataContext.Activator.transform.position += movement;
    }


}
