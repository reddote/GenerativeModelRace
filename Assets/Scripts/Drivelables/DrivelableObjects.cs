using UnityEngine;

namespace Drivelables{
    public class DrivelableObjects : MonoBehaviour
    {
        public float accelerationForce = 10f;
        public float maxSpeed = 50f;
        public float turnSpeed = 150f;
        public float brakeForce = 50f;
        
        protected Vector3 centerOfMass = new Vector3(0,0f,0);//this is object center of mass so it wont easily turn upside down.
    
        protected Rigidbody rb;
    
        protected virtual void Start(){
            rb = GetComponent<Rigidbody>();
            SetupObjectCenterOfMass();
        }
    
        protected virtual void SetupObjectCenterOfMass(){
            rb.centerOfMass = centerOfMass;
            rb.WakeUp();
        }
    
        protected virtual void Update(){
            
        }
    
        protected virtual void FixedUpdate(){
            MovementDrivelableObject();
        }
    
        protected virtual void MovementDrivelableObject(){
            // Get input for acceleration and turning
            float _accelerationInput = Input.GetAxis("Vertical");
            float _turnInput = Input.GetAxis("Horizontal");
            bool _brakeInput = Input.GetKey(KeyCode.Space);
    
            // Calculate acceleration and turning forces
            Vector3 _acceleration = transform.forward * (accelerationForce * _accelerationInput);
            float _turnForce = turnSpeed * _turnInput;
    
            // Apply acceleration and limit max speed
            rb.AddForce(_acceleration, ForceMode.Acceleration);
    
            if (rb.velocity.magnitude > maxSpeed){
                rb.velocity = rb.velocity.normalized * maxSpeed;
            }
    
            // Apply turning force
            if (_accelerationInput != 0){
                Quaternion _turnRotation = Quaternion.Euler(0f, _turnForce * Time.fixedDeltaTime, 0f);
                rb.MoveRotation(rb.rotation * _turnRotation);
            }
     
            // Apply brake force
            if (_brakeInput)
            {
                rb.AddForce(-rb.velocity.normalized * brakeForce, ForceMode.Acceleration);
            }
        }
        protected virtual void OnDrawGizmos(){
            //Gizmos.color = Color.red;
            //Gizmos.DrawSphere(transform.position + transform.rotation * centerOfMass, 1f);
        }
    }
}
