using UnityEngine;
using System.Collections;

public class Menu : MonoBehaviour {

	private GameObject sys;
	private Systems sysPrp;

	void Start(){
		sys = GameObject.Find ("Systems");
		sysPrp = sys.GetComponent<Systems> ();
	}

	public void BackToTitle(){
		Application.LoadLevel ("Title");
	}

	public void BackToStageSelect(){
		Application.LoadLevel ("StageSelect");
	}

	public void ResetClearData(){
		sysPrp.resetStageFlag = true;
	}

	public void QuitGame(){
		Application.Quit ();
	}
}
