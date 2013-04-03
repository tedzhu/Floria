#pragma warning disable 0168 // variable declared but not used.
#pragma warning disable 0219 // variable assigned but not used.
#pragma warning disable 0414 // private field assigned but not used.
using UnityEngine;
using System.Collections;

public class Main : MonoBehaviour
{
	GameObject go0;
	Global g;
	
	public bool PSMoveOn;	
	public bool WebcamOn;
	public string webcamName="EE U2 CAM";	
	
	WebCamTexture wct;
	Texture2D texCamMask;
	bool showWebcamView=false;
		
	public AudioClip bgmForest;
	public AudioClip bgmLake;
	public AudioClip bgmCave;
	public AudioClip bgmBoss;
	[System.NonSerialized] public AudioClip currentBGM=null;
	
	public Texture2D []signs;
	
	bool calibratingLight;
	float currentLight;
	float minLight=8000;
	float maxLight=10000;
	float threshold=8000;
	
	const int MAXLIGHTCOUNT=100;
	float []finLight=new float[MAXLIGHTCOUNT];	
	
	Renderer []rds;
	Color []clTarget;
	
	void Start ()
	{
		Screen.showCursor=false;
		go0 = GameObject.Find ("0");
		g = go0.GetComponent<Global> ();		
		
		// read pref
		webcamName=PlayerPrefs.GetString("webcamName","EE U2 CAM");
		PSMoveOn=PlayerPrefs.GetInt("PSMoveOn")==1;
		WebcamOn=PlayerPrefs.GetInt("WebcamOn")==1;
		threshold=PlayerPrefs.GetFloat("threshold",8000);
		
		//PSMoveOn=false;
		//WebcamOn=false;
		
		print (webcamName);
		print (PSMoveOn);
		print (WebcamOn);
		print (threshold);
		
		
		// PSMove stuff
		if(PSMoveOn){
			PSMoveWrapper psmw;
			psmw=go0.GetComponent<PSMoveWrapper>();
			psmw.Connect();
		}
		
		// Webcam stuff
		if(WebcamOn){
			// webcam texture
			wct=new WebCamTexture();
			wct.deviceName=webcamName;
			wct.requestedWidth=320;
			wct.requestedHeight=240;
			g.goWebcamPlane.renderer.material.mainTexture=wct;			
			wct.Play();
							
			// mask texture
			texCamMask=new Texture2D(wct.width,wct.height);
			g.goMaskPlane.renderer.material.mainTexture=texCamMask;
			//g.goMaskPlaneBoss.renderer.material.mainTexture=texCamMask;
		}

		// lights record
		for(int i=0;i<g.gosLights.Length;i++){
			finLight[i]=g.gosLights[i].light.range;
			g.gosLights[i].light.range=0;
		}
		
		// copy material & set to black
		GameObject goFlowerGroup=GameObject.Find("FlowerGroup");
		rds=goFlowerGroup.GetComponentsInChildren<Renderer>();
		for(int i=0;i<rds.Length;i++){
			rds[i].material=new Material(rds[i].material);
		}
		
		clTarget=new Color[rds.Length];
		for(int i=0;i<rds.Length;i++){
			clTarget[i]=Color.black;
		}	
	
	}
	
	float sumMat(Color []cmat)
	{
		float sum=0;
		for(int i=0;i<cmat.Length;i++){
			sum+=cmat[i].r+cmat[i].g+cmat[i].b;
		}
		return sum;
	}

	public void startCalibrate()
	{
		minLight=Mathf.Infinity;
		maxLight=0;
		calibratingLight=true;
	}
	
	public void stopCalibrate()
	{
		calibratingLight=false;
	}
	
	void Update ()
	{
		// webcam stuff
		if(WebcamOn){
			Color []cmat=wct.GetPixels();
			Color []cmatNew=wct.GetPixels();
			
			currentLight=sumMat(cmat); 			
			if(calibratingLight){
				if(currentLight>maxLight)maxLight=currentLight;
				if(currentLight<minLight)minLight=currentLight;			
				g.main.subtitle(currentLight.ToString()+" ( "+minLight+" , "+maxLight+" )",1f,clNormal);
			}else{				
				if(maxLight!=Mathf.Infinity){
					if(currentLight>threshold){
						if(g.girl.canGather){
							g.girl.gather();
						}
					}				
				}
			}
						
			texCamMask.SetPixels(cmatNew);
			texCamMask.Apply();
			g.goWebcamPlane.renderer.material.mainTexture=texCamMask;
		}
		
		// calibrating
		if(Input.GetKeyDown(KeyCode.LeftShift)){
			startCalibrate();
		}
		if(Input.GetKeyUp(KeyCode.LeftShift)){
			stopCalibrate();		
		}
		
		// bgm
		float d=0.01f;
		if(audio.clip!=currentBGM){
			if(audio.volume>d){
				audio.volume-=d;
			}else{
				audio.Stop();
				audio.clip=currentBGM;
				if(currentBGM)
					audio.Play();
			}
		}else{
			if(audio.volume<1-d)
				audio.volume+=d;
		}
		
		// phase 1 lighting
		for(int i=0;i<rds.Length;i++){
			rds[i].material.color=Color.Lerp(rds[i].material.color,clTarget[i],Time.deltaTime);
		}	
		
		
		//=====================debug====================//
		// switch debug cam
		if(Input.GetKeyDown(KeyCode.Alpha1)){
			showWebcamView=!showWebcamView;
			GameObject go=GameObject.Find ("WebcamPlane");
			go.renderer.enabled=showWebcamView;
		}
		
		// switch mouse / PS Move
		if(Input.GetKeyDown(KeyCode.Alpha2)){
			PSMoveOn=!PSMoveOn;
			g.main.lowerleft(PSMoveOn?"Using PS Move":"Using Mouse",1f); 
		}
		
		// switch blade control
		if(Input.GetKeyDown(KeyCode.Alpha3)){
			g.girl.bladeControlPos=!g.girl.bladeControlPos;
		}
		
		// connect
		if(Input.GetKeyDown(KeyCode.C)){
			GetComponent<PSMoveWrapper>().Connect();
		}		
	}
	
