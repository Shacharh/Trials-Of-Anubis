using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace IACFPSController.Gameplay
{
    [AddComponentMenu("IAC/Axe Game/Objects/Ammo Pickup")]
    public class AmmoPickup : MonoBehaviour
    {
        [Tooltip("How much ammo to add")]
        public int ammoValue = 1;

        [Tooltip("What tag does the player have?")]
        public string playerTag = "Player";

        public bool replaceWeapon;

        [Header("Replaceable Object")]
        public GameObject prefabToThrow;
        public GameObject newWeaponGfx;
        //public Sprite imageToShow;

        private WeaponThrower weaponController;
        private bool isPickedUp = false;

        private void Awake()
        {
            weaponController = FindObjectOfType<WeaponThrower>();
            
            isPickedUp = true;
            Invoke("EnablePickup",1f);
        }


        private void OnTriggerEnter(Collider other)
        {
            if(isPickedUp)
            {
                return;
            }

            if(other.CompareTag(playerTag))
            {
                isPickedUp = true;
                if(replaceWeapon)
                {
                    ReplaceAmmoToPlayer();
                }
                else
                {
                    AddAmmoToPlayer();
                }
            }
        }

        private void OnCollisionEnter(Collision collision)
        {
            if(isPickedUp)
            {
                return;
            }

            if (collision.collider.CompareTag(playerTag))
            {
                isPickedUp = true;
                if(replaceWeapon)
                {
                    ReplaceAmmoToPlayer();
                }
                else
                {
                    AddAmmoToPlayer();
                }
            }
        }

        public void AddAmmoToPlayer()
        {
            if (weaponController)
            {
                weaponController.AddWeaponAmmo(ammoValue);
                Destroy(this.gameObject);
            }
        }

        public void ReplaceAmmoToPlayer()
        {
             if (weaponController)
            {
                 weaponController.UpdateThrowItem(prefabToThrow,newWeaponGfx ,ammoValue);
                Destroy(this.gameObject);
            }
        }

        public void EnablePickup()
        {
            isPickedUp = false;
        }
    }
}
