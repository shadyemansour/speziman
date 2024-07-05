using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BarbaraSceneManager : MonoBehaviour
{
    [SerializeField] private BarbaraController barbara;
    [SerializeField] private HelmutBarbaraSceneController helmut;
    [SerializeField] private CameraFollow cam;

    // Start is called before the first frame update
    void Start()
    {
        // helmut.WalkTo(new Vector3(-0.72f, -2.4f, 0), StopCameraFollow);
        // cam.isFollowingPlayer = true;

        
    }

    public void StopCameraFollow ()
    {
        cam.isFollowingPlayer = false;
    }
}
