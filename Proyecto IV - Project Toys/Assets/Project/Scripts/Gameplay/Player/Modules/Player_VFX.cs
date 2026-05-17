using System.Collections;
using UnityEngine;
using UnityEngine.VFX;

public class Player_VFX : Entity_VFX
{
	[SerializeField] private ParticleSystem swordTrailEffect;
	
	[SerializeField] private Player player;
	
	#region HealingEffect
	[Header("--- Healing Effect ---")]
	[SerializeField]private ParticleSystem healingEffect;
	public Material[] healingMaterials;
	public float healingDuration = 2f;
	#endregion

	protected override void Awake()
	{
		base.Awake();
		player = GetComponent<Player>();
	}



	public void AttackCameraShake()
	{
		CameraManager.instance.CameraShake();
	}
	
	public void ControllerShake()
	{
		RumbleManager.RumblePulse("PlayerAttack");
	}
	
	public void Slash()
	{
		swordTrailEffect.Play();
	}

	public void InterruptSlash()
	{
		swordTrailEffect.Stop(true, ParticleSystemStopBehavior.StopEmittingAndClear);
	}

	public void HitStop()
	{
		StartCoroutine(base.HitStop(player));
	}

	public override void DamageFeedback(Transform damageDealer)
	{
		TriggerMaterialChange();
		Vector3 pushDirection = (transform.position - damageDealer.position).normalized;
		CameraManager.instance.CameraShake();
		SoundManager.instance.PlayRandomInRange(new string[]{"PlayerVoiceGetHit1", "PlayerVoiceGetHit2"});
		SoundManager.instance.Play("PlayerGetHitImpact");
		RumbleManager.RumblePulse("PlayerDamage");
		StartCoroutine(PushFeedback(pushDirection));
		player.ChangePlayerState(player.flinchState);
	}

	protected override IEnumerator PushFeedback(Vector3 direction)
	{
		float elapsed = 0f;
		while (elapsed < pushWaitDuration)
		{
			player.ch.Move(pushStrength * Time.deltaTime * direction);
			elapsed += Time.deltaTime;
			yield return null;
		}
	}
	
	public void HealingEffect()
	{
		healingEffect.Play(); 
		HealingGlow();
		Invoke(nameof(RemoveHealingGlow), healingDuration);
	}

	private void HealingGlow()
	{
		if(renderMesh != null && healingMaterials != null)
		{
			for(int i = 0; i < renderMesh.Length; i++)
			{
				Material[] newMats = new Material[renderMesh[i].materials.Length];
				for(int j = 0; j < renderMesh[i].materials.Length; j++)
				{
					newMats[j] = healingMaterials[i];
				}
				renderMesh[i].materials = newMats;
			}
		}
		else
		{
			Debug.LogError("Render Mesh or Healing Material is not assigned in: " + this.gameObject.name);
		}
	}
	
	private void RemoveHealingGlow()
	{
		if (renderMesh != null && originalMaterials != null)
		{
			for(int i = 0; i < renderMesh.Length; i++)
			{
				renderMesh[i].materials = originalMaterials[i];
			}
		}
		else
		{
			Debug.LogError("Mesh Renderer or Original Materials is not assigned in: " + this.gameObject.name);
		}
	}
}