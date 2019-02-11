using UnityEngine;
using static UnityEngine.ParticleSystem;

public class RibbonTestManager : MonoBehaviour
{

    private ParticleSystem ribbonparticleSystem;

    private void Start()
    {
        ribbonparticleSystem = GetComponent<ParticleSystem>();
        var psMain = ribbonparticleSystem.main;
        psMain.startLifetime = 1 / speed;
        this.P0 = transform;
    }

    public bool launch;
    public float speed;

    private Transform P0;
    public Transform P1;
    public Transform P2;
    public Transform P3;

    private void OnDrawGizmos()
    {
        if (P1 != null)
        {
            Gizmos.DrawWireSphere(P1.transform.position, 2f);
        }
        if (P2 != null)
        {
            Gizmos.DrawWireSphere(P2.transform.position, 2f);
        }
        if (P3 != null)
        {
            Gizmos.DrawWireSphere(P3.transform.position, 2f);
        }
    }

    private float elapsedTime;

    private void Update()
    {
        var d = Time.deltaTime;
        if (launch)
        {
            launch = false;
            elapsedTime = 0f;
            ribbonparticleSystem.Emit(1);
            ribbonparticleSystem.Play();
        }

        var particleCount = ribbonparticleSystem.particleCount;

        if (particleCount > 0)
        {
            elapsedTime = Mathf.Clamp01(elapsedTime);
            var particles = new Particle[particleCount];
            ribbonparticleSystem.GetParticles(particles);
            particles[0].position = P0.position * Mathf.Pow((1 - elapsedTime), 3) + (3 * P1.position * elapsedTime * Mathf.Pow(1 - elapsedTime, 2)) + (3 * P2.position * Mathf.Pow(elapsedTime, 2) * (1 - elapsedTime)) + (P3.position * Mathf.Pow(elapsedTime, 3));
            ribbonparticleSystem.SetParticles(particles);
        }
        else
        {
            launch = true;
        }

        elapsedTime += d * speed;
    }

}
