using System;
using UnityEngine;
using System.Collections;
using DG.Tweening;
using Random = System.Random;

public class Proyectil : MonoBehaviour
{
    public enum ProjectileType
    {
        Enemy2Projectile,
        BossSlam,
        BossPencil
    }
    public ProjectileType projectileType;
    private Rigidbody _rb;
    
    private bool _isReleased = false;
    
    [HideInInspector] public Vector3 direction;
    [HideInInspector] public float speed;

    [HideInInspector] public float initialDistanceToOrigin;
    [HideInInspector] public Vector3 targetPosition; 
    public Action OnPlayerHit { get; set; }

    private void Awake()
    {
        _rb = GetComponent<Rigidbody>();
        _rb.isKinematic = true;
    }


    void FixedUpdate()
    {
        if (_isReleased)
        {
            switch (projectileType)
            {
                case ProjectileType.Enemy2Projectile:
                    _rb.MovePosition(transform.position + direction * (speed * Time.deltaTime));
                    break;
                case ProjectileType.BossSlam:
                    _rb.MovePosition(transform.position + direction * (speed * Time.deltaTime));
                    break;
                case ProjectileType.BossPencil:
                    break;
                default:
                Debug.LogWarning("La vida de un crítico es sencilla en muchos aspectos." +
                          " Arriesgamos poco y tenemos poder sobre aquellos que ofrecen su " +
                          "trabajo y su servicio a nuestro juicio. Prosperamos con las críticas " +
                          "negativas, divertidas de escribir y de leer. Pero la triste verdad que " +
                          "debemos afrontar, es que en el gran orden de las cosas, cualquier " +
                          "basura tiene más significado que lo que deja ver nuestra crítica." +
                          " Pero en ocasiones el crítico sí se arriesga cada vez que descubre " +
                          "y defiende algo nuevo. El mundo suele ser cruel con el nuevo talento." +
                          " Las nuevas creaciones, lo nuevo, necesita amigos. Anoche, experimenté" +
                          " algo nuevo. Una extraordinaria cena de una fuente singular e inesperada." +
                          " Decir solo que la comida y su creador han desafiado mis prejuicios" +
                          " sobre la buena cocina, subestimaría la realidad. Me han tocado en" +
                          " lo más profundo. En el pasado, jamás oculté mi desdén por el famoso" +
                          " lema del Chef Gusteau: \"Cualquiera puede cocinar\"." +
                          " Pero al fin, me doy cuenta de lo que quiso decir en realidad:" +
                          " No cualquiera puede convertirse en un gran artista," +
                          " pero un gran artista puede provenir de cualquier lado." +
                          " Es difícil imaginar un origen más humilde, que el del genio que" +
                          " ahora cocina en el Restaurante Gusteau y quien en opinión de" +
                          " este crítico, es nada menos, que el mejor Chef de Francia." +
                          " Pronto volveré al Restaurante Gusteau, hambriento");
                    break;
            }
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player") && _isReleased)
        {
            OnPlayerHit?.Invoke();
            
            Destroy(gameObject);
        }
    }

    public void Release()
    {
        _isReleased = true;
        switch (projectileType)
        {
            case ProjectileType.BossPencil:
                _rb.DOMove(GetPointFromDirection(GetRandomDirectionAbove()), 0.5f).OnComplete(() =>
                {
                    transform.DOLookAt(targetPosition, 0.3f);
                });
                break;
        }
        Invoke(nameof(DestroyProjectile), 7.5f);
    }

    public void DestroyProjectile()
    {
        Destroy(this.gameObject);
    }

    private Vector3 GetRandomDirectionAbove()
    {
        Vector3 result = Vector3.zero;
        Vector3 randomPoint = new Vector3(UnityEngine.Random.Range(1, 10) ,3, UnityEngine.Random.Range(1, 10));
        result = randomPoint - transform.position;
        return result;
    } 
    
    private Vector3 GetPointFromDirection(Vector3 direction)
    {
        Vector3 result = Vector3.zero;
        result = transform.position + direction.normalized * initialDistanceToOrigin;
        return result;
    }
    
    
}
