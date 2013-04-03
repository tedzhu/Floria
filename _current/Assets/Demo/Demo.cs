#pragma warning disable 0168 // variable declared but not used.
#pragma warning disable 0219 // variable assigned but not used.
#pragma warning disable 0414 // private field assigned but not used.
using UnityEngine;
using System.Collections;
using System.IO;
using System;

public class Demo : MonoBehaviour {
	public GameObject goTextStart;
	public GameObject goTextEnd;	
	
	GameObject goFlowerGroup;
	GameObject goForestLights;	
	GameObject goBlade;
	public GameObject preNote;
	float bladeFollowDamp=6f;
	Vector3 bladeWorldPosTarget;
	
	GameObject goBlade2;
	public GameObject preNote2;
	float blade2FollowDamp=2f;
	Vector3 blade2WorldPosTarget;
	
	Vector3 bladeStartPos;
	Vector3 cameraStartPos;	
	
	bool cameraFollow=false;
	
	Renderer []rds;
	Color []clTarget;
	bool cameraMoving=true;	
	float cameraMoveSpeed=0.8f;	
	float cameraMoveDamp=1f;	
	
	const int MAXNOTECOUNT=10000;	
	
	float maxVolume=1f;
	
	class Track{		
		public int []noteValue;
		public float []noteTime;
		public int noteCount;
		public int currNote;
		public float noteMin=50;
		public float noteMax=100;
		public Track(){
			noteValue=new int[MAXNOTECOUNT];
			noteTime=new float[MAXNOTECOUNT];
			noteCount=0;
			currNote=0;
		}
	};
	Track t1=new Track();
	Track t2=new Track();	
			
	int currNote;
	
	const int MAXBPM=10;
	int bpmCount;
	float []bpms=new float[MAXBPM];
	int []bpmClocks=new int[MAXBPM];	

	float playStartTime;	
	//bool isRunning=false;
	bool isPlayingMidi=false;
	bool inited=false;
	
	string []lines;
	
	float clock2time(int clock)
	{
		float re=0;
		for(int i=0;i<bpmCount;i++){
			if(i+1<bpmCount && clock>bpmClocks[i+1]){
				re+=(bpmClocks[i+1]-bpmClocks[i])/(480*bpms[i])*60;
			}else{
				re+=(clock-bpmClocks[i])/(480*bpms[i])*60;
				break;
			}
		}	
		return re;	
	}
	
	void analyzeTrack(Track track, int tracknum)
	{
		for(int i=0;i<lines.Length;i++){
			string []parts=lines[i].Split(',');
			if(parts[0]=="")
				continue;
			if(Int32.Parse(parts[0])==tracknum && parts[2]==" Note_on_c"){
				track.noteValue[track.noteCount]=Int32.Parse(parts[4]);
				int noteClock=Int32.Parse(parts[1]);
				track.noteTime[track.noteCount++]=clock2time(noteClock);							
				track.noteCount++;
			}
		}
	}
	
	void Start () {
		// reading mid file
		try {
			lines=File.ReadAllLines("FlowerDance.csv");
		}
		catch(Exception) {
			Debug.LogWarning("wrong");
		}
		
		// tempo
		for(int i=0;i<lines.Length;i++){
			string []parts=lines[i].Split(',');
			if(parts[2]==" Tempo"){
				bpmClocks[bpmCount]=Int32.Parse(parts[1]);
				bpms[bpmCount]=60000000f/Int32.Parse(parts[3]);
				bpmCount++;
			}				
		}
		
		// trakcs
		analyzeTrack(t1,2);
		analyzeTrack(t2,3);
		t1.noteMin=50;
		t1.noteMax=100;
		t2.noteMin=30;
		t2.noteMax=80;
		
		// get objects
		goFlowerGroup=GameObject.Find("FlowerGroup");
		goForestLights=GameObject.Find("ForestLights");
		goBlade=GameObject.Find("Blade");
		goBlade2=GameObject.Find("Blade2");
		
		// initialization
		foreach(Transform tLight in goForestLights.transform){
			tLight.gameObject.light.range=0f;
		}		
		rds=goFlowerGroup.GetComponentsInChildren<Renderer>();		
		clTarget=new Color[rds.Length];
		for(int i=0;i<rds.Length;i++){
			rds[i].material=new Material(rds[i].material);
		}
		bladeWorldPosTarget=goBlade.transform.position;				
		blade2WorldPosTarget=goBlade2.transform.position;
		
		bladeStartPos=goBlade.transform.position;
		cameraStartPos=transform.position;
		
		iTween.CameraFadeAdd();
		//iTween.CameraFadeTo(1,0.1f);
		//iTween.CameraFadeDepth(1);
		
		resetScene();		
		startPlay();
	}	
	
	void FixedUpdate ()
	{
		goBlade.transform.Rotate (Vector3.down, 2f);
	}
	
