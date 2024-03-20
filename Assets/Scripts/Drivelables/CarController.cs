using UnityEngine;

namespace Drivelables{
    public class CarController : DrivelableObjects
    {
        protected override void Start(){
            centerOfMass = new Vector3(0, -1f, 0);
            base.Start();
        }
    }
}
