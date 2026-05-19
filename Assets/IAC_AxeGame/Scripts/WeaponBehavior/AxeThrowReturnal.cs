using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[AddComponentMenu("IAC/Axe Game/Player/Weapon Returnal")]
public class AxeThrowReturnal : MonoBehaviour
{
        [Header("Axe Creation Settings")]
        [Tooltip("Place the Axe Prefab here that you want the character to throw")]
        public GameObject axeObject;
        [Tooltip("Place the Axe Origin Empty Game Object here. This is the location from which the axe will be thrown")]
        public Transform axeOrigin;

        [Header("Behavior Settings")]
        [Tooltip("The base power of the axe being thrown")]
        public float throwPower = 20f;
        public float torquePower = 50f;
        [Tooltip("Specify the delay in seconds between throws")]
        public float throwDelay = 0.5f;
        [Tooltip("Should the Axe auto aim towards center of screen?")]
        public bool autoCenter = true;
        [Tooltip("Apporximate the distance from player to which the axe will reach")]
        public float centerDistance = 20f;


        [Header("Advanced Settings")]
        [Tooltip("The gfx object of the axe in hand")]
        public GameObject axeGfx;
        [Tooltip("Connect the axe origin empty game object here, with an animator component")]
        public Animator axeAnimator;


        //Private Variables
        private float powerMultiplier = 0;
        private bool allowThrow;
        private float throwTimer;
        private bool startedThrow;
        private bool isReturning;
        private Vector3 originalAxePos;

        private Vector3 returnOffset;

        //Axe Components
        private Transform axeTransform;
        private Rigidbody axeRB;


        // Start is called before the first frame update
        void Start()
        {
            powerMultiplier = 0f;
            throwTimer = 0;
            startedThrow = false;
            allowThrow = true;
            isReturning = false;

            originalAxePos = axeOrigin.localEulerAngles;
            axeTransform = axeObject.transform;
            axeRB = axeObject.GetComponent<Rigidbody>();
        }

        // Update is called once per frame
        void Update()
        {
            if (!allowThrow)
            {
                if(!isReturning)
                {
                    if(Input.GetButtonDown("Fire1"))
                    {
                        axeRB.linearVelocity = Vector3.zero;
                        axeRB.angularVelocity = Vector3.zero;
                        axeRB.useGravity = false;
                        isReturning = true;
                        returnOffset = axeTransform.position - transform.position;
                    }
                }
           
                if(isReturning)
                {
                    ReturnAxe();
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

                if (axeAnimator)
                {
                    axeAnimator.SetTrigger("Throw");
                }
            }

            if (startedThrow)
            {
                if (Input.GetButton("Fire1"))
                {

                    PositionAxe();
                    powerMultiplier += Time.deltaTime;
                    powerMultiplier = Mathf.Clamp(powerMultiplier, 0.25f, 1.5f);
                }

                if (Input.GetButtonUp("Fire1"))
                {
                    ThrowAxe();

                    if (axeAnimator)
                    {
                        axeAnimator.SetTrigger("Return");
                    }
                    startedThrow = false;
                    powerMultiplier = 0;

                    EnableAxeThrow(false);
                }
            }
        }

        public void PositionAxe()
        {
            if(autoCenter)
            {
                AimTowardCenter();
            }

            axeTransform.position = axeOrigin.position;
            axeTransform.rotation = axeOrigin.rotation;
        }

        public void ThrowAxe()
        {
            axeObject.SetActive(true);

            axeRB.AddForce(axeObject.transform.forward * throwPower * powerMultiplier, ForceMode.Impulse);
            axeRB.AddTorque(axeObject.transform.right * torquePower * powerMultiplier);
        }

        public void ReturnAxe()
        {
        Vector3 targetPosition = transform.position + returnOffset + Vector3.up * 1.5f;

        axeTransform.position = Vector3.Lerp(axeTransform.position, axeOrigin.position, 5f * Time.deltaTime);
        axeRB.angularVelocity = -axeObject.transform.right * 50f * Time.deltaTime;
        
        axeTransform.LookAt(axeOrigin.position);
        
        //add a spherical movement to the axe's return, and add some torque;

         if (Vector3.Distance(axeTransform.position, axeOrigin.position) < 0.5f)
            {
                Debug.Log("is back");
                isReturning = false;
                EnableAxeThrow(true);
                axeRB.linearVelocity = Vector3.zero;
                axeRB.angularVelocity = Vector3.zero;
                axeObject.SetActive(false);
                //reset isHit on axe exploding script;
            }

        }

        public void EnableAxeThrow(bool enable)
        {
            if (axeGfx)
            {
                axeGfx.gameObject.SetActive(enable);
            }

            allowThrow = enable;
            axeRB.useGravity = true;
        }

        public void ZeroAmmo()
        {
            if (axeGfx)
            {
                axeGfx.gameObject.SetActive(false);
            }
        }

        public void AimTowardCenter()
        {
            Vector3 crosshairPoint = Vector3.zero;
            RaycastHit hitInfo;
            
            if(Physics.Raycast(Camera.main.transform.position,Camera.main.transform.forward, out hitInfo, centerDistance * powerMultiplier))
            {
                crosshairPoint = hitInfo.point;
            }
            else
            {
                crosshairPoint = Camera.main.ScreenToWorldPoint(new Vector3(Screen.width / 2, Screen.height / 2, centerDistance * powerMultiplier));
            }
           
            axeOrigin.LookAt(crosshairPoint);
        }
        

      
    }