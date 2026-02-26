using UnityEngine;
using UnityEngine.VFX;

public class Player_VFX : Entity_VFX
{
	[SerializeField]private VisualEffect slashEffect;

	public void Slash()
	{
		slashEffect.Play();
	}

	public void InterruptSlash()
	{
		slashEffect.Stop();
	}
}