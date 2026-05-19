using UnityEngine;
using UnityEngine.Events;

namespace IACFPSController.Puzzle
{
    public enum SwitchType
    {
        /// <summary>Player presses interact key to toggle on/off.</summary>
        Toggle,
        /// <summary>Activates while any object rests on the trigger zone; deactivates when it leaves.</summary>
        Pressure,
        /// <summary>Activates permanently when struck by an object tagged as Weapon.</summary>
        AxeTarget
    }

    [AddComponentMenu("IAC/Axe Game/Puzzle/Switch")]
    public class PuzzleSwitch : PuzzleElement
    {
        [Header("Switch Settings")]
        public SwitchType switchType = SwitchType.Toggle;
        public bool startActive = false;

        [Header("Toggle Settings")]
        [Tooltip("Key the player presses to interact (Toggle type only)")]
        public KeyCode interactKey = KeyCode.E;
        [Tooltip("Player must be inside the trigger collider on this object to interact")]
        public bool requireProximity = true;
        [Tooltip("Show a hint object when player is nearby")]
        public GameObject interactHint;

        [Header("Axe Target Settings")]
        [Tooltip("Tag that the axe/weapon must have to activate this switch (AxeTarget type only)")]
        public string weaponTag = "Weapon";
        [Tooltip("Can the switch be deactivated again after an axe hit?")]
        public bool resetAfterAxeHit = false;

        [Header("Visuals")]
        [Tooltip("Shown when switch is active")]
        public GameObject activeVisual;
        [Tooltip("Shown when switch is inactive")]
        public GameObject inactiveVisual;

        [Header("Switch Events")]
        public UnityEvent onActivated;
        public UnityEvent onDeactivated;

        // ── State ──────────────────────────────────────────────────────
        private bool _isActive;
        private bool _playerNearby;
        private int _pressureCount;

        public override bool IsComplete => _isActive;

        // ── Lifecycle ──────────────────────────────────────────────────

        void Start()
        {
            _isActive = startActive;
            ApplyVisuals();
        }

        void Update()
        {
            if (switchType != SwitchType.Toggle) return;
            if (requireProximity && !_playerNearby) return;
            if (Input.GetKeyDown(interactKey))
                Toggle();
        }

        // ── Public API ─────────────────────────────────────────────────

        public void Activate()
        {
            if (_isActive) return;
            _isActive = true;
            onActivated?.Invoke();
            ApplyVisuals();
            NotifyStateChanged();
        }

        public void Deactivate()
        {
            if (!_isActive) return;
            _isActive = false;
            onDeactivated?.Invoke();
            ApplyVisuals();
            NotifyStateChanged();
        }

        public void Toggle()
        {
            if (_isActive) Deactivate();
            else Activate();
        }

        /// <summary>Call this from a UnityEvent or external script to force-activate.</summary>
        public void ForceActivate() => Activate();

        /// <summary>Call this to reset the switch back to inactive.</summary>
        public void Reset() => Deactivate();

        // ── Trigger and Collision Handling ────────────────────────────

        private void OnTriggerEnter(Collider other)
        {
            if (switchType == SwitchType.Toggle && other.CompareTag("Player"))
            {
                _playerNearby = true;
                if (interactHint) interactHint.SetActive(true);
            }

            if (switchType == SwitchType.Pressure)
            {
                _pressureCount++;
                if (_pressureCount == 1) Activate();
            }
        }

        private void OnTriggerExit(Collider other)
        {
            if (switchType == SwitchType.Toggle && other.CompareTag("Player"))
            {
                _playerNearby = false;
                if (interactHint) interactHint.SetActive(false);
            }

            if (switchType == SwitchType.Pressure)
            {
                _pressureCount = Mathf.Max(0, _pressureCount - 1);
                if (_pressureCount == 0) Deactivate();
            }
        }

        private void OnCollisionEnter(Collision collision)
        {
            if (switchType != SwitchType.AxeTarget) return;
            if (!collision.collider.CompareTag(weaponTag)) return;

            if (!_isActive)
                Activate();
            else if (resetAfterAxeHit)
                Deactivate();
        }

        // ── Internals ──────────────────────────────────────────────────

        private void ApplyVisuals()
        {
            if (activeVisual)   activeVisual.SetActive(_isActive);
            if (inactiveVisual) inactiveVisual.SetActive(!_isActive);
        }
    }
}
