#if UNITY_EDITOR
using UnityEditor;
#endif
using Dummiesman;
using UnityEngine;

namespace ObjImporter
{
    
    public static class DataHolder
    {
        public static GameObject objectToPass;
    }

    
    public class ObjImporter : MonoBehaviour
    {
        private OBJLoader _objLoader = new OBJLoader();
        public string path;
        public Material mat;
        public MeshFilter meshFilter;

        [SerializeField] private GameObject loadedGO;
        [SerializeField] private GameObject prefabGO;
        public string prefabName = "ImportedPrefab";
        // Speed of rotation in degrees per second
        public float rotationSpeed = 90f;

        // Start is called before the first frame update
        void Start()
        {
        }

        public void ObjFileImporter()
        {
            loadedGO = _objLoader.Load(path);

            if (loadedGO == null)
            {
                Debug.LogError("Failed to load the OBJ file.");
                return;
            }

            // Assign the material to all MeshRenderer components in the loaded GameObject and its children
            AssignMaterialToMeshRenderers(loadedGO, mat);

            // Ensure MeshFilter component is present and assign it
            meshFilter = loadedGO.GetComponentInChildren<MeshFilter>();
            if (meshFilter == null)
            {
                Debug.LogError("No MeshFilter found in the loaded GameObject.");
                return;
            }
            else
            {
                Debug.Log($"MeshFilter found: {meshFilter.gameObject.name}");
                LogMeshDetails(meshFilter.mesh);
            }

            // Create prefab from the loaded GameObject
            //CreatePrefabFromLoadedGO();
            DataHolder.objectToPass = loadedGO;
        }

        private void AssignMaterialToMeshRenderers(GameObject parent, Material material)
        {
            MeshRenderer[] meshRenderers = parent.GetComponentsInChildren<MeshRenderer>();
            foreach (MeshRenderer renderer in meshRenderers)
            {
                renderer.material = material;
            }
        }

        private void LogMeshDetails(Mesh mesh)
        {
            if (mesh == null)
            {
                Debug.LogError("Mesh is null.");
                return;
            }

            Debug.Log($"Mesh name: {mesh.name}");
            Debug.Log($"Vertices: {mesh.vertexCount}");
            Debug.Log($"Submeshes: {mesh.subMeshCount}");
        }

        private void CreatePrefabFromLoadedGO()
        {
#if UNITY_EDITOR
            if (loadedGO == null)
            {
                Debug.LogError("No GameObject loaded to create a prefab from.");
                return;
            }

            // Create the directory if it doesn't exist
            string localPath = "Assets/Prefabs";
            if (!AssetDatabase.IsValidFolder(localPath))
            {
                AssetDatabase.CreateFolder("Assets", "Prefabs");
            }

            // Create a unique path for the prefab
            localPath = AssetDatabase.GenerateUniqueAssetPath($"{localPath}/{prefabName}.prefab");

            // Save the prefab
            prefabGO = PrefabUtility.SaveAsPrefabAssetAndConnect(loadedGO.transform.GetChild(0).gameObject, localPath, InteractionMode.UserAction);
            prefabGO.GetComponent<MeshFilter>().mesh = meshFilter.mesh;
            // Validate prefab creation
            if (prefabGO == null)
            {
                Debug.LogError("Failed to create the prefab.");
            }
            else
            {
                Debug.Log($"Prefab created at: {localPath}");
            }
#else
            Debug.LogError("Prefab creation is only supported in the Unity Editor.");
#endif
        }

        public void UpdateMesh(){
            var vertices = loadedGO.GetComponentInChildren<MeshFilter>().mesh.vertices;
            var triangles = loadedGO.GetComponentInChildren<MeshFilter>().mesh.triangles;
            prefabGO.GetComponent<MeshFilter>().sharedMesh.vertices = vertices;
            prefabGO.GetComponent<MeshFilter>().sharedMesh.triangles = triangles;

        }

        // Update is called once per frame
        void Update()
        {
            // Calculate the rotation amount for this frame
            float rotationAmount = rotationSpeed * Time.deltaTime;

            // Apply the rotation around the X-axis
            loadedGO.transform.Rotate(0, rotationAmount, 0);
        }
    }
}
