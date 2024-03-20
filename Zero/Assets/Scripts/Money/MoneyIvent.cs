using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using TMPro;

public class MoneyIvent : MonoBehaviour {
	public int Money;
	public TMP_Text txt;
	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
		txt.text = "$" + Money;
	}
}
