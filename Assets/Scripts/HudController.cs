using UnityEngine;
using TMPro;

public class HudController : MonoBehaviour {

    public TextMeshProUGUI fpsText;

    void Update() {
        fpsText.text = (1.0f / Time.deltaTime).ToString("0.0");
    }
}
