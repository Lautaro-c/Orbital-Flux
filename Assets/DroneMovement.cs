using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class DroneMovement : MonoBehaviour
{
    [Header("Seguimiento del mouse")]
    [SerializeField] private float maxSpeed = 10f;
    [SerializeField] private float followGain = 5f;
    [SerializeField] private float maxAcceleration = 60f;

    private Rigidbody rb;

    private Vector3 mousePos;

    private void Awake()
    {
        rb = GetComponent<Rigidbody>();
    }

    public void GetMouseInWorldPosition(InputAction.CallbackContext cbx)
    {
        Vector3 rawMousePos = cbx.ReadValue<Vector2>();

        mousePos = Camera.main.ScreenToWorldPoint(rawMousePos);
    }

    private void FixedUpdate()
    {
        Vector3 toMouse = mousePos - transform.position;
        toMouse.z = 0f;
        Vector3 desiredVelocity = Vector3.ClampMagnitude(toMouse * followGain, maxSpeed);
        Vector3 velocityError = desiredVelocity - rb.velocity;
        Vector3 acceleration = velocityError / Time.fixedDeltaTime;
        acceleration = Vector3.ClampMagnitude(acceleration, maxAcceleration);
        rb.AddForce(acceleration, ForceMode.Acceleration);
    }
}