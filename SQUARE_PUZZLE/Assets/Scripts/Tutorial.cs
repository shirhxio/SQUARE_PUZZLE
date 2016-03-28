using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using UnityEngine.SceneManagement;

/*
	Scene'Main'の制御であるPuzzleAreaとだいたい同じ
	パズルエリアの操作認識やパネルの生成・破壊、四角形の判定などを行う。
	スコア、時間の制御なども行っている。
	大きく分けて、
	・受け取った操作を処理するなどのゲーム制御関連
	・パネルをセットしたり、消したりするパネル制御関連
	・TimeLimitなどの時間制御関連
	の３つがある。
*/
public class Tutorial : Datas {
	private const int panel_Row = 7;		// panelAreaの行数
	private const int panel_Column = 7;		// panelAreaの列数
	private GameObject[,] panelArray = new GameObject[panel_Row,panel_Column];	// panelを保持しておく配列

	private GameObject[] touchedPanel = new GameObject[2];	// タッチされたパネルを保持しておく配列（最大３つまでタッチできるので２つ保持できるようにしておく）
	private int touchCount = 0;				// 現在、いくつのpanelがタッチされたか数えておくカウンター
	private int touchColor;					// 現在、タッチされているpanelの色
	private float unforcusedAlfa = 0.5f;	// タッチされているパネルと違うpanelTypeのパネルのα値
	private bool isChangedAlfa = false;		// 現在、パネルのα値が下げられているか否か

	private int totalScoreBuff = 100;						// スコアの倍率(%) キャラのバフで増減
	private int[] totalPanelBuff = new int[numOfType];		// パネルスコアの倍率(%) キャラのバフで増減

	private int currentScore;				// 現在のスコアを保持
	private int[] currentPanelScore = new int[numOfType];	// 現在のパネルスコアを保持
	private Text scoreText;					// スコアのテキストコンポーネント
	private Text[] panelScoreText = new Text[numOfType];	// パネルスコアのテキストコンポーネント

	private int countTime = 3; 				// スタート時のカウントダウンの時間
	private float timeLimit = 30.0f;		// ゲームのタイムリミット

	// panelなどのゲームオブジェクトやプレハブ(inspectorから登録)
	[SerializeField]
	private GameObject panelPrefab;
	[SerializeField]
	private GameObject dummyPanel;
	[SerializeField]
	private GameObject[] touchedPanelBack = new GameObject[2];
	[SerializeField]
	private GameObject handcursor;		// Tutorial特有 handcursorのプレハブ
	[SerializeField]
	private GameObject countdownObj;
	[SerializeField]
	private GameObject timerObj;
	[SerializeField]
	private GameObject scoreObj;
	[SerializeField]
	private GameObject[] panelScoreObj = new GameObject[numOfType];
	[SerializeField]
	private Sprite[] panelSprites;
	public GameObject[] partyMember = new GameObject[partySize];
	public GameObject[] attackEffectPrefab = new GameObject[numOfType];

	private GameObject sys;					// Systems参照用
	private Systems sysPrp;					// SystemsのSystemsコンポーネント参照用
	private Panel panelPrp;					// panelのPanelコンポーネント参照用
	private Panel[] touchedPanelPrp = new Panel[2];	// タッチされたpanelのPanelコンポーネントを保持
	private SpriteRenderer panelTexture;	// panelを消す時やα値を変更する時のSpriteRendererコンポーネント参照用
	private GameObject panel;				// panel参照用
	private Text countdownText;				// カウントダウンタイマーのTextコンポーネント参照用
	private Text timerText;					// タイマーのTextコンポーネント参照用
	private Ray2D ray;						// 画面タッチ用のRay
	private RaycastHit2D hit;				// 画面タッチ用のRaycastHit
	private bool isPlaying = false;			// ゲームプレイ中か否か
	private int layerMask = 3 << 8;			// PanelとCharaのレイヤーマスク(タッチした時に別のレイヤーのコライダーに当たらないようにするため)

