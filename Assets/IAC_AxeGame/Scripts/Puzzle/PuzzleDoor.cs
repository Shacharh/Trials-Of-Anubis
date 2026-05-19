using UnityEngine;
using UnityEngine.Events;

namespace IACFPSController.Puzzle
{
    /// <summary>
    /// A door or gate that can be opened, closed, locked, and unlocked.
    /// Wire a PuzzleGroup's onSolved event to UnlockAndOpen() to create puzzle-gated doors.
    /// The actual visual animation is driven by UnityEvents — connect them to your Animator
    /// or moving parts in the Inspector.
    /// </summary>
    [AddComponentMenu("IAC/Axe Game/Puzzle/Door")]
    public class PuzzleDoor : MonoBehaviour
    {
        [Header("Door State")]
        public bool startOpen = false;
        [Tooltip("If true, Open() will fail and fire onLockedAttempt until Unlock() is called")]
        public bool startsLocked = true;

        [Header("Player Interaction")]
        [Tooltip("Allow the player to manually open/close with a key when nearby")]
        public bool playerCanInteract = false;
        public KeyCode interactKey = KeyCode.E;
        public GameObject interactHint;

        [Header("Events")]
        [Tooltip("Wire to your Animator SetTrigger, or move a door mesh, etc.")]
        public UnityEvent onOpen;
        public UnityEvent onClose;
        [Tooltip("Fired when something tries to open a locked door")]
        public UnityEvent onLockedAttempt;

        // ── State ──────────────────────────────────────────────────────
        private bool _isOpen;
        private bool _isLocked;
        private bool _playerNearby;

        public bool IsOpen   => _isOpen;
        public bool IsLocked => _isLocked;

        // ── Lifecycle ──────────────────────────────────────────────────

        void Start()
        {
            _isLocked = startsLocked;
            _isOpen   = false;

            if (startOpen)
                Open();
        }

        void Update()
        {
            if (!playerCanInteract || !_playerNearby) return;
            if (Input.GetKeyDown(interactKey))
                Toggle();
        }

        // ── Public API ─────────────────────────────────────────────────

        public void Open()
        {
            if (_isLocked) { onLockedAttempt?.Invoke(); return; }
            if (_isOpen) return;
            _isOpen = true;
            onOpen?.Invoke();
        }

        public void Close()
        {
            if (!_isOpen) return;
            _isOpen = false;
            onClose?.Invoke();
        }

        public void Toggle()
        {
            if (_isOpen) Close();
            else Open();
        }

        public void Unlock()
        {
            _isLocked = false;
        }

        /// <summary>Connect this to PuzzleGroup.onSolved to gate a door behind a puzzle.</summary>
        public void UnlockAndOpen()
        {
            Unlock();
            Open();
        }

        public void Lock()
        {
            _isLocked = true;
        }

        public void LockAndClose()
        {
            Lock();
            Close();
        }

        // ── Player Proximity ───────────────────────────────────────────

        private void OnTriggerEnter(Collider other)
        {
            if (!playerCanInteract || !other.CompareTag("Player")) return;
            _playerNearby = true;
            if (interactHint) interactHint.SetActive(true);
        }

        private void OnTriggerExit(Collider other)
        {
            if (!playerCanInteract || !other.CompareTag("Player")) return;
            _playerNearby = false;
            if (interactHint) interactHint.SetActive(false);
        }
    }
}
