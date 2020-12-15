public class HandGrabber : OVRGrabber {

    OVRHand hand;

    protected override void Start() {
        base.Start();
        hand = GetComponent<OVRHand>();
    }

     public override void Update() {
        base.Update();
        DetectPinch();
    }

    void DetectPinch() {
        float currPinch = hand.GetFingerPinchStrength(OVRHand.HandFinger.Index);
        if (!m_grabbedObj && currPinch > Settings.PINCH_THRESHOLD) {
            GrabBegin();
        } else if (m_grabbedObj && currPinch < Settings.PINCH_THRESHOLD) {
            GrabEnd();
        }
    }
}