	// 様々な値の初期化やsystemへの参照を設定する
	void Start () {
		// Systems系(scene間の値受け渡し用)の参照、初期化
		sys = GameObject.Find ("Systems");
		sysPrp = sys.GetComponent<Systems> ();

		sysPrp.gameClearFlag = false;

		// スコア倍率、パネルスコア倍率の初期化・計算
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

		// 各スコアの初期化、表示(Textコンポーネント)の参照
		currentScore = 0;
		scoreText = scoreObj.GetComponent<Text> ();
		scoreText.text = currentScore.ToString ("###0");
		for (int i = 0; i < numOfType; i++) {
			currentPanelScore [i] = 0;
			panelScoreText[i] = panelScoreObj[i].GetComponent<Text>();
			panelScoreText[i].text = currentPanelScore[i].ToString("###0");
		}

		// カウントダウンタイマー・タイマーの初期化、表示(Textコンポーネント)の参照
		countdownObj.SetActive (true);
		countdownText = countdownObj.GetComponent<Text> ();
		timerText = timerObj.GetComponent<Text> ();
		timerText.text = timeLimit.ToString("#0.0");

		// touchedPanel配列をdummyPanelで初期化
		for(int i = 0; i < 2; i++){
			touchedPanel[i] = dummyPanel;
		}
		// panelArrayをセット
		SetArray (0, 0, panel_Row - 1, panel_Column - 1);

		// スタート時のカウントダウンを開始
		StartCoroutine("CountDown");
	}

	// プレイヤーの操作を受け取る
	void Update () {
		if (isPlaying && Input.GetMouseButtonDown (0)) {
			hit = Physics2D.Raycast (Camera.main.ScreenToWorldPoint (Input.mousePosition), Vector2.zero, 10, layerMask);
			if(hit){
				if (hit.transform.gameObject.tag == "panel") {
					OnTouchPanel (hit.transform.gameObject);
				}
			}
		}
	}

	/*	Androidとかでやる場合はmousePositionでやるよりはtouchを使う
	void Update () {
		if (isPlaying && Input.GetTouch(0).phase == TouchPhase.Began) {
			hit = Physics2D.Raycast (Camera.main.ScreenToWorldPoint (Input.GetTouch(0).position), Vector2.zero, 10, layerMask);
			if(hit){
				if (hit.transform.gameObject.tag == "panel") {
					OnTouchPanel (hit.transform.gameObject);
				}
			}
		}
	}
*/

/* 	タッチ操作があった時のゲーム制御関連	*/

	// プレイヤーがパネルをタッチした時の制御をする (OK)
	private void OnTouchPanel(GameObject obj){
		panelPrp = obj.GetComponent<Panel> ();

		// タッチされたパネルが１つ目のとき
		if (touchCount == 0) {
			// タッチされたパネルがわかるようにtouchedPanelBackをpanelの後ろに配置
			touchedPanelBack [0].transform.position = obj.transform.position;
			// タッチされたパネルの情報をtouchedPanel[]などに保持
			touchColor = panelPrp.panelType;
			touchedPanel [0] = obj;
			touchedPanelPrp [0] = panelPrp;

			// panelすべてのうち、タッチされたパネルのpanelTypeと違うもののα値をunforcusedAlfaに変更
			for (int i = 0; i < panel_Row; i++) {
				for (int j = 0; j < panel_Column; j++) {
					if (panelArray [i, j].GetComponent<Panel> ().panelType != touchColor) {
						ChangeAlfa (panelArray [i, j], unforcusedAlfa);
					}
				}
			}
			isChangedAlfa = true;

			touchCount++;
		} 
		// タッチされたパネルが２つ目で、panelTypeが正しいとき
		else if (touchCount == 1 && touchColor == panelPrp.panelType) {
			// 一つ目のパネルとタッチされたパネルのx、yともに違うとき(四角形の対角のとき)
			if ((!(panelPrp.x == touchedPanelPrp [0].x)) && (!(panelPrp.y == touchedPanelPrp [0].y))) {
				StartCoroutine (CheckCorner (touchedPanelPrp [0].x, touchedPanelPrp [0].y, panelPrp.x, panelPrp.y, touchColor));
			}
			// タッチされたパネルが一つ目のパネルと同一のとき
			else if ((panelPrp.x == touchedPanelPrp [0].x) && (panelPrp.y == touchedPanelPrp [0].y)) {
				ResetTouchedPanel ();
			}
			// 一つ目のパネルとタッチされたパネルのxまたはyが同じとき(直線上にあるとき)
			else {
				// タッチされたパネルがわかるようにtouchedPanelBackをpanelの後ろに配置
				touchedPanelBack [1].transform.position = obj.transform.position;
				// タッチされたパネルの情報をtouchedPanel[]などに保持
				touchedPanel [1] = obj;
				touchedPanelPrp [1] = panelPrp;

				touchCount++;
			}
		} 
		// タッチされたパネルが３つ目で、panelTypeが正しいとき
		else if (touchCount == 2 && touchColor == panelPrp.panelType) {
			// 一つ目のパネルとタッチされたパネルが四角形の対角となるとき
			if ((!(panelPrp.x == touchedPanelPrp [0].x) && !(panelPrp.y == touchedPanelPrp [0].y))
				&& ((panelPrp.x == touchedPanelPrp [1].x) || (panelPrp.y == touchedPanelPrp [1].y))) {
				StartCoroutine (CheckCorner (touchedPanelPrp [0].x, touchedPanelPrp [0].y, panelPrp.x, panelPrp.y, touchColor));
			} 
			// 二つ目のパネルとタッチされたパネルが四角形の対角となるとき
			else if ((!(panelPrp.x == touchedPanelPrp [1].x) && !(panelPrp.y == touchedPanelPrp [1].y))
				&& ((panelPrp.x == touchedPanelPrp [0].x) || (panelPrp.y == touchedPanelPrp [0].y))) {
				StartCoroutine (CheckCorner (touchedPanelPrp [1].x, touchedPanelPrp [1].y, panelPrp.x, panelPrp.y, touchColor));
			} 
			// 四角形が成立しないとき
			else {
				ResetTouchedPanel ();
			}
		}
		// タッチされたパネルが２つ目以降でpanelTypeが正しくないとき
		else {
			ResetTouchedPanel ();
		}
	}

