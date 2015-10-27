using UnityEngine;
using System.Collections;

public class ApplicationQuit : MonoBehaviour {

	public void SaveTheGame()
	{
		Debug.Log("Saving the Game.");
		PlayerPrefs.Save();
	}
}
