using UnityEngine;

public class ScreenBehavior : MonoBehaviour {

    const float SCALE_SPEED = 6f;

    public Transform screenParent;

    Vector3 startScale;
    Vector3 desiredScale;
    Transform head;

    void Awake() {
        head = Camera.main.transform;
        startScale = screenParent.localScale;
        Reset();
        Open();
    }

    public virtual void Open() {
        desiredScale = startScale;
    }

    public virtual void Close() {
        desiredScale = Vector3.zero;
    }

    void DestroyScreen() {
        GetComponent<HandGrabbable>().ForceRelease();
        Destroy(gameObject);
    }

    public virtual void Reset() {
        screenParent.localScale = Vector3.zero;
        desiredScale = Vector3.zero;
    }

    void Update() {
        if (head.position.y - transform.position.y > Config.TRASH_DISTANCE) {
            DestroyScreen();
        }
        screenParent.localScale = Vector3.Lerp(screenParent.localScale, desiredScale, Time.deltaTime * SCALE_SPEED);
    }
}
