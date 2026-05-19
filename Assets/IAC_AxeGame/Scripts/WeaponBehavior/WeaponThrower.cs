using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Linq.Expressions;

namespace IACFPSController.Gameplay
{
    [AddComponentMenu("IAC/Axe Game/Player/Weapon Throw")]
    public class WeaponThrower : MonoBehaviour
    {
        [Header("Weapon Creation Settings")]
        [Tooltip("Place the Weapon Prefab here that you want the character to throw")]
        public GameObject weaponObject;
        [Tooltip("Place the Weapon Origin Empty Game Object here. This is the location from which the weapon will be thrown")]
        public Transform weaponOrigin;

        [Header("Behavior Settings")]
        [Tooltip("The base power of the weapon being thrown")]
        public float throwPower = 20f;
        public float torquePower = 50f;
        [Tooltip("Specify the delay in seconds between throws")]
        public float throwDelay = 0.5f;
        [Tooltip("Should the weapon auto aim towards center of screen?")]
        public bool autoCenter = true;
        [Tooltip("Apporximate the distance from player to which the weapon will reach")]
        public float centerDistance = 20f;
        [Tooltip("Charge up the power of the throw by holding the Left Mouse Button")]
        public bool chargeUpThrow = true;

        [Header("Ammo Settings")]
        [Tooltip("Start Ammo Count")]
        public int startAmmo = 5;
        [Tooltip("Max Ammo Count - 0 will create 9999 as max ammo")]
        public int maxAmmo = 0;


        [Header("Advanced Settings")]
        [Tooltip("The gfx object of the weapon in hand")]
        public GameObject weaponGfx;
        [Tooltip("Reference to UI element for Ammo Count")]
        public TMP_Text weaponAmmoText; 
        [Tooltip("Auto hide ammo when hiding the weapon gfx?")]   
        public bool hideAmmoText = true;   

        /*
        //UI Elements
        [Header("UI Elements")]
        public Image weaponIcon;
        public Image weaponCooldown;
        public bool hideCooldown = false;
        */


       // [Tooltip("Connect the weapon origin empty game object here, with an animator component")]
        private Animator weaponAnimator;


        //Private Variables
        private float powerMultiplier = 0;
        private int currentAmmo;
        private bool allowThrow;
        private float throwTimer;
        private bool startedThrow;
        private Vector3 originalWeaponPos;


        // Start is called before the first frame update
        void Start()
        {
            powerMultiplier = 0f;
            throwTimer = 0;
            startedThrow = false;

            if (startAmmo == 0)
            {
                ZeroAmmo();
            }
            if (maxAmmo == 0)
            {
                maxAmmo = 99999;
            }
            Animator tempAnimator = weaponOrigin.GetComponent<Animator>();
            if(tempAnimator)
            {
                weaponAnimator = tempAnimator;
            }

            originalWeaponPos = weaponOrigin.localEulerAngles;

            currentAmmo = startAmmo;
            UpdateAmmoUI();

        }

        // Update is called once per frame
        void Update()
        {
            if (currentAmmo <= 0)
            {
                return;
            }

            if (!allowThrow)
            {
                throwTimer += Time.deltaTime;
                if (throwTimer >= throwDelay)
                {
                    throwTimer = 0;
                    weaponOrigin.localEulerAngles = originalWeaponPos;
                    EnableWeaponThrow(true);
                }

                return;
            }

            ThrowInput();
        }

        private void ThrowInput()
        {
            if (Input.GetButtonDown("Fire1"))
            {
                startedThrow = true;

                if (weaponAnimator)
                {
                    weaponAnimator.SetTrigger("Throw");
                }
            }

            if (startedThrow)
            {
                if (Input.GetButton("Fire1") && chargeUpThrow)
                {

                    powerMultiplier += Time.deltaTime;
                    powerMultiplier = Mathf.Clamp(powerMultiplier, 0.25f, 1.5f);
                }

                if (Input.GetButtonUp("Fire1"))
                {
                    if(!chargeUpThrow)
                    {
                        powerMultiplier = 1;
                    }
                    CreateWeapon();

                    if (weaponAnimator)
                    {
                        weaponAnimator.SetTrigger("Return");
                    }
                    startedThrow = false;
                    powerMultiplier = 0;


                    EnableWeaponThrow(false);
                }
            }
        }

