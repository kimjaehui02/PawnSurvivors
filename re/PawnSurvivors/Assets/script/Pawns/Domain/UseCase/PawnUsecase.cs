using UnityEngine;

public class PawnUseCase
{
    //private readonly InputActions _inputActions;

    //public PawnUseCase()
    //{
    //    _inputActions = new InputActions();



    //    _inputActions.Enable();
    //}



    /// 목표를 향하는 방향을 계산합니다.
    public static void GetFaceDirection(GameObject Target, GameObject Attacker, out Vector3 InputDirection)
    {


        if (Target == null)
        {
            InputDirection = Vector3.zero;
            return;
        }

        if (Target.transform == null)
        {
            Debug.LogWarning($"GetFaceDirection: Target '{Target.name}'의 transform이 null입니다.");
            InputDirection = Vector3.zero;
            return;
        }

        Vector3 directionToTarget = Target.transform.position - Attacker.transform.position;

        if (directionToTarget.magnitude > 0)
        {
            InputDirection = directionToTarget.normalized;
            return;
        }

        InputDirection = Vector3.zero;
    }

    /// 플레이어 이동 입력을 처리합니다.
    public static void GetPlayerMovementInput(out Vector3 InputDirection)
    {
        // InputDirection이 out 매개변수이므로, 먼저 값을 할당해야 합니다.
        InputDirection = Vector3.zero; // 기본값 할당
        Vector3 moveDirection = InputDirection.normalized;
        InputDirection = moveDirection;
    }

    /// 오브젝트를 이동시킵니다.
    public static void Move(GameObject attacker, Vector3 inputDirection, float moveSpeed)
    {
        Vector3 movement = moveSpeed * Time.deltaTime * inputDirection;
        attacker.transform.position += movement;
    }



}
