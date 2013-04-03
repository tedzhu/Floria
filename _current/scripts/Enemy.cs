#pragma warning disable 0168 // variable declared but not used.
#pragma warning disable 0219 // variable assigned but not used.
#pragma warning disable 0414 // private field assigned but not used.
using UnityEngine;
using System.Collections;

public class Enemy : MonoBehaviour
{
	Global g;
	GameObject go0;

	[System.NonSerializedAttribute]
	public float attackDistance = 0.4f;
	[System.NonSerializedAttribute]
	public bool hit;
	[System.NonSerializedAttribute]
	public float hitTime = 0.5f;
	[System.NonSerializedAttribute]
	public float moveSpeed = 3f;
	
	public AudioClip[] scream;
			
	void Start ()
	{
		go0 = GameObject.Find ("0");
		g = go0.GetComponent<Global> ();
		
	}
		
	void Update ()
	{
		// animation
		if(tag!= "DeadEnemy"){
			transform.rotation=Quaternion.Euler(0,0,Mathf.Sin (Time.time*10)*4);	
		}
		
		if (!hit) {
			//transform.LookAt (g.goGirl.transform.position, Vector3.forward);
			Vector3 vEnemyGirl = g.goGirl.transform.position - transform.position;
			//g.debug (vEnemyGirl.magnitude.ToString ());
			if (vEnemyGirl.magnitude < attackDistance) {
				hitGirl();
			} else {		
				transform.position += vEnemyGirl.normalized * moveSpeed * Time.deltaTime;
							
			}
		}		
	}
	
	public void onHitBlade ()
	{
		AudioSource.PlayClipAtPoint(scream[Random.Range(0,scream.Length)],Vector3.zero,0.6f);		
		StartCoroutine (ieHitDestroy ("ps2"));		
	}
	
	void hitGirl()
	{	
		g.girl.onHit();
		StartCoroutine (ieHitDestroy ("ps1"));
	}
	
	IEnumerator ieHitDestroy (string psName)
	{	
		gameObject.tag = "DeadEnemy";			
		hit = true;		
		ParticleEmitter pe = transform.Find (psName).gameObject.particleEmitter;
		pe.Emit ();
		pe.emit = false;		
		
		yield return new WaitForSeconds(0.2f);
				
		float alpha=1f;
		while(true){
			alpha-=0.1f;
			Renderer[] rs=GetComponentsInChildren<Renderer>();
			foreach(Renderer r in rs){
				if(r.gameObject.name=="cute_t"){
					Color cl=r.material.color;
					cl.a=alpha;
					r.material.color=cl;
				}
			}
			if(alpha<0)break;
			yield return new WaitForSeconds(0.02f);			
		}
		yield return new WaitForSeconds(1f);
		Destroy (gameObject);
	}
			
//	void Fade ()
//	{
//		if (shouldFade) {
//			t = Time.time / endTime;
//			currentColor = Color.Lerp (startColor, endColor, t);         
//			gameObject.renderer.material.SetColor ("_Color", currentColor);          
//
//			if (currentColor == endColor) {
//				shouldFade = false;
//				startTime = 0.0f;
//				endTime = 0.0f;
//				t = 0.0f;
//			}
//		}
//	}
	
	void OnTriggerEnter (Collider e)
	{
		print (e.name);
	}
			
}
