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

    public int amountOfParticles = 250;
    public float visualScale = 2.0f;

    [SerializeField] Material S1Mat;
    //[SerializeField] Material S2Mat;
    [SerializeField] Material P1Mat;

    void Start()
    {
        // 1s Orbital Setup
        InitializeOrbital(electron1, S1Mat);
        // 2p Orbital Setup
        InitializeOrbital(electron2, P1Mat);
    } 

    // Update is called once per frame
    void Update()
    {
        ShowSOrbital();
        ShowPOrbital();
    }

    void InitializeOrbital(List<GameObject> electronList, Material mat)
    {
        for (int i = 0; i < amountOfParticles; i++)
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

    void ShowSOrbital()
    {
        for (int i = 0; i < amountOfParticles; i++)
        {
            GameObject current = electron1[i];
            Vector3 validPosition = Vector3.zero;

            // Rejection Sampling Loop
            for (int safety = 0; safety < 100; safety++) 
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
            current.transform.localPosition = validPosition * visualScale;
        }
    }

    void ShowPOrbital()
    {
        for (int i = 0; i < amountOfParticles; i++)
        {
            GameObject current = electron2[i];
            Vector3 validPosition = Vector3.zero;

            // Rejection Sampling Loop
            for (int safety = 0; safety < 100; safety++) 
            {
                // 1. Generate a random point in a slightly larger bounding box (P orbitals stretch further)
                Vector3 pos = new Vector3(Random.Range(-8f, 8f), Random.Range(-8f, 8f), Random.Range(-8f, 8f));
                
                float r = pos.magnitude;
                float z = pos.z; // The 2p_z orbital aligns along the Z axis
                
                // 2. Calculate Probability Density P ~ z^2 * e^(-r)
                float probability = (z * z) * Mathf.Exp(-r);
                
                // 3. The mathematical maximum value of this specific density function occurs at z = r = 2
                // The max value is 4 * e^(-2) which is roughly 0.5413
                float maxProbability = 4f * Mathf.Exp(-2f);
                
                // Roll the dice against the max probability
                if (Random.Range(0f, maxProbability) < probability)
                {
                    validPosition = pos;
                    break;
                }
            }

            // Apply position with our visual scale
            current.transform.localPosition = validPosition * visualScale;
        }
    }
}
