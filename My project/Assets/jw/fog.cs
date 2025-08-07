using UnityEngine;
using UnityEngine.Events;

public class fog : MonoBehaviour
{
    [SerializeField]
    private UnityEvent airpurifieron;

    private ParticleSystem[] particleSystem;

    void Start()
    {
        particleSystem = GetComponentsInChildren<ParticleSystem>();

        // ���� ��� ��������
        var main = particleSystem[0].main;

        // ��� ����
        foreach (var particle in particleSystem)
        {
            particle.Play();
        }
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
    }
}