	public IEnumerator litLight()
	{
		int step=20;
		for(int t=0;t<step;t++){
			for(int i=0;i<g.gosLights.Length;i++){
				Vector3 pos=g.gosLights[i].transform.position;
				Vector3 viewportPos=Camera.main.WorldToViewportPoint(pos);
				if(viewportPos.x>0 && viewportPos.x<1){
					g.gosLights[i].light.range=finLight[i]*t/step;
				}
			}
			yield return new WaitForSeconds(0.02f);
		}
	}
	
	public IEnumerator litRange()
	{
		for(int i=0;i<rds.Length;i++){
			Vector3 pos=rds[i].transform.position;
			Vector3 viewportPos=Camera.main.WorldToViewportPoint(pos);
			if(viewportPos.x>0f && viewportPos.x<1f){					
				clTarget[i]=Color.white;
			}
		}
		yield return null;
	}	
	
	//==================== below: triggers ====================//
	string sRaise="Raise your lights to increase fairy's energy.";
	string sLightPass="Raise your lights to show the tunnel.";
	public IEnumerator onTrigger(string triggerName)
	{
		if(triggerName=="instruction1"){
			subtitle("Move wand to guide fairy.",4f,clNormal);
		}
		
		if(triggerName=="instruction2"){
			subtitle("Press trigger and wave to illuminate.",4f,clNormal);
		}		
		
		if(triggerName=="enterForest"){
			//subtitle("Lilian Forest",2f,clNormal);
			StartCoroutine(showSign(0));
			currentBGM=bgmForest;
			delayDialog(3f,g.girl.ac1,g.girl.s1,clFairy);
		}
		
		if(triggerName.Contains("stop")){
			g.cameraControl.following=false;			
			if(triggerName!="stop1"){
				g.main.subtitle(sRaise,0,clNormal);
			}
			g.girl.canGather=true;
		}
		
		if(triggerName=="stop1"){
			delayDialog(3f,g.girl.ac2,g.girl.s2,clFairy);
			yield return new WaitForSeconds(10);
			g.main.subtitle(sRaise,0,clNormal);
		}		
		
		if(triggerName=="waterFall"){
			g.girl.canGather=false;
			StartCoroutine(showSign(1));
			delayDialog(2f,g.girl.ac3,g.girl.s3,clFairy);
		}		

		if(triggerName=="enterLake"){
			StartCoroutine(showSign(2));
			currentBGM=bgmLake;
			
		}
		
		if(triggerName=="exitLake"){			
			g.girl.onLeaveLake();
		}		
		
		if(triggerName=="enterPass"){			
			g.cameraControl.lockScreen=true;
			g.cameraControl.targetPos=GameObject.Find("CameraPosMask").transform.position;
			delayDialog(0f,g.girl.ac5,g.girl.s5,clFairy);
			yield return new WaitForSeconds(8);
			g.main.subtitle(sLightPass,0,clNormal);
		}
		
		if(triggerName=="exitPass"){
			g.cameraControl.lockScreen=false;
			g.cameraControl.targetPos.z=-8.8f;
			centerFadeTime=0;
			delayDialog(1f,g.girl.ac7,g.girl.s7,clFairy);
		}
		
		if(triggerName=="cave"){
			delayDialog(3f,g.girl.ac4,g.girl.s4,clFairy);
			StartCoroutine(showSign(3));
			g.cameraControl.lockScreen=false;
			g.cameraControl.targetPos.z=-8.8f;
			currentBGM=bgmCave;
		}
		
		if(triggerName=="bossRoom"){			
			g.cameraControl.lockScreen=true;
			g.cameraControl.targetPos=GameObject.Find("CameraPosBoss").transform.position;
			delayDialog(3f,g.girl.ac6,g.girl.s6,clFairy);
		}
		
		if(triggerName=="boss"){			
			currentBGM=bgmBoss;
			g.boss.appear();
			yield return new WaitForSeconds(1f);
			StartCoroutine(showSign(4));
		}
		
		yield return null;
	}

	
	
