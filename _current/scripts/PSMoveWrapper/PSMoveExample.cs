#pragma warning disable 0168 // variable declared but not used.
#pragma warning disable 0219 // variable assigned but not used.
#pragma warning disable 0414 // private field assigned but not used.

using UnityEngine;
using System.Collections;
using System;

public class PSMoveExample : MonoBehaviour {
	
	public PSMoveWrapper psMoveWrapper;
	
	public GameObject gem, handle;
	
	public bool isMirror = true;
	
	public float zOffset = 20;
	Quaternion temp = new Quaternion(0,0,0,0);
	
	
	#region GUI Variables
	string connectStr = "Connect";
	string cameraStr = "Camera Switch On";
	string rStr = "0", gStr = "0", bStr = "0";
	string rumbleStr = "0";
	#endregion
	
	
	
	// Use this for initialization
	void Start () {
	}
	
	void Update() {
	/*	if(psMoveWrapper.wasPressed(0, PSMoveWrapper.CIRCLE)) {
		}*/
	}
	
	// Update is called once per frame
	void FixedUpdate () {
	
		Vector3 gemPos, handlePos;
		gemPos = psMoveWrapper.position[0];
		handlePos = psMoveWrapper.handlePosition[0];
		if(isMirror) {
			gem.transform.localPosition = gemPos;
			handle.transform.localPosition = handlePos;
			handle.transform.localRotation = Quaternion.Euler(psMoveWrapper.orientation[0]);
		}
		else {
			/*
			gemPos.z = -gemPos.z + zOffset;
			handlePos.z = -handlePos.z + zOffset;
			gem.transform.localPosition = gemPos;
			handle.transform.localPosition = handlePos;
			handle.transform.localRotation = Quaternion.LookRotation(gemPos - handlePos);
			handle.transform.Rotate(new Vector3(0,0,psMoveWrapper.orientation[0].z));
			*/
			
		/* using quaternion rotation directly
		 * the rotations on the x and y axes are inverted - i.e. left shows up as right, and right shows up as left. This code fixes this in case 
		 * the object you are using is facing away from the screen. Comment out this code if you do want an inversion along these axes
		 * 
		 * Add by Karthik Krishnamurthy*/
			
			temp = psMoveWrapper.qOrientation[0];
			temp.x = -psMoveWrapper.qOrientation[0].x;
			temp.y = -psMoveWrapper.qOrientation[0].y;
			handle.transform.localRotation = temp;
		
		}
	}
	
