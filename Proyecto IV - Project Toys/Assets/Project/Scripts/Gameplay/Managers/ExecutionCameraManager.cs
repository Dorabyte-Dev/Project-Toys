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
        transform.parent = null;
        foreach (CinemachineCamera c in executionCameras)
        {
            c.Priority = -100;
        }
    }


    void Update()
    {
        
    }

    public void MoveCameraParent(Transform target)
    {
        transform.position = target.position;
        transform.eulerAngles = target.eulerAngles;
    }

    public CinemachineCamera GetAvailableCameraOnlyRaycast(GameObject target)
    {
        CinemachineCamera availableCamera = null;
        foreach (CinemachineCamera camera in executionCameras)
        {
            Ray ray = new Ray(camera.transform.position, target.transform.position - camera.transform.position);
            float distanceToTarget = Vector3.Distance(camera.transform.position, target.transform.position);
            if (!Physics.Raycast(ray, out RaycastHit hit, distanceToTarget, obstacleMask))
            {
                Debug.Log("Camera " + camera.name + " has a clear line of sight to the target.");
                availableCamera = camera;
                break;
            }
            else
            {
                Debug.Log("Camera " + camera.name + " is obstructed by " + hit.collider.name);
            }
        }

        if (availableCamera == null)
        {
            availableCamera = executionCameras[0];
        }
        return availableCamera;
    }

    public CinemachineCamera GetAvailableCameraRaycastAndCameraProximity(GameObject target)
    {
        CinemachineCamera availableCamera = null;
        float[] cameraDistances = new float[executionCameras.Length];
        for (int i = 0; i < executionCameras.Length; i++)
        {
            cameraDistances[i] = Vector3.Distance(Camera.main.transform.position, executionCameras[i].transform.position);
        }
        for(int i = 0; i < executionCameras.Length - 1; i++)
        {
            for (int j = 0; j < executionCameras.Length - i - 1; j++)
            {
                if (cameraDistances[j] > cameraDistances[j + 1])
                {
                    // Swap distances
                    (cameraDistances[j], cameraDistances[j + 1]) = (cameraDistances[j + 1], cameraDistances[j]);

                    // Swap cameras
                    (executionCameras[j], executionCameras[j + 1]) = (executionCameras[j + 1], executionCameras[j]);
                }
            }
        }
        foreach (CinemachineCamera camera in executionCameras)
        {
            Ray ray = new Ray(camera.transform.position, target.transform.position - camera.transform.position);
            float distanceToTarget = Vector3.Distance(camera.transform.position, target.transform.position);
            if (!Physics.Raycast(ray, out RaycastHit hit, distanceToTarget, obstacleMask))
            {
                Debug.Log("Camera " + camera.name + " has a clear line of sight to the target.");
                availableCamera = camera;
                break;
            }
        }
        if (availableCamera == null)
        {
            availableCamera = executionCameras[0];
        }
        return availableCamera;
    }

    public CinemachineCamera TestExecutionCamera()
    {
        return executionCameras[0];
    }
}
