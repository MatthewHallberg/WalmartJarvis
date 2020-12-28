using UnityEngine;
    
public class Trash : MonoBehaviour {

    void OnTriggerEnter(Collider other) {
        iSceneObject sceneObject = other.GetComponent<iSceneObject>();
        if (sceneObject != null) {
            sceneObject.Dispose();
            other.GetComponent<Collider>().enabled = false;
        }
    }
}
