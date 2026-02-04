using UnityEngine;
using System.Collections.Generic;
using UnityEngine.UI;
using TMPro;

public class Probability : MonoBehaviour
{
    
    [SerializeField] private GameObject particle; //set to prefab in inspector
    private List<GameObject> particlePool = new List<GameObject>();

    //parameter variables:

    public float L = .2f; //length of box in nanometers
    private float x_c = 0f; //center of box

    public int n_x = 1; //energy level x
    public int n_y = 1; //energy level y
    public int n_z = 1; //energy level z
    private float k_n_x; //wave number x
    private float k_n_y; //wave number y
    private float k_n_z; //wave number z

    //physical parameters for energy
    private float mass = 9.11e-31f; //mass of electron in kg
    private float h = 6.626e-34f; //Planck's constant
    private float particleEnergy;

    public int amountOfParticles = 100; //number of particles to spawn for probability distribution

    //for UI

    public TextMeshProUGUI nXText;
    public TextMeshProUGUI nYText;
    public TextMeshProUGUI nZText;
    public TextMeshProUGUI lText;
    public TextMeshProUGUI energyText;


    //simulated probability function: 
    //P_n (x,t) = 2/L * sin^2 (k_n (x - x_c + L/2))   

    void Start()
    {
        UpdateMathConstants();
        
        //creating particles and then deavicating them
        for (int i = 0; i < amountOfParticles; i++)
        {
            GameObject obj = Instantiate(particle, Vector3.zero, Quaternion.identity);
            obj.transform.SetParent(this.transform);
            obj.SetActive(false);
            particlePool.Add(obj);
        }

        nText.text = "n = " + n.ToString();
        lText.text = "L = " + L.ToString("F2");
    } 

    void Update()
    {
        UpdateParticlePositions();
    }
    
    float CalculateQuantumPosition(float K_N)
    {
        //using rejection sampoling
        //look for a valid x until we find one that fits the probability density.
        while (true)
        {
            //pick a random x within the box boundaries
            float minX = x_c - (L / 2f);
            float maxX = x_c + (L / 2f);
            float testX = Random.Range(minX, maxX);

            //calculate P_n(x) at this testX
            //P_n(x) = (2/L) * sin^2(k_n * (x - x_c + L/2))
            float argument = K_N * (testX - x_c + (L / 2f));
            float sinVal = Mathf.Sin(argument);
            float probDensity = (2f / L) * (sinVal * sinVal);

            //roll the dice against the max possible probability (which is 2/L)
            float maxP = 2f / L;

            if (Random.Range(0f, maxP) <= probDensity)
            {
                return testX; //particle "exists" here
            }
        }
    }

    private void UpdateMathConstants()
    {
        k_n_x = n_x * Mathf.PI / L;
        k_n_y = n_y * Mathf.PI / L;
        k_n_z = n_z * Mathf.PI / L;
    }

    void UpdateParticlePositions()
    {
        foreach (GameObject p in particlePool)
        {
            p.SetActive(true);
            float newX = CalculateQuantumPosition(k_n_x);
            float newZ = CalculateQuantumPosition(k_n_z);
            float newY = CalculateQuantumPosition(k_n_y);
            p.transform.position = new Vector3(newX, newY, newZ);
        }
    }

    public void UpdateEnergyLevelX(float new_n)
    {
        n_x = (int)new_n;
        nXText.text = "n_x = " + n_x.ToString();
        UpdateMathConstants();
    }

    public void UpdateEnergyLevelY(float new_n)
    {
        n_y = (int)new_n;
        nYText.text = "n_y = " + n_y.ToString();
        UpdateMathConstants();
    }

    public void UpdateEnergyLevelZ(float new_n)
    {
        n_z = (int)new_n;
        nZText.text = "n_z = " + n_z.ToString();
        UpdateMathConstants();
    }

    public void UpdateLength(float new_L)
    {
        L = new_L;
        lText.text = "L = " + L.ToString("F2");
        UpdateMathConstants();
        DrawBox();
    }

    public float getLength()
    {
        return L;
    }

    public void calculateParticleEnergy()
    {
        //using E = (n_x^2 + n_y^2 + n_z^2)*h^2/(8mL^2)

        float tempL = L * 1e-9f; //convert nm to m for calculation

        particleEnergy = ( (n_x*n_x) + (n_y*n_y) + (n_z*n_z) ) * (h*h) / (8f * mass * tempL * tempL);
        energyText.text = "E = " + particleEnergy.ToString("E2") + "eV";

    }

        

}
