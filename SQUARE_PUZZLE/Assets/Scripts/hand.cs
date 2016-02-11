using UnityEngine;
using System.Collections;

public class hand : MonoBehaviour {
	public float width = 0.6f;
	private float period = 1.0f;

	private Vector3 pos;
	private float count = 0;

	void Start () {
		pos = transform.position;
	}

	void Update () {
		transform.position = new Vector3 (pos.x + 0.1f, pos.y - 0.6f + (width * Mathf.Sin(2 * Mathf.PI * count / period) / 2 - width / 2), 0);
		if (count > period) {
			count = 0;
		}
		count += Time.deltaTime;
	}

	public void SetPos(float x, float y){
		pos.x = x;
		pos.y = y;
	}

}
