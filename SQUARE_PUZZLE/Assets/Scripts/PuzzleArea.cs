using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using UnityEngine.SceneManagement;

public class PuzzleArea : Datas {
	private const int panel_Row = 7;		//panelArea size (Row)
	private const int panel_Column = 7;		//panelArea size (Column)
	private GameObject[,] panelArray = new GameObject[panel_Row,panel_Column];
	private GameObject[] touchedPanel = new GameObject[2];
	private int touchCount = 0;
	private int touchColor;
	private int totalScoreBuff = 100;						//magnification of Score(%)
	private int[] totalPanelBuff = new int[numOfType];		//magnification of panelscore(%)
	private int currentScore;
	private int[] currentPanelScore = new int[numOfType];
	private Text scoreText;
	private Text[] panelScoreText = new Text[numOfType];
	private int countTime = 3; 
	private float timeLimit = 30.0f;
	public GameObject panelPrefab;
	public GameObject dummyPanel;
	public GameObject timer;
	public GameObject scoreObj;
	public GameObject[] panelScoreObj = new GameObject[numOfType];
	public Sprite[] panelSprites;
	public GameObject[] partyMember = new GameObject[partySize];
	private bool isPlaying = false;

	private GameObject sys;
	private Systems sysPrp;
	private Panel panelPrp;
	private Panel[] touchedPanelPrp = new Panel[2];
	private SpriteRenderer panelTexture;
	private GameObject panel;
	private Text timerText;
	private Ray2D ray;
	private RaycastHit2D hit;

	// Use this for initialization
	void Start () {
		sys = GameObject.Find ("Systems");
		sysPrp = sys.GetComponent<Systems> ();

		sysPrp.gameClearFlag = false;

		totalScoreBuff = 100;
		for (int i = 0; i < partySize; i++) {
			totalScoreBuff += partyMember[i].GetComponent<CharaPanel>().scoreBuff;
		}
		for (int i = 0; i < numOfType; i++) {
			totalPanelBuff [i] = 100;
			for (int j = 0; j < partySize; j++) {
				totalPanelBuff [i] += partyMember [j].GetComponent<CharaPanel> ().panelBuff [i];
			}
		}

		currentScore = 0;
		scoreText = scoreObj.GetComponent<Text> ();
		scoreText.text = currentScore.ToString ("###0");
		for (int i = 0; i < numOfType; i++) {
			currentPanelScore [i] = 0;
			panelScoreText[i] = panelScoreObj[i].GetComponent<Text>();
			panelScoreText[i].text = currentPanelScore[i].ToString("###0");
		}
		timerText = timer.GetComponent<Text> ();
		for(int i = 0; i < 2; i++){
			touchedPanel[i] = dummyPanel;
		}
		SetArray (0, 0, panel_Row - 1, panel_Column - 1);
		StartCoroutine("CountDown");
	}
	
