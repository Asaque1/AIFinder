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

        // 메인 모듈 가져오기
        var main = particleSystem[0].main;

        // 재생 시작
        foreach (var particle in particleSystem)
        {
            particle.Play();
        }
    }

    [ContextMenu("파티클")]
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
