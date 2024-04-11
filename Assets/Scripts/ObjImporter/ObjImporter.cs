using Dummiesman;
using UnityEngine;

namespace ObjImporter{
    public class ObjImporter : MonoBehaviour{
        private OBJLoader _objLoader = new OBJLoader();

        [SerializeField] private GameObject loadedGO;
        // Start is called before the first frame update
        void Start(){
            loadedGO = _objLoader.Load("C:\\Users\\3DDL\\Desktop\\a\\indoor plant_02_obj\\indoor plant_02.obj");
        }

        // Update is called once per frame
        void Update()
        {
        
        }
    }
}
