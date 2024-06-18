using AI;
using UnityEngine;

public class Waypoint : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("CarAI"))
        {
            CarAI carAI = other.GetComponent<CarAI>();
            if (carAI != null)
            {
                carAI.ReachWaypoint();
            }
        }
    }
}