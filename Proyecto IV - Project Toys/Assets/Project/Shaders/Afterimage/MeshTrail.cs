using DG.Tweening;
using System.Collections;
using UnityEngine;
using UnityEngine.Serialization;

public class MeshTrail : MonoBehaviour
{

    [System.Serializable]
    public struct MeshTrailPart
    {
        public SkinnedMeshRenderer Skin;
        public Material Material;
    }
    public float meshDestroyDelay = 2f;
    [Header("Mesh Related")]
    public float meshRefreshRate = 0.05f;

    [SerializeField]private MeshTrailPart[] meshTrailParts;
    [SerializeField] private Material defaultMat;
    public bool toggleTrail;

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
            if(meshTrailParts == null)
            {
                SkinnedMeshRenderer[] skinnedMeshRenderers = GetComponentsInChildren<SkinnedMeshRenderer>();
                meshTrailParts = new  MeshTrailPart[skinnedMeshRenderers.Length];
                for (int i = 0; i < skinnedMeshRenderers.Length; i++)
                {
                    meshTrailParts[i] = new MeshTrailPart
                    {
                        Skin = skinnedMeshRenderers[i],
                        Material = defaultMat
                    };
                }
            }

            foreach (MeshTrailPart part in meshTrailParts)
            {
                GameObject newMesh = new GameObject();
                newMesh.transform.SetPositionAndRotation(part.Skin.transform.position, part.Skin.transform.rotation);
                newMesh.transform.localScale = transform.localScale;

                MeshRenderer rend = newMesh.AddComponent<MeshRenderer>();
                MeshFilter filter = newMesh.AddComponent<MeshFilter>();
                
                Mesh mesh = new Mesh();
                part.Skin.BakeMesh(mesh);

                filter.mesh = mesh;
                if (part.Material != null)
                {
                    rend.material = part.Material;
                }
                else
                {
                    rend.material = defaultMat;
                }
                
                rend.shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.Off;
                rend.material.DOFade(0, meshDestroyDelay);

                Destroy(newMesh, meshDestroyDelay);

                yield return new WaitForSecondsRealtime(meshRefreshRate);
            }
        }
    }
}
