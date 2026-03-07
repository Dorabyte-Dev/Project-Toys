using UnityEngine;
using UnityEngine.VFX;

public class Player_VFX : Entity_VFX
{
	[SerializeField] private ParticleSystem swordTrailEffect;

	public void Slash()
	{
		swordTrailEffect.Play();
	}

	public void InterruptSlash()
	{
		swordTrailEffect.Stop(true, ParticleSystemStopBehavior.StopEmittingAndClear);
	}

	public override void DamageVFX_Feedback(Transform damageDealer)
	{
		base.DamageVFX_Feedback(damageDealer);
		CameraManager.instance.CameraShake();
	}
}