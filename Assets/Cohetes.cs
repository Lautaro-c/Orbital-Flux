using UnityEngine;

public class DestruirConAsteroide : MonoBehaviour
{
    [SerializeField] private GameObject explosionPrefab;

    private int asteroidesLayer;
    private bool destruido;

    private void Awake()
    {
        asteroidesLayer = LayerMask.NameToLayer("Asteroides");
    }

    private void OnCollisionEnter(Collision collision)
    {
        ProcesarImpacto(collision.collider);
    }

    private void OnTriggerEnter(Collider other)
    {
        ProcesarImpacto(other);
    }

    private void ProcesarImpacto(Collider other)
    {
        if (destruido || other.gameObject.layer != asteroidesLayer)
            return;

        destruido = true;

        // Si el collider es hijo, destruye el objeto de su Rigidbody.
        GameObject asteroide = other.attachedRigidbody != null
            ? other.attachedRigidbody.gameObject
            : other.gameObject;

        if (explosionPrefab != null)
        {
            GameObject explosion = Instantiate(
                explosionPrefab,
                transform.position,
                explosionPrefab.transform.rotation
            );

            Destroy(explosion, 0.25f);
        }

        Destroy(asteroide);
        Destroy(gameObject);
    }
}