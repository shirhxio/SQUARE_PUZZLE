using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using UnityEngine.SceneManagement;

public class Gacha : Chara {

	private bool playedGachaFlag = false;
	public int numChara;
	public int typeChara;
	public int noChara;

	public GameObject newCharaobj;
	public GameObject scoreBuffObj;
	public GameObject[] panelBuffObj = new GameObject[numOfType];
	private SpriteRenderer newCharaTexture;
	private Text scoreBuffText;
	private Text[] panelBuffText = new Text[numOfType];
	private Systems sysPrp;
	private Chara sysChr;
	private Ray2D ray;
	private RaycastHit2D hit;

	// Use this for initialization
	void Start () {
		playedGachaFlag = true;
		sysPrp = GameObject.Find ("Systems").GetComponent<Systems> ();
		sysChr = GameObject.Find ("Systems").GetComponent<Chara> ();
		newCharaTexture = newCharaobj.GetComponent<SpriteRenderer> ();
		scoreBuffText = scoreBuffObj.GetComponent<Text> ();
		for (int i = 0; i < numOfType; i++) {
			panelBuffText[i] = panelBuffObj [i].GetComponent<Text> ();
		}
	}
	
	// Update is called once per frame
	void Update () {
		if (playedGachaFlag && Input.GetMouseButtonDown(0)) {
			hit = Physics2D.Raycast (Camera.main.ScreenToWorldPoint (Input.mousePosition), Vector2.zero);
			if(hit){
				if(hit.transform.gameObject.tag == "chara"){
					int i = hit.transform.gameObject.GetComponent<CharaPanel>().partyNo;
					sysPrp.party[i] = numChara;
					sysPrp.savePartyFlag = true;
					SceneManager.LoadScene ("Gacha");
				}
			}
		}
	}

	public void PlayGacha(){
		System.DateTime dt = System.DateTime.Now;
		Random.seed = dt.Second + dt.Millisecond;
		numChara = 100 * (((int)(Random.value * 100)) % numOfType) + (((int)(Random.value * 100)) % numOfType);
		typeChara = numChara / 100;
		noChara = numChara % 100;
		switch(typeChara){
		case 0:
			newCharaTexture.sprite = sysChr.bluetypeSprites[noChara];
			break;
		case 1:
			newCharaTexture.sprite = sysChr.redtypeSprites[noChara];
			break;
		case 2:
			newCharaTexture.sprite = sysChr.yellowtypeSprites[noChara];
			break;
		}
		scoreBuffText.text = sysChr.scoreBuffs [typeChara] [noChara].ToString ("0");
		for (int i = 0; i < numOfType; i++) {
			panelBuffText [i].text = sysChr.panelBuffs [typeChara] [noChara,i].ToString("0");
		}
		playedGachaFlag = true;
	}

	public void GoToTitle(){
		SceneManager.LoadScene ("Title");
	}
}
