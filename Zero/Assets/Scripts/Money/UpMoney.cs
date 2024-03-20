using UnityEngine;
using System.Collections;

public class UpMoney : MonoBehaviour {

	public int upMoney;

	private float time;
	private bool up;
	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
		if (up == true) {
			time += 0.1f;
			if (time >= 2) {
				Destroy (gameObject);
			}
		}
	}
	void OnTriggerEnter2D(Collider2D col)
	{
		if (col.tag == "Player") {
			col.GetComponent<MoneyIvent> ().Money += upMoney;
			gameObject.GetComponent<AudioSource> ().enabled = true;
			up = true;
			gameObject.SetActive(false);
		}
	}
}