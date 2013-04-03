#pragma warning disable 0168 // variable declared but not used.
#pragma warning disable 0219 // variable assigned but not used.
#pragma warning disable 0414 // private field assigned but not used.

using UnityEngine;
using System.Collections;

public class Trigger : MonoBehaviour {
	Global g;
	GameObject go0;
	public bool once=true;
	bool hit=false;
	void Start () {
		go0=GameObject.Find("0");
		g=go0.GetComponent<Global>();	
	}
	
	void OnTriggerEnter (Collider e)
	{
		if(hit && once)
			return;
		hit=true;
		StartCoroutine(g.main.onTrigger(name));
	}
}
