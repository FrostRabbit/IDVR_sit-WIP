using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Oculus.Interaction.Input;

public class WIP : MonoBehaviour
{
    
    [Header("General")]
    public GameObject CameraRig;
    public OVRHand lefthand;
    public float TimeInterval = 1.0f;
    public float refreshTime = 0.001f;
    public float sampleTime = 0.05f;
    public float timeWindow = 1.0f;
    private Vector3 currentPos;
    private float refreshTimeElapsed = 0.0f;
    private bool refresh = true;

    [Header("Running")]
    public float runUpperThreshold = 0.05f;
    public float runLowerThreshold = 0.02f;
    public float runSpeedFactor = 1.0f;
    public float moveSpeed = 0.1f;
    public float baseRunSpeed = 2.0f;
    private float runtimeElapsed = 0.0f;
    public bool isMove = false;
    private List<float> movementDataY = new List<float>();

    [Header("Flying")]
    public float jumpThreshold = 0.12f;
    public float handOpenThreshold = 0.1f;
    public float waveThreshold = 0.1f;
    public float flySpeedFactor = 1.5f;
    public float flySpeed = 0.1f;
    private float flytimeElapsed = 0.0f;
    public float jumpForce = 2.0f;
    public float waveForce = 1.0f;
    public float currentJumpforce = 0.0f;
    public bool isJump = false;
    private int waveCount = 0;
    private List<float> handY = new List<float>();
    private List<int> controllerY = new List<int>();
    private bool isopen = true;
    private bool ispositive = false;
    public float lefthand_y = 0.0f;
    public float righthand_y = 0.0f;



    // Start is called before the first frame update
    void Start()
    {
        currentPos = CameraRig.transform.position;

    }

    // Update is called once per frame
    void Update()
    {   
        //Debug.Log(OVRInput.GetLocalControllerVelocity(OVRInput.Controller.RTouch).y);
        //Debug.Log(OVRInput.GetLocalControllerPosition(OVRInput.Controller.LTouch));
        if (refresh){
            refreshTimeElapsed += Time.deltaTime;
        }

        if (refreshTimeElapsed > refreshTime){
            refreshTimeElapsed = 0.0f;
            refresh = false;
            currentPos = CameraRig.transform.position;
        }else if (refresh){
            return;
        }

        if(OVRInput.GetDown(OVRInput.Button.One)){
            currentPos = CameraRig.transform.position;
        }

        fly();
        if(!isJump){
            run();
        }
    }
    void OnCollisionEnter(Collision collision) {
        if (collision.gameObject.CompareTag("Ground")) {
            isJump = false;
            Debug.Log("Landed on the ground");
        }
    }

    void fly(){
        flytimeElapsed += Time.deltaTime;
        if (flytimeElapsed <= sampleTime){
            return;
        }

        flytimeElapsed = 0.0f;
        Vector3 newPos = CameraRig.transform.position;
        //Debug.Log(newPos.y);
        //Debug.Log(currentPos.y);

        float diffY = newPos.y - currentPos.y;

        if (isJump){
            currentJumpforce = 0.0f;
        }

        if (diffY > jumpThreshold && !isJump){ 
            diffY = 0.0f;
            isJump = true;
            currentJumpforce = jumpForce;
            Debug.Log("Jumping");
        }

        if(!isJump){
            flySpeed = 0.0f;
            waveCount = 0;
            handY.Clear();
            controllerY.Clear();
            return;
        }

        if (!IsOpen()){
            flySpeed = 0.0f;
            waveCount = 0;
            isopen = true;
            return;
        }

        if (isopen){
            Debug.Log("Open");
            ispositive = false;
            isopen = false;
            righthand_y = OVRInput.GetLocalControllerPosition(OVRInput.Controller.RTouch).y;
            lefthand_y = OVRInput.GetLocalControllerPosition(OVRInput.Controller.LTouch).y;
        }

        handWaveDetection();
        flySpeedCalculate();
    }
    

