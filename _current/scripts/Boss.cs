#pragma warning disable 0168 // variable declared but not used.
#pragma warning disable 0219 // variable assigned but not used.
#pragma warning disable 0414 // private field assigned but not used.

using UnityEngine;
using System.Collections;

public class Boss : MonoBehaviour {
	Global g;
	GameObject go0;
	string modelName="boss_version 01";
	GameObject goAnim;
	Animation anim;		
		
	float currentWidth=0;
	int lookAt=2;
	
	
	//public Texture2D texHP;
	//public Texture2D texHPBack;
	public GameObject goHPBar;
	public GameObject goEyeLight;
	public ParticleSystem psDie;
	HPBar hpbar;
	
	public GameObject []gosAttackPlane;
	Vector3 targetPos;
	Vector3 []attackPos=new Vector3[5];
	string []prepare=new string[4];
	string []finish=new string[4];
	float vulnerableTime=4f;
	float attackRate=1f;
	bool vulnerable=true;
	
	int life=0;
	int girlLife=0;
	
	public enum Status {
		notActive,
		appear,
		moving,
		rampage,
		die
	};
	public Status status=Status.notActive;		
	
	string []lines=new string[6]{
		"Poor girl. Poor elves!",
		"You think you can conquere me?",
		"",
		"I'll teach you something:",
		"Life and light are evanescent,",
		"only death and darkness are everlasting!"	
	};	
	public AudioClip []acBoss;
	public AudioClip acAttack;
	public AudioClip acMagicwave;
	public AudioClip acHurt;	
	public AudioClip acDying;	
	public AudioClip acParry;
	public AudioClip acBigHurt;
	public AudioClip acRock;
	
	void Start () {
		go0=GameObject.Find("0");
		g=go0.GetComponent<Global>();
		goAnim=GameObject.Find(modelName);
		anim=goAnim.animation;
		hpbar=goHPBar.GetComponent<HPBar>();
		//appear();
		//die ();
	}
		
	void Update () {
		if(lookAt==0){
		}else if(lookAt==1){			
			Vector3 lookPos=2*transform.position-g.goGirl.transform.position;
			transform.LookAt(lookPos);
		}else if(lookAt==2){
			Vector3 lookPos=2*transform.position-Camera.main.transform.position;
			transform.LookAt(lookPos);
		}
		
		float damp=2f;
		
		if(status==Status.notActive){
		}
		
		if(status==Status.appear){
		}
		
		
		if(status==Status.moving || status==Status.rampage){
			transform.position=Vector3.Lerp(transform.position,targetPos,damp*Time.deltaTime);			
		}
		
		if(status==Status.moving){
			goHPBar.active=true;
			hpbar.amountLeft=Mathf.Lerp(hpbar.amountLeft,0.8f*girlLife/100f,1*Time.deltaTime);
			hpbar.amountRight=Mathf.Lerp(hpbar.amountRight,0.8f*life/100f,1*Time.deltaTime);
		}else{
			goHPBar.active=false;
		}
		
	}

	//===========================//
	public void appear()
	{
		StartCoroutine(ieAppear());}IEnumerator ieAppear(){
		status=Status.appear;
		
		yield return StartCoroutine(FadeDirLight(0,0.1f,20));		
		iTween.MoveTo(gameObject, iTween.Hash(
			"path", iTweenPath.GetPath("bossAppear"),
			"time", 10,
			"easeType", iTween.EaseType.linear
			));
		
		yield return StartCoroutine(FadeDirLight(0.1f,0.1f,20));
		yield return new WaitForSeconds(5);		
		Vector3 v=g.girl.transform.position;
		for(int i=0;i<100;i++){
			v.z=Mathf.Lerp (v.z,-6,0.1f);
			g.girl.transform.position=v;
			yield return new WaitForSeconds(0.01f);			
		}
		
		yield return new WaitForSeconds(1);
		
		lookAt=1;		
		for(int i=0;i<3;i++){
			AudioSource.PlayClipAtPoint(acBoss[i],Vector3.zero);
			float delay=acBoss[i].length;
			g.main.subtitle(lines[i], delay,g.main.clBoss);
			yield return new WaitForSeconds(delay);
		}
		
		anim.Play("horizontalAtkPre");
		yield return new WaitForSeconds(anim["horizontalAtkPre"].length);
		anim.PlayQueued("horizontalAtkFinish");				
		yield return new WaitForSeconds(0.2f);
		g.cameraControl.shake (0.1f,1f);
		AudioSource.PlayClipAtPoint(acMagicwave,Vector3.zero);
		yield return new WaitForSeconds(0.2f);

		g.girl.onBlind();
	}
	
