using UnityEngine;

[RequireComponent(typeof(ParticleSystem))]
public class AtomParticles : MonoBehaviour
{
    [Header("Atom Settings")]
    [Range(1, 10)] public int atomicNumber = 6; // Default to Carbon
    public int particlesPerElectron = 5000;
    public float visualScale = 2.0f;
    public float particleSize = 0.05f;
    public float jitterAmount = 0.05f;
    
    [Tooltip("Minimum probability required to spawn a particle. Higher values create sharper orbital boundaries.")]
    [Range(0f, 0.5f)] public float probabilityCutoff = 0.01f; // NEW: The hard cutoff threshold

    [Header("Orbital Colors")]
    public Color color1s = Color.red;
    public Color color2s = Color.green;
    public Color color2p = Color.cyan;

    private ParticleSystem pSystem;
    private ParticleSystem.Particle[] particles;
    private Vector3[] basePositions; 
    private int lastAtomicNumber;

    private const int max_1s = 2;
    private const int max_2s = 2;
    private const int max_2p = 6;

    void Start()
    {
        pSystem = GetComponent<ParticleSystem>();
        lastAtomicNumber = atomicNumber;

        var main = pSystem.main;
        main.simulationSpace = ParticleSystemSimulationSpace.Local;
        main.playOnAwake = false;
        
        pSystem.Pause(); 

        GenerateAtom();
    }

    void Update()
    {
        if (atomicNumber != lastAtomicNumber)
        {
            GenerateAtom();
            lastAtomicNumber = atomicNumber;
        }

        int numParticles = pSystem.GetParticles(particles);
        for (int i = 0; i < numParticles; i++)
        {
            Vector3 nudge = new Vector3(
                Random.Range(-jitterAmount, jitterAmount),
                Random.Range(-jitterAmount, jitterAmount),
                Random.Range(-jitterAmount, jitterAmount)
            );
            particles[i].position = basePositions[i] + nudge; 
        }

        pSystem.SetParticles(particles, numParticles);
    }

    public void GenerateAtom()
    {
        int e1s = Mathf.Min(atomicNumber, max_1s);
        int remaining = Mathf.Max(0, atomicNumber - max_1s);

        int e2s = Mathf.Min(remaining, max_2s);
        remaining = Mathf.Max(0, remaining - max_2s);

        int e2p = Mathf.Min(remaining, max_2p);

        int totalParticles = (e1s + e2s + e2p) * particlesPerElectron;
        particles = new ParticleSystem.Particle[totalParticles];
        basePositions = new Vector3[totalParticles]; 

        int currentIndex = 0;

        currentIndex = Generate1s(e1s, currentIndex);
        currentIndex = Generate2s(e2s, currentIndex);
        Generate2p(e2p, currentIndex);

        pSystem.SetParticles(particles, totalParticles);
    }

    private int Generate1s(int count, int startIndex)
    {
        float maxRadius = 10f;
        int limit = startIndex + (count * particlesPerElectron);

        for (int i = startIndex; i < limit; i++)
        {
            Vector3 validPos = Vector3.zero;

            for (int safety = 0; safety < 1000; safety++)
            {
                Vector3 pos = new Vector3(Random.Range(-maxRadius, maxRadius), Random.Range(-maxRadius, maxRadius), Random.Range(-maxRadius, maxRadius));
                float r = pos.magnitude;
                if (r > maxRadius) continue;

                // NEW: Calculate probability first, then apply the hard cutoff
                float probability = Mathf.Exp(-2f * r);
                if (probability < probabilityCutoff) continue;

                if (Random.value < probability)
                {
                    validPos = pos;
                    break;
                }
            }

            basePositions[i] = validPos * visualScale; 
            particles[i].position = basePositions[i];
            particles[i].startColor = color1s;
            particles[i].startSize = particleSize;
        }
        return limit;
    }

    private int Generate2s(int count, int startIndex)
    {
        float maxRadius = 18f;
        int limit = startIndex + (count * particlesPerElectron);

        for (int i = startIndex; i < limit; i++)
        {
            Vector3 validPos = Vector3.zero;

            for (int safety = 0; safety < 2000; safety++)
            {
                Vector3 pos = new Vector3(Random.Range(-maxRadius, maxRadius), Random.Range(-maxRadius, maxRadius), Random.Range(-maxRadius, maxRadius));
                float r = pos.magnitude;
                if (r > maxRadius) continue;

                float probability = Mathf.Pow(2f - r, 2) * Mathf.Exp(-r);
                
                // NEW: Apply the hard cutoff
                if (probability < probabilityCutoff) continue;

                if (Random.Range(0f, 4f) < probability)
                {
                    validPos = pos;
                    break;
                }
            }

            basePositions[i] = validPos * visualScale; 
            particles[i].position = basePositions[i];
            particles[i].startColor = color2s;
            particles[i].startSize = particleSize;
        }
        return limit;
    }

    private void Generate2p(int count, int startIndex)
    {
        float maxRadius = 18f;
        float maxProb = 0.541f; 
        int limit = startIndex + (count * particlesPerElectron);

        for (int i = startIndex; i < limit; i++)
        {
            Vector3 validPos = Vector3.zero;
            
            int electronNumber = (i - startIndex) / particlesPerElectron;
            int axisIndex = electronNumber % 3; 

            for (int safety = 0; safety < 2000; safety++)
            {
                Vector3 pos = new Vector3(Random.Range(-maxRadius, maxRadius), Random.Range(-maxRadius, maxRadius), Random.Range(-maxRadius, maxRadius));
                float r = pos.magnitude;
                if (r > maxRadius) continue;

                float probability = 0f;
                if (axisIndex == 0) probability = (pos.x * pos.x) * Mathf.Exp(-r);
                else if (axisIndex == 1) probability = (pos.y * pos.y) * Mathf.Exp(-r);
                else probability = (pos.z * pos.z) * Mathf.Exp(-r);

                // NEW: Apply the hard cutoff
                if (probability < probabilityCutoff) continue;

                if (Random.Range(0f, maxProb) < probability)
                {
                    validPos = pos;
                    break;
                }
            }

            basePositions[i] = validPos * visualScale; 
            particles[i].position = basePositions[i];
            particles[i].startColor = color2p;
            particles[i].startSize = particleSize;
        }
    }
}
