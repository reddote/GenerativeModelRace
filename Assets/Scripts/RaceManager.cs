using UnityEngine;
using System.Collections.Generic;
using System.Linq;

public class RaceManager : MonoBehaviour
{
    public Transform[] checkpoints;
    private List<CarProgress> cars = new List<CarProgress>();

    void Start()
    {
        // Initialize the list of cars
        cars = FindObjectsOfType<CarProgress>().ToList();
    }

    public void UpdateCarProgress(CarProgress car)
    {
        // Sort cars by checkpoint index and then by distance to next checkpoint
        cars = cars.OrderByDescending(c => c.currentCheckpointIndex)
            .ThenBy(c => c.GetDistanceToNextCheckpoint())
            .ToList();

        // Display the current order
        for (int i = 0; i < cars.Count; i++)
        {
            Debug.Log($"Car {cars[i].name} is in position {i + 1}");
        }
    }
}