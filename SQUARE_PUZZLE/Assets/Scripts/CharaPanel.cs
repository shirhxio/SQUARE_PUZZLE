using UnityEngine;
using System.Collections;

public class CharaPanel : Datas {

	private GameObject sys;		//for Systems
	private Systems sysPrp;
	private Chara sysChr;
	private SpriteRenderer charaTexture;

	public int partyNo;

	public int numChara;
	public int typeChara;
	public int noChara;
	public int scoreBuff;
	public int[] panelBuff = new int[numOfType];
	private Sprite spriteChara;

	void Start () {
		sys = GameObject.Find ("Systems");
		sysPrp = sys.GetComponent<Systems> ();
		sysChr = sys.GetComponent<Chara> ();
		numChara = sysPrp.party [partyNo];
		typeChara = numChara / 100;
		noChara = numChara % 100;
		scoreBuff = sysChr.scoreBuffs [typeChara] [noChara];
		for (int i = 0; i < numOfType; i++) {
			panelBuff [i] = sysChr.panelBuffs [typeChara] [noChara, i];
		}
		switch (typeChara) {
		case 0:
			spriteChara = sysChr.bluetypeSprites[noChara];
			break;
		case 1:
			spriteChara = sysChr.redtypeSprites[noChara];
			break;
		case 2:
			spriteChara = sysChr.yellowtypeSprites[noChara];
			break;
		}

		charaTexture = transform.gameObject.GetComponent<SpriteRenderer> ();
		charaTexture.sprite = spriteChara;
	}
	
	// Update is called once per frame
	void Update () {
	
	}
}
