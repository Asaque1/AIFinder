using UnityEngine;
using UnityEngine.Rendering.PostProcessing;

public class airpurifier : MonoBehaviour
{
    [SerializeField] private Transform player;      // �÷��̾� Transform
    [SerializeField] private fog fogScript;         // Fog ��ũ��Ʈ ����
    [SerializeField] private float triggerDistance = 5f;

    private bool triggered = false;

    void Update()
    {
        if (triggered || player == null || fogScript == null)
            return;

        float distance = Vector3.Distance(player.position, transform.position);
        if (distance <= triggerDistance)
        {
            fogScript.stopParticle();
            triggered = true;
        }
    }
}
