using System.Collections;
using System.Collections.Generic;
using ObjImporter;
using UnityEngine;

public class SceneInit : MonoBehaviour{
    public GameObject car;
    public Transform player;
    
    // Start is called before the first frame update
    void Start(){
        car = DataHolder.objectToPass;
        GameObject go = Instantiate(car, player);
        go.transform.localRotation = Quaternion.Euler(0, 140f, 0);
        go.transform.localScale = new Vector3(3, 3, 3);
        go.transform.localPosition = new Vector3(0.08f, -0.28f, 0.2f);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
