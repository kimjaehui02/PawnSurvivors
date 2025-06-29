using System;
using UnityEngine;

public class AIPawn : Pawn
{
    public GameObject player; // Inspector에서 플레이어 GameObject를 드래그앤드롭하세요.

    // 이동가능합니다
    //private IMoveable m_Moveable;
    // 체력이 존재합니다
    //private IDamageable m_Damageable;
    // 체력이 존재합니다
    //private IDamageDealer m_DamageDealer;
    // 그래픽이 존재합니다
    //private IGraphic m_Graphic;
    // 플레이어의 이동로직
    //private IPawnMover m_PawnMover;

    event IMoveable.MoveableDelegate OnMove;

    // 지금은 컴포넌트를 직접 다 알고잇는데
    // 컴포넌트는 하나도 모르고 이벤트만 알고싶음


    public override void PawnAwake() // Pawn 클래스에서 정의된 가상 메서드라고 가정
    {
        // 컴포넌트 참조 얻기
        //m_Moveable = GetComponent<IMoveable>(); // 다른 컴포넌트도 여기서 초기화
        //m_Graphic = GetComponent<IGraphic>();

        //m_Damageable = GetComponent<IDamageable>();
        //m_DamageDealer = GetComponent<IDamageDealer>();

        //m_PawnMover = GetComponent<IPawnMover>();

    }

    public override void PawnUpdate()
    {
        if (player != null) // null 참조 오류 방지를 위해 player가 할당되었는지 확인
        {
            // --- 이 줄이 변경되었습니다! ---
            // player.GetComponent<Vector3>() 대신 player.transform.position을 사용합니다.
            //m_Moveable.Move(m_PawnMover.GetFaceDirection(player.transform.position));
            OnMove(player.transform.position);
        }
        else
        {
            //Debug.LogWarning("AIPawn: 추적할 player GameObject가 할당되지 않았습니다!");
            //m_Moveable.Move(Vector3.zero); // 플레이어가 없으면 움직이지 않음
        }
    }

    #region Default
    private void Start()
    {
        
    }

    private void Awake()
    {
        PawnAwake();
    }
    private void Update()
    {
        PawnUpdate();
    }
    #endregion
}