using UnityEngine;
using System.Collections.Generic;
using System.Linq;
using TMPro;

public class RaceManager : MonoBehaviour
{
	public Transform[] checkpoints;
	public CarProgress playerCar;
	private List<CarProgress> cars;
	[SerializeField] private TextMeshProUGUI trackPosition;
	[SerializeField] private TextMeshProUGUI allCarCount;

	void Start()
	{
		cars = FindObjectsOfType<CarProgress>().ToList();
		allCarCount.text = "2";
	}

	void LateUpdate()
	{
		UpdateCarPositions();
	}

	void UpdateCarPositions()
	{
		// Sort cars by checkpoint index and then by distance to next checkpoint
		cars = cars.OrderByDescending(c => c.currentCheckpointIndex)
			.ThenBy(c => c.GetDistanceToNextCheckpoint())
			.ToList();

		// Display the current order
		for (int i = 0; i < cars.Count; i++)
		{
			if (cars[i] == playerCar){
				trackPosition.text = (i + 1) + "";
			}
			Debug.Log($"Car {cars[i].name} is in position {i + 1}");
		}
	}
}