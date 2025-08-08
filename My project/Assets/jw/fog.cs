using UnityEngine;
using UnityEngine.Events;

public class fog : MonoBehaviour
{
    [SerializeField]
    private UnityEvent airpurifieron;

    private ParticleSystem[] particleSystem;
    private Collider fogCollider;

    void Start()
    {
        particleSystem = GetComponentsInChildren<ParticleSystem>();
        fogCollider = GetComponent<Collider>();

        // ���� ��� ��������
        var main = particleSystem[0].main;

        // ��� ����
        foreach (var particle in particleSystem)
        {
            particle.Play();
        }

        if (fogCollider != null)
            fogCollider.enabled = true;
    }

    [ContextMenu("��ƼŬ")]
    public void stopParticle()
    {
        if(airpurifieron != null)
        {
            foreach (var particle in particleSystem)
            {
                particle.Stop();
            }
        }

        if (fogCollider != null)
            fogCollider.enabled = false;
    }
}
