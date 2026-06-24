using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class GravityBody : MonoBehaviour
{
    public float mass { get; private set; }
    public Vector3 initialVelocity { get; private set; }
    public Rigidbody rb { get; private set; }

    public TrailRenderer trail { get; private set; } // Add this property

    void Awake()
    {
        rb = GetComponent<Rigidbody>();
        trail = GetComponent<TrailRenderer>(); // Get the Trail Renderer

        rb.useGravity = false;
        rb.isKinematic = true;

        // Prevent the trail from drawing while waiting for the simulation to start
        if (trail != null)
        {
            trail.emitting = false;
            trail.Clear(); // Clears any glitches from instantiation
        }
    }

    public void Initialize(float newMass, float newRadius, Vector3 startVelocity)
    {
        mass = newMass;
        rb.mass = mass;
        transform.localScale = Vector3.one * (newRadius * 2f); // Assuming radius dictates scale
        initialVelocity = startVelocity;
    }
}
