using UnityEngine;

public class Button : MonoBehaviour
{
    // ��ư�� ���ȴٴ� ��ȣ�� �̺�Ʈ�� Bool
    public bool isPressed = false;

    // �浹 ���� �� ����
    private void OnTriggerEnter(Collider other)
    {
        // Layer �̸��� "Box"���� Ȯ��
        if (other.gameObject.layer == LayerMask.NameToLayer("Box"))
        {
            isPressed = true;
            Debug.Log("��ư�� ���Ƚ��ϴ�.");
            // ���⼭ ���ϴ� �̺�Ʈ�� ȣ���� �� �ֽ��ϴ�.
        }
    }

    // �浹�� ������ �� ���� (���� ����)
    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject.layer == LayerMask.NameToLayer("Box"))
        {
            isPressed = false;
            Debug.Log("��ư���� ���� �ý��ϴ�.");
        }
    }
}
