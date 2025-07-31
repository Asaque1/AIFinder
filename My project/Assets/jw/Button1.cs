using UnityEngine;
using UnityEngine.Events;

public class Button1 : MonoBehaviour
{
    [SerializeField]
    UnityEvent openevent;

    [SerializeField]
    UnityEvent closeevent;
    // ��ư�� ���ȴٴ� ��ȣ�� �̺�Ʈ�� Bool
    public bool isPressed = false;

    // �浹 ���� �� ����
    private void OnCollisionEnter(Collision collision)
    {
        // Layer �̸��� "Box"���� Ȯ��
        if (collision.gameObject.layer == LayerMask.NameToLayer("Box"))
        {
            isPressed = true;
            Debug.Log("��ư�� ���Ƚ��ϴ�.");
            openevent.Invoke();
            // ���⼭ ���ϴ� �̺�Ʈ�� ȣ���� �� �ֽ��ϴ�.
        }
    }

    // �浹�� ������ �� ���� (���� ����)
    private void OnCollisionExit(Collision collision)
    {
        if (collision.gameObject.layer == LayerMask.NameToLayer("Box"))
        {
            isPressed = false;
            Debug.Log("��ư���� ���� �ý��ϴ�.");
            closeevent.Invoke();
        }
    }


}