	// Update is called once per frame
	void Update () {
		if (isPlaying && Input.GetMouseButtonDown (0)) {
			hit = Physics2D.Raycast (Camera.main.ScreenToWorldPoint (Input.mousePosition), Vector2.zero);
			if(hit){
				if (hit.transform.gameObject.tag == "panel") {
					OnTouchPanel (hit.transform.gameObject);
				}
			}
		}
	}
//Set panels to panelArray[] at random (OK)
	private void SetArray(int x1,int y1,int x2,int y2){
		int temp;
		if (x1 > x2) {
			temp = x1;
			x1 = x2;
			x2 = temp;
		}
		if (y1 > y2) {
			temp = y1;
			y1 = y2;
			y2 = temp;
		}
		System.DateTime dt = System.DateTime.Now;
		Random.seed = dt.Second + dt.Millisecond;
		for (int i=x1; i<=x2; i++) {
			for (int j =y1; j<=y2; j++) {
				panelArray[i,j] = 
					(GameObject) Instantiate (panelPrefab,new Vector3(i*1.25f-3.75f,0.25f-j*1.25f,0),Quaternion.identity);
				panel = panelArray[i,j];
				panelPrp = panel.GetComponent<Panel>();
				panelPrp.panelType = ((int)(100*Random.value)) % 3;
				panelPrp.x = i;
				panelPrp.y = j;
				panelTexture = panel.GetComponent<SpriteRenderer>();
				panelTexture.sprite = panelSprites [panelPrp.panelType];
			}
		}
	}
//Destroy panels (OK)
	private void DestroyPanel(int x1, int y1, int x2, int y2){
		int temp,numOfPanels;
		if (x1 > x2) {
			temp = x1;
			x1 = x2;
			x2 = temp;
		}
		if (y1 > y2) {
			temp = y1;
			y1 = y2;
			y2 = temp;
		}
		numOfPanels = (x2 - x1 + 1) * (y2 - y1 + 1);
		panelPrp = panelArray [x1, y1].GetComponent<Panel> ();
		currentScore += (numOfPanels * (totalScoreBuff + numOfPanels)) / 10;
		currentPanelScore [panelPrp.panelType] += (numOfPanels * totalPanelBuff [panelPrp.panelType]) / 100;
		scoreText.text = currentScore.ToString ("###0");
		panelScoreText [panelPrp.panelType].text = currentPanelScore [panelPrp.panelType].ToString ("###0");
		for (int i = x1; i <= x2; i++) {
			for (int j = y1; j <= y2; j++) {
				Destroy (panelArray [i, j]);
			}
		}
	}
//Change the type of panel (OK)
	private void ExchangePanel(int x1,int y1,int x2,int y2, int type){
		int temp;
		if (x1 > x2) {
			temp = x1;
			x1 = x2;
			x2 = temp;
		}
		if (y1 > y2) {
			temp = y1;
			y1 = y2;
			y2 = temp;
		}
		for (int i = x1; i <= x2; i++) {
			for (int j = y1; j <= y2; j++) {
				panelPrp = panelArray [i, j].GetComponent<Panel> ();
				panelPrp.panelType = type;
				panelTexture = panelArray[i,j].GetComponent<SpriteRenderer> ();
				panelTexture.sprite = panelSprites [panelPrp.panelType];
			}
		}
	}
//Control when player touch panel(OK)
	private void OnTouchPanel(GameObject obj){
		panelPrp = obj.GetComponent<Panel> ();
		if (touchCount == 0) {
			ChangeColor (obj, 0.7f);
			touchColor = panelPrp.panelType;
			touchedPanel[0] = obj;
			touchedPanelPrp[0] = panelPrp;
			touchCount++;
		} else if (touchCount == 1) {
			if (touchColor == panelPrp.panelType) {
				if ((!(panelPrp.x == touchedPanelPrp[0].x)) && (!(panelPrp.y == touchedPanelPrp[0].y))){
					StartCoroutine(CheckCorner(touchedPanelPrp[0].x,touchedPanelPrp[0].y,panelPrp.x,panelPrp.y,touchColor));
				}else if((panelPrp.x == touchedPanelPrp[0].x)
				         && (panelPrp.y == touchedPanelPrp[0].y)){
					ResetTouchedPanel();
				}else{
					ChangeColor(obj, 0.7f);
					touchedPanel[1] = obj;
					touchedPanelPrp[1] = panelPrp;
					touchCount++;
				}
			}else{
				ResetTouchedPanel();
			}
		} else if (touchCount == 2) {
			if (touchColor == panelPrp.panelType){
				if((!(panelPrp.x == touchedPanelPrp[0].x) && !(panelPrp.y == touchedPanelPrp[0].y))
				         &&((panelPrp.x == touchedPanelPrp[1].x) || (panelPrp.y == touchedPanelPrp[1].y))){
					StartCoroutine(CheckCorner(touchedPanelPrp[0].x,touchedPanelPrp[0].y,panelPrp.x,panelPrp.y,touchColor));
				}else if((!(panelPrp.x == touchedPanelPrp[1].x) && !(panelPrp.y == touchedPanelPrp[1].y))
				         &&((panelPrp.x == touchedPanelPrp[0].x) || (panelPrp.y == touchedPanelPrp[0].y))){
					StartCoroutine(CheckCorner(touchedPanelPrp[1].x,touchedPanelPrp[1].y,panelPrp.x,panelPrp.y,touchColor));
				}else{
					ResetTouchedPanel();
				}
			}else{
				ResetTouchedPanel();
			}
		}
	}
//Check touched panel and change,destroy,set (OK) 
	private IEnumerator CheckCorner(int x1,int y1,int x2,int y2,int type){
		if ((panelArray [x1, y2].GetComponent<Panel> ().panelType == type) 
			&& (panelArray [x2, y1].GetComponent<Panel> ().panelType == type)) {
				ExchangePanel(x1,y1,x2,y2,type);
				yield return new WaitForSeconds(0.1f);
				DestroyPanel(x1,y1,x2,y2);
				SetArray(x1,y1,x2,y2);
		} 
		ResetTouchedPanel ();
	}
//Reset touched panels (OK)
	private void ResetTouchedPanel(){
		touchCount = 0;
		foreach(GameObject touched in touchedPanel){
			ChangeColor(touched,1);
		}
		for(int i = 0; i < 2; i++){
			touchedPanel[i] = dummyPanel;
		}
	}
//Change the transparency of obj (OK)
	private void ChangeColor(GameObject obj, float transparency){
		panelTexture = obj.GetComponent<SpriteRenderer>();
		Color pcolor = panelTexture.color;
		pcolor.a = transparency;
		panelTexture.color = pcolor;
	}
//CountDown of Start (OK)
	private IEnumerator CountDown(){
		for(int count = countTime; count > 0; count--) {
			timerText.text = count.ToString();
			yield return new WaitForSeconds (1);
		}
		timerText.text = "Start!";
		isPlaying = true;
		yield return new WaitForSeconds (1);
		StartCoroutine("StartTimer");
	}
//Print Timer (OK)
	private IEnumerator StartTimer(){
		float count = timeLimit;
		while (count > 0) {
			timerText.text = count.ToString("#0.0");
			yield return new WaitForSeconds(0.1f);
			count -= 0.1f;
		}
		timerText.text = "Finish";
		isPlaying = false;
		sysPrp.resultScore = currentScore;
		sysPrp.resultPanel = currentPanelScore;
		if ((currentScore >= sysPrp.normaScore)
			&& (currentPanelScore [0] >= sysPrp.normaPanel [0])
			&& (currentPanelScore [1] >= sysPrp.normaPanel [1])
			&& (currentPanelScore [2] >= sysPrp.normaPanel [2])) {
			sysPrp.saveStageFlag = true;
			sysPrp.gameClearFlag = true;
		}
		yield return new WaitForSeconds(2);
		SceneManager.LoadScene ("Result");
	}
//Pause for Menu and Skill (OK)
	public void Pause(){
		isPlaying = false;
		Time.timeScale = 0;
	}
//Restart from pause (OK)
	public void Restart(){
		isPlaying = true;
		Time.timeScale = 1;
	}
}
