using UnityEngine;

public class AICharacterController : MonoBehaviour
{
    public void MoveForward() => Debug.Log("AI�� ������ �̵��մϴ�.");
    public void MoveBackward() => Debug.Log("AI�� �ڷ� �̵��մϴ�.");
    public void TurnLeft() => Debug.Log("AI�� �������� ȸ���մϴ�.");
    public void TurnRight() => Debug.Log("AI�� ���������� ȸ���մϴ�.");
    public void OpenDoor() => Debug.Log("AI�� ���� ���ϴ�.");
}
