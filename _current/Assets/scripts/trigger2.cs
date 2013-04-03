#pragma warning disable 0168 // variable declared but not used.
#pragma warning disable 0219 // variable assigned but not used.
#pragma warning disable 0414 // private field assigned but not used.

using UnityEngine;
using System.Collections;

public class trigger2 : MonoBehaviour {
	Global g;
	GameObject go0;
	
	public Light directionLight;
	
	public ParticleSystem parEnterSmall;
	public ParticleSystem parEnterBig;
	
	private bool bubbleStart = false;
	public bool once=true;
	bool hit=false;	
	public AudioClip acFallIn;
	
	void Start () {
		go0=GameObject.Find("0");
		g=go0.GetComponent<Global>();
		
	}
	
	void OnTriggerEnter (Collider e)
	{
		if(hit && once)
			return;
		hit=true;	
		
		Time.timeScale = 1.0f;
		directionLight.intensity = 0.4f;
		
		g.girl.anim.PlayQueued("enterDown");
		g.girl.anim.PlayQueued("enterUp");
		g.girl.anim.PlayQueued("enterFinish");
		
		iTween.MoveTo(g.goGirl, iTween.Hash("path", iTweenPath.GetPath("path2"), "time", 3.2, "easeType", iTween.EaseType.linear));
		StartCoroutine("EnterWater");	
		
	}
	
	IEnumerator EnterWater()
	{
		parEnterSmall.Play();
		parEnterBig.Play();
		bubbleStart = true;
		
		AudioSource.PlayClipAtPoint(acFallIn,Vector3.zero);
		
		yield return new WaitForSeconds(3.2f);
		g.girl.onEnterLake();
		
		g.girl.bladeControlPos = true;
		g.blade.showBlade=true;
		g.cameraControl.snap=false;
	}
	
	void Update () {
	
		if( bubbleStart == true )
		{
			parEnterSmall.transform.position = g.girl.transform.position - new Vector3(0.0f, 2.0f, 0.0f);
		}
	}
	
}