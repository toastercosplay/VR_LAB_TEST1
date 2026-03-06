using UnityEngine;

public class SampleSine : MonoBehaviour
{
    float timeElapsed = 0f;
    bool timeEnabled = true;
    public float frequency = 1f;
    public float amplitude = 1f;
    [SerializeField] float maximum = 4;
    //[SerializeField] float startTime = 0;


    //Transform zero;
    Transform self;
    
    void Start()
    {
        self = GetComponent<Transform>();
        timeElapsed = ((self.position.x / maximum) / frequency) + ((self.position.z / maximum) / frequency); 
    }

    void Update()
    {
        if (timeEnabled == false)
        {
            return;
        }

        timeElapsed += Time.deltaTime;
        float y = amplitude * Mathf.Sin(2 * Mathf.PI * frequency * timeElapsed);
        //float x = timeElapsed;
        self.position = new Vector3(self.position.x, y, self.position.z);

        if (timeElapsed > 1/frequency)
        {
            timeElapsed = 0f;
        }
    }

    public void ToggleTime()
    {
        timeEnabled = !timeEnabled;
    }
}
