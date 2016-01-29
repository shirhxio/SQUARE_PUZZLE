using UnityEngine;
using System.Collections;

public class Enemy : MonoBehaviour {
	public GameObject explosionPrefab;
	private int counter = 4;

	void OnTriggerEnter2D(Collider2D c){
		Destroy(c.gameObject);
		counter++;
		if (counter >= 5) {
			Instantiate (explosionPrefab, new Vector3 (-2.5f + Random.value * 5, 3.0f + Random.value * 3, 0), Quaternion.identity);
			counter = 0;
		}
	}
}
