using UnityEngine;
using UnityEngine.Events;

namespace IACFPSController.Puzzle
{
    /// <summary>
    /// Base class for anything that can be tracked as part of a puzzle.
    /// Derive from this to create switches, levers, pressure plates, etc.
    /// </summary>
    public abstract class PuzzleElement : MonoBehaviour
    {
        [Header("Puzzle Element Identity")]
        [Tooltip("Descriptive name shown in logs and the Puzzle Manager inspector")]
        public string elementName = "Unnamed Element";

        /// <summary>True when this element's puzzle condition is satisfied.</summary>
        public abstract bool IsComplete { get; }

        [Header("Element Events")]
        public UnityEvent onCompleted;
        public UnityEvent onReset;

        // Internal event that PuzzleGroup subscribes to.
        // Not shown in inspector; fires whenever IsComplete may have changed.
        public System.Action onStateChanged;

        protected void NotifyStateChanged()
        {
            onStateChanged?.Invoke();

            if (IsComplete)
                onCompleted?.Invoke();
            else
                onReset?.Invoke();
        }
    }
}
