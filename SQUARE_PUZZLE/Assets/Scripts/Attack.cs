using UnityEngine;
using System.Collections;

public class Attack : MonoBehaviour {
	private float speed = 2.0f;
	private Vector3 vec;
	private Vector3 enemyPos = new Vector3 (0, 4.5f, 0);

	void Start () {
		vec = enemyPos - transform.position;
	}

	void Update () {
		transform.position += vec * speed * Time.deltaTime;
	}
}
