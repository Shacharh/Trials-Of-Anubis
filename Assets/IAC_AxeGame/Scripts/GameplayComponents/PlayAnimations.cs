using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace IACFPSController.Gameplay
{

    [RequireComponent(typeof(Animation))]
    [AddComponentMenu("IAC/Axe Game/Objects/Animation Player")]
    public class PlayAnimations : MonoBehaviour
    {
        [Header("Settings:")]
        [SerializeField] private bool _isLooping = true;
        [SerializeField] private bool _playOnAwake = true;
        [SerializeField] private string _triggerTag = "Player";
    
        private Animation _animation;

        private void Reset()
        {
            GetAnimationComponentIfNeeded();
        }

        private void Awake()
        {
            Init();
            if (_playOnAwake)
            {
                PlayAnimation();
            }
        }
        private void Init()
        {
            GetAnimationComponentIfNeeded();
            var clip = _animation.clip;
            if (clip == null)
            {
                return;
            }

            clip.legacy = true;
            clip.wrapMode = _isLooping ? WrapMode.Loop : WrapMode.Default;
        }

        private void GetAnimationComponentIfNeeded()
        {
            _animation ??= GetComponent<Animation>();
            _animation.playAutomatically = false;
        }

        private void OnTriggerEnter(Collider other)
        {
            if (other.CompareTag(_triggerTag))
            {
                PlayAnimation();
            }
        }

        //Public to be called from unity events in the inspector
        public void PlayAnimation()
        {
            if (_animation.clip == null)
            {
                Debug.LogError("AnimationPlayer PlayAnimation Error: No clip available in the animation component");
                return;
            }

            _animation.Play();
        }

        public void PlayAnimationByName(string name)
        {
             if (_animation.clip == null)
            {
                Debug.LogError("AnimationPlayer PlayAnimation Error: No clip available in the animation component");
                return;
            }

            _animation.Play(name);
        }
    }

}