using SimpleFileBrowser;
using UnityEngine;
using System.Collections;
using UnityEngine.Serialization;

// Make sure to include this namespace

namespace ObjImporter{
    public class FileDialog : MonoBehaviour{
        [FormerlySerializedAs("_objImporter")] [SerializeField] private ObjImporter objImporter;
        
        void Start()
        {
            
        }

        public void FileDialogOpener(){
            // Set filters (optional)
            FileBrowser.SetFilters(true, new FileBrowser.Filter("Objects", ".obj"), new FileBrowser.Filter("Text Files", ".txt", ".pdf"));

            // Set default filter that includes all files
            FileBrowser.SetDefaultFilter(".jpg");

            // Set optional settings
            FileBrowser.SetExcludedExtensions(".lnk", ".tmp", ".zip");
            FileBrowser.AddQuickLink("Users", "C:\\Users", null);

            // Coroutine to open file browser
            StartCoroutine(ShowLoadDialogCoroutine());
        }

        IEnumerator ShowLoadDialogCoroutine()
        {
            // Show a load file dialog and wait for a response from user
            // Load files/folders: both, File, Folder
            yield return FileBrowser.WaitForLoadDialog(FileBrowser.PickMode.Files, false, null, "Load File", "Load");

            // Dialog is closed
            // Print whether the user has selected some files
            if (FileBrowser.Success)
            {
                // FileBrowser.Result returns selected file(s) as string array
                objImporter.path = FileBrowser.Result[0];
                Debug.Log("Selected file: " + FileBrowser.Result[0]);
                // You can do something with the selected file here
            }
        }
    }
}
