using UnityEngine;
using UnityEngine.VFX;

public class Player_VFX : Entity_VFX
{
	[SerializeField]private VisualEffect slashEffect;
	[SerializeField] private MeshTrail swordTrail;

	public void Slash()
	{
		//slashEffect.Play();
		swordTrail.ToggleTrail();
	}

	public void InterruptSlash()
	{
		//slashEffect.Stop();
		swordTrail.UnToggleTrail();
	}
}