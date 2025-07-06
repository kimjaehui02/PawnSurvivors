using UnityEngine;

public class PlayerOLD : Pawn
{
    // 이동가능합니다
    private MoveableComponent m_Moveable;
    // 체력이 존재합니다
    private DamageableComponent m_Damageable;
    // 그래픽이 존재합니다
    private GraphicComponent m_Graphic;
    // 플레이어의 이동로직
    private PlayerMoverComponent m_PlayerMover;


    public override void PawnAwake() // Pawn 클래스에서 정의된 가상 메서드라고 가정
    {
        // 컴포넌트 참조 얻기
        m_Moveable = GetComponent<MoveableComponent>(); // 다른 컴포넌트도 여기서 초기화
        m_Damageable = GetComponent<DamageableComponent>();
        m_Graphic = GetComponent<GraphicComponent>();
        m_PlayerMover = GetComponent<PlayerMoverComponent>();

        // DamageableComponent의 OnHit 이벤트 구독
        // 메서드 이름만 전달해야 합니다!
        if (m_Damageable != null) // Null 체크는 항상 좋습니다.
        {
            m_Damageable.OnDamaged += OnPlayerHit; // OnHit과 이름 중복 방지를 위해 OnPlayerHit으로 변경
            m_Damageable.OnDamaged += OnPlayerHit22; // OnHit과 이름 중복 방지를 위해 OnPlayerHit으로 변경
            //만약 플레이어 사망 시 특정 로직이 있다면, OnDeath도 구독할 수 있습니다.
            //m_Damageable.OnDeath += OnPlayerDeath;
        }
    }

    public override void PawnUpdate()
    {
        //m_Moveable.Move(m_PlayerMover.GetPlayerMovementInput());
    }



    // 플레이어가 피격되었을 때 호출될 메서드
    private void OnPlayerHit(float input) // 이벤트 핸들러 메서드 이름은 보통 On[EventName] 형태로 짓습니다.
    {
        Debug.Log("Player was hit!");
        // 여기에 플레이어가 피격되었을 때 수행할 로직을 추가합니다.
        // 예: 피격 애니메이션 재생, 피격 사운드 재생, 화면 깜빡임 효과 등.
        if (m_Graphic != null)
        {
            //m_Graphic.PlayHitAnimation(); // GraphicComponent에 PlayHitAnimation() 추가 필요
        }
    }

    private void OnPlayerHit22(float input) // 이벤트 핸들러 메서드 이름은 보통 On[EventName] 형태로 짓습니다.
    {
        Debug.Log("Player was hit!22");
        // 여기에 플레이어가 피격되었을 때 수행할 로직을 추가합니다.
        // 예: 피격 애니메이션 재생, 피격 사운드 재생, 화면 깜빡임 효과 등.
        if (m_Graphic != null)
        {
            //m_Graphic.PlayHitAnimation(); // GraphicComponent에 PlayHitAnimation() 추가 필요
        }
    }

    #region Default

    private void Awake()
    {
        PawnAwake();
    }

    private void Start()
    {
        m_Damageable.TakeDamage(100);


    }
    private void Update()
    {
        PawnUpdate();
    }

    #endregion
}
