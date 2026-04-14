using UnityEngine;
using System.Collections.Generic;

public class BrakeFluidFlowController : MonoBehaviour
{
    [Header("Path Configuration")]
    [Tooltip("Assign your waypoint transforms in order from master cylinder to caliper")]
    public Transform[] pathPoints;
    
    [Header("Flow Settings")]
    [Range(0f, 5f)]
    public float flowSpeed = 1f;
    public bool isFlowing = false;
    public Color fluidColor = new Color(1f, 0.8f, 0.2f, 0.8f); // Amber color
    
    [Header("Visual Components")]
    public LineRenderer pipeLine;
    public ParticleSystem flowParticles;
    
    [Header("Advanced Settings")]
    [Range(0.001f, 0.05f)]
    public float pipeWidth = 0.015f;
    public int lineResolution = 50; // Points between waypoints for smooth curves
    
    private Material fluidMaterial;
    private float textureOffset = 0f;
    private List<Vector3> smoothPath = new List<Vector3>();
    private ParticleSystem.Particle[] particles;
    private int particleIndex = 0;
    
    void Start()
    {
        SetupPipeline();
        SetupParticles();
        GenerateSmoothPath();
    }
    
    void SetupPipeline()
    {
        if (pipeLine == null)
        {
            pipeLine = gameObject.AddComponent<LineRenderer>();
        }
        
        // Create material for animated flow
        fluidMaterial = new Material(Shader.Find("Standard"));
        fluidMaterial.color = fluidColor;
        fluidMaterial.SetFloat("_Metallic", 0.3f);
        fluidMaterial.SetFloat("_Glossiness", 0.8f);
        
        pipeLine.material = fluidMaterial;
        pipeLine.startWidth = pipeWidth;
        pipeLine.endWidth = pipeWidth;
        pipeLine.useWorldSpace = true;
        pipeLine.numCapVertices = 5;
        pipeLine.numCornerVertices = 5;
    }
    
    void SetupParticles()
    {
        if (flowParticles == null)
        {
            GameObject particleObj = new GameObject("FlowParticles");
            particleObj.transform.SetParent(transform);
            flowParticles = particleObj.AddComponent<ParticleSystem>();
        }
        
        var main = flowParticles.main;
        main.startColor = fluidColor;
        main.startSize = pipeWidth * 0.5f;
        main.startLifetime = 2f;
        main.simulationSpace = ParticleSystemSimulationSpace.World;
        main.maxParticles = 100;
        
        var emission = flowParticles.emission;
        emission.enabled = false; // We'll emit manually
        
        var shape = flowParticles.shape;
        shape.enabled = false;
    }
    
    void GenerateSmoothPath()
    {
        smoothPath.Clear();
        
        if (pathPoints == null || pathPoints.Length < 2)
        {
            Debug.LogError("Need at least 2 path points!");
            return;
        }
        
        // Generate smooth curve through waypoints using Catmull-Rom
        for (int i = 0; i < pathPoints.Length - 1; i++)
        {
            Vector3 p0 = i > 0 ? pathPoints[i - 1].position : pathPoints[i].position;
            Vector3 p1 = pathPoints[i].position;
            Vector3 p2 = pathPoints[i + 1].position;
            Vector3 p3 = i < pathPoints.Length - 2 ? pathPoints[i + 2].position : pathPoints[i + 1].position;
            
            for (int j = 0; j < lineResolution; j++)
            {
                float t = j / (float)lineResolution;
                smoothPath.Add(GetCatmullRomPosition(t, p0, p1, p2, p3));
            }
        }
        
        // Add final point
        smoothPath.Add(pathPoints[pathPoints.Length - 1].position);
        
        // Update line renderer
        pipeLine.positionCount = smoothPath.Count;
        pipeLine.SetPositions(smoothPath.ToArray());
    }
    
    Vector3 GetCatmullRomPosition(float t, Vector3 p0, Vector3 p1, Vector3 p2, Vector3 p3)
    {
        // Catmull-Rom spline for smooth curves
        float t2 = t * t;
        float t3 = t2 * t;
        
        Vector3 result = 0.5f * (
            (2f * p1) +
            (-p0 + p2) * t +
            (2f * p0 - 5f * p1 + 4f * p2 - p3) * t2 +
            (-p0 + 3f * p1 - 3f * p2 + p3) * t3
        );
        
        return result;
    }
    
    void Update()
    {
        if (isFlowing)
        {
            AnimateFlow();
            EmitFlowParticles();
        }
    }
    
    void AnimateFlow()
    {
        // Animate texture offset for flow appearance
        textureOffset += flowSpeed * Time.deltaTime;
        if (textureOffset > 1f) textureOffset -= 1f;
        
        fluidMaterial.SetTextureOffset("_MainTex", new Vector2(textureOffset, 0));
        
        // Pulse the emission color for "pressure wave" effect
        float pulse = Mathf.PingPong(Time.time * flowSpeed, 1f);
        Color emissionColor = fluidColor * Mathf.LinearToGammaSpace(pulse * 0.5f);
        fluidMaterial.SetColor("_EmissionColor", emissionColor);
        fluidMaterial.EnableKeyword("_EMISSION");
    }
    
    void EmitFlowParticles()
    {
        if (smoothPath.Count < 2) return;
        
        // Emit particles along the path
        float pathProgress = (Time.time * flowSpeed * 0.2f) % 1f;
        int currentIndex = Mathf.FloorToInt(pathProgress * (smoothPath.Count - 1));
        
        if (currentIndex != particleIndex && currentIndex < smoothPath.Count)
        {
            particleIndex = currentIndex;
            
            ParticleSystem.EmitParams emitParams = new ParticleSystem.EmitParams();
            emitParams.position = smoothPath[currentIndex];
            emitParams.velocity = Vector3.zero;
            emitParams.startColor = fluidColor;
            emitParams.startSize = pipeWidth * 0.6f;
            
            flowParticles.Emit(emitParams, 1);
        }
    }
    
    // Public methods for triggering flow
    public void StartFlow()
    {
        isFlowing = true;
        flowParticles.Play();
    }
    
    public void StopFlow()
    {
        isFlowing = false;
        flowParticles.Stop();
    }
    
    public void SetFlowSpeed(float speed)
    {
        flowSpeed = Mathf.Clamp(speed, 0f, 5f);
    }
    
    // Editor helper to visualize path
    void OnDrawGizmos()
    {
        if (pathPoints == null || pathPoints.Length < 2) return;
        
        Gizmos.color = Color.yellow;
        for (int i = 0; i < pathPoints.Length - 1; i++)
        {
            if (pathPoints[i] != null && pathPoints[i + 1] != null)
            {
                Gizmos.DrawLine(pathPoints[i].position, pathPoints[i + 1].position);
                Gizmos.DrawWireSphere(pathPoints[i].position, 0.02f);
            }
        }
        
        if (pathPoints[pathPoints.Length - 1] != null)
        {
            Gizmos.DrawWireSphere(pathPoints[pathPoints.Length - 1].position, 0.02f);
        }
    }
}