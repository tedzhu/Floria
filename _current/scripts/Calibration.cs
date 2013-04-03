#pragma warning disable 0168 // variable declared but not used.
#pragma warning disable 0219 // variable assigned but not used.
#pragma warning disable 0414 // private field assigned but not used.
using UnityEngine;
using System.Collections;
using System.IO;
using System;


public class Calibration : MonoBehaviour {
	string webcamName="EE U2 CAM";	
	public bool WebcamOn=true;
	public bool PSMoveOn=true;
	public GameObject goWebcamPlane;
	WebCamTexture wct;

	bool showWebcamView=false;
	public GameObject goText;
	public GameObject goValue;

	bool hasDevice=false;
	
	void Start () {
		Screen.showCursor=false;
		
		// readin configure file
		try{
			StreamReader inFile=new StreamReader("params.txt");
			webcamName=inFile.ReadLine();
			PSMoveOn=inFile.ReadLine()!="";
			WebcamOn=inFile.ReadLine()!="";
			inFile.Close();
		}catch (Exception){
			print ("wrong read file");
			webcamName="EE U2 CAM";
			PSMoveOn=true;
			WebcamOn=true;
		}
		
		PlayerPrefs.SetString("webcamName",webcamName);
		PlayerPrefs.SetInt("PSMoveOn",PSMoveOn?1:0);
		PlayerPrefs.SetInt("WebcamOn",WebcamOn?1:0);
				
		// webcam	
		if(WebcamOn){
			wct=new WebCamTexture();
			print (WebCamTexture.devices.Length);
			
			if(WebCamTexture.devices.Length==1)
				webcamName=WebCamTexture.devices[0].name;
			
			for(int i=0;i<WebCamTexture.devices.Length;i++){
				print (WebCamTexture.devices[i].name);
				if(WebCamTexture.devices[i].name==webcamName){
					hasDevice=true;
				}
			}
			
			if(hasDevice){
				wct.deviceName=webcamName;
				wct.requestedWidth=320;
				wct.requestedHeight=240;
				goWebcamPlane.renderer.material.mainTexture=wct;							
				wct.Play();
				print(wct.width+" "+wct.height);								
				
			}else{
				
			}

		}else{
			Application.LoadLevel("OP");
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
	
	float maxLight=0;
	// Update is called once per frame
	void Update () {
		if(WebcamOn){
			if(hasDevice){
				Color []cmat=wct.GetPixels();
				float currentLight=sumMat(cmat); 			
				if(currentLight>maxLight)maxLight=currentLight;			
				string s=(currentLight.ToString()+ " ( max: "+maxLight+" )");
				goValue.GetComponent<TextMesh>().text=s;
			}else{
				goWebcamPlane.renderer.enabled=false;
				goText.GetComponent<TextMesh>().text="Camera Not Connected!";
				goValue.GetComponent<TextMesh>().text="";
			}
		}
	
		if(Input.GetKeyDown(KeyCode.Space)){			
			PlayerPrefs.SetFloat("threshold",maxLight*0.8f);
			wct.Stop();
			Application.LoadLevel("OP");
		}
	}
}
