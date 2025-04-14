using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Oculus.Interaction.Input;

public class HandDetect : MonoBehaviour
{
    public OVRHand lefthand;
    public OVRHand righthand;
    public Camera cam;
    public float threshold = 0.8f;
    public float cooltime = 1.0f;
    public GameObject shield;
    private float current = 0.0f;
    // Start is called before the first frame update
    void Start()
    {   

    }

    // Update is called once per frame
    void Update()
    {
        current += Time.deltaTime;
        if (IsPalmForward(lefthand)) {
            if(current > cooltime) {
                if(!shield.activeSelf)
                {
                    shield.SetActive(true);
                }
            }
        }else if (current > cooltime)
        {
            if(shield.activeSelf)
            {
                shield.SetActive(false);
            }
            current = 0;
        }

        shield.transform.position = lefthand.transform.position + lefthand.transform.up * 0.2f;
        Vector3 currentAngle = lefthand.transform.rotation.eulerAngles;
        currentAngle.z += 180;
        currentAngle.x = 0;
        shield.transform.rotation = Quaternion.Euler(currentAngle);
    }

    bool IsHandOpen(OVRHand hand)
    {
        if (hand == null || !hand.IsTracked) { return false; }

        bool a = false;

        if (!hand.GetFingerIsPinching(OVRHand.HandFinger.Thumb) &&
            !hand.GetFingerIsPinching(OVRHand.HandFinger.Index) &&
            !hand.GetFingerIsPinching(OVRHand.HandFinger.Middle) &&
            !hand.GetFingerIsPinching(OVRHand.HandFinger.Ring) &&
            !hand.GetFingerIsPinching(OVRHand.HandFinger.Pinky)) 
        { a = true; }

        return a;
    }

    bool IsPalmForward(OVRHand hand)
    {
        if (hand == null || !hand.IsTracked) { return false; }
        Vector3 hand_direction = hand.transform.up;
        Vector3 camera_direction = cam.transform.forward;

        float dotProduct = Vector3.Dot(hand_direction, camera_direction);

        return dotProduct > threshold;
    }

    // void shieldActive(OVRHand hand)
    // {
    //     shield = Instantiate(shieldPrefab);
    //     shield.transform.position = hand.transform.position + hand.transform.up * 0.1f;
    //     //shield.transform.rotation = hand.transform.rotation;
    //     Vector3 currentAngle = hand.transform.rotation.eulerAngles;
    //     currentAngle.z += 180;
    //     currentAngle.x = 0;
    //     shield.transform.rotation = Quaternion.Euler(currentAngle);


    // }
}   
