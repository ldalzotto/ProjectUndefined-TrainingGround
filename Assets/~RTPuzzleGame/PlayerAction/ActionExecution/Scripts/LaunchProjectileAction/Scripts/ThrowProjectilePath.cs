using UnityEngine;

namespace RTPuzzle
{
    public class ThrowProjectilePath : MonoBehaviour
    {
        public float AnimationSpeed;

        private ParticleSystem throwProjectileparticleSystem;
        private BeziersControlPoints BeziersControlPoints;

        private Collider throwerCollider;

        private float elapsedTime;

        public void Init(Collider throwerCollider)
        {
            throwProjectileparticleSystem = GetComponent<ParticleSystem>();
            var psMain = throwProjectileparticleSystem.main;
            psMain.startLifetime = 1 / AnimationSpeed;
            this.throwerCollider = throwerCollider;
            BeziersControlPoints = new BeziersControlPoints();
        }

        public void Tick(float d, Vector3 targetPosition)
        {
            transform.position = throwerCollider.bounds.center;

            BeziersControlPoints.P0 = throwerCollider.bounds.center;
            BeziersControlPoints.P3 = targetPosition;

            var upProjectedPath = Vector3.ProjectOnPlane(targetPosition - BeziersControlPoints.P0, Vector3.up);
            var upProjectedPathLength = upProjectedPath.magnitude;

            BeziersControlPoints.P1 = BeziersControlPoints.P0 + (upProjectedPath.normalized + Vector3.up) * upProjectedPathLength / 3;
            BeziersControlPoints.P2 = BeziersControlPoints.P1 + (upProjectedPath.normalized) * upProjectedPathLength / 3;


            var particleCount = throwProjectileparticleSystem.particleCount;

            if (particleCount > 0)
            {
                elapsedTime = Mathf.Clamp01(elapsedTime);
                var particles = new ParticleSystem.Particle[particleCount];
                throwProjectileparticleSystem.GetParticles(particles);
                particles[0].position = BeziersControlPoints.ResolvePoint(elapsedTime);
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
            if (BeziersControlPoints != null)
            {
                BeziersControlPoints.GizmoSelectedTick();
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
