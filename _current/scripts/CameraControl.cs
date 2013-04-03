#pragma warning disable 168 // variable declared but not used.
#pragma warning disable 0219 // variable assigned but not used.
#pragma warning disable 0414 // private field assigned but not used.

using UnityEngine;
using System.Collections;

public class CameraControl : MonoBehaviour {
	Global g;
	GameObject go0;
	float shakeStopTime;	
	float maxShake=1f;
	float cameraSpeed=4f;		
	float deltaFollow=2f;
	float deltaY;	
	
	[System.NonSerializedAttribute]	public bool lockScreen=false;
	[System.NonSerializedAttribute]	public bool following=true;
	[System.NonSerializedAttribute]	public float damp=4;
	[System.NonSerializedAttribute]	public bool snap=false;
	[System.NonSerializedAttribute]	public Vector3 targetPos;
	
	public void shake(float shakeTime)
	{
		shakeStopTime=Time.time+shakeTime;
	}
	
	public void shake(float shakeTime, float _maxShake)
	{
		shakeStopTime=Time.time+shakeTime;
		maxShake=_maxShake;
	}
	
	void Start () {
		go0=GameObject.Find("0");
		g=go0.GetComponent<Global>();
		shakeStopTime=Mathf.Infinity;
		targetPos=transform.position;
	}
	
	void Update () {
		Vector3 origPos=transform.position;
		
		// shake
		if(shakeStopTime==Mathf.Infinity){
		}else{
			if(Time.time>shakeStopTime){
				shakeStopTime=Mathf.Infinity;
				transform.rotation=Quaternion.identity;
			}else{							
				float x=Random.Range(-maxShake,maxShake);
				float y=Random.Range(-maxShake,maxShake);
				float z=Random.Range(-maxShake,maxShake);				
				transform.rotation=Quaternion.Euler(x,y,z);
			}		
		}

		// camera follow
		if(following && !lockScreen){		
			Vector3 girlCamPos=g.girl.transform.position;
			girlCamPos.z=targetPos.z;
							
			if(snap){
				targetPos=girlCamPos;
			}else{
				Vector3 vog=girlCamPos-origPos;
				
				if(Mathf.Abs(vog.x)>2f){
					//cameraPos.x=Mathf.Lerp(cameraPos.x,vct.x,damp*Time.deltaTime);
					targetPos.x+=3*vog.x*Time.deltaTime;
				}	
				
				if(Mathf.Abs(vog.y)>2f){
					//cameraPos.x=Mathf.Lerp(cameraPos.x,vct.x,damp*Time.deltaTime);
					targetPos.y+=1*vog.y*Time.deltaTime;
				}
			}
		}
		if(lockScreen)
			damp=0.5f;
		else
			damp=4;
		
		// mouse wheel
		if(Input.GetAxis("Mouse ScrollWheel") < 0){
			targetPos.z-=0.5f;
		}
		if(Input.GetAxis("Mouse ScrollWheel") > 0){
			targetPos.z+=0.5f;
		}		

		// keyboard test
		if(Input.GetKey(KeyCode.E)){
			targetPos+=Vector3.right*cameraSpeed*Time.deltaTime;
		}		
		if(Input.GetKey(KeyCode.Q)){
			targetPos-=Vector3.right*cameraSpeed*Time.deltaTime;
		}
		
		// snap or lerp?
		if(snap){
			transform.position=targetPos;
		}else{
			transform.position=Vector3.Lerp(transform.position,targetPos,damp*Time.deltaTime);
		}
	}
}