	void OnGUI() {
		GUI.Label(new Rect(10, 10, 150, 100),  "PS Move count : " + psMoveWrapper.moveCount);
		GUI.Label(new Rect(140, 10, 150, 100),  "PS Nav count : " + psMoveWrapper.navCount);
		
		if(GUI.Button(new Rect(20, 40, 100, 35), connectStr)) {
			if(connectStr == "Connect") {
				psMoveWrapper.Connect();
				if(psMoveWrapper.isConnected) {
					connectStr = "Disconnect";
				}
			}
			else {
				psMoveWrapper.Disconnect();
				connectStr = "Connect";
				Reset();
			}
		}
		
		if(psMoveWrapper.isConnected) {
			//camera stream on/off
			if(GUI.Button(new Rect(5, 80, 130, 35), cameraStr)) {
				if(cameraStr == "Camera Switch On") {
					psMoveWrapper.CameraFrameResume();
					cameraStr = "Camera Switch Off";
				}
				else {
					psMoveWrapper.CameraFramePause();
					cameraStr = "Camera Switch On";
				}
			}
			
			//color and rumble for move number 0
			if(psMoveWrapper.moveConnected[0]) {
				//Set Color and Track
				GUI.Label(new Rect(300, 50, 200,20), "R,G,B are floats that fall in 0 ~ 1");
				GUI.Label(new Rect(260, 20, 20, 20), "R");
				rStr = GUI.TextField(new Rect(280, 20, 60, 20), rStr);
				GUI.Label(new Rect(350, 20, 20, 20), "G");
				gStr = GUI.TextField(new Rect(370, 20, 60, 20), gStr);
				GUI.Label(new Rect(440, 20, 20, 20), "B");
				bStr = GUI.TextField(new Rect(460, 20, 60, 20), bStr);
				if(GUI.Button(new Rect(550, 30, 160, 35), "SetColorAndTrack")) {
					try {
						float r = float.Parse(rStr);
						float g = float.Parse(gStr);
						float b = float.Parse(bStr);
						psMoveWrapper.SetColorAndTrack(0, new Color(r,g,b));
					}
					catch(Exception e) {
						Debug.Log("input problem");
					}
				}
				//Rumble
				rumbleStr = GUI.TextField(new Rect(805, 20, 40, 20), rumbleStr);
				GUI.Label(new Rect(800, 50, 200,20), "0 ~ 19");
				if(GUI.Button(new Rect(870, 30, 100, 35), "Rumble")) {
					try {
						int rumbleValue = int.Parse(rumbleStr);
						psMoveWrapper.SetRumble(0, rumbleValue);
					}
					catch(Exception e) {
						Debug.Log("input problem");
					}
				}
			}
			
			//move controller information
			for(int i=0; i<PSMoveWrapper.MAX_MOVE_NUM; i++)
			{
				if(psMoveWrapper.moveConnected[i]) {
					string display = "PS Move #" + i + 
						"\nPosition:\t\t"+psMoveWrapper.position[i] + 
						"\nVelocity:\t\t"+psMoveWrapper.velocity[i] + 
						"\nAcceleration:\t\t"+psMoveWrapper.acceleration[i] + 
						"\nOrientation:\t\t"+psMoveWrapper.orientation[i] + 
						"\nAngular Velocity:\t\t"+psMoveWrapper.angularVelocity[i] + 
						"\nAngular Acceleration:\t\t"+psMoveWrapper.angularAcceleration[i] + 
						"\nHandle Position:\t\t"+psMoveWrapper.handlePosition[i] + 
						"\nHandle Velocity:\t\t"+psMoveWrapper.handleVelocity[i] + 
						"\nHandle Acceleration:\t\t"+psMoveWrapper.handleAcceleration[i] +
						"\n" +
						"\nTrigger Value:\t\t" + psMoveWrapper.valueT[i] +
						"\nButtons:\t\t" + GetButtonStr(i) +
						"\nSphere Color:\t\t" + psMoveWrapper.sphereColor[i] +
						"\nIs Tracking:\t\t" + psMoveWrapper.isTracking[i] +
						"\nTracking Hue:\t\t" + psMoveWrapper.trackingHue[i]+
						"\ncansee:\t\t"+psMoveWrapper.sphereVisible[i];
					GUI.Label(new Rect( 10 + 650 * (i/2), 120+310*(i%2), 300, 400),   display);
				}
			}
			for(int j = 0; j < PSMoveWrapper.MAX_NAV_NUM; j++) {
				if(psMoveWrapper.navConnected[j]) {	
					string navDisplay = "PS Nav #" + j + 
						"\nAnalog X:\t\t" + psMoveWrapper.valueNavAnalogX[j] +
						"\nAnalog Y:\t\t" + psMoveWrapper.valueNavAnalogY[j] +
						"\nL2 Value:\t\t" + psMoveWrapper.valueNavL2[j] +
						"\nButtons:\t\t" + GetNavButtonStr(j);
					GUI.Label(new Rect(400, 100 + 95 * j, 150, 95),   navDisplay);
				}
			}
		}
		
		
	}
	
	private string GetButtonStr(int num) {
		string result = "";
		if(psMoveWrapper.isButtonMove[num]) {
			result += "MOVE ";
		}
		if(psMoveWrapper.isButtonCircle[num]) {
			result += "CIRCLE ";
		}
		if(psMoveWrapper.isButtonSquare[num]) {
			result += "SQUARE ";
		}
		if(psMoveWrapper.isButtonCross[num]) {
			result += "CROSS ";
		}
		if(psMoveWrapper.isButtonTriangle[num]) {
			result += "TRIANGLE ";
		}
		if(psMoveWrapper.isButtonStart[num]) {
			result += "START ";
		}
		if(psMoveWrapper.isButtonSelect[num]) {
			result += "SELECT ";
		}
		return result;
	}

	private string GetNavButtonStr(int num) {
		string result = "";
		if(psMoveWrapper.isNavButtonCircle[num]) {
			result += "CIRCLE ";
		}
		if(psMoveWrapper.isNavButtonCross[num]) {
			result += "CROSS ";
		}
		if(psMoveWrapper.isNavUp[num]) {
			result += "UP ";
		}
		if(psMoveWrapper.isNavDown[num]) {
			result += "DOWN ";
		}
		if(psMoveWrapper.isNavLeft[num]) {
			result += "LEFT ";
		}
		if(psMoveWrapper.isNavRight[num]) {
			result += "RIGHT ";
		}
		if(psMoveWrapper.isNavButtonL1[num]) {
			result += "L1 ";
		}
		if(psMoveWrapper.isNavButtonL3[num]) {
			result += "L3 ";
		}
		return result;
	}	
	
	private void Reset() {
		cameraStr = "Camera Switch On";
		rStr = "0"; 
		gStr = "0"; 
		bStr = "0";
		rumbleStr = "0";
	}
}