	// 引数に与えられた対角の四角形の四隅が同じ色かを判定する (OK) 
	private IEnumerator CheckCorner(int x1,int y1,int x2,int y2,int type){
		if ((panelArray [x1, y2].GetComponent<Panel> ().panelType == type) 
			&& (panelArray [x2, y1].GetComponent<Panel> ().panelType == type)) {
			ResetAlfas ();
			ExchangeArray(x1,y1,x2,y2,type);
			yield return new WaitForSeconds(0.1f);
			DestroyArray(x1,y1,x2,y2);
			SetArray(x1,y1,x2,y2);
		} 
		ResetTouchedPanel ();
	}

/*	panelをセットしたり、消したり、色を変えたりするpanel操作関連		*/

	// panelArray[]の(x1, y1)から(x2, y2)のエリアにパネルをセットする (OK)
	// x1 <= x2, y1 <= y2である必要はない
	private void SetArray(int x1, int y1, int x2, int y2){
		// (x1, y1)を左上、(x2, y2)を右下になるように入れ替え
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

		// 毎回ランダムの値が異なるようにseedを設定
		System.DateTime dt = System.DateTime.Now;
		Random.seed = dt.Second + dt.Millisecond;

		// panelArray[]にパネルをセット
		for (int i = x1; i <= x2; i++) {
			for (int j = y1; j <= y2; j++) {
				SetPanel (i, j);
			}
		}
	}

	// panelArray[x,y]にパネルをセットする (OK)
	private void SetPanel(int x, int y){
		panelArray[x, y] = 
			(GameObject) Instantiate (panelPrefab, new Vector3(x * 1.25f - 3.75f, 0.25f - y * 1.25f, 0), Quaternion.identity);
		panel = panelArray[x, y];

		// panel情報の設定
		panelPrp = panel.GetComponent<Panel>();
		panelPrp.panelType = ((int)(100 * Random.value)) % 3;
		panelPrp.x = x;
		panelPrp.y = y;
		// panelの画像の変更
		panelTexture = panel.GetComponent<SpriteRenderer>();
		panelTexture.sprite = panelSprites [panelPrp.panelType];
	}

	// panelArray[]の(x1, y1)から(x2, y2)のエリアのpanelを消し、スコアを加算、アタックエフェクトの生成する (OK)
	// x1 <= x2, y1 <= y2である必要はない
	private void DestroyArray(int x1, int y1, int x2, int y2){
		// (x1, y1)を左上、(x2, y2)を右下に入れ替え
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

		// スコアを消したパネルのスコアを加算、表示の更新
		numOfPanels = (x2 - x1 + 1) * (y2 - y1 + 1);
		panelPrp = panelArray [x1, y1].GetComponent<Panel> ();
		int type_DP = panelPrp.panelType;
		currentScore += (numOfPanels * (totalScoreBuff + numOfPanels)) / 10;
		currentPanelScore [type_DP] += (numOfPanels * totalPanelBuff [type_DP]) / 100;
		scoreText.text = currentScore.ToString ("###0");
		panelScoreText [type_DP].text = currentPanelScore [type_DP].ToString ("###0");

		// panelArray[]のパネルを消し、アタックエフェクトを生成
		for (int i = x1; i <= x2; i++) {
			for (int j = y1; j <= y2; j++) {
				Instantiate (attackEffectPrefab [type_DP], panelArray [i, j].transform.position, Quaternion.identity);
				Destroy (panelArray [i, j]);
			}
		}
	}

