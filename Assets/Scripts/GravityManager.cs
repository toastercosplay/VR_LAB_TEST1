using System.Collections.Generic;
using UnityEngine;

public class GravityManager : MonoBehaviour
{
    [Header("Physics Settings")]
    [Tooltip("Scaled Gravitational Constant for game physics")]
    public float G = 6.674f;
    [Tooltip("Prevents infinite forces if objects perfectly overlap")]
    public float softening = 0.1f;
    public bool isRunning { get; private set; } = false;

    [Header("XR Bindings")]
    public Transform leftHandController;
    public GameObject bodyPrefab;
    public GameObject previewPrefab;
    [Tooltip("Optional: A LineRenderer prefab to visualize the velocity vector")]
    public LineRenderer velocityLinePrefab;

    [Header("Creation Parameters")]
    public float currentMass = 1000f;
    public float currentRadius = 1f;
    public float currentSpeed = 5f;

    private GameObject previewObject;
    private LineRenderer previewLine;
    private List<GravityBody> activeBodies = new List<GravityBody>();

    void Start()
    {
        CreatePreview();
    }

    void Update()
    {
        if (previewObject != null)
        {
            // Keep the preview object dynamically updated based on UI parameters
            previewObject.transform.localScale = Vector3.one * (currentRadius * 2f);

            // Visualize the velocity vector pointing straight forward from the hand
            if (previewLine != null)
            {
                previewLine.SetPosition(0, previewObject.transform.position);
                previewLine.SetPosition(1, previewObject.transform.position + (leftHandController.forward * currentSpeed));
            }
        }
    }

    void FixedUpdate()
    {
        if (!isRunning) return;

        // N-Body Gravity Loop
        for (int i = 0; i < activeBodies.Count; i++)
        {
            Vector3 totalForce = Vector3.zero;

            for (int j = 0; j < activeBodies.Count; j++)
            {
                if (i == j) continue; // Don't apply gravity to self

                Vector3 direction = activeBodies[j].transform.position - activeBodies[i].transform.position;
                float distanceSqr = direction.sqrMagnitude;

                // Add softening to prevent division by zero
                float distance = Mathf.Sqrt(distanceSqr + (softening * softening));

                // F = G * (m1 * m2) / r^2
                float forceMag = G * (activeBodies[i].mass * activeBodies[j].mass) / (distance * distance);

                totalForce += direction.normalized * forceMag;
            }
            // Apply the combined gravitational pull of all other bodies
            activeBodies[i].rb.AddForce(totalForce);
        }
    }

    // --- PUBLIC API FOR XR UI / BUTTONS ---

    public void SetMass(float mass)
    {
        currentMass = mass;
    }

    public void SetRadius(float radius)
    {
        currentRadius = radius;
    }

    public void SetSpeed(float speed)
    {
        currentSpeed = speed;
    }

    public void InstantiateNewBody()
    {
        if (previewObject == null) return;

        // Spawn actual body, detach from hand, and initialize
        GameObject newBodyObj = Instantiate(bodyPrefab, previewObject.transform.position, previewObject.transform.rotation);
        GravityBody gBody = newBodyObj.GetComponent<GravityBody>();

        Vector3 startVel = leftHandController.forward * currentSpeed;
        gBody.Initialize(currentMass, currentRadius, startVel);

        // If spawned while simulation is active, launch it immediately
        if (isRunning)
        {
            gBody.rb.isKinematic = false;
            gBody.rb.linearVelocity = startVel;
        }

        activeBodies.Add(gBody);
    }

    public void ToggleSimulation()
    {
        isRunning = !isRunning;

        foreach (var body in activeBodies)
        {
            body.rb.isKinematic = !isRunning;

            // Turn the trail on when running, off when paused
            if (body.trail != null)
            {
                body.trail.emitting = isRunning;
            }

            // Only apply initial velocity on the very first frame of simulation
            if (isRunning && body.rb.linearVelocity == Vector3.zero)
            {
                body.rb.linearVelocity = body.initialVelocity;
            }
        }
    }

    public void ResetSimulation()
    {
        isRunning = false;
        foreach (var body in activeBodies)
        {
            Destroy(body.gameObject);
        }
        activeBodies.Clear();
    }

    // --- INTERNAL HELPERS ---

    private void CreatePreview()
    {
        if (previewPrefab == null)
        {
            Debug.LogError("GravityManager: Preview Prefab is missing in the Inspector!", this);
            return;
        }

        // Instantiate your customized preview prefab directly onto the hand
        previewObject = Instantiate(previewPrefab, leftHandController);

        // Strip out any Colliders or Rigidbodies if they accidentally exist on the preview prefab
        // to prevent it from messing with hand tracking physics or physical collisions
        if (previewObject.TryGetComponent<Collider>(out Collider col)) Destroy(col);
        if (previewObject.TryGetComponent<Rigidbody>(out Rigidbody rb)) Destroy(rb);

        // Position it slightly floating above the hand controller base
        previewObject.transform.localPosition = new Vector3(0, 0.2f, 0);
        previewObject.transform.localRotation = Quaternion.identity;

        // Setup the forward-pointing vector line
        if (velocityLinePrefab != null)
        {
            previewLine = Instantiate(velocityLinePrefab, previewObject.transform);
            previewLine.positionCount = 2;
            previewLine.startWidth = 0.05f;
            previewLine.endWidth = 0.01f;
        }
    }
}