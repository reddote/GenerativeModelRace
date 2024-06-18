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
			// Notify race manager (not necessary if updating in Update in RaceManager)
			// raceManager.UpdateCarProgress(this);
		}
	}

	void UpdateNextCheckpoint()
	{
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