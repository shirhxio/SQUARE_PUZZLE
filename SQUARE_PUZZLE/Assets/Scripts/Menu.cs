using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;

public class Menu : MonoBehaviour {

	private GameObject sys;
	private Systems sysPrp;

	void Start(){
		sys = GameObject.Find ("Systems");
		sysPrp = sys.GetComponent<Systems> ();
	}

	public void BackToTitle(){
		Destroy (sys);
		SceneManager.LoadScene ("Title");
	}

	public void BackToStageSelect(){
		SceneManager.LoadScene ("StageSelect");
	}

	public void ResetClearData(){
		sysPrp.resetStageFlag = true;
	}

	public void QuitGame(){
		Application.Quit ();
	}
}
