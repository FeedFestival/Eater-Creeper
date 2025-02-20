using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Projection : MonoBehaviour {
    private Scene _simulationScene;
    private PhysicsScene _physicsScene;
    [SerializeField]
    private Transform _obstaclesParent;
    [SerializeField]
    private LineRenderer _line;
    [SerializeField]
    private int _maxPhysicsFrameIterations = 100;

    void Start() {
        createPhysicsScene();
    }

    public void SimulateTrajectory(Ball ballPrefab, Vector3 pos, Vector3 velocity) {

        var ghostObj = spawnGhostObject(ballPrefab.gameObject, pos, Quaternion.identity);
        var ghostBall = ghostObj.GetComponent<Ball>();

        ghostBall.Init(velocity);

        _line.positionCount = _maxPhysicsFrameIterations;

        for (int i = 0; i < _maxPhysicsFrameIterations; i++) {
            _physicsScene.Simulate(Time.fixedDeltaTime);
            _line.SetPosition(i, ghostObj.transform.position);
        }

        Destroy(ghostObj);
    }


    void createPhysicsScene() {
        _simulationScene = SceneManager.CreateScene("Simulation", new CreateSceneParameters(LocalPhysicsMode.Physics3D));
        _physicsScene = _simulationScene.GetPhysicsScene();

        moveObjColliderInScene(_obstaclesParent);
    }

    void moveObjColliderInScene(Transform t) {
        foreach (Transform ct in t) {

            if (ct.childCount > 0) {
                moveObjColliderInScene(ct);
            } else {

                if (ct.gameObject.tag != "OBJ_COLLIDER") return;

                spawnGhostObject(ct.gameObject, ct.position, ct.rotation);
            }
        }
    }

    GameObject spawnGhostObject(GameObject prefab, Vector3 pos, Quaternion rot) {
        var ghostObj = Instantiate(prefab, pos, rot);
        ghostObj.GetComponent<Renderer>().enabled = false;
        SceneManager.MoveGameObjectToScene(ghostObj, _simulationScene);

        return ghostObj;
    }
}
