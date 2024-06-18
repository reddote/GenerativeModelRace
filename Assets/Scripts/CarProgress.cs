using UnityEngine;

public class CarProgress : MonoBehaviour
{
    public int currentCheckpointIndex = 0;
    public Transform nextCheckpoint;
    private RaceManager raceManager;

    void Start()
    {
        raceManager = FindObjectOfType<RaceManager>();
        UpdateNextCheckpoint();
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Checkpoint") && other.transform == nextCheckpoint)
        {
            currentCheckpointIndex++;
            UpdateNextCheckpoint();
            raceManager.UpdateCarProgress(this);
        }
    }

    void UpdateNextCheckpoint()
    {
        // Assuming checkpoints are stored in an array in the race manager
        if (currentCheckpointIndex < raceManager.checkpoints.Length)
        {
            nextCheckpoint = raceManager.checkpoints[currentCheckpointIndex];
        }
    }

    public float GetDistanceToNextCheckpoint()
    {
        if (nextCheckpoint != null)
        {
            return Vector3.Distance(transform.position, nextCheckpoint.position);
        }
        return float.MaxValue;
    }
}