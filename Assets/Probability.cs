using UnityEngine;
using System.Collections.Generic;
using UnityEngine.UI;
using TMPro;

public class Probability : MonoBehaviour
{
    
    [SerializeField] private GameObject particle; //set to prefab in inspector
    private List<GameObject> particlePool = new List<GameObject>();

    //parameter variables:

    public float L = 10f; //length of box
    private float x_c = 0f; //center of box

    public int n = 1; //energy level
    private float k_n; //wave number

    public int amountOfParticles = 100; //number of particles to spawn for probability distribution

    //for UI

    public TextMeshProUGUI nText;
    public TextMeshProUGUI lText;

    LineRenderer box;


    //simulated probability function: 
    //P_n (x,t) = 2/L * sin^2 (k_n (x - x_c + L/2))   

    void Start()
    {
        UpdateMathConstants();

        box = GetComponent<LineRenderer>();
        //box.useWorldSpace = true;
        //box.alignment = LineAlignment.Local;
        DrawBox();
        
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
    
    float CalculateQuantumPosition()
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
            float argument = k_n * (testX - x_c + (L / 2f));
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
        k_n = n * Mathf.PI / L;
    }

    void UpdateParticlePositions()
    {
        foreach (GameObject p in particlePool)
        {
            p.SetActive(true);
            float newX = CalculateQuantumPosition();
            float newZ = CalculateQuantumPosition();
            float newY = CalculateQuantumPosition();
            p.transform.position = new Vector3(newX, newY, newZ);
        }
    }

    public void UpdateEnergyLevel(float new_n)
    {
        n = (int)new_n;
        nText.text = "n = " + n.ToString();
        UpdateMathConstants();
    }

    public void UpdateLength(float new_L)
    {
        L = new_L;
        lText.text = "L = " + L.ToString("F2");
        UpdateMathConstants();
        DrawBox();
    }

    public void DrawBox()
    {
        box.positionCount = 16;
        float halfL = L / 2f;


        Vector3 LBB =    new Vector3(x_c - halfL, -halfL, x_c - halfL);
        Vector3 RBB =    new Vector3(x_c + halfL, -halfL, x_c - halfL);
        Vector3 RFB =    new Vector3(x_c + halfL, -halfL, x_c + halfL);
        Vector3 LFB =    new Vector3(x_c - halfL, -halfL, x_c + halfL);

        Vector3 LBT =    new Vector3(x_c - halfL, halfL, x_c - halfL);
        Vector3 RBT =    new Vector3(x_c + halfL, halfL, x_c - halfL);
        Vector3 RFT =    new Vector3(x_c + halfL, halfL, x_c + halfL);
        Vector3 LFT =    new Vector3(x_c - halfL, halfL, x_c + halfL);


        Vector3[] path = new Vector3[]
        {
            LBB, RBB, RFB, LFB, LBB, LBT, RBT, RFT, LFT, LBT, LFT, LFB, RFB, RFT, RBT, RBB
        };

        box.SetPositions(path);
    }

}
