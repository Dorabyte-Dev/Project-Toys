using System;
using System.Collections;
using Unity.Cinemachine;
using UnityEngine;

public class ExecutionCameraManager : MonoBehaviour
{
    public CinemachineCamera[] executionCameras;
    public LayerMask obstacleMask;
    public CinemachineBrain brain;
    private Transform originalParent;

    private void Awake()
    {
        if (executionCameras == null)
        {
            executionCameras = GetComponentsInChildren<CinemachineCamera>();
        }
        if (brain == null)
        {
            brain = Camera.main != null ? Camera.main.GetComponent<CinemachineBrain>() : FindAnyObjectByType<CinemachineBrain>();
        }
    }

    private void Start()
    {
        originalParent = transform.parent;
        transform.parent = null;
        foreach (CinemachineCamera c in executionCameras)
        {
            c.Priority = -100;
        }
    }


    void Update()
    {
        
    }

    public void BackToDefaultCamera()
    {
        StartCoroutine(BackToDefaultCameraCoroutine());
    }
    
    private IEnumerator BackToDefaultCameraCoroutine()
    {
        transform.parent = null;
        foreach (CinemachineCamera c in executionCameras)
        {
            c.Priority = -100;
        }
        yield return null;

        while (brain.IsBlending)
        {
            yield return null;
        }

        transform.parent = originalParent;
        
    }
    

    public void MoveCamera(Vector3 position, Vector3 forward)
    {
        transform.position = position;
        transform.forward = forward;
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
