using UnityEngine;
using UnityEngine.VFX;

public class Player_VFX : Entity_VFX
{
	[SerializeField]private VisualEffect slashEffect;
	[SerializeField] private MeshTrail swordTrail;
	[SerializeField] private ParticleSystem swordTrailEffect;

	public void Slash()
	{
		//slashEffect.Play();
		//swordTrail.ToggleTrail();
		swordTrailEffect.Play();
	}

	public void InterruptSlash()
	{
		//slashEffect.Stop();
		//swordTrail.UnToggleTrail();
		swordTrailEffect.Stop(true, ParticleSystemStopBehavior.StopEmittingAndClear);
	}
}