using UnityEngine;
using UnityEngine.UI;

public class EnemyUI : MonoBehaviour
{
    [Header("Enemy Health")]
    public Image fill;
    public GameObject lifeBar;
    [Space(20)]
    private Enemy_Health enemyLifeScript;

    void Start()
    {
        enemyLifeScript = GetComponentInParent<Enemy_Health>();
        if(fill == null)
        {
            fill = GetComponent<Image>();
        }
    }

    void Update()
    {
        transform.LookAt(Camera.main.transform);

        if (enemyLifeScript.currentHp >= enemyLifeScript.maxHp)
        {
            lifeBar.SetActive(false);
        }
        else
        {
            lifeBar.SetActive(true);
        }
        fill.fillAmount = enemyLifeScript.currentHp / enemyLifeScript.maxHp;
    }
}
