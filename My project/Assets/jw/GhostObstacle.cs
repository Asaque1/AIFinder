using UnityEngine;
using UnityEngine.SceneManagement;

/// <summary>
/// 유령 장애물이 일정 방향으로 움직였다가 되돌아오기를 반복하며,
/// 벽을 통과하고, 플레이어와 부딪히면 씬을 재시작하는 스크립트입니다.
/// Rigidbody.MovePosition을 사용해 물리 충돌 문제를 방지하며,
/// 이동 방향으로 유령이 자연스럽게 바라보게 합니다.
/// </summary>
[RequireComponent(typeof(Collider))]
[RequireComponent(typeof(Rigidbody))]
public class GhostObstacle : MonoBehaviour
{
    [Header("이동 설정")]
    public Vector3 moveDirection = Vector3.right;  // 이동 시작 방향 (기본 오른쪽)
    public float moveSpeed = 2f;                    // 이동 속도
    public float moveDistance = 5f;                 // 최대 이동 거리

    private Vector3 startPosition;                   // 유령 시작 위치 저장
    private Vector3 direction;                       // 현재 이동 방향 (단위 벡터)
    private Rigidbody rb;                            // Rigidbody 컴포넌트 참조

    [Header("충돌 설정")]
    public string targetTag = "player";              // 충돌 감지 대상 태그

    void Start()
    {
        rb = GetComponent<Rigidbody>();

        // Rigidbody가 물리엔진 영향을 받지 않고, 직접 움직임을 제어하도록 설정
        rb.isKinematic = true;
        rb.useGravity = false;

        // Collider는 Trigger로 설정해 벽을 통과하게 함
        Collider col = GetComponent<Collider>();
        col.isTrigger = true;

        // 시작 위치 저장
        startPosition = transform.position;

        // 이동 방향을 정규화(길이 1)하여 일정한 속도 유지
        direction = moveDirection.normalized;

        // 유령 처음 방향 바라보도록 설정
        UpdateLookDirection();
    }

    void Update()
    {
        // 이동할 다음 위치 계산
        Vector3 nextPos = transform.position + direction * moveSpeed * Time.deltaTime;

        // Rigidbody.MovePosition을 통해 안전하게 이동 처리
        rb.MovePosition(nextPos);

        // 시작 위치부터 이동 거리 계산
        float distance = Vector3.Distance(startPosition, nextPos);

        // 최대 이동 거리에 도달하면 방향 반전 및 바라보는 방향 업데이트
        if (distance >= moveDistance)
        {
            direction = -direction;    // 방향 전환
            UpdateLookDirection();     // 방향에 맞춰 회전 변경
        }
    }

    /// <summary>
    /// 이동 방향에 맞게 유령이 바라보는 방향을 회전시키는 함수
    /// </summary>
    private void UpdateLookDirection()
    {
        // direction이 0벡터가 아니면 회전 적용
        if (direction != Vector3.zero)
        {
            // 방향 벡터를 바라보는 회전 값 계산 (Y축을 기준으로 회전)
            Quaternion lookRotation = Quaternion.LookRotation(direction);

            // 유령 오브젝트의 회전값 변경 (즉시 회전)
            transform.rotation = lookRotation;
        }
    }

    // 플레이어와 충돌했을 때 호출되는 함수
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag(targetTag))
        {
            Debug.Log("유령이 " + targetTag + "와 충돌하여 씬을 재시작합니다.");
            SceneManager.LoadScene(SceneManager.GetActiveScene().name);
        }
    }
}
