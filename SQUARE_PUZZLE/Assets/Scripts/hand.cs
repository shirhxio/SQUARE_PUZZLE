using UnityEngine;
using System.Collections;


public class hand : MonoBehaviour {
	public float width = 0.6f;	//上下の動きの幅
	public float period = 1.0f;	//上下の動きの周期(s)

	private Vector2 offset = new Vector2 (0.1f, -0.6f);	//カーソルの指先をposの位置に持ってくるためのoffset
	private Vector3 pos;		//カーソルの指し示す位置
	private float count = 0;	//周期を制御するためのタイムカウンター

	void Start () {
		pos = transform.position;	//初期状態ではposはtransform.positionとする
	}

	void Update () {
		//Y軸上を幅width、周期periodで上下運動(sin)する
		transform.position = new Vector3 (pos.x + offset.x, pos.y + offset.y + (width * (Mathf.Sin(2 * Mathf.PI * count / period) -1) / 2), pos.z);

		//タイムカウンターによる周期制御
		count += Time.deltaTime;
		if (count > period) {
			count = 0;
		}
	}

	//位置を変えるときはSetPos()を使いましょう。
	public void SetPos(float x, float y){
		pos.x = x;
		pos.y = y;
	}

}
