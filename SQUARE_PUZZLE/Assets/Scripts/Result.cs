using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class Result : Datas {
	
	private Systems sysPrp;

	public GameObject normaScoreObj;
	private Text normaScoreText;
	public GameObject[] normaPanelObj = new GameObject[numOfType];
	private Text[] normaPanelText = new Text[numOfType];
	public GameObject resultScoreObj;
	private Text resultScoreText;
	public GameObject[] resultPanelObj = new GameObject[numOfType];
	private Text[] resultPanelText = new Text[numOfType];
	public GameObject resultTextObj;
	private Text resultTextText;

	// Use this for initialization
	void Start () {
		sysPrp = GameObject.Find ("Systems").GetComponent<Systems> ();

		normaScoreText = normaScoreObj.GetComponent<Text> ();
		resultScoreText = resultScoreObj.GetComponent<Text> ();
		resultTextText = resultTextObj.GetComponent<Text> ();
		for (int i = 0; i < numOfType; i++) {
			normaPanelText [i] = normaPanelObj [i].GetComponent<Text> ();
			resultPanelText [i] = resultPanelObj [i].GetComponent<Text> ();
		}

		normaScoreText.text = sysPrp.normaScore.ToString ("0");
		resultScoreText.text = sysPrp.resultScore.ToString ("0");
		for (int i = 0; i < numOfType; i++) {
			normaPanelText [i].text = sysPrp.normaPanel [i].ToString ("0");
			resultPanelText [i].text = sysPrp.resultPanel [i].ToString ("0");
		}
		if (sysPrp.gameClearFlag) {
			resultTextText.text = "YOU WIN!";
		} else {
			resultTextText.text = "YOU LOSE...";
		}

		normaScoreObj.SetActive (false);
		resultScoreObj.SetActive (false);
		resultTextObj.SetActive (false);
		for (int i = 0; i < numOfType; i++) {
			normaPanelObj [i].SetActive (false);
			resultPanelObj [i].SetActive (false);
		}

		StartCoroutine ("ShowText");
	}
	
	// Update is called once per frame
	void Update () {

	}

	IEnumerator ShowText(){
		yield return new WaitForSeconds (1);
		normaScoreObj.SetActive (true);
		resultScoreObj.SetActive (true);
		yield return new WaitForSeconds (0.5f);
		for (int i = 0; i < numOfType; i++) {
			normaPanelObj [i].SetActive (true);		//count = 0
			resultPanelObj [i].SetActive (true);
			yield return new WaitForSeconds (0.5f);
		}
		yield return new WaitForSeconds (0.5f);
		resultTextObj.SetActive (true);
	}

	public void GoToStageSelect(){
		sysPrp.gameClearFlag = false;
		Application.LoadLevel ("StageSelect");
	}
}
