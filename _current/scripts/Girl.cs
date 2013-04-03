#pragma warning disable 0168 // variable declared but not used.
#pragma warning disable 0219 // variable assigned but not used.
#pragma warning disable 0414 // private field assigned but not used.

using UnityEngine;
using System.Collections;

public class Girl : MonoBehaviour {
	Global g;
	GameObject go0;
	string modelName="Fairy_animation_in place_ver 01";
	GameObject goAnim;	
	[System.NonSerialized] public Animation anim;		
	[System.NonSerialized] public fxFairyEnergy fxFE;
	AnimationState currentAnim;
	float goAnimStartY;	
	float faceAngleTarget;	
	float faceAngle;
	bool mirror=true;
	
	[System.NonSerialized] public bool bladeControlPos=true;
	[System.NonSerialized] public bool bladeControlRot=true;		
	[System.NonSerialized] public bool canGather=false;
	[System.NonSerialized] public bool canCast=true;
	[System.NonSerialized] public bool blind=false;
	
	string status; // "run" / "swim"
	public bool gathered=false;
	float girlMoveSpeed=1f;
		
	public AudioClip acBlind;
	public AudioClip acHurt;	
	public AudioClip acShine;
	public AudioClip acShineLow;	
	public AudioClip []ac1;
	public AudioClip []ac2;
	public AudioClip []ac3;
	public AudioClip []ac4;
	public AudioClip []ac5;
	public AudioClip []ac6;
	public AudioClip []ac7;	
	
	[System.NonSerialized] public string []s1=new string[4]{
		"This is the end of Lilian Forest.",
		"The flowers have gone black.",
		"This darkness is repelling me.",
		"I need to light them."
	};
	[System.NonSerialized] public string []s2=new string[3]{
		"This darkness is so immense.",
		"My spell is not strong enough.",
		"Please, help me!"
	};
	[System.NonSerialized] public string []s3=new string[2]{
		"I can't see or hear the end of the waterfall.",
		"But I have to do this.",
	};
	[System.NonSerialized] public string []s4=new string[2]{
		"Knorx is near.",
		"I can feel it.",
	};
	[System.NonSerialized] public string []s5=new string[2]{
		"This tunnel is so treacherous.",
		"I can't pass without seeing it!",
	};
	[System.NonSerialized] public string []s6=new string[2]{
		"These amethysts captured the lights of Floria.",
		"I have to free them!",
	};
	[System.NonSerialized] public string []s7=new string[1]{
		"Thanks for your help!",
	};

	IEnumerator Start () {
		go0=GameObject.Find("0");
		g=go0.GetComponent<Global>();		
		fxFE=GetComponent<fxFairyEnergy>();	
		goAnim=transform.FindChild(modelName).gameObject;
		goAnimStartY=goAnim.transform.localPosition.y;
		anim=goAnim.animation;		
		anim.Play("run");
		currentAnim=anim["run"];
		status="run";

		bladeControlPos=false;
		bladeControlRot=false;
		yield return new WaitForSeconds(0.5f);
		bladeControlPos=true;
		bladeControlRot=true;		
	}	
	
	void Update () {		
		// following blade
		Vector3 girlPos=transform.position;
		Vector3 vgb=g.blade.transform.position-girlPos;
		if(bladeControlPos){			
			if(currentAnim)
				currentAnim.speed=vgb.magnitude*0.5f;
			girlPos+=vgb*girlMoveSpeed*Time.deltaTime;				
			transform.position=girlPos;
		}		
		if(bladeControlRot){
			if(status=="run"){
				faceAngleTarget=vgb.x*10;
			}else{
				faceAngleTarget=vgb.x*50;
			}
			if(mirror)
				faceAngleTarget=-faceAngleTarget;
			faceAngleTarget=Mathf.Clamp(faceAngleTarget,-90f,90f);
			faceAngle=Mathf.Lerp (faceAngle,faceAngleTarget,0.1f);
			transform.rotation=Quaternion.Euler(0,faceAngle,0);
		}
				
		// hovering
		goAnim.transform.localPosition=new Vector3(0,goAnimStartY+0.2f*Mathf.Sin(Time.time*4),0);		
				
		// FX
		if(gathered && fxFE.GetFEStatus()==fxFairyEnergy.FEStatus.fe_Idle){
			if(g.boss.status==Boss.Status.notActive){
				fxFE.Gather();
			}else{
				fxFE.GatherBoss();
			}
		}
				
		// keyboard testing
		if(Input.GetKeyDown(KeyCode.Space)){
			cast(gathered);
		}
		if(Input.GetKeyDown(KeyCode.V)){
			gather();
		}
	}	
	