        public void CreateWeapon()
        {
            if(autoCenter)
            {
                AimTowardCenter();
            }
            GameObject weaponTemp = Instantiate(weaponObject, weaponOrigin.position,  weaponOrigin.rotation);

            weaponTemp.GetComponent<Rigidbody>().AddForce(weaponTemp.transform.forward * throwPower * powerMultiplier, ForceMode.Impulse);
            weaponTemp.GetComponent<Rigidbody>().AddTorque(weaponTemp.transform.right * torquePower * powerMultiplier);

            currentAmmo--;

            if (currentAmmo <= 0)
            {
                currentAmmo = 0;
                ZeroAmmo();
            }

            UpdateAmmoUI();
        }

        public void EnableWeaponThrow(bool enable)
        {
            if (weaponGfx)
            {
                weaponGfx.gameObject.SetActive(enable);
                if(weaponAmmoText && hideAmmoText)
                {
                    weaponAmmoText.gameObject.SetActive(enable);
                }

            }
            //UI Elements
            /*
            if(weaponCooldown && hideCooldown)
            {
                weaponCooldown.transform.parent.gameObject.SetActive(!enable);
            }
            */
            
            allowThrow = enable;
        }

        public void ZeroAmmo()
        {
            if (weaponGfx)
            {
                weaponGfx.gameObject.SetActive(false);
                if(weaponAmmoText && hideAmmoText)
                {
                    weaponAmmoText.gameObject.SetActive(false);
                }

            }
            //UI Elements
            /*
            if (weaponCooldown)
            {
                weaponCooldown.transform.parent.gameObject.SetActive(false);
            }
            */
        }

        public void AimTowardCenter()
        {
            Vector3 crosshairPoint = Vector3.zero;
            RaycastHit hitInfo;
            
            if(Physics.Raycast(Camera.main.transform.position,Camera.main.transform.forward, out hitInfo, centerDistance * powerMultiplier, -1, QueryTriggerInteraction.Ignore))
            {
                crosshairPoint = hitInfo.point;
            }
            else
            {
                crosshairPoint = Camera.main.ScreenToWorldPoint(new Vector3(Screen.width / 2, Screen.height / 2, centerDistance * powerMultiplier));
            }
           
            weaponOrigin.LookAt(crosshairPoint);
        }
        


        public void AddWeaponAmmo(int ammoCount)
        {

            currentAmmo += ammoCount;
            if (currentAmmo >= 0)
            {
                if (weaponGfx)
                {
                    weaponGfx.gameObject.SetActive(true);
                    if(weaponAmmoText && hideAmmoText)
                    {
                        weaponAmmoText.gameObject.SetActive(true);
                    }
                }
            }
            UpdateAmmoUI();
        }

        public void UpdateAmmoUI()
        {
            currentAmmo = Mathf.Clamp(currentAmmo, 0, maxAmmo);
            if (weaponAmmoText)
            {
                weaponAmmoText.text = currentAmmo.ToString();
            }
        }



         public void UpdateThrowItem (GameObject throwItem, GameObject newWeaponGfx, int ammoToSet)
        {
            weaponObject = throwItem;

            //Create the new weapon, place in the same location as current weapon, destroy the current weapon.
            GameObject newWeapon = Instantiate(newWeaponGfx, weaponGfx.transform); 
            newWeapon.transform.SetParent(weaponGfx.transform.parent);
            Destroy(weaponGfx);
            weaponGfx = newWeapon;       

            currentAmmo = 0;
            AddWeaponAmmo(ammoToSet);
        }

    }
}
