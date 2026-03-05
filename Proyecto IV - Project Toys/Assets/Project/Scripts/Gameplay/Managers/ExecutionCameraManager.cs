using System;
using Unity.Cinemachine;
using UnityEngine;

public class ExecutionCameraManager : MonoBehaviour
{
    public CinemachineCamera[] executionCameras;
    public LayerMask obstacleMask;

    private void Awake()
    {
        if (executionCameras == null)
        {
            executionCameras = GetComponentsInChildren<CinemachineCamera>();
        }
    }

    private void Start()
    {
        foreach (CinemachineCamera c in executionCameras)
        {
            c.Priority = -100;
        }
    }


    void Update()
    {
        
    }

    public CinemachineCamera GetAvailableCamera(GameObject target)
    {
        CinemachineCamera availableCamera = null;
        foreach (CinemachineCamera camera in executionCameras)
        {
            Ray ray = new Ray(camera.transform.position, target.transform.position - camera.transform.position);
            if (!Physics.Raycast(ray, out RaycastHit hit, Mathf.Infinity, obstacleMask))
            {
                Debug.Log("Camera " + camera.name + " has a clear line of sight to the target.");
                availableCamera = camera;
                break;
            }
        }
        return availableCamera;
    }

    public CinemachineCamera TestExecutionCamera()
    {
        return executionCameras[0];
    }
}
