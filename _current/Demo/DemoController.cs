using UnityEngine;
using System.Collections;
using System;

public class DemoController : MonoBehaviour {
	public int station;
	public bool undecorate;
	public int ScreenWidth = 1920;
	public int ScreenHeight = 1080;
	public string WindowName;
	public Vector2 ScreenCoordinates;
	public GameObject goOSCController;
	
	IEnumerator Start () {
		// read in args
		string []args=ArgumentHandler.GetCommandLineArguments();	
		if(Application.isEditor){
			station=6;
		}else{
			if(args.Length>1){
				station=int.Parse(args[1]);
			}
		}
		
		// set volume
		if(station==1 || station==6){
			Camera.main.GetComponent<Demo>().setVolume(0.9f);
		}else{
			Camera.main.GetComponent<Demo>().setVolume(0.1f);
		}
		
		// set window
		IntPtr windowHandle = WindowHandler.GetForegroundWindow();
		Screen.SetResolution(ScreenWidth, ScreenHeight, false);	
		yield return null;		
		if(Application.isEditor) {
		}else{
			if(undecorate) {
				WindowHandler.UndecorateAndPlace((int)ScreenCoordinates.x, (int)ScreenCoordinates.y, ScreenWidth, ScreenHeight);
			}else{
				WindowHandler.PlaceWindow((int)ScreenCoordinates.x, (int)ScreenCoordinates.y, ScreenWidth, ScreenHeight);
			}			
			WindowHandler.RenameWindow(windowHandle, WindowName);
		}
				
		print ("finished init");
	}
	
	void Update () {
		if(Input.GetKeyDown(KeyCode.Space)){
			goOSCController.SendMessage("SendOSCMessage","/start");	
		}
	}
	
	string debug;
	void ReceivedOSCmessage(string data)
	{
		print ("receive:"+data);
		debug=data;
	}
	
	void OnGUI()
	{
		GUI.Label(new Rect(10,10,400,400), debug);
	}
}
