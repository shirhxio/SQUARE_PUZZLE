using UnityEngine;
using System.Collections;


public class Hand : MonoBehaviour {

	private Vector3 pos;			// カーソルの指し示す位置
	private Vector3 startPos;		// カーソル移動の始点
	private Vector3 endPos;			// カーソル移動の終点
	private float moveTimer = 0;	// カーソル移動のタイマー
	private float swingTimer = 0;	// 上下運動の周期を制御するためのタイマー

	private bool isMove = false;	// カーソル移動中か否か
	[SerializeField, Header("SerializeFields")]
	private bool isSwing = true;	// 上下運動中か否か

	[SerializeField, Range(0, 5)]
	private float moveTime = 1.0f;		// カーソル移動にかかる時間
	[SerializeField]
	private float swingWidth = 0.6f;	// 上下の動きの幅
	[SerializeField]
	private float swingPeriod = 1.0f;	// 上下の動きの周期(s)


	void Start () {
		pos = transform.position;	// 初期状態ではposはtransform.positionとする
	}

	void Update () {
		// カーソル移動時
		if (isMove) {
			moveTimer += Time.deltaTime;
			if (moveTimer >= moveTime) {
				transform.position = endPos;
				pos = endPos;
				isMove = false;
			} else {
				transform.position = Vector3.Lerp (startPos, endPos, moveTimer / moveTime);
			}
		}

		// 上下運動時
		if (isSwing && !isMove) {
			// swingTimerによる周期制御
			swingTimer += Time.deltaTime;
			if (swingTimer > swingPeriod) {
				swingTimer = 0;
			}

			// 幅swingWidth、周期swingPeriodで上下運動(cos)する
			transform.position = pos + (swingWidth * (Mathf.Cos (2 * Mathf.PI * swingTimer / swingPeriod) - 1) / 2) * transform.up;
		}
	}

	// 一気に位置を変えるときはSetPos()を使う
	public void SetPos(float x, float y){
		pos.x = x;
		pos.y = y;
	}

	// handcursorをendへと移動させる
	public void MoveHand(Vector3 end){
		StartCoroutine ("SetMove",end);
	}
	// 上下運動を止め、現在の位置からendへの移動を開始させる
	IEnumerator SetMove(Vector3 end){
		yield return StartCoroutine ("StopSwing");	// まずは上下運動を止める
		startPos = transform.position;
		endPos = end;
		moveTimer = 0;
		isMove = true;
	}

	// handcursorを上下運動させる
	public void SwingHand(){
		StartCoroutine ("SetSwing");
	}
	// カーソル移動が終わり次第、上下運動を開始させる
	IEnumerator SetSwing(){
		if (!isSwing) {					// すでに上下運動してたら無視
			while (isMove) {			// 移動中ならば移動し終えるまで待つ
				yield return null;
			}
			transform.position = pos;
			swingTimer = 0;
			isSwing = true;
		}
	}

	// 上下運動を止めて、posを指差す
	public void PointHand(){
		StartCoroutine ("StopSwing");
	}
	// 次にposの位置に来るのを待ち、上下運動を止める
	IEnumerator StopSwing(){
		if (isSwing) {
			while (swingTimer != 0) {
				yield return null;
			}
			transform.position = pos;
			isSwing = false;
		}
	}
}