	/*	// panelArray[x, y]のpanelを消し、スコアを加算、アタックエフェクトの生成する
	private void DestroyPanel(int x,int y){
		// スコアを消したパネルのスコアを加算、表示の更新
		panelPrp = panelArray [x, y].GetComponent<Panel> ();
		int type_DP = panelPrp.panelType;
		currentScore += (totalScoreBuff + 1) / 10;
		currentPanelScore [type_DP] += totalPanelBuff [type_DP] / 100;
		scoreText.text = currentScore.ToString ("###0");
		panelScoreText [type_DP].text = currentPanelScore [type_DP].ToString ("###0");

		// panelArray[x, y]のパネルを消し、アタックエフェクトを生成
		Instantiate (attackEffectPrefab [type_DP], panelArray [x, y].transform.position, Quaternion.identity);
		Destroy (panelArray [x, y]);
	}*/

	// panelArray[]の(x1, y1)から(x2, y2)のエリアのpanelType(色、属性)を変更する (OK)
	// x1 <= x2, y1 <= y2である必要はない
	private void ExchangeArray(int x1,int y1,int x2,int y2, int type){
		// (x1, y1)を左上、(x2, y2)を右下に入れ替え
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

		// panelArray[]のpanelTypeを変更
		for (int i = x1; i <= x2; i++) {
			for (int j = y1; j <= y2; j++) {
				ExchangePanel (i, j, type);
			}
		}
	}

	// panelArray[x, y]のpanelType(色、属性)を変更する
	private void ExchangePanel(int x, int y, int type){
		panelPrp = panelArray [x, y].GetComponent<Panel> ();
		panelPrp.panelType = type;
		panelTexture = panelArray[x, y].GetComponent<SpriteRenderer> ();
		panelTexture.sprite = panelSprites [panelPrp.panelType];
	}

	// panelのα値を戻し、touchedPanel配列をリセットする (OK)
	private void ResetTouchedPanel(){
		ResetAlfas ();

		// touchedPanel[]をdummyPanelでリセット
		for(int i = 0; i < 2; i++){
			touchedPanel[i] = dummyPanel;
			touchedPanelBack [i].transform.position = new Vector3 (-8, 0, 0);
		}
		touchCount = 0;
	}

	// objのα値をalfaに変更 (OK)
	private void ChangeAlfa(GameObject obj, float alfa){
		panelTexture = obj.GetComponent<SpriteRenderer>();
		Color pColor = panelTexture.color;
		pColor.a = alfa;
		panelTexture.color = pColor;
	}

	// isChangedAlfa == trueならα値を元に戻す (OK)
	private void ResetAlfas(){
		if (isChangedAlfa) {
			for (int i = 0; i < panel_Row; i++) {
				for (int j = 0; j < panel_Column; j++) {
					ChangeAlfa (panelArray [i, j], 1);
				}
			}
			isChangedAlfa = false;
		}
	}

/*	スタート時のカウントダウンやタイマーなどの時間制御関連	*/

	// スタート時のカウントダウン用コルーチン (OK)
	private IEnumerator CountDown(){
		// 1秒ごとに画面中央のカウントダウンを更新
		for(int count = countTime; count > 0; count--) {
			countdownText.text = count.ToString();
			yield return new WaitForSeconds (1);
		}
		countdownText.text = "START!";				// 現在、この辺りの順番がタイマーは作動してないが操作可能になっている。
		isPlaying = true;							// カウントダウン表示位置を変えるなど工夫して、スタート表示と同時に操作可能にすべき
		yield return new WaitForSeconds (0.5f);
		countdownObj.SetActive (false);
		StartCoroutine("StartTimer");
	}

	// タイマーの制御、表示用コルーチン (OK)
	private IEnumerator StartTimer(){
		float count = timeLimit;

		// 0.1秒ごとにタイマーを更新
		while (count > 0) {
			timerText.text = count.ToString("#0.0");
			yield return new WaitForSeconds(0.1f);
			count -= 0.1f;
		}

		// 時間がなくなったら、タイマーと画面中央に"Finish"と表示
		timerText.text = "Finish";
		countdownObj.SetActive (true);
		countdownText.text = "Finish!";
		// 操作を受け付けないようにし、Systemsに各スコア受け渡し
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

		yield return new WaitForSeconds(1.5f);
		SceneManager.LoadScene ("Result");
	}

	// MENUボックスなどが表示されてるときはポーズする (OK)
	public void Pause(){
		isPlaying = false;
		Time.timeScale = 0;
	}

	// MENUボックスなどを閉じる時はポーズを切り、ゲームを再開する。 (OK)
	public void Restart(){
		isPlaying = true;
		Time.timeScale = 1;
	}
}
