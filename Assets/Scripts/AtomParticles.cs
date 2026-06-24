using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;
using TMPro;

[RequireComponent(typeof(ParticleSystem))]
public class AtomParticles : MonoBehaviour
{
    [System.Flags]
    public enum OrbitalFilter
    {
        None = 0,
        Orbit1s = 1 << 0,
        Orbit2s = 1 << 1,
        Orbit2p = 1 << 2,
        Orbit3s = 1 << 3,
        Orbit3p = 1 << 4,
        Orbit4s = 1 << 5,
        Orbit3d = 1 << 6,
        Orbit4p = 1 << 7,
        All = ~0
    }

    [Header("Atom Settings")]
    [Range(1, 36)] public int atomicNumber = 1;
    public int particlesPerElectron = 3000;
    public float visualScale = 2.0f;
    public float particleSize = 0.05f;
    public float jitterAmount = 0.05f;

    [Tooltip("Minimum probability required to spawn a particle. Higher values create sharper orbital boundaries.")]
    [Range(0f, 0.5f)] public float probabilityCutoff = 0.0075f;
    public OrbitalFilter visibleOrbitals = OrbitalFilter.All;

    [Header("Orbital Colors")]
    public Color color1s = Color.red;
    public Color color2s = Color.green;
    public Color color2p = Color.cyan;
    public Color color3s = Color.yellow;
    public Color color3p = Color.magenta;
    public Color color4s = new Color(1f, 0.5f, 0f);
    public Color color3d = new Color(0.5f, 0f, 1f);
    public Color color4p = new Color(0f, 1f, 0.5f);

    private ParticleSystem pSystem;
    private ParticleSystem.Particle[] particles;
    private Vector3[] basePositions;
    private int[] particleOrbitalID;
    private int lastAtomicNumber;
    private OrbitalFilter lastFilter;

    private const int max_1s = 2;
    private const int max_2s = 2;
    private const int max_2p = 6;
    private const int max_3s = 2;
    private const int max_3p = 6;
    private const int max_4s = 2;
    private const int max_3d = 10;
    private const int max_4p = 6;

    private Coroutine generationCoroutine;

    void Start()
    {
        pSystem = GetComponent<ParticleSystem>();
        lastAtomicNumber = atomicNumber;
        lastFilter = visibleOrbitals;

        var main = pSystem.main;
        main.simulationSpace = ParticleSystemSimulationSpace.Local;
        main.playOnAwake = false;
        pSystem.Pause();

        TriggerGeneration();
    }

    void Update()
    {
        if (atomicNumber != lastAtomicNumber)
        {
            TriggerGeneration();
            lastAtomicNumber = atomicNumber;
        }

        if (visibleOrbitals != lastFilter)
        {
            UpdateVisibility();
            lastFilter = visibleOrbitals;
        }

        int numParticles = pSystem.GetParticles(particles);
        for (int i = 0; i < numParticles; i++)
        {
            Vector3 nudge = new Vector3(
                Random.Range(-jitterAmount, jitterAmount),
                Random.Range(-jitterAmount, jitterAmount),
                Random.Range(-jitterAmount, jitterAmount)
            );

            if (particles[i].startSize > 0)
            {
                particles[i].position = basePositions[i] + nudge;
            }
        }
        pSystem.SetParticles(particles, numParticles);
    }

    private void TriggerGeneration()
    {
        if (generationCoroutine != null) StopCoroutine(generationCoroutine);
        generationCoroutine = StartCoroutine(GenerateAtomRoutine());
    }

    private IEnumerator GenerateAtomRoutine()
    {
        int remaining = atomicNumber;

        // Aufbau Principle configuration allocation up to Z=36
        int e1s = Mathf.Min(remaining, max_1s); remaining -= e1s;
        int e2s = Mathf.Min(remaining, max_2s); remaining -= e2s;
        int e2p = Mathf.Min(remaining, max_2p); remaining -= e2p;
        int e3s = Mathf.Min(remaining, max_3s); remaining -= e3s;
        int e3p = Mathf.Min(remaining, max_3p); remaining -= e3p;
        int e4s = Mathf.Min(remaining, max_4s); remaining -= e4s;
        int e3d = Mathf.Min(remaining, max_3d); remaining -= e3d;
        int e4p = Mathf.Min(remaining, max_4p);

        int totalParticles = (e1s + e2s + e2p + e3s + e3p + e4s + e3d + e4p) * particlesPerElectron;
        particles = new ParticleSystem.Particle[totalParticles];
        basePositions = new Vector3[totalParticles];
        particleOrbitalID = new int[totalParticles];

        int currentIndex = 0;

        if (e1s > 0) { currentIndex = Generate1s(e1s, currentIndex); yield return null; }
        if (e2s > 0) { currentIndex = Generate2s(e2s, currentIndex); yield return null; }
        if (e2p > 0) { currentIndex = Generate2p(e2p, currentIndex); yield return null; }
        if (e3s > 0) { currentIndex = Generate3s(e3s, currentIndex); yield return null; }
        if (e3p > 0) { currentIndex = Generate3p(e3p, currentIndex); yield return null; }
        if (e4s > 0) { currentIndex = Generate4s(e4s, currentIndex); yield return null; }
        if (e3d > 0) { currentIndex = Generate3d(e3d, currentIndex); yield return null; }
        if (e4p > 0) { Generate4p(e4p, currentIndex); yield return null; }

        UpdateVisibility();
    }