    void handWaveDetection(){
        //float diffController = OVRInput.GetLocalControllerPosition(OVRInput.Controller.RTouch).y - righthand_y;
        //float diffHand = OVRInput.GetLocalControllerPosition(OVRInput.Controller.LTouch).y - lefthand_y;
        float diffController = OVRInput.GetLocalControllerVelocity(OVRInput.Controller.RTouch).y;
        float diffHand = OVRInput.GetLocalControllerVelocity(OVRInput.Controller.LTouch).y;
        int iswave = 0;
        if (diffController > waveThreshold && diffHand > waveThreshold && !ispositive){
            //Debug.Log("up");
            currentJumpforce = waveForce;
            ispositive = true;
            waveCount++;
            iswave = 1;
            //Debug.Log("Wave: "+waveCount);
        }

        if (diffController < -waveThreshold && diffHand < -waveThreshold && ispositive){
            //Debug.Log("down");
            currentJumpforce = waveForce;
            ispositive = false;
            waveCount++;
            iswave = 1;
            //Debug.Log("Wave: "+waveCount);
        }

        controllerY.Add(iswave);
        if (controllerY.Count > timeWindow / sampleTime){
            controllerY.RemoveAt(0);
        }
    }

    void flySpeedCalculate(){

        if (waveCount <= 2){
            //Debug.Log("not enough");
            //flySpeed = 0.0f;
            return;
        }

        List<float> intervals = new List<float>();
        float average = 0.0f;  

        for (int i = 0; i < controllerY.Count - 1; i++){
            if (controllerY[i] == 1){
                //Debug.Log("Hand Wave"+i * sampleTime);
                intervals.Add(i * sampleTime);
            }
        }

        if (intervals.Count <= 2){
            //flySpeed = 0.0f;
            return;
        }

        for (int i = 0; i < intervals.Count - 1; i++){
            average += intervals[i + 1] - intervals[i];
        }

        average /= intervals.Count - 1;
        Debug.Log("Average: "+average);

        float targetSpeed = 0.0f;

        if (average < TimeInterval && average > 0.0f){

            targetSpeed = flySpeedFactor * (1.0f / average);
            Debug.Log("Flying: "+flySpeed);

        }else{
            waveCount = 0;
        }

        flySpeed = Mathf.Lerp(flySpeed, targetSpeed, Time.deltaTime * 2.0f);
        
    }

    bool IsOpen(){
        float length;
        //Debug.Log("controller: "+OVRInput.GetLocalControllerPosition(OVRInput.Controller.RTouch));
        //Debug.Log("lefthand: "+lefthand.transform.position);
        length = Vector3.Distance(OVRInput.GetLocalControllerPosition(OVRInput.Controller.RTouch) , OVRInput.GetLocalControllerPosition(OVRInput.Controller.LTouch));
        //Debug.Log("Distance: "+length);
        if (length > handOpenThreshold){
            return true;
        }
        return false;
    }


    void run(){
        runtimeElapsed += Time.deltaTime;
        int ismovement = 0;
        if (runtimeElapsed > sampleTime){
            Vector3 newPos = CameraRig.transform.position;
            float diffY = newPos.y - currentPos.y;
            //Debug.Log(diffY);
            if (diffY > runUpperThreshold && !isMove){
                //Debug.Log("up");
                isMove = true;
                ismovement = 1;
            }else if(diffY < runLowerThreshold){
                //Debug.Log("down");
                isMove = false;
            }

            movementDataY.Add(ismovement);
            if (movementDataY.Count > timeWindow / sampleTime){
                movementDataY.RemoveAt(0);
            }

            runtimeElapsed = 0.0f;
        }

        moveSpeed = IsMovementX();
    }


    float IsMovementX(){
        List<float> intervals = new List<float>();
        for (int i = 0; i < movementDataY.Count - 1; i++){
            if (movementDataY[i] == 1){
                //Debug.Log("running");
                intervals.Add(i * sampleTime);
            }
        }

        if (intervals.Count > 2){
            float average = 0.0f;
            for (int i = 0; i < intervals.Count - 1; i++){
                average += Mathf.Abs(intervals[i + 1] - intervals[i]);
            }

            average /= intervals.Count - 1;
            if (average < TimeInterval && average > 0.0f){
                return baseRunSpeed * Mathf.Pow(average , -runSpeedFactor);
            }else{
                return 0.0f;
            }
        }
        return 0.0f;
    }
}
