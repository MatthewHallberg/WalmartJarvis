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

    public void ForceRelease() {
        m_grabbedObj = null;
    }

    void DetectPinch() {
        float currPinch = hand.GetFingerPinchStrength(OVRHand.HandFinger.Index);
        if (!m_grabbedObj && currPinch > Config.PINCH_THRESHOLD) {
            GrabBegin();
        } else if (m_grabbedObj && currPinch < Config.PINCH_THRESHOLD) {
            GrabEnd();
        }
    }
}
