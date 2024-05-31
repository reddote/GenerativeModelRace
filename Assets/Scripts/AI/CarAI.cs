using System.Collections.Generic;
using Drivelables;
using UnityEngine;

namespace AI{
    public class CarAI : MonoBehaviour
    {
        public float accelerationForce = 10f;
        public float maxSpeed = 50f;
        public float turnSpeed = 150f;
        public float brakeForce = 50f;

        protected Vector3 centerOfMass = new Vector3(0, 0f, 0); // This is object center of mass so it won't easily turn upside down.
        protected Rigidbody rb;
        private bool _onGround;
        private UnityEngine.AI.NavMeshAgent agent;

        public List<Transform> waypoints; // List of waypoints
        private int currentWaypointIndex = 0;

        protected virtual void Start()
        {
            rb = GetComponent<Rigidbody>();
            SetupObjectCenterOfMass();
            agent = GetComponent<UnityEngine.AI.NavMeshAgent>();

            if (agent == null)
            {
                agent = gameObject.AddComponent<UnityEngine.AI.NavMeshAgent>();
            }

            if (waypoints.Count == 0)
            {
                Debug.LogError("No waypoints assigned!");
                return;
            }

            SetNextWaypoint();
        }

        protected virtual void SetupObjectCenterOfMass()
        {
            rb.centerOfMass = centerOfMass;
            rb.WakeUp();
        }

        protected virtual void Update()
        {
            if (waypoints.Count == 0) return;

            // Check if we have reached the waypoint
            if (!agent.pathPending && agent.remainingDistance < 0.5f)
            {
                SetNextWaypoint();
            }

            // Control the car's movement based on the agent's velocity
            Vector3 localVelocity = transform.InverseTransformDirection(agent.desiredVelocity);
            float accelerationInput = localVelocity.z / maxSpeed;
            float turnInput = Mathf.Atan2(localVelocity.x, localVelocity.z);
            bool brakeInput = false; // You can define brake logic here

            MoveCar(accelerationInput, turnInput, brakeInput);
        }

        protected virtual void FixedUpdate()
        {
            if (_onGround)
            {
                // MovementDrivelableObject(); // This will be handled in Update() now
            }
        }

        protected virtual void MoveCar(float accelerationInput, float turnInput, bool brakeInput)
        {
            // Calculate acceleration and turning forces
            Vector3 acceleration = transform.forward * (accelerationForce * accelerationInput);
            float turnForce = turnSpeed * turnInput;

            // Apply acceleration and limit max speed
            rb.AddForce(acceleration, ForceMode.Acceleration);

            if (rb.velocity.magnitude > maxSpeed)
            {
                rb.velocity = rb.velocity.normalized * maxSpeed;
            }

            // Apply turning force
            if (accelerationInput != 0)
            {
                Quaternion turnRotation = Quaternion.Euler(0f, turnForce * Time.fixedDeltaTime, 0f);
                rb.MoveRotation(rb.rotation * turnRotation);
            }

            // Apply brake force
            if (brakeInput)
            {
                rb.AddForce(-rb.velocity.normalized * brakeForce, ForceMode.Acceleration);
            }
        }

        private void OnCollisionStay(Collision other)
        {
            if (other.gameObject.layer == 3)
            {
                _onGround = true;
                Debug.Log("car is on the ground");
            }
        }

        private void OnCollisionExit(Collision other)
        {
            if (other.gameObject.layer == 3)
            {
                _onGround = false;
                Debug.Log("car is not on the ground");
            }
        }

        private void SetNextWaypoint()
        {
            if (waypoints.Count == 0) return;

            agent.SetDestination(waypoints[currentWaypointIndex].position);
            currentWaypointIndex = (currentWaypointIndex + 1) % waypoints.Count;
        }

        protected virtual void OnDrawGizmos()
        {
            // Gizmos.color = Color.red;
            // Gizmos.DrawSphere(transform.position + transform.rotation * centerOfMass, 1f);
        }
    }
}