	//==================== UI ====================//
	
	public Font font;
	public int fontSize=40;
	public float outlineD=2.5f;
	public Color clCenterIn;
	public Color clLowerleftIn;
	public Color clCenterOut;
	public Color clLowerleftOut;	
	public Color clNormal;
	public Color clFairy;
	public Color clBoss;
	
	[System.NonSerializedAttribute] public string centerString;	
	[System.NonSerializedAttribute] public string lowerleftString;
	
	float lowerleftResetTime=Mathf.Infinity;
	public float centerFadeTime=0;
	float centerAlpha=0;
	float centerAlphaDelta=0.02f;
	
	void outlinedText(Rect rtRange,string text,GUIStyle gs, Color inColor, Color outColor)
	{		
		float d=outlineD;
		float [,]offsets=new float[,]{{0,d},{0,-d},{-d,0},{d,0},{d,-d},{d,-d},{-d,d},{-d,-d}};				
				
		gs.normal.textColor=outColor;
		for(int i=0;i<offsets.GetLength(0);i++){
			gs.contentOffset=new Vector2(offsets[i,0],offsets[i,1]);
			GUI.Label(rtRange,text,gs);
		}
		
		gs.normal.textColor=inColor;
		gs.contentOffset=new Vector2(0,0);
		GUI.Label(rtRange,text,gs);
	}
	
	void OnGUI(){
		//GUI.Button(new Rect(10,20,40,50),"");
		float margin=40;
		Rect rtRange=new Rect(margin,margin,Screen.width-2*margin,Screen.height-2*margin);
		
		// fading
		if(Time.time<centerFadeTime){
			if(centerAlpha<1)
				centerAlpha+=centerAlphaDelta;
		}else{
			if(centerAlpha>0)
				centerAlpha-=centerAlphaDelta;
		}
		clCenterIn.a=centerAlpha;
		clCenterOut.a=centerAlpha;		
		
		// drawing
		GUIStyle gs=new GUIStyle();
		gs.font=font;
		gs.fontSize=fontSize;
		gs.alignment=TextAnchor.UpperCenter;
		outlinedText(rtRange,centerString,gs,clCenterIn,clCenterOut);
		
		gs.alignment=TextAnchor.LowerLeft;
		outlinedText(rtRange,lowerleftString,gs,clLowerleftIn,clLowerleftOut);
		
		// 
		if(Time.time>lowerleftResetTime){
			lowerleftResetTime=Mathf.Infinity;
			lowerleftString="";
		}
	}	
	
	bool lowerleftInUse=false;
	bool centerInUse=false;
	
	public void subtitle(string text, float delay, Color clIn)
	{
		clCenterIn=clIn;
		centerString=text;
		if(delay==0f)
			centerFadeTime=Mathf.Infinity;
		else
			centerFadeTime=Time.time+delay;
	}
	
	public void lowerleft(string text,float delay)
	{
		lowerleftString=text;
		if(delay==0f){
			lowerleftResetTime=Mathf.Infinity;
		}else{
			lowerleftResetTime=Time.time+delay;
		}
	}
	
	void delayDialog(float delay, AudioClip[] ac,string[] sub,Color cl)
	{		
		StartCoroutine(ieDelayDialog(delay,ac,sub,cl));
	}
	
	IEnumerator ieDelayDialog(float delay, AudioClip[] ac,string[] sub,Color cl)
	{
		yield return new WaitForSeconds(delay);
		StartCoroutine(dialog(ac,sub,cl));
	}
	
	public IEnumerator dialog(AudioClip[] ac,string[] sub,Color cl)
	{
		for(int i=0;i<ac.Length;i++){
			AudioSource.PlayClipAtPoint(ac[i],Vector3.zero);
			float delay=ac[i].length;
			g.main.subtitle(sub[i],delay,cl);
			yield return new WaitForSeconds(delay);
		}		
	}
		
	IEnumerator showSign(int index)
	{		
		g.goRoadSign.renderer.material.mainTexture=signs[index];	
		float d=0.05f;
		Color cl=g.goRoadSign.renderer.material.color;
		for(float f=0;f<1;f+=d){
			cl.a=f;
			g.goRoadSign.renderer.material.color=cl;
			yield return new WaitForSeconds(0.02f);
		}
		yield return new WaitForSeconds(3f);
		for(float f=1;f>0;f-=d){
			cl.a=f;
			g.goRoadSign.renderer.material.color=cl;
			yield return new WaitForSeconds(0.02f);
		}
		cl.a=0;
		g.goRoadSign.renderer.material.color=cl;
		yield return null;
	}	
}
