using Dummiesman;
using UnityEngine;

namespace ObjImporter{
    public class ObjImporter : MonoBehaviour{
        private OBJLoader _objLoader = new OBJLoader();
        public string path;

        [SerializeField] private GameObject loadedGO;
        // Start is called before the first frame update
        void Start(){
        }

        public void ObjFileImporter(){
            loadedGO = _objLoader.Load(path);
        }

        // Update is called once per frame
        void Update()
        {
        
        }
    }
}
