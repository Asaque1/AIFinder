using UnityEngine;

public class Broom : MonoBehaviour
{
    public float broomDetectionRadius = 3f;     // Broom ���� �ݰ�
    public LayerMask broomLayer;               // Broom�� ���� ���̾�
    public float requiredTime = 3f;            // Broom�� �ӹ����� �� �ð�

    private float broomStayTimer = 0f;         // ���� �ð�
    private bool isCleared = false;            // Dust ���� ����

    void Update()
    {
        if (isCleared) return;

        // Dust �ֺ��� Broom �ִ��� Ȯ��
        Collider[] nearbyBrooms = Physics.OverlapSphere(transform.position, broomDetectionRadius, broomLayer);

        if (nearbyBrooms.Length > 0)
        {
            // Ÿ�̸� ����
            broomStayTimer += Time.deltaTime;
        }
        else
        {
            // Broom�� ������ Ÿ�̸� �ʱ�ȭ
            broomStayTimer = 0f;
        }
    }
    // ������ ���� ���� ǥ��
    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, broomDetectionRadius);
    }
}
