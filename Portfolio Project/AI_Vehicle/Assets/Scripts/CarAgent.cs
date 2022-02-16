using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.MLAgents;
using Unity.MLAgents.Sensors;
using Unity.MLAgents.Actuators;

public class CarAgent : Agent
{
    private Rigidbody rb;
    public List<AxleInfo> axleInfos; // the information about each individual axle
    public float maxMotorTorque; // maximum torque the motor can apply to wheel
    public float maxSteeringAngle; // maximum steer angle the wheel can have
    public float maxBrakingTorque; // max brake torque
    public Map_Generation map;
    public uint trialPerMapCount;

    private float motor; // Current motor torque
    private float steering; // Current steering angle

    private float braking; // Current braking torque
    private uint curTrialNum; // Number of current trial
    private uint collisionCount;
    private List<string> TAGS = new List<string>();

    private float distFull;
    private float rewardScalar;

    private bool reachedTarget; //True if target was reached

    [HideInInspector]
    public Transform goal;

    // Start is called before the first frame update
    void Awake()
    {
        print("Starting");
        rb = GetComponent<Rigidbody>(); // Get attached rigidbody
        curTrialNum = 1;
        this.map.MapGen();

        // Set Tags
        TAGS.Add("branch");
        TAGS.Add("bush");
        TAGS.Add("rock");
        TAGS.Add("stump");
        TAGS.Add("tree");

        if (MaxStep > 0) {
            this.rewardScalar = 1.0f / this.MaxStep;
        }

        this.reachedTarget = false;
    }

    private void RespawnAgent() {
        this.motor = 0;
        this.steering = 0;
        this.braking = 0;
        this.rb.velocity = Vector3.zero;
        this.rb.angularVelocity = Vector3.zero;
        
        this.transform.rotation = this.map.transform.Find("Start").transform.rotation;
        this.transform.Rotate(0, 180, 0);

        Vector3 temp = this.map.transform.Find("Start").transform.Find("Target").transform.position; // Get position of target
        temp.y = temp.y + 1;
        this.transform.position = temp;
    }

    public void FixedUpdate() {

        // Collisions and reset states
        if (this.transform.position.y < 0) {
            //AddReward(-1.0f);
            print("**OFF THE CLIFF WE GO** Score: " + GetCumulativeReward());
            EndEpisode();
        }

        // Check distance to target
        if (Vector3.Distance(this.transform.position, this.goal.position) < 1.0f) {
            AddReward(1.0f);
            print(StepCount);
            this.reachedTarget = true;
            print("**YA DONE REACHED THE GOAL DUDE** Score: " + GetCumulativeReward());
            EndEpisode();
        }

        // Check if car has tipped over
        if ((this.transform.rotation.eulerAngles.x >= 60 && this.transform.rotation.eulerAngles.x < 300) ||
        (this.transform.rotation.eulerAngles.z > 60 && this.transform.rotation.eulerAngles.z < 300)) {
            //AddReward(-1.0f);
            print("**DONT LEAN TOO MUCH** Score: " + GetCumulativeReward());
            EndEpisode();
        }

        this.ApplyWheelPhysics();
    }

    public override void OnEpisodeBegin()
    {
        if(!(this.reachedTarget)) {
            this.RespawnAgent();
        }
        else {
            this.reachedTarget = false;
            this.map.MapReGen();
            this.RespawnAgent();
        }
        
        print("EPISODE START");

        // TODO Map regen based on count
        // if (this.curTrialNum > this.trialPerMapCount) {
        //     rb.velocity = Vector3.zero;
        //     rb.angularVelocity = Vector3.zero;
        //     this.map.generating = true;
        //     rb.useGravity = false;
        //     this.curTrialNum = 1;
        //     this.map.MapReGen();

        //     while(this.map.generating == true) {
        //         this.transform.position = new Vector3(0, 1000, 0);
        //     }
        // }
        // else {
        //     this.curTrialNum += 1;
        // }

        // // Set variables
        // this.motor = 0;
        // this.steering = 0;
        // this.collisionCount = 0;

        // // Reset Velocities
        // rb.velocity = Vector3.zero;
        // rb.angularVelocity = Vector3.zero;

        // // Reset position
        // this.transform.rotation = this.map.transform.Find("Start").transform.rotation;
        // this.transform.Rotate(0, 180, 0);

        // temp = this.map.transform.Find("Start").transform.Find("Target").transform.position; // Get position of target
        // temp.y = temp.y + 1;
        // this.transform.position = temp;

        
        // rb.useGravity = true;

        // distFull = Mathf.Abs(Vector3.Distance(this.map.transform.Find("Start").transform.position, this.goal.position));
    }
    public override void CollectObservations(VectorSensor sensor)
    {
        Vector3 goalDirection = (this.goal.position - this.transform.position).normalized;
        sensor.AddObservation(this.transform.InverseTransformPoint(this.goal.position)); // Location of goal relative to agent position
        sensor.AddObservation(this.transform.InverseTransformVector(this.rb.velocity)); //  Velocity relative to agent
        sensor.AddObservation(this.transform.InverseTransformDirection(goalDirection)); // Direction to goal relative to agent

        // Add small reward if agent is moving towards the goal
        AddReward(this.rewardScalar * (Vector3.Dot(goalDirection, this.rb.velocity)));
    }

