using System;
using UnityEngine;

public class Proyectil : MonoBehaviour
{
    public enum ProjectileType
    {
        V1
    }
    public ProjectileType projectileType;
    private Rigidbody _rb;
    
    private bool _isReleased = false;
    
    [HideInInspector] public Vector3 direction;
    [HideInInspector] public float speed;

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
                case ProjectileType.V1:
                    _rb.MovePosition(transform.position + direction * (speed * Time.deltaTime));
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

    public void Release()
    {
        _isReleased = true;
    }
}
