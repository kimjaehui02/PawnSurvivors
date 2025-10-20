using UnityEngine;
using UnityEngine.InputSystem;

public class mono : MonoBehaviour
{
    /// <summary>
    /// 플레이어 입력(WASD/화살표 등)을 받아 이동 방향을 계산합니다.
    /// </summary>
    public void GetPlayerMovementInput(out Vector2 inputDirection)
    {
        inputDirection = Vector2.zero;

        // 새 Input System 사용 중일 때
        if (Keyboard.current != null)
        {
            if (Keyboard.current.wKey.isPressed) inputDirection.y += 1;
            if (Keyboard.current.sKey.isPressed) inputDirection.y -= 1;
            if (Keyboard.current.aKey.isPressed) inputDirection.x -= 1;
            if (Keyboard.current.dKey.isPressed) inputDirection.x += 1;
        }

        inputDirection = inputDirection.normalized;
    }

    /// <summary>
    /// 오브젝트를 이동시킵니다. (2D 기준)
    /// </summary>
    public void Move(GameObject attacker, Vector2 inputDirection, float moveSpeed)
    {
        if (attacker == null) return;

        // 이동 계산
        Vector3 movement = new Vector3(inputDirection.x, inputDirection.y, 0) * moveSpeed * Time.deltaTime;

        attacker.transform.position += movement;
    }

    // Update is called once per frame
    public float moveSpeed = 5f;

    void Update()
    {
        // 1️⃣ 입력 받아오기
        GetPlayerMovementInput(out Vector2 inputDirection);

        // 2️⃣ 이동 처리
        Move(gameObject, inputDirection, moveSpeed);
    }
}
