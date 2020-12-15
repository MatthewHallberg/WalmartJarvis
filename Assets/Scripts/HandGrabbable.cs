using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class HandGrabbable : OVRGrabbable {

    const float ROTATE_SPEED = 6f;

    Quaternion desiredRotation;
    bool canRotate;
    float lastScaleDelta = -1;
    List<OVRHand> hands;
    int currHandCount;

    protected override void Start() {
        base.Start();
        desiredRotation = transform.rotation;
        hands = FindObjectsOfType<OVRHand>().ToList();
    }

    void OnTriggerEnter(Collider other) {
        OVRHand hand = other.GetComponent<OVRHand>();
        if (hand != null) {
            currHandCount++;
        }
        lastScaleDelta = -1;
    }

    void OnTriggerExit(Collider other) {
        OVRHand hand = other.GetComponent<OVRHand>();
        if (hand != null) {
            currHandCount--;
        }
    }

    public override void GrabBegin(OVRGrabber hand, Collider grabPoint) {
        base.GrabBegin(hand, grabPoint);
        canRotate = true;
    }

    public override void GrabEnd(Vector3 linearVelocity, Vector3 angularVelocity) {
        base.GrabEnd(linearVelocity, angularVelocity);
        canRotate = false;
        SetDesiredRotation();
    }

    void Update() {
        if (!canRotate) {
            transform.rotation = Quaternion.Lerp(transform.rotation, desiredRotation, Time.deltaTime * ROTATE_SPEED);
        }

        if (currHandCount == 2) {
            float firstPinch = hands[0].GetFingerPinchStrength(OVRHand.HandFinger.Index);
            float secondPinch = hands[1].GetFingerPinchStrength(OVRHand.HandFinger.Index);

            if (firstPinch > Settings.PINCH_THRESHOLD && secondPinch > Settings.PINCH_THRESHOLD) {
                float currScaleDelta = Vector3.Distance(hands[0].transform.position, hands[1].transform.position);
                if (lastScaleDelta != -1) {
                    float currScaleAmount = currScaleDelta - lastScaleDelta;
                    transform.localScale += Vector3.one * currScaleAmount;
                }
                lastScaleDelta = currScaleDelta;
            }
        }
    }

    void SetDesiredRotation() {
        //only allow y axis rotation
        Vector3 currAngle = transform.eulerAngles;
        currAngle.x = 0;
        currAngle.z = 0;
        desiredRotation = Quaternion.Euler(currAngle);
    }
}
