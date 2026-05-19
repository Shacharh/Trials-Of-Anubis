using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace IACFPSController.Gameplay
{
    [AddComponentMenu("IAC/Axe Game/Objects/Destroy Object")]
    public class DestroyAfterTime : MonoBehaviour
    {
        [Tooltip("Destroy this object after x Seconds")]
        public bool destroyOnStart = true;
        public float destroyTime = 5f;

        public void Start()
        {
            if(destroyOnStart)
            {
                Invoke("Destruction", destroyTime);
            }
        }

        public void DestoryThisObject(float delayedTime)
        {
            Invoke("Destruction", delayedTime);
        }

        private void Destruction()
        {
             Destroy(this.gameObject);
        }
    }
}