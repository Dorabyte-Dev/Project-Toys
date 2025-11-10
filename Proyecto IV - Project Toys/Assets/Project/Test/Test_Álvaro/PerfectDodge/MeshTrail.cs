using DG.Tweening;
using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Windows;

public class MeshTrail : MonoBehaviour
{
    public Player player;

    public float meshDestroyDelay = 2f;
    [Header("Mesh Related")]
    public float meshRefreshRate = 0.05f;
    private MeshFilter meshFilter;

    [Header("Shader Related")]
    public Material mat;

    public bool toggleTrail;
    private bool isTrailActive;

    private void Start()
    {
        player = GetComponent<Player>();
    }
    void Update()
    {
        if (toggleTrail && !isTrailActive)
        {
            toggleTrail = false;
            isTrailActive = true;
            StartCoroutine(ActivateTrail(player.dashDuration));
        }
    }

    IEnumerator ActivateTrail (float timeActive)
    {
        while(timeActive > 0)
        {
            timeActive -= meshRefreshRate;

            if(meshFilter == null)
            {
                meshFilter = GetComponent<MeshFilter>();
            }
            GameObject newMesh = new GameObject();
            newMesh.transform.SetPositionAndRotation(transform.position, transform.rotation);
            newMesh.transform.localScale = transform.localScale;

            MeshRenderer rend = newMesh.AddComponent<MeshRenderer>();
            MeshFilter filter = newMesh.AddComponent<MeshFilter>();

            filter.mesh = meshFilter.mesh;
            rend.material = mat;
            rend.shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.Off;
            rend.material.DOFade(0, meshDestroyDelay);

            Destroy(newMesh, meshDestroyDelay);

            yield return new WaitForSeconds(meshRefreshRate);
        }

        isTrailActive = false;
    }
}
