using UnityEngine;

public class dust : MonoBehaviour
{
    public float broomDetectionRadius = 3f;   // Broom 감지 반경
    public LayerMask broomLayer;             // Broom이 속한 레이어
    public float requiredTime = 3f;          // 제거되기까지 필요한 시간

    private float broomStayTimer = 0f;       // 현재 머문 시간
    private bool isCleared = false;

    void Update()
    {
        if (isCleared) return;

        // 주변에 Broom 있는지 검사
        Collider[] nearbyBrooms = Physics.OverlapSphere(transform.position, broomDetectionRadius, broomLayer);

        if (nearbyBrooms.Length > 0)
        {
            // 타이머 누적
            broomStayTimer += Time.deltaTime;

            if (broomStayTimer >= requiredTime)
            {
                ClearDust();
            }
        }
        else
        {
            // Broom이 멀어졌으므로 타이머 초기화
            broomStayTimer = 0f;
        }
    }

    void ClearDust()
    {
        isCleared = true;
        Debug.Log("Dust cleared after Broom stayed nearby for " + requiredTime + " seconds.");
        Destroy(gameObject);
    }

    // 디버깅용: 감지 범위 시각화
    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.cyan;
        Gizmos.DrawWireSphere(transform.position, broomDetectionRadius);
    }
}
