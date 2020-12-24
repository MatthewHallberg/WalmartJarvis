using System.Collections;
using UnityEngine;
using UnityEngine.Windows.Speech;
using SpeechLib;

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

        //to add additional voices you must make them available for 32 bit apps
        //https://winaero.com/unlock-extra-voices-windows-10/
        foreach (ISpeechObjectToken name in voice.GetVoices()) {
            print("Voice Available: " + name.GetDescription());
        }
        voice.Voice = voice.GetVoices().Item(2);

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

    void OnApplicationFocus(bool focus) {
        //restart speech since it stops in the background
        OnSpeechResult("error");
    }

    IEnumerator OnSpeechResult(string speech) {
        //if valid return result
        if (speech != "error" && speech.Length > 0) {
            speechRecognized?.Invoke(speech);
            yield return new WaitForEndOfFrame();
        } else {
            print(speech);
        }

        m_DictationRecognizer.Stop();
        //wait for dictation to stop and listen for keyword again
        while (m_DictationRecognizer.Status == SpeechSystemStatus.Running) {
            yield return new WaitForEndOfFrame();
        }
        ListenForKeyword();
        StopAllCoroutines();
    }

    public void StartSpeechRoutine() {
        keywordRecognizer.Start();
        StartCoroutine(ForceSpeechStopRoutine());
        print("Speech Started...");
    }

    IEnumerator ForceSpeechStopRoutine() {
        yield return new WaitForSeconds(5f);
        OnSpeechResult("error");
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
