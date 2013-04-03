#pragma warning disable 0168 // variable declared but not used.
#pragma warning disable 0219 // variable assigned but not used.
#pragma warning disable 0414 // private field assigned but not used.

using UnityEngine;
using System.Collections;

public class trigger1 : MonoBehaviour {
	Global g;
	GameObject go0;
	
	public GameObject moon;
	public bool once=true;
	public AudioClip acMoon;
	bool hit=false;
	// Use this for initialization
	void Start () {
		go0=GameObject.Find("0");
		g=go0.GetComponent<Global>();
		
	}
	
	void OnTriggerEnter (Collider e)
	{
		if(hit && once)
			return;
		hit=true;	
		
		g.girl.bladeControlPos = false;
		g.blade.showBlade=false;
		g.cameraControl.snap=true;
		
		
		g.girl.anim.Play("jump");		
		g.girl.anim.PlayQueued("fall").speed=0.22f;
		
		iTween.MoveTo(g.goGirl, iTween.Hash("path", iTweenPath.GetPath("path1"), "time", 10, "easeType", iTween.EaseType.linear));
		
		Time.timeScale *= 0.70f;
		StartCoroutine("Jump");		
	}
	
	IEnumerator Jump()
	{
		yield return new WaitForSeconds(1.5f);
		Time.timeScale = 0.70f;
		g.girl.anim.animation["jump"].speed = 0.7f;
		
		//print("1111");
		
		yield return new WaitForSeconds(0.1f);
		Time.timeScale *= 0.80f;
		g.girl.anim.animation["jump"].speed = 0.5f;
		//print("2222");
		
		yield return new WaitForSeconds(0.1f);
		//put sound effect here
		AudioSource.PlayClipAtPoint(acMoon,Vector3.zero);
		
		moon.animation.Play("animMoonLight");
		Time.timeScale *= 0.75f;
		g.girl.anim.animation["jump"].speed = 0.3f;
		//print("3333");
		
		yield return new WaitForSeconds(0.1f);
		Time.timeScale *= 0.70f;
		g.girl.anim.animation["jump"].speed = 0.2f;
		//print("4444");
		
		yield return new WaitForSeconds(0.1f);
		Time.timeScale *= 0.70f;
		//print("5555");
		
		yield return new WaitForSeconds(0.1f);
		Time.timeScale = 0.35f;
		//print("6666");
		
		yield return new WaitForSeconds(0.1f);
		Time.timeScale = 0.5f;
		//print("7777");
		
		yield return new WaitForSeconds(0.1f);
		Time.timeScale = 0.7f;
		//print("8888");
		
		yield return new WaitForSeconds(0.3f);
		Time.timeScale = 3.0f;
		
		//yield return new WaitForSeconds(0.3f);
		//Time.timeScale = 3.0f;
		
		//yield return new WaitForSeconds(0.3f);
		//Time.timeScale = 2f;
		
	}
}
