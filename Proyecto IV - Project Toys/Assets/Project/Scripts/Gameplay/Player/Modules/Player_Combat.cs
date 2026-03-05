using System;
using UnityEngine;

public class Player_Combat : Entity_Combat
{
    [SerializeField] private Player player;

    public override void Awake()
    {
        base.Awake();
        player = GetComponent<Player>();
    }
    // Llamado por Player_ComboSystem antes de que se ejecute el ataque

    public override void PerformAttack()
    {
        if (player.currentAttack == null)
        {
            Debug.LogWarning("[Player_Combat] No AttackData assigned, using base damage.");
            base.PerformAttack();
            return;
        }

        finalDamage = baseDamage * player.currentAttack.motionValue;
        Debug.Log("Final Damage: " + finalDamage);

        foreach (var target in GetDetectedColliders())
        {
            Entity_Health targetHealth = target.GetComponent<Entity_Health>();
            if (targetHealth != null)
            {
                targetHealth.TakeDamage(finalDamage, this.transform);
                if (targetHealth.invincibleMode) return;
                targetHit?.Invoke();
            }
            else if (target.CompareTag("dObject"))
            {
                Break_Object breakObject = target.GetComponent<Break_Object>();
                if (breakObject != null)
                    breakObject.ActivateDestruction();
            }
            else
            {
                Debug.LogWarning("Entity_Health not found on " + target.name);
            }
        }
    }

    /*protected override Collider[] GetDetectedColliders()
    {
        BoxCollider colliderUsed = player.GetColliderUsed(player.currentAttack.colliderUsed);
        Debug.Log("Box used: " + colliderUsed.name);
        Vector3 centerPoint = transform.TransformPoint(colliderUsed.center);

        Vector3 halfExtents = Vector3.Scale(colliderUsed.size, transform.lossyScale) * 0.5f;

        Quaternion rotation = transform.rotation;

        return Physics.OverlapBox(centerPoint, halfExtents, rotation, whatIsTarget);
    }*/
    
    private void OnDrawGizmos()
    {
        if (player.currentAttack)
        {
            // En modo edición, 'miCollider' puede ser null porque Awake() no se ha ejecutado.
            // Lo buscamos "al vuelo" si hace falta para que el Gizmo se vea sin darle al Play.
            BoxCollider col = player.GetColliderUsed(player.currentAttack.colliderUsed) != null ? player.GetColliderUsed(player.currentAttack.colliderUsed) : GetComponent<BoxCollider>();
        
            if (col == null) return;

            // 1. Guardamos la matriz original para no estropear otros Gizmos que Unity deba dibujar luego
            Matrix4x4 matrizOriginal = Gizmos.matrix;

            // 2. Le decimos al sistema de Gizmos que trabaje usando el espacio (posición, rotación y escala) de este objeto
            Gizmos.matrix = transform.localToWorldMatrix;

            // 3. Dibujamos un cubo sólido semitransparente
            Gizmos.color = new Color(1f, 0f, 0f, 0.3f); // Rojo al 30% de opacidad
            Gizmos.DrawCube(col.center, col.size); // Como ya aplicamos la matriz, usamos las coordenadas locales!

            // 4. Dibujamos las líneas de los bordes para que quede bien definido
            Gizmos.color = Color.red;
            Gizmos.DrawWireCube(col.center, col.size);

            // 5. Restauramos la matriz original (¡Muy importante!)
            Gizmos.matrix = matrizOriginal;
        }
    }
}