    public void UpdateVisibility()
    {
        if (particles == null) return;

        for (int i = 0; i < particles.Length; i++)
        {
            int orbitalID = particleOrbitalID[i];
            bool isVisible = ((int)visibleOrbitals & (1 << orbitalID)) != 0;

            particles[i].startSize = isVisible ? particleSize : 0f;
        }
        pSystem.SetParticles(particles, particles.Length);
    }

    #region Orbital Generation Engines

    private int Generate1s(int count, int startIndex)
    {
        float maxRadius = 6f;
        int limit = startIndex + (count * particlesPerElectron);

        for (int i = startIndex; i < limit; i++)
        {
            Vector3 validPos = Vector3.zero;
            for (int safety = 0; safety < 500; safety++)
            {
                Vector3 pos = Random.insideUnitSphere * maxRadius;
                float r = pos.magnitude;
                float probability = Mathf.Exp(-2f * r);

                if (probability < probabilityCutoff) continue;
                if (Random.value < probability) { validPos = pos; break; }
            }
            SetupParticle(i, validPos, color1s, 0);
        }
        return limit;
    }

    private int Generate2s(int count, int startIndex)
    {
        float maxRadius = 14f;
        int limit = startIndex + (count * particlesPerElectron);

        for (int i = startIndex; i < limit; i++)
        {
            Vector3 validPos = Vector3.zero;
            for (int safety = 0; safety < 800; safety++)
            {
                Vector3 pos = Random.insideUnitSphere * maxRadius;
                float r = pos.magnitude;
                float probability = Mathf.Pow(2f - r, 2) * Mathf.Exp(-r);

                if (probability < probabilityCutoff) continue;
                if (Random.Range(0f, 4f) < probability) { validPos = pos; break; }
            }
            SetupParticle(i, validPos, color2s, 1);
        }
        return limit;
    }

    private int Generate2p(int count, int startIndex)
    {
        float maxRadius = 16f;
        float maxProb = 0.541f;
        int limit = startIndex + (count * particlesPerElectron);

        for (int i = startIndex; i < limit; i++)
        {
            Vector3 validPos = Vector3.zero;
            int axisIndex = ((i - startIndex) / particlesPerElectron) % 3;

            for (int safety = 0; safety < 800; safety++)
            {
                Vector3 pos = Random.insideUnitSphere * maxRadius;
                float r = pos.magnitude;
                float probability = 0f;

                if (axisIndex == 0) probability = (pos.x * pos.x) * Mathf.Exp(-r);
                else if (axisIndex == 1) probability = (pos.y * pos.y) * Mathf.Exp(-r);
                else probability = (pos.z * pos.z) * Mathf.Exp(-r);

                if (probability < probabilityCutoff) continue;
                if (Random.Range(0f, maxProb) < probability) { validPos = pos; break; }
            }
            SetupParticle(i, validPos, color2p, 2);
        }
        return limit;
    }

    private int Generate3s(int count, int startIndex)
    {
        float maxRadius = 24f;
        int limit = startIndex + (count * particlesPerElectron);

        for (int i = startIndex; i < limit; i++)
        {
            Vector3 validPos = Vector3.zero;
            for (int safety = 0; safety < 1200; safety++)
            {
                Vector3 pos = Random.insideUnitSphere * maxRadius;
                float r = pos.magnitude;
                float probability = Mathf.Pow(27f - 18f * r + 2f * r * r, 2) * Mathf.Exp(-2f * r / 3f) * 0.01f;

                if (probability < probabilityCutoff) continue;
                if (Random.Range(0f, 5f) < probability) { validPos = pos; break; }
            }
            SetupParticle(i, validPos, color3s, 3);
        }
        return limit;
    }

    private int Generate3p(int count, int startIndex)
    {
        float maxRadius = 26f;
        float maxProb = 1.5f;
        int limit = startIndex + (count * particlesPerElectron);

        for (int i = startIndex; i < limit; i++)
        {
            Vector3 validPos = Vector3.zero;
            int axisIndex = ((i - startIndex) / particlesPerElectron) % 3;

            for (int safety = 0; safety < 1200; safety++)
            {
                Vector3 pos = Random.insideUnitSphere * maxRadius;
                float r = pos.magnitude;
                float probability = 0f;

                float radialPart = (6f * r - r * r) * Mathf.Exp(-r / 3f);
                if (axisIndex == 0) probability = Mathf.Pow(radialPart * (pos.x / (r + 0.001f)), 2);
                else if (axisIndex == 1) probability = Mathf.Pow(radialPart * (pos.y / (r + 0.001f)), 2);
                else probability = Mathf.Pow(radialPart * (pos.z / (r + 0.001f)), 2);

                if (probability < probabilityCutoff) continue;
                if (Random.Range(0f, maxProb) < probability) { validPos = pos; break; }
            }
            SetupParticle(i, validPos, color3p, 4);
        }
        return limit;
    }

