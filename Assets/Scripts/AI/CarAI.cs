using System;
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
        private bool _isCheck = false;

        public List<Transform> waypoints; // List of waypoints
        private int _currentWaypointIndex = 0;

        [SerializeField] private GameObject[] particles;

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

            // Apply turning force only if velocity is not zero
            if (rb.velocity.magnitude > 0)
            {
                float _turnForce = turnSpeed * turnInput * Time.fixedDeltaTime;
                Quaternion _turnRotation = Quaternion.Euler(0f, _turnForce, 0f);
                rb.MoveRotation(rb.rotation * _turnRotation);
            }

            // Apply brake force
            if (brakeInput)
            {
                rb.AddForce(-rb.velocity.normalized * brakeForce, ForceMode.Acceleration);
            }
        }

        public void ReachWaypoint()
        {
            SetNextWaypoint();
        }

        private void SetNextWaypoint()
        {
            if (waypoints.Count == 0) return;

            agent.SetDestination(waypoints[_currentWaypointIndex].position);
            _currentWaypointIndex = (_currentWaypointIndex + 1) % waypoints.Count;
            _isCheck = false;
        }

        protected void SmokeParticleController(bool isActive){
            foreach (var x in particles){
                x.gameObject.SetActive(isActive);
            }
        }

        private void OnCollisionStay(Collision other)
        {
            if (other.gameObject.layer == 3)
            {
                _onGround = true;
                //Debug.Log("car is on the ground");
                SmokeParticleController(true);
            }
        }

        private void OnCollisionExit(Collision other)
        {
            if (other.gameObject.layer == 3)
            {
                _onGround = false;
                //Debug.Log("car is not on the ground");
                SmokeParticleController(false);
            }
        }

        private void OnTriggerEnter(Collider other){
            if (other.CompareTag("Checkpoint") && !_isCheck){
                SetNextWaypoint();
                _isCheck = true;
            }
        }

        private void OnTriggerExit(Collider other){
            if (other.CompareTag("Checkpoint") && _isCheck){
                _isCheck = false;
            }
        }

        protected virtual void OnDrawGizmos()
        {
            // Gizmos.color = Color.red;
            // Gizmos.DrawSphere(transform.position + transform.rotation * centerOfMass, 1f);
        }
    }
}