	void Update () {
		// camera moving
		Vector3 currCamPos=transform.position;
		Vector3 nextCamPos=currCamPos;		
		if(cameraMoving){			
			nextCamPos.x+=cameraMoveSpeed*Time.deltaTime;
			transform.position=nextCamPos;
			// lighting
			for(int i=0;i<rds.Length;i++){
				Vector3 worldPos=rds[i].transform.position;
				Vector3 viewportPos=Camera.main.WorldToViewportPoint(worldPos);				
				if(viewportPos.x<0.66f){
					clTarget[i]=Color.white;
				}else{
					clTarget[i]=Color.black;
				}				
			}									
		}
		
		// env light lerping
		for(int i=0;i<rds.Length;i++){
			rds[i].material.color=Color.Lerp(rds[i].material.color,clTarget[i],cameraMoveDamp*Time.deltaTime);
		}		
		
		// get note & blade moving
		if(isPlayingMidi){
			float playTime=Time.time-playStartTime;
			
			// track 1
			while(t1.noteTime[t1.currNote]<playTime){
				if(t1.currNote>=t1.noteCount)
					break;
				
				Vector3 explodePos=new Vector3(0.66f,0.5f,-currCamPos.z);
				explodePos.y=(t1.noteValue[t1.currNote]-t1.noteMin)/(t1.noteMax-t1.noteMin);
				explodePos=Camera.main.ViewportToWorldPoint(explodePos);
				GameObject go=(GameObject)Instantiate(preNote,explodePos,Quaternion.identity);
				go.particleEmitter.Emit();
				go.particleEmitter.emit=false;
				
				t1.currNote++;
				if(t1.currNote>=t1.noteCount){
					// midi finished
					
					StartCoroutine(resetAndStart());
					break;
				}
			}
			Vector3 v=new Vector3(0.66f,0.5f,-currCamPos.z);			
			v.y=(t1.noteValue[t1.currNote]-t1.noteMin)/(t1.noteMax-t1.noteMin);
			bladeWorldPosTarget=Camera.main.ViewportToWorldPoint(v);		
			goBlade.transform.position=Vector3.Lerp(goBlade.transform.position,bladeWorldPosTarget,bladeFollowDamp*Time.deltaTime);		
			
//			// track 2
//			while(t2.noteTime[t2.currNote]<playTime){
//				Vector3 explodePos=new Vector3(0.66f,0.5f,-currCamPos.z);
//				explodePos.y=(t2.noteValue[t2.currNote]-t2.noteMin)/(t2.noteMax-t2.noteMin);
//				explodePos=Camera.main.ViewportToWorldPoint(explodePos);
//				GameObject go=(GameObject)Instantiate(preNote2,explodePos,Quaternion.identity);
//				go.particleEmitter.Emit();
//				go.particleEmitter.emit=false;
//				
//				t2.currNote++;
//				if(t2.currNote>=t2.noteCount){
//					isPlayingMidi=false;
//				}
//			}
//			Vector3 v2=new Vector3(0.66f,0.5f,-currCamPos.z);			
//			v2.y=(t2.noteValue[t2.currNote]-t2.noteMin)/(t2.noteMax-t2.noteMin);
//			blade2WorldPosTarget=Camera.main.ViewportToWorldPoint(v2);			
//			goBlade2.transform.position=Vector3.Lerp(goBlade2.transform.position,blade2WorldPosTarget,blade2FollowDamp*Time.deltaTime);		
//				
		}
	
		//
		if(Input.GetKeyDown(KeyCode.R)){
			StartCoroutine(resetAndStart());
		}
	}
		
	
	
	IEnumerator resetAndStart()
	{
		yield return StartCoroutine(ieReset());
		startPlay();
	}
	
	void startPlay()
	{
		iTween.CameraFadeTo(0,1f);
				
		// track reset
		t1.currNote=0;
		t2.currNote=0;		
		isPlayingMidi=true;
		
		playStartTime=Time.time;				
		audio.volume=maxVolume;
		audio.Stop();
		audio.Play();	
	}
	
	bool resetting=false;
	IEnumerator ieReset()
	{				
		if(resetting)
			yield break;
		resetting=true;	
		
		// stop midi
		isPlayingMidi=false;
		
		// video fade out
		iTween.CameraFadeTo(1,1f);
		
		// audio fade out		
		for(int i=10;i>=0;i--){
			audio.volume-=0.1f;
			yield return new WaitForSeconds(0.1f);
		}
		audio.Stop();
		
		// reset scene
		resetScene();
		
		resetting=false;
	}
	
	void resetScene()
	{		
		for(int i=0;i<rds.Length;i++){
			clTarget[i]=Color.black;
		}								
		goBlade.transform.position=bladeStartPos;
		transform.position=cameraStartPos;
		
		
		string []bglines=new string[1];
		try {
			bglines=File.ReadAllLines("BackgroundLines.txt");
		}catch(Exception) {
			Debug.LogWarning("wrong");
		}
		if(bglines.Length==0)
			bglines=new string[1];
		
		//delete old ones
		GameObject []gos=GameObject.FindGameObjectsWithTag("bgtext");
		for(int i=0;i<gos.Length;i++)
			Destroy(gos[i]);		
		
		//new ones
		float xStart=goTextStart.transform.position.x;
		float xEnd=goTextEnd.transform.position.x;		
		for(int i=0;i<bglines.Length;i++){
			GameObject go=(GameObject)Instantiate(goTextStart);
			Vector3 pos=goTextStart.transform.position;
			pos.x=xStart+(xEnd-xStart)/bglines.Length*i;			
			go.transform.position=pos;
			go.tag="bgtext";
			go.GetComponent<TextMesh>().text=bglines[i];
		}
		
	}
	
	
	public void setVolume(float v)
	{
		if(v>1)v=1;
		maxVolume=v;
	}
}

