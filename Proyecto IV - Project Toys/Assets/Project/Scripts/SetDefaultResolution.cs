using UnityEngine;

public class SetDefaultResolution : MonoBehaviour
{
	private void Awake()
	{
		Screen.SetResolution(Screen.currentResolution.width,Screen.currentResolution.height, FullScreenMode.FullScreenWindow);
	}
}
