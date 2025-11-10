using UnityEngine;
using UnityEngine.UI;

public class EnemyUI : MonoBehaviour
{
    public Image lifeBar;
    private Enemy_Health enemyLifeScript;

    void Start()
    {
        enemyLifeScript = GetComponentInParent<Enemy_Health>();
        if(lifeBar == null)
        {
            lifeBar = GetComponent<Image>();
        }
    }

    void Update()
    {
        transform.LookAt(Camera.main.transform);
        Debug.Log("Enemy health: " + enemyLifeScript.currentHp);
        lifeBar.fillAmount = enemyLifeScript.currentHp / enemyLifeScript.maxHp;
    }
}
