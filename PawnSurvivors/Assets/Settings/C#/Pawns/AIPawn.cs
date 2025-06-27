using UnityEngine;

public class AIPawn : Pawn
{
    // 이동가능합니다
    private MoveableComponent m_Moveable;
    // 체력이 존재합니다
    private DamageableComponent m_Damageable;
    // 체력이 존재합니다
    private DamageDealerComponent m_DamageDealer;
    // 그래픽이 존재합니다
    private GraphicComponent m_Graphic;

    public override void PawnAwake() // Pawn 클래스에서 정의된 가상 메서드라고 가정
    {
        // 컴포넌트 참조 얻기
        m_Moveable = GetComponent<MoveableComponent>(); // 다른 컴포넌트도 여기서 초기화
        m_Graphic = GetComponent<GraphicComponent>();

        m_Damageable = GetComponent<DamageableComponent>();
        m_DamageDealer = GetComponent<DamageDealerComponent>();


    }

    private void Awake()
    {
        PawnAwake();
    }


}
