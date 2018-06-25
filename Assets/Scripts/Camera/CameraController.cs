using System;
using UnityEngine;

public class CameraController : MonoBehaviour {

    public CameraBounds BoundsX = new CameraBounds();
    public CameraBounds BoundsZ = new CameraBounds();

    public Camera Camera;
    public int ScrollSpeed;
    public int MouseThreshold;
	
    // Update is called once per frame
    void Update ()
    {
        HandleCameraControl();
    }

    private void HandleCameraControl()
    {
        if (Input.mousePosition.x >= Screen.width - MouseThreshold && transform.position.x < BoundsX.Max) //right
            transform.Translate(ScrollSpeed * Time.deltaTime, 0, 0);

        if (Input.mousePosition.x <= MouseThreshold && transform.position.x > BoundsX.Min) //left
            transform.Translate(-ScrollSpeed * Time.deltaTime, 0, 0);

        if (Input.mousePosition.y >= Screen.height - MouseThreshold && transform.position.z < BoundsZ.Max) //top
            transform.Translate(0, 0, ScrollSpeed * Time.deltaTime);

        if (Input.mousePosition.y <= MouseThreshold && transform.position.z > BoundsZ.Min) //bottom
            transform.Translate(0, 0, -ScrollSpeed * Time.deltaTime);
    }
}

[Serializable]
public struct CameraBounds
{
    public int Min;
    public int Max;
}