    public override void OnActionReceived(ActionBuffers actionBuffers)
    {
        this.motor = maxMotorTorque * Mathf.Clamp(actionBuffers.ContinuousActions[0], -1, 1);
        this.steering = maxSteeringAngle * Mathf.Clamp(actionBuffers.ContinuousActions[1], -1, 1);
        this.braking = maxBrakingTorque * Mathf.Clamp(actionBuffers.ContinuousActions[2], 0, 1);



        // New Ratio Penalty
        //float distCur = Mathf.Abs(Vector3.Distance(this.transform.position, this.goal.position));

        //this.AddReward(rewardScalar * (distCur/distFull));

        // Existential Penalty
        //AddReward(-0.00005f);

        // Reward for moving closer to goal
        // Updates incrementally
        //if ((this.StepCount % 10) == 0) {
        //    AddReward(rewardScalar * (1.0f/((Mathf.Abs(Vector3.Distance(this.transform.position, this.goal.position))))));
        //}
    }

    // Extending Heuristic() allows testing with manual inputs
    public override void Heuristic(in ActionBuffers actionsOut)
    {
        var continuousActionsOut = actionsOut.ContinuousActions;
        continuousActionsOut[0] = Input.GetAxis("Vertical");
        continuousActionsOut[1] = Input.GetAxis("Horizontal");
        continuousActionsOut[2] = Input.GetAxis("Jump");
    }

    public void ApplyLocalPositionToVisuals(WheelCollider collider)
    {
        if (collider.transform.childCount == 0) {
            return;
        }
     
        Transform visualWheel = collider.transform.GetChild(0);
     
        Vector3 position;
        Quaternion rotation;
        collider.GetWorldPose(out position, out rotation);
     
        visualWheel.transform.position = position;
        visualWheel.transform.rotation = rotation;
    }

    void OnCollisionEnter(Collision collision) {
        foreach (string item in TAGS) {
            if(collision.transform.CompareTag(item) == true) {

                print(("**AHHH I HIT A " + item + "** Score: " + this.GetCumulativeReward()));
                //AddReward(-1.0f);
                EndEpisode();
                //collisionCount += 1;

                //if (collisionCount >= 10) {
                //    AddReward(-5.0f);
                //    print("***STOP HITTING THINGS YA DOLT*** Score: " + GetCumulativeReward());
                //    EndEpisode();
                //}
            }
        }

    }

    IEnumerator waiter() {
        // Function just designed to wait for 1 second while map is regening;
        yield return new WaitForSeconds(1);
    }

    private void ApplyWheelPhysics() {

        foreach (AxleInfo axleInfo in axleInfos) {
            if (axleInfo.steering) {
                axleInfo.leftWheel.steerAngle = this.steering;
                axleInfo.rightWheel.steerAngle = this.steering;
            }
            if (axleInfo.motor) {
                axleInfo.leftWheel.motorTorque = this.motor;
                axleInfo.rightWheel.motorTorque = this.motor;
            }
            if (axleInfo.brake) {
                //axleInfo.leftWheel.brakeTorque = this.braking;
                //axleInfo.rightWheel.brakeTorque = this.braking;
                axleInfo.leftWheel.wheelDampingRate = this.braking;
                axleInfo.rightWheel.wheelDampingRate = this.braking;
            }
            ApplyLocalPositionToVisuals(axleInfo.leftWheel);
            ApplyLocalPositionToVisuals(axleInfo.rightWheel);
        }
    }
}

[System.Serializable]
public class AxleInfo {
    public WheelCollider leftWheel;
    public WheelCollider rightWheel;
    public bool motor; // is this wheel attached to motor?
    public bool steering; // does this wheel apply steer angle?
    public bool brake; // does this wheel apply braking
}
