using NUnit.Framework;
using System.Collections.Generic;
using UnityEngine;

public class Launcher : MonoBehaviour {
    public Rigidbody ball;
    public Transform target;

    [SerializeField]
    private LineRenderer _line;

    public float h = 25;

    private float gravity;

    private bool launchPrepared = false;

    private List<Vector3> positionList;

    private void Start() {

        gravity = Physics.gravity.y;

        ball.useGravity = false;
        ball.transform.position = transform.position;
    }

    void Update() {
        if (Input.GetKeyDown(KeyCode.Space)) {
            prepareLaunch();
            calculatePathPositions();

            _line.positionCount = positionList.Count;

            var i = 0;
            foreach (var pos in positionList) {
                _line.SetPosition(i, pos);
                i++;
            }
        }

        if (!launchPrepared) return;

        if (Input.GetKeyDown(KeyCode.Mouse0)) {
            launch();
        }
    }

    void prepareLaunch() {
        ball.useGravity = false;
        ball.ResetInertiaTensor();
        ball.linearVelocity = Vector3.zero;
        ball.angularVelocity = Vector3.zero;
        ball.transform.rotation = Quaternion.Euler(Vector3.zero);
        ball.transform.position = transform.position;

        launchPrepared = true;
    }

    void launch() {
        ball.useGravity = true;
        ball.angularVelocity = new Vector3(0, 0, 4.2f);

        var launchData = calculateLaunchVelocity();
        Debug.Log("launchData: " + launchData.initialVelocity);
        ball.linearVelocity = launchData.initialVelocity;

        launchPrepared = false;
    }

    LaunchData calculateLaunchVelocity() {

        float displacementY = target.position.y - ball.position.y;
        Vector3 displacementXZ = new Vector3(target.position.x - ball.position.x, 0, target.position.z - ball.position.z);

        var time = Mathf.Sqrt(-2 * h / gravity) + Mathf.Sqrt(2 * (displacementY - h) / gravity);

        var velocityY = Vector3.up * Mathf.Sqrt(-2 * gravity * h);
        var velocityXZ = displacementXZ / time;

        return new LaunchData(velocityXZ + velocityY, time);
    }

    void calculatePathPositions() {
        var launchData = calculateLaunchVelocity();

        Debug.Log("launchData.timeToTarget: " + launchData.timeToTarget);

        positionList = new List<Vector3>() { ball.position };

        int resolution = 64;

        for (int i = 1; i <= resolution; i++) {
            float simulationTime = i / (float)resolution * launchData.timeToTarget;
            Vector3 displacement = launchData.initialVelocity * simulationTime + (Vector3.up * gravity) * (simulationTime * simulationTime) / 2;
            Vector3 drawPoint = ball.position + displacement;

            positionList.Add(drawPoint);
        }
    }

    struct LaunchData {
        public readonly Vector3 initialVelocity;
        public readonly float timeToTarget;

        public LaunchData(Vector3 initialVelocity, float timeToTarget) {
            this.initialVelocity = initialVelocity;
            this.timeToTarget = timeToTarget;
        }
    }
}
