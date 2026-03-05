using UnityEngine;
using Oculus.Interaction;
using Oculus.Interaction.PoseDetection; // Necessary for Sequence

public class SequenceBridge : MonoBehaviour, IActiveState
{
    // 1. Reference the Sequence component on the same object
    [SerializeField] public Sequence _sequence;

    // 2. Implement the Active property from IActiveState
    public bool Active => _sequence != null && _sequence.Active;

    // 3. Optional: Validation to ensure the component is attached
    private void OnValidate()
    {
        if (_sequence == null)
        {
            _sequence = GetComponent<Sequence>();
        }
    }
}