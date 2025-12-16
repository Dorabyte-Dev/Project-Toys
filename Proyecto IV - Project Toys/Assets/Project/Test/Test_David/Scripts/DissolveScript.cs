using System.Collections;
using UnityEngine;
using UnityEngine.VFX;

public class DissolveScript : MonoBehaviour
{
    public Renderer renderMesh;
    public VisualEffect vfxGraph;
    private Material[] meshMaterials;
    public float dissolveRate = 0.0125f;
    public float refreshRate = 0.025f;
    void Start()
    {
        if (renderMesh)
        {
            meshMaterials = renderMesh.materials;
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            StartCoroutine(Dissolve());
        }
    }

    public IEnumerator Dissolve()
    {
        if (vfxGraph)
        {
            vfxGraph.Play();
        }
        if (meshMaterials.Length > 0)
        {
            while(meshMaterials[0].GetFloat("_DissolveAmount") < 1f)
            {
                for (int i = 0; i < meshMaterials.Length; i++)
                {
                    float currentDissolve = meshMaterials[i].GetFloat("_DissolveAmount");
                    meshMaterials[i].SetFloat("_DissolveAmount", currentDissolve + dissolveRate);
                }
                yield return new WaitForSeconds(refreshRate);
            }
        }
    }
}
