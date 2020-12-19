using SpeechLib;
using System.Collections;
using UnityEngine;
using UnityEngine.Windows.Speech;

public class Speech : MonoBehaviour {

    public delegate void OnSpeekStateChanged(bool isTalking);
    public static OnSpeekStateChanged speakStateChanged;

    public delegate void OnKeywordRecognized();
    public static OnKeywordRecognized keywordRecognized;

    public delegate void OnSpeechRecognized(string speech);
    public static OnSpeechRecognized speechRecognized;

    SpVoice voice = new SpVoice();
    KeywordRecognizer keywordRecognizer;
    DictationRecognizer m_DictationRecognizer;
    bool isSpeaking;

    void Awake() {
        //setup keyword reecognizer
        keywordRecognizer = new KeywordRecognizer(new string[] { Config.KEYWORD }, ConfidenceLevel.Low);
        keywordRecognizer.OnPhraseRecognized += KeywordRecognizer_OnPhraseRecognized;

        //setup dictation recognizer
        m_DictationRecognizer = new DictationRecognizer();
        m_DictationRecognizer.AutoSilenceTimeoutSeconds = 2;
        m_DictationRecognizer.DictationResult += (text, confidence) => {
            StartCoroutine(OnSpeechResult(text));
        };
        m_DictationRecognizer.DictationComplete += (completionCause) => {
            if (completionCause != DictationCompletionCause.Complete)
                StartCoroutine(OnSpeechResult("error"));
        };
        m_DictationRecognizer.DictationError += (error, hresult) => {
            StartCoroutine(OnSpeechResult("error"));
        };
    }

    IEnumerator OnSpeechResult(string speech) {
        //if valid return result
        if (speech != "error" && speech.Length > 0) {
            speechRecognized?.Invoke(speech);
            yield return new WaitForEndOfFrame();
            m_DictationRecognizer.Stop();
        } else {
            print(speech);
        }
        //wait for dictation to stop and listen for keyword again
        while (m_DictationRecognizer.Status == SpeechSystemStatus.Running) {
            yield return new WaitForEndOfFrame();
        }
        ListenForKeyword();
    }

    public void StartSpeechRoutine() {
        keywordRecognizer.Start();
        print("Speech Started...");
    }

    public void SpeakWords(string words) {
        voice.Speak(words, SpeechVoiceSpeakFlags.SVSFlagsAsync);
        isSpeaking = true;
        speakStateChanged?.Invoke(true);
    }

    public void ListenForSpeech() {
        m_DictationRecognizer.Start();
        print("Listening...");
    }

    void ListenForKeyword() {
        PhraseRecognitionSystem.Restart();
        print("Waiting...");
    }

    // this gets called late so we cant start listening here
    void OnDoneSpeaking() {
        isSpeaking = false;
        speakStateChanged?.Invoke(false);
    }

    void KeywordRecognizer_OnPhraseRecognized(PhraseRecognizedEventArgs args) {
        PhraseRecognitionSystem.Shutdown();
        ListenForSpeech();
        keywordRecognized?.Invoke();
    }

    void Update() {
        if (isSpeaking && voice.Status.RunningState == SpeechRunState.SRSEDone) {
            OnDoneSpeaking();
        }
    }
}
