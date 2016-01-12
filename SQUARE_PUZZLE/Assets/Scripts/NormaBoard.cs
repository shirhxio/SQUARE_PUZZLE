using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class NormaBoard : Datas {

	public GameObject scoreObj;
	public GameObject[] panelObj = new GameObject[numOfType];

	private Text scoreText;
	private Text[] panelText = new Text[numOfType];
	private int score;
	private int[] panel = new int[numOfType];
	private GameObject sys;
	private Systems sysPrp;
	private bool loadFlag = true;

	// Use this for initialization
	void Start () {
		sys = GameObject.Find ("Systems");
		sysPrp = sys.GetComponent<Systems> ();
		scoreText = scoreObj.GetComponent<Text> ();
		for (int i = 0; i < numOfType; i++) {
			panelText [i] = panelObj [i].GetComponent<Text> ();
		}
	}
	
	// Update is called once per frame
	void Update () {
		if (transform.gameObject.activeSelf) {
			if (loadFlag) {
				score = sysPrp.normaScore;
				panel = sysPrp.normaPanel;
				scoreText.text = score.ToString("0");
				for(int i = 0; i < numOfType; i++){
					panelText[i].text = panel[i].ToString("0");
				}
				loadFlag = false;
			}
		} 
	}

	public void SetLoadFlag(){
		loadFlag =true;
	}
}
