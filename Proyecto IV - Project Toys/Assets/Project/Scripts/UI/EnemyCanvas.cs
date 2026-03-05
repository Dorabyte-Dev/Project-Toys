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
    public GameObject keyboardAffordance;
    public GameObject genericControllerAffordance;
    public GameObject XboxAffordance;
    public GameObject PlayStationAffordance;
    [Space(10)]

    [Header("Enemy Damage Number")]
    public GameObject[] damageNumbers;
    private RectTransform[] rectTransforms;
    private Vector2 initialPosition;
    private TMP_Text[] damagesText;
}
