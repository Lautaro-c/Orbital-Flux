using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UIElements;

public enum ForceDirection
{
    Push = 1, Pull = -1, Off = 0
}

public class ApplyForceDrone : MonoBehaviour
{
    [SerializeField] private float force;
    [SerializeField] private float radius;
    [SerializeField] private float distanceForceMultiplier;
    [SerializeField] private ForceStrengthController strength;

    private RaycastHit[] hits;

    private ForceDirection forceDirection;

    public void Push(InputAction.CallbackContext cb)
    {
        if (cb.started)
        {
            if (IsPointerOverUI()) return;
            forceDirection = ForceDirection.Push;
        }
        else if (cb.canceled)
        {
            if (forceDirection == ForceDirection.Push)
            {
                forceDirection = ForceDirection.Off;
            }
        }
    }

    public void Pull(InputAction.CallbackContext cb)
    {
        if (cb.started)
        {
            if (IsPointerOverUI()) return;
            forceDirection = ForceDirection.Pull;
        }
        else if (cb.canceled)
        {
            if (forceDirection == ForceDirection.Pull)
            {
                forceDirection = ForceDirection.Off;
            }
        }
    }

    private void FixedUpdate()
    {
        if (forceDirection != ForceDirection.Off)
        {
            float strengthMult = 1f;
            if (strength != null)
            {
                strengthMult = strength.GetForceMultiplier(forceDirection);
            }

            hits = Physics.SphereCastAll(transform.position, radius, Vector3.up);

            foreach (var hit in hits)
            {
                Rigidbody rb = hit.collider.GetComponent<Rigidbody>();

                if (rb != null)
                {
                    Vector3 dir = hit.transform.position - transform.position;

                    float distanceMult = Mathf.Clamp01(dir.magnitude / distanceForceMultiplier);
                    dir = dir.normalized;

                    Vector3 finalForce = dir * (force * distanceMult * strengthMult) * (int)forceDirection; // NUEVO: * strengthMult

                    rb.AddForce(finalForce, ForceMode.Impulse);
                }
            }
        }
    }

    // NUEVO: evita activar la fuerza al hacer click sobre los botones de la UI
    private bool IsPointerOverUI()
    {
        UnityEngine.EventSystems.EventSystem eventSystem = UnityEngine.EventSystems.EventSystem.current;
        return eventSystem != null && eventSystem.IsPointerOverGameObject();
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.green;
        Gizmos.DrawWireSphere(transform.position, radius);
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, distanceForceMultiplier);
    }
}