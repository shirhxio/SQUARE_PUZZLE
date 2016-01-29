using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;

public class StageSelect : MonoBehaviour {

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}

	public void GoToMain(){
		SceneManager.LoadScene ("Main");
	}
}
