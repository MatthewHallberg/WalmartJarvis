using UnityEngine;
using TMPro;

public class HudController : MonoBehaviour {

    public TextMeshProUGUI fpsText;
    public GameObject spectrumAnalyzer;

    void Start() {
        ActivateSpectrumAnalyzer(false);
    }

    void OnEnable() {
        Speech.speakStateChanged += ActivateSpectrumAnalyzer;
    }

    void OnDisable() {
        Speech.speakStateChanged -= ActivateSpectrumAnalyzer;
    }

    void Update() {
        fpsText.text = (1.0f / Time.deltaTime).ToString("0.0");
    }

    public void ActivateSpectrumAnalyzer(bool activate) {
        spectrumAnalyzer.SetActive(activate);
    }
}
