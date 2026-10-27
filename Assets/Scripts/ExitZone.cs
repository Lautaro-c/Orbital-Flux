using UnityEngine;

[RequireComponent(typeof(Collider))]
public class ExitZone : MonoBehaviour
{
    private void Reset()
    {
        GetComponent<Collider>().isTrigger = true;
    }

    private void OnTriggerEnter(Collider other)
    {
        Rigidbody body = other.attachedRigidbody;
        if (body == null) return;

        AstronautHealth astronaut = body.GetComponentInParent<AstronautHealth>();
        if (astronaut == null || astronaut.CoreBody != body) return;

        if (LevelManager.Instance != null)
            LevelManager.Instance.NotifyRescued(astronaut);
    }
}