	//===========================//
	
	
	// === moving === //
	
	public void startMoving()
	{
		StartCoroutine(ieMoving());}IEnumerator ieMoving(){
		status=Status.moving;
		
		life=100;
		girlLife=100;
		lookAt=2;		
		float z=-Camera.main.transform.position.z;
		attackPos[0]=Camera.main.ViewportToWorldPoint(new Vector3(0.25f,0.5f,z));
		attackPos[1]=Camera.main.ViewportToWorldPoint(new Vector3(0.7f,0.5f,z));
		attackPos[2]=Camera.main.ViewportToWorldPoint(new Vector3(0.5f,0.75f,z));
		attackPos[3]=Camera.main.ViewportToWorldPoint(new Vector3(0.5f,0.25f,z));
		attackPos[4]=Camera.main.ViewportToWorldPoint(new Vector3(0.5f,0.5f,z));
		for(int i=0;i<4;i++)
			attackPos[i].z=0;
		prepare[0]="verticalAtkPre";
		prepare[1]="verticalAtkPre";
		prepare[2]="horizontalAtkPre";
		prepare[3]="horizontalAtkPre";
		finish[0]="verticalAtkFinish";
		finish[1]="verticalAtkFinish";
		finish[2]="horizontalAtkFinish";
		finish[3]="horizontalAtkFinish";
		
		while(status==Status.moving){
			int t=Random.Range(0,4);
			targetPos=attackPos[t];
			yield return new WaitForSeconds(1f);
			
			anim.PlayQueued(prepare[t]).speed=0.2f*attackRate;
			
			yield return new WaitForSeconds(0.2f);
			
			g.main.subtitle("Tell the fairy where to go!", 0f, g.main.clNormal);
			for(int i=0;i<180;i++){
				Color cl=gosAttackPlane[t].renderer.material.color;
				cl.a=i/255f;
				gosAttackPlane[t].renderer.material.color=cl;
				yield return new WaitForSeconds(0.002f );
			}			
			yield return new WaitForSeconds(0.5f);
			g.main.centerFadeTime=0;
			anim.PlayQueued(finish[t]);
			
			yield return new WaitForSeconds(0.2f);
			
			g.cameraControl.shake (0.4f,3f);
			AudioSource.PlayClipAtPoint(acAttack,Vector3.zero);
			Vector3 girlScreenPos=Camera.main.WorldToViewportPoint(g.girl.transform.position);
			if((t==0 && girlScreenPos.x<0.5)
				||(t==1 && girlScreenPos.x>0.5)
				||(t==2 && girlScreenPos.y>0.5)
				||(t==3 && girlScreenPos.y<0.5))
			{
				g.cameraControl.shake (0.6f,5f);
				girlLife-=30;
				g.girl.onHit();
			}
		
			Color cc=gosAttackPlane[t].renderer.material.color;
			cc.a=0;
			gosAttackPlane[t].renderer.material.color=cc;
			
			yield return new WaitForSeconds(0.2f);
			
			vulnerable=true;
			g.main.subtitle("Attack!", 0f, g.main.clNormal);
			yield return new WaitForSeconds(vulnerableTime);
			g.main.centerFadeTime=0;
			vulnerable=false;			
		}
	}
	
