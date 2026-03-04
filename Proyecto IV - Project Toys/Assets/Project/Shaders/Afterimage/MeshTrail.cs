using DG.Tweening;
using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Windows;

public class MeshTrail : MonoBehaviour
{

    public float meshDestroyDelay = 2f;
    [Header("Mesh Related")]
    public float meshRefreshRate = 0.05f;
    //private MeshFilter meshFilter;
    [SerializeField]private SkinnedMeshRenderer[] skinnedMeshRenderers;

    [Header("Shader Related")]
    public Material mat;

    public bool toggleTrail;
    private bool isTrailActive;

    private void Start()
    {
    }
    void Update()
    {
        /*if (toggleTrail && !isTrailActive)
        {
            toggleTrail = false;
            isTrailActive = true;
            StartCoroutine(ActivateTrail(player.dashDuration));
        }*/
    }

    public void ToggleTrail()
    {
        toggleTrail = true;
        StartCoroutine(ActivateTrail());
    }

    public void UnToggleTrail()
    {
        toggleTrail = false;
    }

    IEnumerator ActivateTrail()
    {
        while(toggleTrail)
        {
            if(skinnedMeshRenderers == null)
            {
                skinnedMeshRenderers = GetComponentsInChildren<SkinnedMeshRenderer>();
            }

            foreach (SkinnedMeshRenderer skin in skinnedMeshRenderers)
            {
                GameObject newMesh = new GameObject();
                newMesh.transform.SetPositionAndRotation(skin.transform.position, skin.transform.rotation);
                newMesh.transform.localScale = transform.localScale;

                MeshRenderer rend = newMesh.AddComponent<MeshRenderer>();
                MeshFilter filter = newMesh.AddComponent<MeshFilter>();
                
                Mesh mesh = new Mesh();
                skin.BakeMesh(mesh);

                filter.mesh = mesh;
                rend.material = mat;
                rend.shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.Off;
                rend.material.DOFade(0, meshDestroyDelay);

                Destroy(newMesh, meshDestroyDelay);

                yield return new WaitForSecondsRealtime(meshRefreshRate);
            }
        }
        isTrailActive = false;
    }
}
