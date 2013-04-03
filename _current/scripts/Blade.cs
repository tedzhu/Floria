#pragma warning disable 0168 // variable declared but not used.
#pragma warning disable 0219 // variable assigned but not used.
#pragma warning disable 0414 // private field assigned but not used.
using UnityEngine;
using System.Collections;

public class Blade : MonoBehaviour
{
	GameObject go0;
	Global g;
	PSMoveWrapper psmw;
	
	public int moveId;
	public float bladeRange = 2;
	float minSpeed = 1f;
		
	[System.NonSerializedAttribute]	public bool showBlade=true;
	[System.NonSerializedAttribute]	public float rumbleStopTime;
	bool isMoveGood=false;
	bool pressing;
	bool lastPressing;
	Vector3 lastWorldPos;
	
	float xmin = -15f;
	float xmax = 15f;
	float ymin = -2.3f;
	float ymax = 13f;
	bool calibrating = false;
	
	public AudioClip acStartGather;
	
	void Start ()
	{
		go0 = GameObject.Find ("0");
		g = go0.GetComponent<Global> ();
		psmw = go0.GetComponent<PSMoveWrapper> ();		

		rumbleStopTime = Mathf.Infinity;
		Vector3 v=Camera.main.transform.position;
		v.z=g.girl.transform.position.z;
		transform.position=v;
	}				
		
	void FixedUpdate ()
	{
		transform.Rotate (Vector3.down, 2f);
		renderer.enabled=showBlade;
		GetComponentInChildren<Light>().enabled=showBlade;
	}

	void Update ()
	{
		if (Input.GetKeyDown (KeyCode.C)) {
			psmw.Connect ();
		}
		
		//GameObject.Find ("_connect").guiText.text = psmw.isConnected ? "connected" : "not connected";
		
		isMoveGood = psmw && psmw.isConnected && psmw.moveConnected [moveId];
		
		//  Sphere Visible?
		if (isMoveGood) {
		//	GameObject.Find ("_visible").guiText.text = psmw.sphereVisible [moveId] ? "visible" : "not visible";
		}
		
		// Calibration
		if (isMoveGood) {
			if (psmw.WasPressed (moveId, PSMoveWrapper.CIRCLE)) {
				xmin=ymin=100;
				xmax=ymax=-100;
				calibrating = true;				
			}
			if (psmw.isButtonCircle [moveId]) {
				calibrating = false;
				Vector3 movePos = psmw.position [moveId];
				if (movePos.x < xmin)
					xmin = movePos.x;
				if (movePos.x > xmax)
					xmax = movePos.x;
				if (movePos.y < ymin)
					ymin = movePos.y;
				if (movePos.y > ymax)
					ymax = movePos.y;
				g.main.lowerleft( "PS Move calibrating: ([ " + xmin + " , " + xmax + " ] , [ " + ymin + " , " + ymax+"])",1f);
			}else{
				calibrating = false;
			}
		}						
		if (calibrating)
			return;
		
				
		/*
		 * z=10,x=-6 ~ 6, y=-4.5 ~ 4.5
		 */
		
		if(isMoveGood){
			if (psmw.WasPressed (moveId, PSMoveWrapper.CROSS)) {
				g.girl.gather();
			}
		}
		
		// Get blade world pos and click
		// mouse or psmove
		Vector3 worldPos = lastWorldPos;
		if (g.main.PSMoveOn) {		
			if (isMoveGood && !calibrating) {
				Vector3 movePos = psmw.position [moveId];
				pressing=psmw.valueT[moveId]>=psmw.thresholdT;
				
				Vector3 viewportPos = new Vector3 ();
				viewportPos.x = (movePos.x - xmin) / (xmax - xmin);
				viewportPos.y = (movePos.y - ymin) / (ymax - ymin);
				viewportPos.z = -Camera.main.transform.position.z;
				worldPos = Camera.main.ViewportToWorldPoint (viewportPos);								
			}			
		}else{
			Vector3 mousePos = Input.mousePosition;
			mousePos.z = -Camera.main.transform.position.z+g.girl.transform.position.z;
			worldPos = Camera.main.ScreenToWorldPoint (mousePos);
			pressing=Input.GetMouseButton(0);					
		}
		worldPos.z=g.girl.transform.position.z;
		
		// ======= below is same for psmove and mouse ======= //
		
		// detect slice		
		Vector3 dvPos = worldPos - lastWorldPos;		
		if(pressing && !lastPressing){
			AudioSource.PlayClipAtPoint(acStartGather,Vector3.zero);
		}
		if(pressing){
			if (dvPos.magnitude > minSpeed) {
				g.girl.onAttack();
				rumbleStopTime=Time.time+0.3f;
			}
		}
		
		// psmove rumble
		if (Time.time > rumbleStopTime) {
			rumbleStopTime = Mathf.Infinity;
			psmw.SetRumble (moveId, 0);
		}
		
		// pressing FX
		TrailRenderer tr=GetComponentInChildren<TrailRenderer>();
		tr.enabled=pressing;
		ParticleEmitter pe=GetComponentInChildren<ParticleEmitter>();
		pe.emit=pressing;
		
		// 
		transform.position = worldPos;
		lastWorldPos=worldPos;		
		lastPressing=pressing;
	}
	
	public void rumble(int level, float d)
	{
		if(isMoveGood){
			psmw.SetRumble(moveId,level);
			rumbleStopTime=Time.time+d;
		}
	}
}