	// === hit === //
	public void onHit(bool strong)
	{
		if(status==Status.moving){
			if(!vulnerable){
				AudioSource.PlayClipAtPoint(acParry,Vector3.zero);
			}else{
				AudioSource.PlayClipAtPoint(acBigHurt,Vector3.zero);
				life-=20;			
			}
			
			if(life<=40){
				vulnerableTime=2f;
				//attackRate=3f;
			}			
			if(life<=0 || girlLife<=0){
				StartCoroutine(rampage ());
			}
		}
	}
	
	// ======= rampage ======= //
	IEnumerator rampage()
	{		
		status=Status.rampage;
		targetPos=attackPos[4];
		g.girl.canCast=false;
		
		for(int i=3;i<6;i++){
			AudioSource.PlayClipAtPoint(acBoss[i],Vector3.zero);
			float delay=acBoss[i].length;
			g.main.subtitle(lines[i], delay,g.main.clBoss);
			yield return new WaitForSeconds(delay);
		}
		
		GetComponent<BossRampage>().Rampage();
		g.girl.canGather=true;		
		g.goMaskPlaneBoss.renderer.enabled=true;		
		
		g.main.subtitle("Raise all your lights for the strike!",0,g.main.clNormal);
		float t=0;		
		while(status==Status.rampage){
			g.cameraControl.shake (1f,0.2f);
			
			if(g.girl.gathered){
				t+=1;
				if(t>=100){
					g.girl.canCast=true;
					t=100;
				}
				g.girl.fxFE.SetEnergyLevel(t);
			}
			yield return new WaitForSeconds(0.1f);
		}
		g.main.centerFadeTime=0;		
	}
	
	// ======= die ======= //
	
	public void die(){
		StartCoroutine(ieDie());}IEnumerator ieDie(){		
		status=Status.die;
		
		g.goMaskPlaneBoss.renderer.enabled=false;		
		g.main.lowerleft("",1);
		
		AudioSource.PlayClipAtPoint(acDying,Vector3.zero);
		psDie.Play();
		int step=20;
		for(int i=0;i<step;i++){
			psDie.startSize=0.07f+(0.12f-0.07f)/step*i;
			yield return new WaitForSeconds(0.1f);			
		}
		psDie.startSize=0.12f;		
		g.main.currentBGM=null;		
		g.cameraControl.shake (4f,1f);
		AudioSource.PlayClipAtPoint(acRock,Vector3.zero);
		
//		for(float a=4;a>0;a-=0.4f){
//			goEyeLight.light.intensity=a;
//			yield return new WaitForSeconds(0.1f);
//		}
		yield return new WaitForSeconds(4);							
		
		iTween.CameraFadeAdd();
		iTween.CameraFadeTo( 1.0f, 2.0f);
		yield return new WaitForSeconds(3f);
		Application.LoadLevel ("ED_new");							
	}
	
	
	// ===================== utility ===================== //
	
	IEnumerator FadeDirLight(float ed,float unitDelay,int step)
	{
		float st=g.goDirLightMain.light.intensity;
		for(int i=0;i<=step;i++){			
			g.goDirLightMain.light.intensity=st+(ed-st)*i/step;
			yield return new WaitForSeconds(unitDelay);
		}		
		g.goDirLightMain.light.intensity=ed;
	}
	
	IEnumerator FadeAmbient(Color ed,float unitDelay,int step)
	{
		Color st=RenderSettings.ambientLight;
		for(int i=0;i<=step;i++){			
			RenderSettings.ambientLight=st+(ed-st)*i/step;
			yield return new WaitForSeconds(unitDelay);
		}		
	}	
	
	IEnumerator WaitForAnimation ( Animation animation )
	{
	    do
	    {
	        yield return null;
	    } while ( animation.isPlaying );
	}
}
