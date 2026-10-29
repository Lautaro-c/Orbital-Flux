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


    [Header("Visuals")]
    [SerializeField] private GameObject pullParticles;
    [SerializeField] private ParticleSystem pullParticleSystem;
    [SerializeField] private ParticleSystem pushParticleSystem;
    [SerializeField] private Animator droneAnimator;

    public void Push(InputAction.CallbackContext cb)
    {
        if (cb.started)
        {
            if (IsPointerOverUI()) return;
            forceDirection = ForceDirection.Push;
            droneAnimator.SetTrigger("Push");

            if (pullParticles.activeSelf)
            {
                pullParticles.SetActive(false);
                pullParticleSystem.Stop();
            }
            pushParticleSystem.Play();
        }
        else if (cb.canceled)
        {
            if (forceDirection == ForceDirection.Push)
            {
                forceDirection = ForceDirection.Off;
                droneAnimator.SetTrigger("Off");
                pushParticleSystem.Stop();
            }
        }
    }

    public void Pull(InputAction.CallbackContext cb)
    {
        if (cb.started)
        {
            if (IsPointerOverUI()) return;
            forceDirection = ForceDirection.Pull;
            droneAnimator.SetTrigger("Pull");

            //Start Particles
            pullParticles.SetActive(true);
            pullParticleSystem.Play();
            pushParticleSystem.Stop();
        }
        else if (cb.canceled)
        {
            if (forceDirection == ForceDirection.Pull)
            {
                forceDirection = ForceDirection.Off;
                droneAnimator.SetTrigger("Off");

                //Stop Particles
                if (pullParticles.activeSelf) pullParticles.SetActive(false);
                pullParticleSystem.Stop();
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