using UnityEngine;
using System.Collections.Generic;
using UnityEngine.UI;
using TMPro;

public class Atom : MonoBehaviour
{
    [SerializeField] private GameObject particle; //set to prefab in inspector
    private List<GameObject> electron1 = new List<GameObject>(); //1s
    private List<GameObject> electron2 = new List<GameObject>(); //2p
    private List<GameObject> electron3 = new List<GameObject>();

    public int atomicNumber = 1;

    public int amountOfParticles = 250;
    public float visualScale = 2.0f;

    [SerializeField] Material S1Mat;
    [SerializeField] Material S2Mat;
    [SerializeField] Material P1Mat;

    private const int max_1s = 2;
    private const int max_2s = 2;
    private const int max_2p = 6;

    void Start()
    {
             
        // 1s Orbital Setup
        InitializeOrbital(electron1, S1Mat, max_1s * amountOfParticles);
        InitializeOrbital(electron2, S2Mat, max_2s * amountOfParticles);
        // 2p Orbital Setup
        InitializeOrbital(electron3, P1Mat, max_2p * amountOfParticles);
        
    } 

    // Update is called once per frame
    void Update()
    {
        int electrons1s = Mathf.Min(atomicNumber, max_1s);
        int remaining = Mathf.Max(0, atomicNumber - max_1s);

        int electrons2s = Mathf.Min(remaining, max_2s);
        remaining = Mathf.Max(0, remaining - max_2s);

        int electrons2p = Mathf.Min(remaining, max_2p);

        Draw1sOrbital(electrons1s);
        Draw2sOrbital(electrons2s);
        Draw2pOrbital(electrons2p);
    }

    void InitializeOrbital(List<GameObject> electronList, Material mat, int maxParticles)
    {
        for (int i = 0; i < maxParticles; i++)
        {
            GameObject obj = Instantiate(particle, Vector3.zero, Quaternion.identity);
            obj.transform.SetParent(this.transform);
            
            if (obj.TryGetComponent<Renderer>(out Renderer ren))
            {
                ren.material = mat;
            }
            
            obj.SetActive(true);
            electronList.Add(obj);
        }
    }

    //information found here:
    //https://chem.libretexts.org/Bookshelves/Inorganic_Chemistry/Inorganic_Chemistry_(LibreTexts)/02%3A_Atomic_Structure/2.02%3A_The_Schrodinger_equation_particle_in_a_box_and_atomic_wavefunctions/2.2.02%3A_Quantum_Numbers_and_Atomic_Wave_Functions


    //wave function for 1s orbital:
    //2[Z/a_0]^3/2 * e^(Zr/a_0) + 

    // 1s Wave function: psi_1s = (1 / sqrt(pi * a_0^3)) * e^(-r/a_0)
    // Probability Density (psi^2) is proportional to: e^(-2r/a_0)
    //
    // 2pz Wave function: psi_2pz = (1 / 4*sqrt(2*pi * a_0^3)) * (z/a_0) * e^(-r/2a_0)
    // Probability Density (psi^2) is proportional to: z^2 * e^(-r/a_0)

    void Draw1sOrbital(int electronCount)
    {
        int targetParticles = electronCount * amountOfParticles;
        
        for (int i = 0; i < electron1.Count; i++)
        {
            if (i>=targetParticles)
            {
                electron1[i].SetActive(false);
                continue;
            }
            electron1[i].SetActive(true);
            
            //GameObject current = electron1[i];
            Vector3 validPosition = Vector3.zero;

            // Rejection Sampling Loop
            for (int safety = 0; safety < 50; safety++) 
            {
                // 1. Generate a random point in a local 3D bounding box
                Vector3 pos = new Vector3(Random.Range(-4f, 4f), Random.Range(-4f, 4f), Random.Range(-4f, 4f));
                float r = pos.magnitude; // distance from nucleus
                
                // 2. Calculate Probability Density P ~ e^(-2r)
                float probability = Mathf.Exp(-2f * r); 
                
                // 3. Roll a random chance against the highest possible probability (which is 1.0 at r=0)
                if (Random.value < probability)
                {
                    validPosition = pos;
                    break; // We found a valid point, exit the while loop!
                }
            }

            // Apply position with our visual scale
            electron1[i].transform.localPosition = validPosition * visualScale;
        }
    }

    void Draw2sOrbital(int electronCount)
    {
        int targetParticles = electronCount * amountOfParticles;

        for (int i = 0; i < electron2.Count; i++)
        {
            if (i >= targetParticles)
            {
                electron2[i].SetActive(false);
                continue;
            }
            electron2[i].SetActive(true);

            //GameObject current = electron1[i];
            Vector3 validPosition = Vector3.zero;

            // Rejection Sampling Loop
            for (int safety = 0; safety < 50; safety++)
            {
                // 1. Generate a random point in a local 3D bounding box
                Vector3 pos = new Vector3(Random.Range(-8f, 8f), Random.Range(-8f, 8f), Random.Range(-8f, 8f));
                float r = pos.magnitude; // distance from nucleus

                // 2. Calculate Probability Density P ~ e^(-2r)
                float probability = Mathf.Pow(2f - r, 2) * Mathf.Exp(-r);

                // 3. Roll a random chance against the highest possible probability (which is 1.0 at r=0)
                if (Random.Range(0f, 4f) < probability)
                {
                    validPosition = pos;
                    break; // We found a valid point, exit the while loop!
                }
            }

            // Apply position with our visual scale
            electron2[i].transform.localPosition = validPosition * visualScale;
        }
    }

    void Draw2pOrbital(int electronCount)
    {
        int targetParticles = electronCount * amountOfParticles;

        for (int i = 0; i < electron3.Count; i++)
        {
            if (i >= targetParticles)
            {
                electron3[i].SetActive(false);
                continue;
            }
            electron3[i].SetActive(true);

            //GameObject current = electron1[i];
            Vector3 validPosition = Vector3.zero;

            // Rejection Sampling Loop
            for (int safety = 0; safety < 50; safety++)
            {
                // 1. Generate a random point in a local 3D bounding box
                Vector3 pos = new Vector3(Random.Range(-8f, 8f), Random.Range(-8f, 8f), Random.Range(-8f, 8f));
                float r = pos.magnitude; // distance from nucleus
                float z = pos.z;

                // 2. Calculate Prob(ability Density P ~ e^(-2r)
                float probability = (z * z) * Mathf.Exp(-r);
                float maxProb = .541f;

                // 3. Roll a random chance against the highest possible probability (which is 1.0 at r=0)
                if (Random.Range(0f, maxProb) < probability)
                {
                    validPosition = pos;
                    break; // We found a valid point, exit the while loop!
                }
            }

            // Apply position with our visual scale
            electron3[i].transform.localPosition = validPosition * visualScale;
        }
    }
}
