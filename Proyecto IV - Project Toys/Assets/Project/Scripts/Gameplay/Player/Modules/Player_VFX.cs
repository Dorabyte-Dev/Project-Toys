using System.Collections;
using UnityEngine;
using UnityEngine.VFX;

public class Player_VFX : Entity_VFX
{
	[SerializeField] private ParticleSystem swordTrailEffect;
	[SerializeField] private Player player;
	
	protected override void Awake()
	{
		base.Awake();
		player = GetComponent<Player>();
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
		RumbleManager.RumblePulse(1f, 0.5f, 0.2f);
		StartCoroutine(PushFeedback(pushDirection));
		player.ChangePlayerState(player.flinchState);
	}

	protected override IEnumerator PushFeedback(Vector3 direction)
	{
		float elapsed = 0f;
		while (elapsed < pushWaitDuration)
		{
			player.ch.Move(direction * pushStrength * Time.unscaledDeltaTime);
			elapsed += Time.unscaledDeltaTime;
			yield return null;
		}
	}
}