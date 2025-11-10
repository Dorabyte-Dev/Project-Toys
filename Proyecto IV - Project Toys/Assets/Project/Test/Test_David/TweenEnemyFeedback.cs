using DG.Tweening;
using System.Collections.Generic;
using UnityEngine;

public class TweenEnemyFeedback : MonoBehaviour
{
    private List<Material[]> originalMaterials = new List<Material[]>();
    private List<Color[]> originalColors = new List<Color[]>();
    //private Coroutine changeCoroutine;
    //private Coroutine revertCoroutine;
    private bool isStarted;
    private DOTween changeColorsTween;
    public Color feedbackColor;
    void Start()
    {
        SkinnedMeshRenderer[] skinnedMeshRenderers = GetComponentsInChildren<SkinnedMeshRenderer>();
        foreach (SkinnedMeshRenderer renderer in skinnedMeshRenderers)
        {
            List<Color> colors = new List<Color>();
            originalMaterials.Add(renderer.materials);
            for (int i = 0; i < renderer.materials.Length; i++)
            {
                if (renderer.materials[i].HasProperty("_Color"))
                {
                    colors.Add(renderer.materials[i].color);
                }
                else
                {
                    colors.Add(Color.black);
                }
            }
            originalColors.Add(colors.ToArray());
        }
    }

    public void TriggerMaterialChange()
    {
        if(isStarted == false)
        {
            ChangeMaterialsColors(feedbackColor);
        }
        else
        {
            DOTween.Clear();
            ChangeMaterialsColors(feedbackColor);
        }
        
    }

   

    //private IEnumerator ChangeMaterialsTemporarily()
    //{
    //    ChangeMaterialsToColor(feedbackColor); // 255, 255, 255
    //    yield return new WaitForSeconds(0.2f);
    //    revertCoroutine = StartCoroutine(RevertMaterialsSmoothly(1.0f));
    //}


    private void ChangeMaterialsToColor(Color color)
    {
        SkinnedMeshRenderer[] skinnedMeshRenderers = GetComponentsInChildren<SkinnedMeshRenderer>();
        foreach (SkinnedMeshRenderer renderer in skinnedMeshRenderers)
        {
            Material[] newMaterials = new Material[renderer.materials.Length];
            for (int i = 0; i < renderer.materials.Length; i++)
            {
                // Create a new temporary material instance
                newMaterials[i] = new Material(renderer.materials[i]);
                if (newMaterials[i].HasProperty("_Color"))
                {
                    newMaterials[i].color = color;
                }
            }
            renderer.materials = newMaterials;
        }
    }

    private void ChangeMaterialsColors(Color color)
    {
        isStarted = true;
        SkinnedMeshRenderer[] skinnedMeshRenderers = GetComponentsInChildren<SkinnedMeshRenderer>();
        foreach(SkinnedMeshRenderer renderer in skinnedMeshRenderers)
        {
            renderer.material.DOColor(color, 0).OnComplete(() =>
            {
                for (int i = 0; i < skinnedMeshRenderers.Length; i++)
                {
                    for (int j = 0; j < skinnedMeshRenderers[i].materials.Length; j++)
                    {
                        if (skinnedMeshRenderers[i].materials[j].HasProperty("_Color"))
                        {
                            renderer.material.DOColor(originalColors[i][j], 1.0f);
                            isStarted = false;
                        }
                    }
                }
            });
        }
    }


    //private IEnumerator RevertMaterialsSmoothly(float duration)
    //{
    //    SkinnedMeshRenderer[] skinnedMeshRenderers = GetComponentsInChildren<SkinnedMeshRenderer>();


    //    List<Color[]> currentColors = new List<Color[]>();
    //    foreach (SkinnedMeshRenderer renderer in skinnedMeshRenderers)
    //    {
    //        List<Color> colors = new List<Color>();
    //        for (int i = 0; i < renderer.materials.Length; i++)
    //        {
    //            if (renderer.materials[i].HasProperty("_Color"))
    //            {
    //                colors.Add(renderer.materials[i].color);
    //            }
    //            else
    //            {
    //                colors.Add(Color.black); // Dummy color for materials without _Color property
    //            }
    //        }
    //        currentColors.Add(colors.ToArray());
    //    }


    //    float elapsedTime = 0f;


    //    while (elapsedTime < duration)
    //    {
    //        elapsedTime += Time.deltaTime;
    //        float t = elapsedTime / duration;


    //        for (int i = 0; i < skinnedMeshRenderers.Length; i++)
    //        {
    //            for (int j = 0; j < skinnedMeshRenderers[i].materials.Length; j++)
    //            {
    //                if (skinnedMeshRenderers[i].materials[j].HasProperty("_Color"))
    //                {
    //                    skinnedMeshRenderers[i].materials[j].color = Color.Lerp(feedbackColor, originalColors[i][j], t);
    //                }
    //            }
    //        }


    //        yield return null;
    //    }


    //    for (int i = 0; i < skinnedMeshRenderers.Length; i++)
    //    {
    //        skinnedMeshRenderers[i].materials = originalMaterials[i];
    //    }
    //}


    /*private void Update()
    {
        if (Input.GetKeyDown(KeyCode.P))
        {
            TriggerMaterialChange();
        }
    }*/
}
