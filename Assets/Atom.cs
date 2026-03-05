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
        
    }

    void ShowSOrbital()
    {
        for (int i = 0; i < amountOfParticles; i++)
        {
            electron1[i].SetActive(true);
            electron1[i].transform.localPosition = new Vector3(Random.Range(-1f, 1f), Random.Range(-1f, 1f), Random.Range(-1f, 1f));
        }
    }

    void ShowPOrbital()
    {
        for (int i = 0; i < amountOfParticles; i++)
        {
            electron2[i].SetActive(true);
            electron2[i].transform.localPosition = new Vector3(Random.Range(-1f, 1f), Random.Range(-1f, 1f), Random.Range(-1f, 1f));
        }
    }
}
