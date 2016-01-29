using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;

public class StageSelect : MonoBehaviour {
	public void GoToMain(){
		SceneManager.LoadScene ("Main");
	}
}
