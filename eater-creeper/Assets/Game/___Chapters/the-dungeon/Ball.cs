using UnityEngine;

public class Ball : MonoBehaviour {
    public void Init(Vector3 velocity) {
        GetComponent<Rigidbody>().AddForce(velocity, ForceMode.Impulse);
    }
}
