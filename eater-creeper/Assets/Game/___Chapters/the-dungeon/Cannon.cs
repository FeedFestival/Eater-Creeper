using UnityEngine;
using static UnityEngine.GraphicsBuffer;

public class Cannon : MonoBehaviour {
    [SerializeField]
    private Ball _ball;
    [SerializeField]
    private float _force;

    [SerializeField]
    private Projection _projection;

    private bool _simulate;

    [SerializeField]
    private Transform target;
    [SerializeField]
    private Transform cannonPivot;
    [SerializeField]
    private Transform firePoint;

    private float _lastAngle;

    // Update is called once per frame
    void Update() {

        _projection.SimulateTrajectory(_ball, cannonPivot.position, cannonPivot.forward * _force);

        Vector3 directionToTarget = target.position - transform.position;
        Quaternion lookRotation = Quaternion.LookRotation(directionToTarget);
        transform.rotation = Quaternion.Euler(lookRotation.eulerAngles.x, lookRotation.eulerAngles.y, 0);

        //

        directionToTarget = target.position - firePoint.position;

        // Calculate the required launch angle
        float? angle = CalculateLaunchAngle(directionToTarget, _force);

        if (angle.HasValue) // If an angle is found
        {
            // Rotate the cannon towards the target with the correct elevation angle
            RotateCannon(directionToTarget, angle.Value);
        }

        // Fire the projectile when the player clicks
        if (Input.GetMouseButtonDown(0)) {
            FireProjectile();
        }
    }

    float? CalculateLaunchAngle(Vector3 direction, float speed) {
        float gravity = Mathf.Abs(Physics.gravity.y); // Use the global gravity
        float horizontalDistance = new Vector2(direction.x, direction.z).magnitude; // Horizontal distance
        float verticalDifference = direction.y; // Vertical height difference

        // Ensure the target is within range
        float speedSquared = speed * speed;
        float discriminant = (speedSquared * speedSquared) - gravity * (gravity * horizontalDistance * horizontalDistance + 2 * verticalDifference * speedSquared);

        if (discriminant >= 0) // Check if the target is reachable
        {
            // Calculate the two possible angles (low arc and high arc)
            float sqrtDiscriminant = Mathf.Sqrt(discriminant);
            float lowArc = Mathf.Atan((speedSquared - sqrtDiscriminant) / (gravity * horizontalDistance));
            float highArc = Mathf.Atan((speedSquared + sqrtDiscriminant) / (gravity * horizontalDistance));

            // Return the low arc for practical purposes
            return lowArc; // In radians
        }

        // Target is out of reach
        return null;
    }

    void RotateCannon(Vector3 direction, float angle) {
        // Get the horizontal rotation (look at the target along XZ plane)
        Vector3 horizontalDirection = new Vector3(direction.x, 0, direction.z);
        Quaternion horizontalRotation = Quaternion.LookRotation(horizontalDirection);

        // Apply the pitch angle (elevation)
        var newRotation = Quaternion.Euler(-Mathf.Rad2Deg * angle, horizontalRotation.eulerAngles.y, 0);
        Debug.Log("newRotation: " + newRotation);
        cannonPivot.rotation = newRotation;
    }

    void FireProjectile() {
        _ball.transform.position = firePoint.position;
        _ball.GetComponent<Rigidbody>().linearVelocity = Vector3.zero;
        _ball.GetComponent<Rigidbody>().linearVelocity = Vector3.zero;

        _ball.Init(cannonPivot.forward * _force);
    }
}
