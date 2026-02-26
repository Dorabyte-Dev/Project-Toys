using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class EnemyCanvas : MonoBehaviour
{
    [Header("Enemy Health")]
    public Image fill;
    public GameObject lifeBar;
    [Space(10)]
    
    [Header("Enemy Execution Affordance")]
    public GameObject executionAffordance;
    [Space(10)]

    [Header("Enemy Damage Number")]
    public GameObject[] damageNumbers;
    private RectTransform[] rectTransforms;
    private Vector2 initialPosition;
    private TMP_Text[] damagesText;
    [Space(10)]

    [Header("Config")]
    [SerializeField]private float damageNumberVelocity;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