    private int Generate4s(int count, int startIndex)
    {
        float maxRadius = 36f;
        int limit = startIndex + (count * particlesPerElectron);

        for (int i = startIndex; i < limit; i++)
        {
            Vector3 validPos = Vector3.zero;
            for (int safety = 0; safety < 1500; safety++)
            {
                Vector3 pos = Random.insideUnitSphere * maxRadius;
                float r = pos.magnitude;

                // 4s approximation: Spherical with multiple inner radial nodes
                float rho = r * 0.5f;
                float probability = Mathf.Pow(24f - 36f * rho + 12f * rho * rho - rho * rho * rho, 2) * Mathf.Exp(-rho) * 0.0005f;

                if (probability < probabilityCutoff) continue;
                if (Random.Range(0f, 2f) < probability) { validPos = pos; break; }
            }
            SetupParticle(i, validPos, color4s, 5);
        }
        return limit;
    }

    private int Generate3d(int count, int startIndex)
    {
        float maxRadius = 28f;
        float maxProb = 4.0f;
        int limit = startIndex + (count * particlesPerElectron);

        for (int i = startIndex; i < limit; i++)
        {
            Vector3 validPos = Vector3.zero;
            // Distribute across the 5 structural d-orbital configurations (xy, yz, xz, x^2-y^2, z^2)
            int shapeIndex = ((i - startIndex) / particlesPerElectron) % 5;

            for (int safety = 0; safety < 1500; safety++)
            {
                Vector3 pos = Random.insideUnitSphere * maxRadius;
                float r = pos.magnitude;
                if (r < 0.001f) continue;

                float radialPart = (r * r) * Mathf.Exp(-r / 3f) * 0.05f;
                float angularPart = 0f;

                switch (shapeIndex)
                {
                    case 0: angularPart = (pos.x * pos.y) / (r * r); break; // d_xy
                    case 1: angularPart = (pos.y * pos.z) / (r * r); break; // d_yz
                    case 2: angularPart = (pos.x * pos.z) / (r * r); break; // d_xz
                    case 3: angularPart = (pos.x * pos.x - pos.y * pos.y) / (r * r); break; // d_x2-y2
                    case 4: angularPart = (3f * pos.z * pos.z - r * r) / (r * r); break; // d_z2
                }

                float probability = Mathf.Pow(radialPart * angularPart, 2);

                if (probability < probabilityCutoff) continue;
                if (Random.Range(0f, maxProb) < probability) { validPos = pos; break; }
            }
            SetupParticle(i, validPos, color3d, 6);
        }
        return limit;
    }

    private void Generate4p(int count, int startIndex)
    {
        float maxRadius = 40f;
        float maxProb = 2.0f;
        int limit = startIndex + (count * particlesPerElectron);

        for (int i = startIndex; i < limit; i++)
        {
            Vector3 validPos = Vector3.zero;
            int axisIndex = ((i - startIndex) / particlesPerElectron) % 3;

            for (int safety = 0; safety < 1500; safety++)
            {
                Vector3 pos = Random.insideUnitSphere * maxRadius;
                float r = pos.magnitude;
                if (r < 0.001f) continue;

                // 4p radial component with node structures shifted further outwards
                float radialPart = (20f * r - 5f * r * r + 0.25f * r * r * r) * Mathf.Exp(-r / 4f) * 0.01f;
                float probability = 0f;

                if (axisIndex == 0) probability = Mathf.Pow(radialPart * (pos.x / r), 2);
                else if (axisIndex == 1) probability = Mathf.Pow(radialPart * (pos.y / r), 2);
                else probability = Mathf.Pow(radialPart * (pos.z / r), 2);

                if (probability < probabilityCutoff) continue;
                if (Random.Range(0f, maxProb) < probability) { validPos = pos; break; }
            }
            SetupParticle(i, validPos, color4p, 7);
        }
    }

    private void SetupParticle(int index, Vector3 pos, Color color, int orbitalID)
    {
        basePositions[index] = pos * visualScale;
        particles[index].position = basePositions[index];
        particles[index].startColor = color;
        particles[index].startSize = particleSize;
        particleOrbitalID[index] = orbitalID;
    }

    #endregion

    public void UpdateAtomicNumber(float new_z)
    {
        atomicNumber = Mathf.Clamp((int)new_z, 1, 36);
    }

    public void IsolateActiveOrbital(int orbitalIndex)
    {
        if (orbitalIndex < 0) visibleOrbitals = OrbitalFilter.All;
        else visibleOrbitals = (OrbitalFilter)(1 << orbitalIndex);
    }
}