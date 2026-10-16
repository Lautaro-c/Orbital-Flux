using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class DroneMovement : MonoBehaviour
{
    [SerializeField] private float speed;
    private Rigidbody rb;
    [SerializeField] private float distanceMultiplier;

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
        Vector2 dir = Vector2.zero;

        dir = (mousePos - transform.position).normalized;
        float distanceMult = Mathf.Clamp01(dir.magnitude / distanceMultiplier);


        rb.AddForce(dir * speed * Time.fixedDeltaTime * distanceMult, ForceMode.Impulse);
    }
}
