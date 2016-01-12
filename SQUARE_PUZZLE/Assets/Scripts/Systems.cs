using UnityEngine;
using System.Collections;

public class Systems: Datas {
	
	public bool savePartyFlag = false;
	public bool saveStageFlag = false;
	public bool resetStageFlag = false;
	public string[] PARTY_KEY = new string[partySize];
	public const string STAGE_KEY = "STAGE_KEY";

	public int[] party = new int[partySize];
	public int openedStage;
	public int playingStage;
	public bool gameClearFlag = false;
	public int normaScore = 0;
	public int[] normaPanel = new int[numOfType];
	public int resultScore = 0;
	public int[] resultPanel = new int[numOfType];

	void Awake(){
		DontDestroyOnLoad (transform.gameObject);
	}

	// Use this for initialization
	void Start () {
		for (int i = 0; i < partySize; i++) {
			PARTY_KEY[i] = "PARTY" + i + "_KEY";
			party[i] = PlayerPrefs.GetInt(PARTY_KEY[i],(i%3)*100);
		}
		openedStage = PlayerPrefs.GetInt (STAGE_KEY, 0);
	}
	
	// Update is called once per frame
	void Update () {
		if (savePartyFlag) {
			for (int i = 0; i < 5; i++) {
				PlayerPrefs.SetInt(PARTY_KEY[i],party[i]);
			}
			PlayerPrefs.Save();
			savePartyFlag = false;
		}
		if (saveStageFlag) {
			if (openedStage == playingStage) {
				openedStage++;
				PlayerPrefs.SetInt (STAGE_KEY, openedStage);
				PlayerPrefs.Save();
			}
			saveStageFlag = false;
		}
		if (resetStageFlag) {
			openedStage = 0;
			PlayerPrefs.DeleteKey (STAGE_KEY);
			resetStageFlag = false;
		}
	}
}
