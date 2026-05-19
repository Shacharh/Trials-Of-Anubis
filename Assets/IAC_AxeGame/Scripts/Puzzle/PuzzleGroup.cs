using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace IACFPSController.Puzzle
{
    public enum PuzzleCompletionMode
    {
        /// <summary>Every element in the list must be complete.</summary>
        AllRequired,
        /// <summary>At least one element must be complete.</summary>
        AnyOne
    }

    /// <summary>
    /// Groups PuzzleElements together and fires events when the puzzle is solved or reset.
    /// Wire the onSolved event to open doors, trigger platforms, play cutscenes, etc.
    /// </summary>
    [AddComponentMenu("IAC/Axe Game/Puzzle/Puzzle Group")]
    public class PuzzleGroup : MonoBehaviour
    {
        [Header("Puzzle Identity")]
        public string puzzleName = "Unnamed Puzzle";

        [Header("Completion Logic")]
        [Tooltip("AllRequired: every element must be complete. AnyOne: only one needs to be.")]
        public PuzzleCompletionMode completionMode = PuzzleCompletionMode.AllRequired;

        [Header("Elements")]
        [Tooltip("Drag PuzzleSwitch or any PuzzleElement components here")]
        public List<PuzzleElement> elements = new List<PuzzleElement>();

        [Header("Events")]
        [Tooltip("Fired when the puzzle transitions from unsolved to solved")]
        public UnityEvent onSolved;
        [Tooltip("Fired if the puzzle resets after being solved (e.g. a pressure plate is released)")]
        public UnityEvent onReset;

        public bool IsSolved { get; private set; }

        // ── Lifecycle ──────────────────────────────────────────────────

        void Start()
        {
            foreach (PuzzleElement el in elements)
            {
                if (el != null)
                    el.onStateChanged += EvaluateCompletion;
            }

            EvaluateCompletion();
        }

        void OnDestroy()
        {
            foreach (PuzzleElement el in elements)
            {
                if (el != null)
                    el.onStateChanged -= EvaluateCompletion;
            }
        }

        // ── Evaluation ─────────────────────────────────────────────────

        private void EvaluateCompletion()
        {
            bool nowSolved = false;

            switch (completionMode)
            {
                case PuzzleCompletionMode.AllRequired:
                    nowSolved = true;
                    foreach (PuzzleElement el in elements)
                    {
                        if (el == null || !el.IsComplete) { nowSolved = false; break; }
                    }
                    break;

                case PuzzleCompletionMode.AnyOne:
                    foreach (PuzzleElement el in elements)
                    {
                        if (el != null && el.IsComplete) { nowSolved = true; break; }
                    }
                    break;
            }

            if (nowSolved && !IsSolved)
            {
                IsSolved = true;
                onSolved?.Invoke();
                PuzzleManager.Instance?.NotifySolved(this);
                Debug.Log($"[PuzzleGroup] '{puzzleName}' SOLVED.");
            }
            else if (!nowSolved && IsSolved)
            {
                IsSolved = false;
                onReset?.Invoke();
                PuzzleManager.Instance?.NotifyReset(this);
                Debug.Log($"[PuzzleGroup] '{puzzleName}' reset.");
            }
        }

        /// <summary>Force a re-evaluation (useful if you add elements at runtime).</summary>
        public void ForceEvaluate() => EvaluateCompletion();
    }
}
