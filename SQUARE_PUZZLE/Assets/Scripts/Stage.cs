using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class Stage : Datas {

	public int stageNo;
	public int stageNormaScore;
	public int[] stageNorma = new int[numOfType];
	private GameObject sys;
	private Systems sysPrp;
	private Button button;
	void Start (){
		sys = GameObject.Find ("Systems");
		sysPrp = sys.GetComponent<Systems> ();
		button = transform.gameObject.GetComponent<Button>();
		if (stageNo <= sysPrp.openedStage) {
			button.interactable = true;
		} else {
			button.interactable = false;
		}
	}

	public void SetStagePrp(){
		sysPrp.playingStage = stageNo;
		sysPrp.normaScore = stageNormaScore;
		sysPrp.normaPanel = stageNorma;
	}

}
