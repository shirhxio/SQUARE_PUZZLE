using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;

public class LetsGacha : MonoBehaviour {

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}

	public void GoToGacha(){
		SceneManager.LoadScene ("Gacha");
	}
}
