using UnityEngine;

public class AICharacterController : MonoBehaviour
{
    public void MoveForward() => Debug.Log("AI가 앞으로 이동합니다.");
    public void MoveBackward() => Debug.Log("AI가 뒤로 이동합니다.");
    public void TurnLeft() => Debug.Log("AI가 왼쪽으로 회전합니다.");
    public void TurnRight() => Debug.Log("AI가 오른쪽으로 회전합니다.");
    public void OpenDoor() => Debug.Log("AI가 문을 엽니다.");
}
