using UnityEngine;

public class LevelBorder : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        if (other.TryGetComponent(out RocketController rocket))
        {
            rocket.PlayerDeath();
        }
    }
}