using UnityEngine;
using System.Collections.Generic;
using UnityEngine.UI;
using TMPro;

public class Atom : MonoBehaviour
{
    [SerializeField] private GameObject particle; //set to prefab in inspector
    private List<GameObject> electron1 = new List<GameObject>();
    private List<GameObject> electron2 = new List<GameObject>();
    private List<GameObject> electron3 = new List<GameObject>();

    public int amountOfParticles = 100;
    [SerializeField] Material S1Mat;
    [SerializeField] Material S2Mat;
    [SerializeField] Material P1Mat;

    void Start()
    {
        //creating particles and then deactivating them
        for (int i = 0; i < amountOfParticles; i++)
        {
            GameObject obj = Instantiate(particle, Vector3.zero, Quaternion.identity);
            obj.transform.SetParent(this.transform);
            obj.SetActive(false);
            electron1.Add(obj);
        }
        for (int i = 0; i < amountOfParticles; i++)
        {
            GameObject obj = Instantiate(particle, Vector3.zero, Quaternion.identity);
            obj.transform.SetParent(this.transform);
            obj.SetActive(false);
            electron2.Add(obj);
        }
        for (int i = 0; i < amountOfParticles; i++)
        {
            GameObject obj = Instantiate(particle, Vector3.zero, Quaternion.identity);
            obj.transform.SetParent(this.transform);
            obj.SetActive(false);
            electron3.Add(obj);
        }
    } 

    // Update is called once per frame
    void Update()
    {
        ShowSOrbital(S1Mat);
        ShowPOrbital(P1Mat);
    }

    //information found here:
    //https://chem.libretexts.org/Bookshelves/Inorganic_Chemistry/Inorganic_Chemistry_(LibreTexts)/02%3A_Atomic_Structure/2.02%3A_The_Schrodinger_equation_particle_in_a_box_and_atomic_wavefunctions/2.2.02%3A_Quantum_Numbers_and_Atomic_Wave_Functions


    //wave function for 1s orbital:
    //2[Z/a_0]^3/2 * e^(Zr/a_0) + 

    void ShowSOrbital(Material mat)
    {
        for (int i = 0; i < amountOfParticles; i++)
        {
            GameObject current = electron1[i];
            
            current.SetActive(true);
            current.transform.localPosition = new Vector3(0,0, 0);
            //rotate in a random direction and move forward a random value between 0 and 1
            current.transform.localRotation = Quaternion.Euler(Random.Range(0f, 360f), Random.Range(0f, 360f), Random.Range(0f, 360f));
            float randomDistance = Random.Range(0f, 1f);
            current.transform.Translate(Vector3.forward * randomDistance);
            //set material
            if (current.TryGetComponent<Renderer>(out Renderer ren))
            {
                ren.material = mat;
            }
        }
    }

    void ShowPOrbital(Material mat)
    {
        for (int i = 0; i < amountOfParticles; i++)
        {
            GameObject current = electron2[i];

            current.SetActive(true);
            current.transform.localPosition = new Vector3(Random.Range(-1f, 1f), Random.Range(-1f, 1f), Random.Range(-1f, 1f));

            if (current.TryGetComponent<Renderer>(out Renderer ren))
            {
                ren.material = mat;
            }
        }
    }
}
