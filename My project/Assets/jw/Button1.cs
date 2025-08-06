using UnityEngine;
using UnityEngine.Events;

public class Button1 : MonoBehaviour
{
    [SerializeField]
    UnityEvent openevent;

    [SerializeField]
    UnityEvent closeevent;
    // 버튼이 눌렸다는 신호용 이벤트나 Bool
    public bool isPressed = false;

    // 충돌 시작 시 감지
    private void OnCollisionEnter(Collision collision)
    {
        // Layer 이름이 "Box"인지 확인
        if (collision.gameObject.layer == LayerMask.NameToLayer("Box"))
        {
            isPressed = true;
            Debug.Log("버튼이 눌렸습니다.");
            openevent.Invoke();
            // 여기서 원하는 이벤트를 호출할 수 있습니다.
        }
    }

    // 충돌이 끝났을 때 감지 (선택 사항)
    private void OnCollisionExit(Collision collision)
    {
        if (collision.gameObject.layer == LayerMask.NameToLayer("Box"))
        {
            isPressed = false;
            Debug.Log("버튼에서 손을 뗐습니다.");
            closeevent.Invoke();
        }
    }


}
