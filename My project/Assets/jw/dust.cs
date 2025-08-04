using UnityEngine;

public class dust : MonoBehaviour
{
    public float broomDetectionRadius = 3f;   // Broom ���� �ݰ�
    public LayerMask broomLayer;             // Broom�� ���� ���̾�
    public float requiredTime = 3f;          // ���ŵǱ���� �ʿ��� �ð�

    private float broomStayTimer = 0f;       // ���� �ӹ� �ð�
    private bool isCleared = false;

    void Update()
    {
        if (isCleared) return;

        // �ֺ��� Broom �ִ��� �˻�
        Collider[] nearbyBrooms = Physics.OverlapSphere(transform.position, broomDetectionRadius, broomLayer);

        if (nearbyBrooms.Length > 0)
        {
            // Ÿ�̸� ����
            broomStayTimer += Time.deltaTime;

            if (broomStayTimer >= requiredTime)
            {
                ClearDust();
            }
        }
        else
        {
            // Broom�� �־������Ƿ� Ÿ�̸� �ʱ�ȭ
            broomStayTimer = 0f;
        }
    }

    void ClearDust()
    {
        isCleared = true;
        Debug.Log("Dust cleared after Broom stayed nearby for " + requiredTime + " seconds.");
        Destroy(gameObject);
    }

    // ������: ���� ���� �ð�ȭ
    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.cyan;
        Gizmos.DrawWireSphere(transform.position, broomDetectionRadius);
    }
}