	public void cast(bool strong)
	{
		print ("11"+canCast);
		if(canCast==false){
			print ("15"+canCast);
			return;
		}
		print ("22"+canCast);
		if(!strong){
			currentAnim=anim.PlayQueued("wipeChangeColor",QueueMode.PlayNow);
			currentAnim.speed=2f;			
			currentAnim=anim.PlayQueued(status,QueueMode.CompleteOthers);
			AudioSource.PlayClipAtPoint(acShineLow,Vector3.zero);
			
		}else{
			currentAnim=anim.PlayQueued("wipeAttack",QueueMode.PlayNow);
			currentAnim.speed=2f;
			currentAnim=anim.PlayQueued(status,QueueMode.CompleteOthers);
			AudioSource.PlayClipAtPoint(acShine,Vector3.zero);
			g.main.centerFadeTime=0;				
			g.cameraControl.following=true;
			
			if(g.boss.status==Boss.Status.moving){
				fxFE.Cast(g.goBoss);
			}else{
				fxFE.Cast();
			}
		}
		print ("33"+canCast);
		
		if(g.boss.status==Boss.Status.notActive){
			if(strong){
				StartCoroutine(g.main.litRange());
				StartCoroutine(g.main.litLight());
			}else{
				StartCoroutine(g.main.litRange());				
			}			
		}
		
		if(g.boss.status==Boss.Status.moving){
			g.boss.onHit(gathered);
		}
		
		if(g.boss.status==Boss.Status.rampage){
			if(fxFE.fairyEnergy>90)
				g.boss.die();
		}
		
		gathered=false;
		fxFE.SetEnergyLevel(0);
	}
	
	float lastAttackTime=0;
	float attackCDTime=1f;
	public void onAttack()
	{
		if(Time.time>lastAttackTime+attackCDTime){			
			cast (gathered);
			lastAttackTime=Time.time;
		}
	}
	
	public void gather()
	{
		if(Time.time<lastAttackTime+attackCDTime)
			return;
		if(gathered)
			return;
		gathered=true;
		canGather=false;
		fxFE.SetEnergyLevel(100);
	}
	
	public void onHit()
	{
		g.blade.rumble(10,0.4f);
		AudioSource.PlayClipAtPoint(acHurt,Vector3.zero);
	}
		
	public void onBlind()
	{
		StartCoroutine(ieBlind());}IEnumerator ieBlind(){		
				
		anim.Play("blind");
		anim.PlayQueued("blindFly");
		AudioSource.PlayClipAtPoint(acBlind,Vector3.zero);
		yield return new WaitForSeconds(0.2f);	
		g.main.subtitle("My eyes! I...I can't see!",3f,g.main.clFairy);
		yield return new WaitForSeconds(5f);
		anim.Play("blind-run");
		currentAnim=anim.PlayQueued("run");
		yield return new WaitForSeconds(1f);
		
		mirror=false;		
		blind=true;
		
		Vector3 girlPos=transform.position;
		int step=180;
		for(int i=0;i<=step;i++){
			Vector3 te=goAnim.transform.localEulerAngles;
			te.y=step-i;
			goAnim.transform.localEulerAngles=te;			
			transform.position=girlPos;
			yield return new WaitForSeconds(0.005f);
		}

		
		g.boss.startMoving();
	}
	
	public void onEnterLake()
	{
		anim.Play("swim");		
		currentAnim=anim["swim"];
		status="swim";		
	}
	
	public void onLeaveLake()
	{
		anim.Play("swim-run");
		currentAnim=anim.PlayQueued("run");
		status="run";
	}
}
