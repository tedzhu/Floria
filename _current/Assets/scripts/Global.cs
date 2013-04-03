#pragma warning disable 0168 // variable declared but not used.
#pragma warning disable 0219 // variable assigned but not used.
#pragma warning disable 0414 // private field assigned but not used.

using UnityEngine;
using System.Collections;

public class Global : MonoBehaviour {
	bool initedInMain=false;
	[System.NonSerialized] public GameObject goGirl;
	[System.NonSerialized] public Girl girl;
	[System.NonSerialized] public GameObject goBlade;
	[System.NonSerialized] public Blade blade;
	[System.NonSerialized] public GameObject goCamera;	
	[System.NonSerialized] public CameraControl cameraControl;
	[System.NonSerialized] public GameObject goDebug;
	[System.NonSerialized] public GameObject goMain;
	[System.NonSerialized] public Main main; 
	[System.NonSerialized] public GameObject goWebcamPlane;
	[System.NonSerialized] public GameObject goMaskPlane;
	[System.NonSerialized] public GameObject goMaskPlaneBoss;
	[System.NonSerialized] public GameObject goLights;
	[System.NonSerialized] public GameObject []gosLights;
	[System.NonSerialized] public GameObject goDirLightMain;
	[System.NonSerialized] public GameObject goBoss;
	[System.NonSerialized] public Boss boss;
	[System.NonSerialized] public GameObject goRoadSign;	
	
	void Start () {
		DontDestroyOnLoad(this);
		Screen.showCursor=true;
		initInMain();
	}
	
	public void initInMain()		
	{
		if(Application.loadedLevelName!="main"){
			initedInMain=false;
			return;
		}
		if(initedInMain)
			return;
		initedInMain=true;
		print ("initInMain");
		
		goMain=GameObject.Find("_1");
		main=goMain.GetComponent<Main>();
		goRoadSign=GameObject.Find ("roadSign");
		goGirl=GameObject.Find("Girl");
		girl=goGirl.GetComponent<Girl>();
		goBlade=GameObject.Find("Blade");
		blade=goBlade.GetComponent<Blade>();
		goCamera=GameObject.Find("Main Camera");
		cameraControl=goCamera.GetComponent<CameraControl>();
		goWebcamPlane=GameObject.Find("WebcamPlane");
		goMaskPlane=GameObject.Find("MaskPlane");
		goMaskPlaneBoss=GameObject.Find("MaskPlaneBoss");		
		goLights=GameObject.Find("ForestLights");
		goDirLightMain=GameObject.Find("DirLightMain");
		int c=goLights.transform.childCount;
		gosLights=new GameObject[c];
		for(int i=0;i<c;i++){
			gosLights[i]=GameObject.Find("ForestLight"+i);
		}		
		goBoss=GameObject.Find("BossShxtThisIsNotAGoodName");
		boss=goBoss.GetComponent<Boss>();
	}
	
	void Update () {
		initInMain();
		if(!initedInMain)
			return;				
	}
}
