using UnityEngine;
using UnityEngine.Serialization;

namespace Drivelables{
    public class DrivelableObjects : MonoBehaviour
    {
        public float accelerationForce = 10f;
        public float maxSpeed = 50f;
        public float minSpeed = 2f;
        public float turnSpeed = 150f;
        public float brakeForce = 50f;
        
        protected Vector3 centerOfMass = new Vector3(0,0f,0);//this is object center of mass so it wont easily turn upside down.
        protected Rigidbody rb;
        private bool _onGround;

        [SerializeField]protected GameObject[] particles;
     
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
            if (_onGround){
                MovementDrivelableObject();
            }
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
        
        protected bool IsCarMovingBackward(){
            // Check if the velocity is in the opposite direction to the car's forward direction
            return Vector3.Dot(rb.velocity, transform.forward) < 0;
        }

        protected virtual void ParticleRotation(){
            if (IsCarMovingBackward()){
                foreach (var x in particles){
                    x.transform.rotation = Quaternion.Euler(0, 0, 0);
                }
            }
            else{
                foreach (var x in particles){
                    x.transform.rotation = Quaternion.Euler(-180, 0, 0);
                }
            }
        }

        protected virtual void SmokeParticleController(bool isActive){
            foreach (var x in particles){
                x.gameObject.SetActive(isActive);
            }
        }
        
        private void OnCollisionStay(Collision other){
            if (other.gameObject.layer == 3){
                _onGround = true;
                Debug.Log("car is not flying");
                ParticleRotation();
                SmokeParticleController(true);
                if (rb.velocity.magnitude < minSpeed){
                    SmokeParticleController(false);
                }
            }
        }
        
        private void OnCollisionExit(Collision other)
        {
            if (other.gameObject.layer == 3)
            {
                _onGround = false;
                Debug.Log("car is not on the ground");
                SmokeParticleController(false);
            }
        }
        protected virtual void OnDrawGizmos(){
            //Gizmos.color = Color.red;
            //Gizmos.DrawSphere(transform.position + transform.rotation * centerOfMass, 1f);
        }
    }
}
