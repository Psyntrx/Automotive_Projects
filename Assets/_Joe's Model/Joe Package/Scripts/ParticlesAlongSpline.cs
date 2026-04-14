using UnityEngine;
using UnityEngine.Splines;

public class ParticlesAlongSpline : MonoBehaviour
{
     public ParticleSystem ps;
    public SplineContainer splineContainer;
    public float flowSpeed = 1f; // speed of flow along the spline

    private ParticleSystem.Particle[] particles;

    void LateUpdate()
    {
        if (ps == null || splineContainer == null) return;

        int count = ps.particleCount;
        if (count == 0) return;

        particles = new ParticleSystem.Particle[count];
        ps.GetParticles(particles);

        for (int i = 0; i < count; i++)
        {
            // t = position along spline (0 = start, 1 = end)
            float t = Mathf.Repeat(Time.time * flowSpeed + i * 0.01f, 1f);
            particles[i].position = splineContainer.Spline.EvaluatePosition(t);
        }

        ps.SetParticles(particles, count);
    }
}
