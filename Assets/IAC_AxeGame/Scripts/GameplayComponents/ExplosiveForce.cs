using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace IACFPSController.Gameplay
{
    [AddComponentMenu("IAC/Axe Game/Objects/Explosive Force")]
    public class ExplosiveForce : MonoBehaviour
    {
        public Transform explosionPoint;
        public float explosiveForce = 10f;
        public float explosiveRange = 5f;

        public bool destroyObjects = false;
        public float destoryDelayTime = 5f;

        // Update is called once per frame
        public void CreateExplosion()
        {
            Rigidbody[] destructableParts = this.GetComponentsInChildren<Rigidbody>();
                foreach (var rb in destructableParts)
                {
                    if (rb)
                    {
                        rb.AddExplosionForce(explosiveForce * Random.Range(0.1f, 0.5f),
                                            explosionPoint.position, explosiveRange * Random.Range(0.5f, 2f),
                                            0.1f,
                                            ForceMode.Impulse);
                    }
                }

            if(destroyObjects)
            {
                Destroy(this, destoryDelayTime);
            }    
        }
    }
}
