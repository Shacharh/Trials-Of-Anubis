using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[AddComponentMenu("IAC/Axe Game/Player/Weapon Explode")]
public class AxeBehavior_Explosion : MonoBehaviour
{
    public float explosionForce = 75f; // the force of the explosion
    public float explosionRadius = 5f; // the radius of the explosion
    public float upwardsModifier = 0.5f; // how much the explosion will push objects upwards

    public string targetTag = "Destructable";

    private bool hasExploded = false;

    private void OnCollisionEnter(Collision collision)
    {
        if (!hasExploded && collision.collider.CompareTag(targetTag))
        {
            hasExploded = true;
            Explode(collision.contacts[0].point, collision.impulse.magnitude);
        }
    }

    private void Explode(Vector3 explosionPoint, float impactForce)
    {
        Collider[] colliders = Physics.OverlapSphere(explosionPoint, explosionRadius);
        foreach (Collider nearbyObject in colliders)
        {
            if(nearbyObject.CompareTag("Player"))
            {
                return;
            }

            Rigidbody rb = nearbyObject.GetComponent<Rigidbody>();

            if (rb != null)
            {
                rb.AddExplosionForce(explosionForce * impactForce, explosionPoint, explosionRadius, upwardsModifier);
            }
        }
    }
}
