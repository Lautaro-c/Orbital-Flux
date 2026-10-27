using UnityEngine;

[DisallowMultipleComponent]
public class LimbImpactReporter : MonoBehaviour
{
    [Tooltip("Multiplicador de daño de esta parte del cuerpo.")]
    public float damageMultiplier = 1f;

    private AstronautHealth owner;

    public void Init(AstronautHealth astronaut)
    {
        owner = astronaut;
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (owner == null) return;
        if (collision.collider.transform.IsChildOf(owner.transform)) return;
        if (collision.collider.GetComponentInParent<DroneMovement>() != null) return;
        owner.ReportImpact(collision.relativeVelocity.magnitude, damageMultiplier);
    }
}