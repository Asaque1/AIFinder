using UnityEngine;
using UnityEngine.SceneManagement;

/// <summary>
/// ���� ��ֹ��� ���� �������� �������ٰ� �ǵ��ƿ��⸦ �ݺ��ϸ�,
/// ���� ����ϰ�, �÷��̾�� �ε����� ���� ������ϴ� ��ũ��Ʈ�Դϴ�.
/// Rigidbody.MovePosition�� ����� ���� �浹 ������ �����ϸ�,
/// �̵� �������� ������ �ڿ������� �ٶ󺸰� �մϴ�.
/// </summary>
[RequireComponent(typeof(Collider))]
[RequireComponent(typeof(Rigidbody))]
public class GhostObstacle : MonoBehaviour
{
    [Header("�̵� ����")]
    public Vector3 moveDirection = Vector3.right;  // �̵� ���� ���� (�⺻ ������)
    public float moveSpeed = 2f;                    // �̵� �ӵ�
    public float moveDistance = 5f;                 // �ִ� �̵� �Ÿ�

    private Vector3 startPosition;                   // ���� ���� ��ġ ����
    private Vector3 direction;                       // ���� �̵� ���� (���� ����)
    private Rigidbody rb;                            // Rigidbody ������Ʈ ����

    [Header("�浹 ����")]
    public string targetTag = "player";              // �浹 ���� ��� �±�

    void Start()
    {
        rb = GetComponent<Rigidbody>();

        // Rigidbody�� �������� ������ ���� �ʰ�, ���� �������� �����ϵ��� ����
        rb.isKinematic = true;
        rb.useGravity = false;

        // Collider�� Trigger�� ������ ���� ����ϰ� ��
        Collider col = GetComponent<Collider>();
        col.isTrigger = true;

        // ���� ��ġ ����
        startPosition = transform.position;

        // �̵� ������ ����ȭ(���� 1)�Ͽ� ������ �ӵ� ����
        direction = moveDirection.normalized;

        // ���� ó�� ���� �ٶ󺸵��� ����
        UpdateLookDirection();
    }

    void Update()
    {
        // �̵��� ���� ��ġ ���
        Vector3 nextPos = transform.position + direction * moveSpeed * Time.deltaTime;

        // Rigidbody.MovePosition�� ���� �����ϰ� �̵� ó��
        rb.MovePosition(nextPos);

        // ���� ��ġ���� �̵� �Ÿ� ���
        float distance = Vector3.Distance(startPosition, nextPos);

        // �ִ� �̵� �Ÿ��� �����ϸ� ���� ���� �� �ٶ󺸴� ���� ������Ʈ
        if (distance >= moveDistance)
        {
            direction = -direction;    // ���� ��ȯ
            UpdateLookDirection();     // ���⿡ ���� ȸ�� ����
        }
    }

    /// <summary>
    /// �̵� ���⿡ �°� ������ �ٶ󺸴� ������ ȸ����Ű�� �Լ�
    /// </summary>
    private void UpdateLookDirection()
    {
        // direction�� 0���Ͱ� �ƴϸ� ȸ�� ����
        if (direction != Vector3.zero)
        {
            // ���� ���͸� �ٶ󺸴� ȸ�� �� ��� (Y���� �������� ȸ��)
            Quaternion lookRotation = Quaternion.LookRotation(direction);

            // ���� ������Ʈ�� ȸ���� ���� (��� ȸ��)
            transform.rotation = lookRotation;
        }
    }

    // �÷��̾�� �浹���� �� ȣ��Ǵ� �Լ�
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag(targetTag))
        {
            Debug.Log("������ " + targetTag + "�� �浹�Ͽ� ���� ������մϴ�.");
            SceneManager.LoadScene(SceneManager.GetActiveScene().name);
        }
    }
}
