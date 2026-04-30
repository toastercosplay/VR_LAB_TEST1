using UnityEngine;

[RequireComponent(typeof(ParticleSystem))]
public class AtomParticles : MonoBehaviour
{
    [Header("Atom Settings")]
    [Range(1, 10)] public int atomicNumber = 6; // Default to Carbon
    public int particlesPerElectron = 5000; // You can crank this way up now!
    public float visualScale = 2.0f;
    public float particleSize = 0.05f;
    public float jitterAmount = 0.05f;

    [Header("Orbital Colors")]
    public Color color1s = Color.red;
    public Color color2s = Color.green;
    public Color color2p = Color.cyan;

    private ParticleSystem pSystem;
    private ParticleSystem.Particle[] particles;
    private int lastAtomicNumber;

    private const int max_1s = 2;
    private const int max_2s = 2;
    private const int max_2p = 6;

    void Start()
    {
        pSystem = GetComponent<ParticleSystem>();
        lastAtomicNumber = atomicNumber;

        // Optimize the Particle System for static rendering
        var main = pSystem.main;
        main.simulationSpace = ParticleSystemSimulationSpace.Local;
        main.playOnAwake = false;
        
        // Stop the built-in physics simulation so it doesn't overwrite our positions
        pSystem.Pause(); 

        GenerateAtom();
    }

    void Update()
    {
        // 1. Check if we need to REGENERATE the whole cloud
        if (atomicNumber != lastAtomicNumber)
        {
            GenerateAtom();
            lastAtomicNumber = atomicNumber;
        }

        // 2. Add MOVEMENT to existing particles (The "Buzz")
        // We iterate through the existing array and nudge them slightly
        int numParticles = pSystem.GetParticles(particles);
        for (int i = 0; i < numParticles; i++)
        {
            // Simple random jitter creates a "static" vibration effect
            Vector3 nudge = new Vector3(
                Random.Range(-jitterAmount, jitterAmount),
                Random.Range(-jitterAmount, jitterAmount),
                Random.Range(-jitterAmount, jitterAmount)
            );
            particles[i].position += nudge;
        }

        // Apply the updated positions back to the Particle System
        pSystem.SetParticles(particles, numParticles);
    }

    public void GenerateAtom()
    {
        // Calculate electron distribution
        int e1s = Mathf.Min(atomicNumber, max_1s);
        int remaining = Mathf.Max(0, atomicNumber - max_1s);

        int e2s = Mathf.Min(remaining, max_2s);
        remaining = Mathf.Max(0, remaining - max_2s);

        int e2p = Mathf.Min(remaining, max_2p);

        // Initialize the master particle array
        int totalParticles = (e1s + e2s + e2p) * particlesPerElectron;
        particles = new ParticleSystem.Particle[totalParticles];

        int currentIndex = 0;

        // Fill the array sequentially
        currentIndex = Generate1s(e1s, currentIndex);
        currentIndex = Generate2s(e2s, currentIndex);
        Generate2p(e2p, currentIndex);

        // Push the calculated array to the GPU in one single call
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

                if (Random.value < Mathf.Exp(-2f * r))
                {
                    validPos = pos;
                    break;
                }
            }

            // Assign properties directly to the struct
            particles[i].position = validPos * visualScale;
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

                if (Random.Range(0f, 4f) < probability)
                {
                    validPos = pos;
                    break;
                }
            }

            particles[i].position = validPos * visualScale;
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
            
            // Determine if this electron is px (0), py (1), or pz (2)
            int electronNumber = (i - startIndex) / particlesPerElectron;
            int axisIndex = electronNumber / 2;

            for (int safety = 0; safety < 2000; safety++)
            {
                Vector3 pos = new Vector3(Random.Range(-maxRadius, maxRadius), Random.Range(-maxRadius, maxRadius), Random.Range(-maxRadius, maxRadius));
                float r = pos.magnitude;
                if (r > maxRadius) continue;

                float probability = 0f;
                if (axisIndex == 0) probability = (pos.x * pos.x) * Mathf.Exp(-r);
                else if (axisIndex == 1) probability = (pos.y * pos.y) * Mathf.Exp(-r);
                else probability = (pos.z * pos.z) * Mathf.Exp(-r);

                if (Random.Range(0f, maxProb) < probability)
                {
                    validPos = pos;
                    break;
                }
            }

            particles[i].position = validPos * visualScale;
            particles[i].startColor = color2p;
            particles[i].startSize = particleSize;
        }
    }
}
