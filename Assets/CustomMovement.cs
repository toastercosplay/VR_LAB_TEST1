using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit.Inputs.Readers;

[RequireComponent(typeof(CharacterController))]
public class CustomMovement : MonoBehaviour
{
    //actions
    [SerializeField] XRInputValueReader<Vector2> moveAction = new XRInputValueReader<Vector2>("Thumbstick");
    [SerializeField] XRInputValueReader<float> ascendAction = new XRInputValueReader<float>("Button");
    [SerializeField] XRInputValueReader<float> descendAction = new XRInputValueReader<float>("Button");

    //movement speeds
    [SerializeField] private float moveSpeed = 3f;
    [SerializeField] private float flySpeed = 2f;

    [SerializeField] private Transform headTransform;
    private CharacterController characterController;

    void Start()
    {
        characterController = GetComponent<CharacterController>();
    }

    void Update()
    {
        //joystick input
        Vector2 inputMove = moveAction.ReadValue();
        float ascend = ascendAction.ReadValue();
        float descend = descendAction.ReadValue();

        //horizontal movement in xz plane (global)
        Vector3 forward = Vector3.ProjectOnPlane(headTransform.forward, Vector3.up).normalized;
        Vector3 right = Vector3.ProjectOnPlane(headTransform.right, Vector3.up).normalized;

        Vector3 horizontalMove = (forward * inputMove.y + right * inputMove.x) * moveSpeed;

        //vertical movement
        float verticalVelocity = (ascend - descend) * flySpeed;

        //move that john
        Vector3 finalVelocity = horizontalMove + (Vector3.up * verticalVelocity);

        characterController.Move(finalVelocity * Time.deltaTime);
    }
}
