using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace IACFPSController.Puzzle
{
    /// <summary>
    /// Scene-level singleton. Place one on a dedicated Manager GameObject in your scene.
    /// Tracks all PuzzleGroups and fires onLevelComplete when the win condition is met.
    /// </summary>
    [AddComponentMenu("IAC/Axe Game/Puzzle/Puzzle Manager")]
    public class PuzzleManager : MonoBehaviour
    {
        public static PuzzleManager Instance { get; private set; }

        [Header("Level Puzzle Groups")]
        [Tooltip("Drag every PuzzleGroup in this scene here so the Manager can track them")]
        public List<PuzzleGroup> puzzleGroups = new List<PuzzleGroup>();

        [Header("Level Completion")]
        [Tooltip("If true, onLevelComplete fires only when ALL listed groups are solved. If false, fires when any one is.")]
        public bool requireAllGroups = true;

        [Header("Events")]
        [Tooltip("Fired when the level's win condition is met")]
        public UnityEvent onLevelComplete;
        [Tooltip("Fired whenever any group is reset")]
        public UnityEvent onAnyGroupReset;

        private readonly HashSet<PuzzleGroup> _solved = new HashSet<PuzzleGroup>();
        private bool _levelComplete;

        // ── Lifecycle ──────────────────────────────────────────────────

        void Awake()
        {
            if (Instance != null && Instance != this) { Destroy(gameObject); return; }
            Instance = this;
        }

        // ── Called by PuzzleGroup ──────────────────────────────────────

        public void NotifySolved(PuzzleGroup group)
        {
            _solved.Add(group);
            EvaluateLevelComplete();
        }

        public void NotifyReset(PuzzleGroup group)
        {
            _solved.Remove(group);
            _levelComplete = false;
            onAnyGroupReset?.Invoke();
        }

        // ── Evaluation ─────────────────────────────────────────────────

        private void EvaluateLevelComplete()
        {
            if (_levelComplete) return;

            bool complete = requireAllGroups
                ? puzzleGroups.TrueForAll(g => _solved.Contains(g))
                : _solved.Count > 0;

            if (complete)
            {
                _levelComplete = true;
                onLevelComplete?.Invoke();
                Debug.Log("[PuzzleManager] Level complete!");
            }
        }

        // ── Public Queries ─────────────────────────────────────────────

        public bool IsSolved(PuzzleGroup group) => _solved.Contains(group);
        public bool IsLevelComplete() => _levelComplete;
        public int SolvedCount => _solved.Count;
        public int TotalCount => puzzleGroups.Count;
    }
}
