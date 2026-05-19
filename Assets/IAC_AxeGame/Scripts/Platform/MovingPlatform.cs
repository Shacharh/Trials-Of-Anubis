using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace IACFPSController.Gameplay
{
    public enum PlatformFlowMode { Sequential, Branching }
    public enum PlatformLoopMode { Loop, PingPong, Once }

    [System.Serializable]
    public class PlatformWaypoint
    {
        [Tooltip("The Transform this platform moves toward (use an empty GameObject)")]
        public Transform target;

        [Tooltip("Seconds to wait after arriving. Set to -1 to wait until TriggerMove() is called externally")]
        public float waitDuration = 1f;

        [Tooltip("Branching mode only: indices of waypoints this point can branch to (e.g. {1, 2} means branch 0 goes to waypoint 1, branch 1 goes to waypoint 2)")]
        public int[] nextWaypointOptions;

        [Tooltip("Fired when the platform arrives at this waypoint")]
        public UnityEvent onArrival;
    }

    [AddComponentMenu("IAC/Axe Game/Platform/Moving Platform")]
    public class MovingPlatform : MonoBehaviour
    {
        [Header("Waypoints")]
        [Tooltip("Add waypoints in visit order. Platform starts at its current Transform position, then moves through this list.")]
        public List<PlatformWaypoint> waypoints = new List<PlatformWaypoint>();

        [Header("Movement Settings")]
        public float speed = 3f;
        [Tooltip("Sequential: visits waypoints in list order. Branching: can fork to different next points (set via SetBranch()).")]
        public PlatformFlowMode flowMode = PlatformFlowMode.Sequential;
        [Tooltip("Loop: wraps back to start. PingPong: reverses direction. Once: stops at final waypoint.")]
        public PlatformLoopMode loopMode = PlatformLoopMode.Loop;

        [Header("Start Settings")]
        public bool moveOnStart = true;
        [Tooltip("Which waypoint index the platform moves to first when it starts")]
        public int startIndex = 0;

        [Header("Events")]
        public UnityEvent onReachedFinalWaypoint;

        // ── Private State ──────────────────────────────────────────────

        private int _currentIndex = -1;
        private int _nextIndex = 0;
        private int _pingPongDir = 1;
        private int _selectedBranch = 0;
        private bool _waitingForTrigger;
        private bool _running;

        // Passenger tracking (delta-based so it works with CharacterController)
        private readonly List<Transform> _passengers = new List<Transform>();
        private Vector3 _prevPosition;

        // ── Unity Lifecycle ────────────────────────────────────────────

        void Start()
        {
            _prevPosition = transform.position;
            _nextIndex = Mathf.Clamp(startIndex, 0, Mathf.Max(0, waypoints.Count - 1));

            if (moveOnStart && waypoints.Count > 0)
                StartMoving();
        }

        void Update()
        {
            // Record position BEFORE the coroutine moves the platform this frame.
            // LateUpdate then computes the delta and applies it to passengers.
            _prevPosition = transform.position;
        }

        void LateUpdate()
        {
            if (_passengers.Count == 0) return;

            Vector3 delta = transform.position - _prevPosition;
            if (delta.sqrMagnitude < 0.000001f) return;

            foreach (Transform passenger in _passengers)
            {
                if (passenger == null) continue;

                CharacterController cc = passenger.GetComponent<CharacterController>();
                if (cc != null)
                    cc.Move(delta);
                else
                    passenger.position += delta;
            }
        }

        // ── Movement ──────────────────────────────────────────────────

        public void StartMoving()
        {
            if (_running || waypoints.Count == 0) return;
            StartCoroutine(MovementRoutine());
        }

        private IEnumerator MovementRoutine()
        {
            _running = true;

            while (true)
            {
                if (_nextIndex < 0 || _nextIndex >= waypoints.Count) break;

                PlatformWaypoint target = waypoints[_nextIndex];
                Vector3 destination = target.target.position;

                // Move toward waypoint
                while (Vector3.Distance(transform.position, destination) > 0.02f)
                {
                    transform.position = Vector3.MoveTowards(transform.position, destination, speed * Time.deltaTime);
                    yield return null;
                }

                transform.position = destination;
                _currentIndex = _nextIndex;

                target.onArrival?.Invoke();

                // Check if this is the last reachable point in Once mode
                if (loopMode == PlatformLoopMode.Once && IsLastWaypoint(_currentIndex))
                {
                    onReachedFinalWaypoint?.Invoke();
                    _running = false;
                    yield break;
                }

                // Wait at waypoint
                if (target.waitDuration < 0f)
                {
                    _waitingForTrigger = true;
                    yield return new WaitUntil(() => !_waitingForTrigger);
                }
                else if (target.waitDuration > 0f)
                {
                    yield return new WaitForSeconds(target.waitDuration);
                }

                _nextIndex = ResolveNextIndex(_currentIndex);
            }

            _running = false;
        }

        private int ResolveNextIndex(int current)
        {
            if (flowMode == PlatformFlowMode.Branching)
            {
                int[] options = waypoints[current].nextWaypointOptions;
                if (options != null && options.Length > 0)
                {
                    int branch = Mathf.Clamp(_selectedBranch, 0, options.Length - 1);
                    return options[branch];
                }
            }

            switch (loopMode)
            {
                case PlatformLoopMode.Loop:
                    return (current + 1) % waypoints.Count;

                case PlatformLoopMode.PingPong:
                    int next = current + _pingPongDir;
                    if (next >= waypoints.Count) { _pingPongDir = -1; next = current - 1; }
                    else if (next < 0)            { _pingPongDir =  1; next = current + 1; }
                    return Mathf.Clamp(next, 0, waypoints.Count - 1);

                case PlatformLoopMode.Once:
                    return Mathf.Min(current + 1, waypoints.Count - 1);

                default:
                    return (current + 1) % waypoints.Count;
            }
        }

        private bool IsLastWaypoint(int index)
        {
            return index == waypoints.Count - 1 ||
                   (loopMode == PlatformLoopMode.PingPong && index == 0 && _pingPongDir == -1);
        }

        // ── Public Controls ───────────────────────────────────────────

        /// <summary>Resumes a platform that is waiting (waitDuration = -1).</summary>
        public void TriggerMove()
        {
            _waitingForTrigger = false;
        }

        /// <summary>Branching mode: choose which exit to take from the next waypoint (0-based index into nextWaypointOptions).</summary>
        public void SetBranch(int branchIndex)
        {
            _selectedBranch = branchIndex;
        }

        /// <summary>Branching mode: set branch AND immediately resume if waiting.</summary>
        public void SetBranchAndMove(int branchIndex)
        {
            _selectedBranch = branchIndex;
            _waitingForTrigger = false;
        }

        /// <summary>Directly override the next waypoint index and resume.</summary>
        public void SetNextWaypoint(int waypointIndex)
        {
            _nextIndex = Mathf.Clamp(waypointIndex, 0, waypoints.Count - 1);
            _waitingForTrigger = false;
        }

        public void StopPlatform()
        {
            StopAllCoroutines();
            _running = false;
            _waitingForTrigger = false;
        }

        // ── Passenger Zone (requires a Trigger Collider on this GameObject) ──

        private void OnTriggerEnter(Collider other)
        {
            Transform t = other.transform;
            if (!_passengers.Contains(t))
                _passengers.Add(t);
        }

        private void OnTriggerExit(Collider other)
        {
            _passengers.Remove(other.transform);
        }
    }
}
