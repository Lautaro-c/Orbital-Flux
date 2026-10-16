using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UIElements;

public class ApplyForceDrone : MonoBehaviour
{
    [SerializeField] private float forcePushback;
    [SerializeField] private float radius;

    public void Push(InputAction.CallbackContext cb)
    {
        if (cb.started)
        {
            var hits = Physics.SphereCastAll(transform.position, radius, Vector3.up);

            foreach (var hit in hits)
            {
                Rigidbody rb3d = hit.collider.GetComponent<Rigidbody>();
                Rigidbody2D rb = hit.collider.GetComponent<Rigidbody2D>();

                if (rb3d != null)
                {
                    Vector3 dir = hit.transform.position - transform.position;

                    dir = dir.normalized;

                    rb3d.AddForce(dir * forcePushback, ForceMode.Impulse);
                }
                if (rb != null)
                {
                    Vector3 dir = hit.transform.position - transform.position;

                    dir = dir.normalized;

                    rb.AddForce(dir * forcePushback, ForceMode2D.Impulse);
                }
            }
        }
    }

    public void Pull(InputAction.CallbackContext cb)
    {
        if (cb.started)
        {
            var hits = Physics.SphereCastAll(transform.position, radius, Vector3.up);

            foreach (var hit in hits)
            {
                Rigidbody rb3d = hit.collider.GetComponent<Rigidbody>();
                Rigidbody2D rb = hit.collider.GetComponent<Rigidbody2D>();

                if (rb3d != null)
                {
                    Vector3 dir = hit.transform.position - transform.position;

                    dir = dir.normalized;

                    rb3d.AddForce(dir * -forcePushback, ForceMode.Impulse);
                }
                if (rb != null)
                {
                    Vector3 dir = hit.transform.position - transform.position;

                    dir = dir.normalized;

                    rb.AddForce(dir * -forcePushback, ForceMode2D.Impulse);
                }
            }
        }
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.green;
        Gizmos.DrawWireSphere(transform.position, radius);
    }
}
