using UnityEngine;

namespace RTPuzzle
{
    public class ThrowProjectilePath : MonoBehaviour
    {
        public float AnimationSpeed;

        private ParticleSystem throwProjectileparticleSystem;
        private BeziersControlPoints beziersControlPoints;

        private Collider throwerCollider;

        private float elapsedTime;

        public BeziersControlPoints BeziersControlPoints { get => beziersControlPoints; }

        public void Init(Collider throwerCollider)
        {
            throwProjectileparticleSystem = GetComponent<ParticleSystem>();
            var psMain = throwProjectileparticleSystem.main;
            psMain.startLifetime = 1 / AnimationSpeed;
            this.throwerCollider = throwerCollider;
            beziersControlPoints = new BeziersControlPoints();
        }

        public void Tick(float d, Vector3 targetPosition)
        {
            transform.position = throwerCollider.bounds.center;

            beziersControlPoints.P0 = throwerCollider.bounds.center;
            beziersControlPoints.P3 = targetPosition;

            var upProjectedPath = Vector3.ProjectOnPlane(targetPosition - beziersControlPoints.P0, Vector3.up);
            var upProjectedPathLength = upProjectedPath.magnitude;

            beziersControlPoints.P1 = beziersControlPoints.P0 + (upProjectedPath.normalized + Vector3.up) * upProjectedPathLength / 3;
            beziersControlPoints.P2 = beziersControlPoints.P1 + (upProjectedPath.normalized) * upProjectedPathLength / 3;


            var particleCount = throwProjectileparticleSystem.particleCount;

            if (particleCount > 0)
            {
                elapsedTime = Mathf.Clamp01(elapsedTime);
                var particles = new ParticleSystem.Particle[particleCount];
                throwProjectileparticleSystem.GetParticles(particles);
                particles[0].position = beziersControlPoints.ResolvePoint(elapsedTime);
                throwProjectileparticleSystem.SetParticles(particles);
            }
            else
            {
                LaunchParticle();
            }

            elapsedTime += d * AnimationSpeed;
        }

        public void GizmoSelectedTick()
        {
            if (beziersControlPoints != null)
            {
                beziersControlPoints.GizmoSelectedTick();
            }
        }

        private void LaunchParticle()
        {
            elapsedTime = 0f;
            throwProjectileparticleSystem.Emit(1);
            throwProjectileparticleSystem.Play();
        }

    }


}
