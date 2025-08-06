using UnityEngine;

public class Broom : MonoBehaviour
{
    public float broomDetectionRadius = 3f;     // Broom 감지 반경
    public LayerMask broomLayer;               // Broom이 속한 레이어
    public float requiredTime = 3f;            // Broom이 머물러야 할 시간

    private float broomStayTimer = 0f;         // 누적 시간
    private bool isCleared = false;            // Dust 제거 여부

    void Update()
    {
        if (isCleared) return;

        // Dust 주변에 Broom 있는지 확인
        Collider[] nearbyBrooms = Physics.OverlapSphere(transform.position, broomDetectionRadius, broomLayer);

        if (nearbyBrooms.Length > 0)
        {
            // 타이머 누적
            broomStayTimer += Time.deltaTime;
        }
        else
        {
            // Broom이 없으면 타이머 초기화
            broomStayTimer = 0f;
        }
    }
    // 디버깅용 감지 범위 표시
    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, broomDetectionRadius);
    }
}
