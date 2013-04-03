using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Threading;
using PSMoveSharp;

/**
 * 
 * PlayStation Move Wrapper
 * Working with Move.me server on PS3.
 * 
 * Developed by Xun Zhang (lxjk001@gmail.com)
 * 2012.3.14
 * 
 **/

public class PSMoveWrapper : MonoBehaviour {
	
	#region const num
	
	public const int MAX_MOVE_NUM = 4;
	public const int MAX_NAV_NUM = 7;
	
	#endregion
	
	#region const string
	
	public const string SQUARE = "Square";
	public const string CROSS = "Cross";
	public const string CIRCLE = "Circle";
	public const string TRIANGLE = "Triangle";
	public const string MOVE = "Move";
	public const string START = "Start";
	public const string SELECT = "Select";
	public const string T = "T";
	
	public const string NAV_UP = "NavUp";
	public const string NAV_DOWN = "NavDown";
	public const string NAV_LEFT = "NavLeft";
	public const string NAV_RIGHT = "NavRight";
	public const string NAV_CROSS = "NavCross";
	public const string NAV_CIRCLE = "NavCircle";
	public const string NAV_L1 = "NavL1";
	public const string NAV_L2 = "NavL2";
	public const string NAV_L3 = "NavL3";
	#endregion
	
	#region gem state field
	[HideInInspector]
	public Vector3[] position;
	[HideInInspector]
	public Vector3[] velocity;
	[HideInInspector]
	public Vector3[] acceleration;
	
	[HideInInspector]
	public Vector3[] orientation;
	[HideInInspector]
	public Quaternion[] qOrientation;
	[HideInInspector]
	public Vector3[] angularVelocity;
	[HideInInspector]
	public Vector3[] angularAcceleration;
	
	[HideInInspector]
	public Vector3[] handlePosition;
	[HideInInspector]
	public Vector3[] handleVelocity;
	[HideInInspector]
	public Vector3[] handleAcceleration;
	
	[HideInInspector]
	public bool[] isButtonSquare;
	[HideInInspector]
	public bool[] isButtonCross;
	[HideInInspector]
	public bool[] isButtonCircle;
	[HideInInspector]
	public bool[] isButtonTriangle;
	[HideInInspector]
	public bool[] isButtonMove;
	[HideInInspector]
	public bool[] isButtonStart;
	[HideInInspector]
	public bool[] isButtonSelect;
	/// <summary>
	/// range from 0 to 255.
	/// </summary>
	[HideInInspector]
	public int[] valueT;
	/// <summary>
	/// It is used for <c>WasPressed()</c> and <c>WasReleased()</c>.
	/// It represents whether the button TRIGGER can be treated as "pressed".
	/// The default value is 250. This field is modifiable.
	/// </summary>
	public int thresholdT = 250;
	#endregion
	
	#region sphere state field
	[HideInInspector]
	public Color[] sphereColor;
	[HideInInspector]
	public bool[] isTracking;
	[HideInInspector]
	public int[] trackingHue;
	//public int[] hueRequest;
	#endregion
	
	#region rumble field
	[HideInInspector]
	public int[] rumbleLevel;
	#endregion
		
	#region image state field
	[HideInInspector]
	public Vector2[] spherePixelPosition;
	[HideInInspector]
	public float[] spherePixelRadius;
	[HideInInspector]
	public Vector2[] sphereProjectionPosition;
	[HideInInspector]
	public float[] sphereDistance;
	[HideInInspector]
	public bool[] sphereVisible;
	[HideInInspector]
	public bool[] sphereRadiusValid;
	#endregion
	
	
	#region nav data
	[HideInInspector]
	public bool[] isNavUp;
	[HideInInspector]
	public bool[] isNavDown;
	[HideInInspector]
	public bool[] isNavLeft;
	[HideInInspector]
	public bool[] isNavRight;
	
	[HideInInspector]
	public bool[] isNavButtonCross;
	[HideInInspector]
	public bool[] isNavButtonCircle;
	
	[HideInInspector]
	public bool[] isNavButtonL1;
	[HideInInspector]
	public bool[] isNavButtonL3;
	
	/// <summary>
	/// range from -128 to 127
	/// </summary>
	[HideInInspector]
	public int[] valueNavAnalogX;
	/// <summary>
	/// range from -128 to 127
	/// </summary>
	[HideInInspector]
	public int[] valueNavAnalogY;
	/// <summary>
	/// range from 0 to 255
	/// </summary>
	[HideInInspector]
	public int[] valueNavL2;
	public int thresholdNavL2 = 250;
	#endregion
	
	
	#region system field
	
	/// <summary>
	/// It represents whether a single PS Move is connected and calibrated. 
	/// </summary>
	[HideInInspector]
	public bool[] moveConnected;
	[HideInInspector]
	public bool[] navConnected;
	[HideInInspector]
	public int moveCount = 0;
	[HideInInspector]
	public int navCount = 0;
	/// <summary>
	/// It represents the complete state information of Move.me Server and all Move controllers. 
	/// Use it when you need additional information which is not provided by PSMoveWrapper.
	/// </summary>
	public PSMoveSharpState state;
	public PSMoveSharpCameraFrameState cameraFrameState;
	/// <summary>
	/// whether the program connected to Move.me on PS3.
	/// </summary>
	[HideInInspector]
	public bool isConnected = false;
	/// <summary>
	/// whether receiving camera image stream.
	/// </summary>
	[HideInInspector]
	public bool isCameraResume = false;
	public string ipAddress = "128.2.237.237";
	public int port = 7899;
	/// <summary>
	/// If it is false, PSMoveWrapper will try to read the first line from an "IPAddress.txt" file under the same path as the .exe file, and to update the ipAddress.
	/// </summary>
	public bool isFixedIP = false;
	public bool enableDefaultInGameCalibrate = true;
	/// <summary>
	/// If it is false, the Connect function will NOT connect to PS3.
	/// This prevents possible interference with other games testing on Move.me.
	/// You can also use this value to determine whether to activate your hot keys or simulator.
	/// </summary>
	public bool onlineMode = true;
	#endregion
	
	#region for WasPressed and WasReleased
	
	private bool[] isCheckPressSquare;
	private bool[] isCheckPressCross;
	private bool[] isCheckPressCircle;
	private bool[] isCheckPressTriangle;
	private bool[] isCheckPressMove;
	private bool[] isCheckPressStart;
	private bool[] isCheckPressSelect;
	private bool[] isCheckPressT;
	
	private bool[] isCheckPressNavUp;
	private bool[] isCheckPressNavDown;
	private bool[] isCheckPressNavLeft;
	private bool[] isCheckPressNavRight;
	private bool[] isCheckPressNavCross;
	private bool[] isCheckPressNavCircle;
	private bool[] isCheckPressNavL1;
	private bool[] isCheckPressNavL2;
	private bool[] isCheckPressNavL3;
	
	private bool[] isCheckReleaseSquare;
	private bool[] isCheckReleaseCross;
	private bool[] isCheckReleaseCircle;
	private bool[] isCheckReleaseTriangle;
	private bool[] isCheckReleaseMove;
	private bool[] isCheckReleaseStart;
	private bool[] isCheckReleaseSelect;
	private bool[] isCheckReleaseT;
	
	private bool[] isCheckReleaseNavUp;
	private bool[] isCheckReleaseNavDown;
	private bool[] isCheckReleaseNavLeft;
	private bool[] isCheckReleaseNavRight;
	private bool[] isCheckReleaseNavCross;
	private bool[] isCheckReleaseNavCircle;
	private bool[] isCheckReleaseNavL1;
	private bool[] isCheckReleaseNavL2;
	private bool[] isCheckReleaseNavL3;
	
	private Dictionary<string, bool[]> dicIsButton = new Dictionary<string, bool[]>();
	private Dictionary<string, bool[]> dicIsCheckPress = new Dictionary<string, bool[]>();
	private Dictionary<string, bool[]> dicIsCheckRelease = new Dictionary<string, bool[]>();
	private bool[] isButtonT;
	private bool[] isNavButtonL2;
	
	#endregion
	
	
	private const uint PICK_FOR_ME = (4<<24);
    private const uint DONT_TRACK = (2<<24);
	
	private Texture2D imageTex;
	private List<Color32> finalImage;
		
	private static PSMoveClientThreadedRead moveClient;
	private static bool client_connected;

	// Use this for initialization
	void Start () {
		DontDestroyOnLoad(gameObject);
		
		if(!isFixedIP) {
			ReadIPAddress();
		}
		
		DeclareArray();
		PrepareButtonCheck();
		imageTex =  new Texture2D(0,0);
		finalImage = new List<Color32>();
		
	}
	
	void Update() {		
		if(enableDefaultInGameCalibrate) {
			if(isConnected) {
				for(int i = 0; i < MAX_MOVE_NUM; i++) {
					if(WasPressed(i, MOVE)) {
						if(sphereColor[i] == new Color(0,0,0,1)) {
							CalibrateAndTrack(i);
						}
						else {
							CalibrateAndTrack(i, sphereColor[i]);
						}
					}
					if(WasPressed(i, SELECT)) {
						Reset(i);
					}
				}
			}
		}
		
	}
	
	// Update is called once per frame
	void FixedUpdate () {
		
		if(isConnected) {
			UpdateState();			
		}
	}	
	
	private void DeclareArray() {
		//for move
		position = new Vector3[MAX_MOVE_NUM];
		velocity = new Vector3[MAX_MOVE_NUM];
		acceleration = new Vector3[MAX_MOVE_NUM];
		orientation = new Vector3[MAX_MOVE_NUM];
		qOrientation = new Quaternion[MAX_MOVE_NUM];
		angularVelocity = new Vector3[MAX_MOVE_NUM];
		angularAcceleration = new Vector3[MAX_MOVE_NUM];		
		handlePosition = new Vector3[MAX_MOVE_NUM];
		handleVelocity = new Vector3[MAX_MOVE_NUM];
		handleAcceleration = new Vector3[MAX_MOVE_NUM];
		sphereColor = new Color[MAX_MOVE_NUM];
		//hueRequest = new int[MAX_MOVE_NUM];
		spherePixelPosition = new Vector2[MAX_MOVE_NUM];
		sphereProjectionPosition = new Vector2[MAX_MOVE_NUM];
		for(int i = 0; i<MAX_MOVE_NUM; i++) {
			position[i] = new Vector3();
			velocity[i] = new Vector3();
			acceleration[i] = new Vector3();
			orientation[i] = new Vector3();
			qOrientation[i] = new Quaternion ();
			angularVelocity[i] = new Vector3();
			angularAcceleration[i] = new Vector3();
			handlePosition[i] = new Vector3();
			handleVelocity[i] = new Vector3();
			handleAcceleration[i] = new Vector3();
			sphereColor[i] = new Color(0,0,0,1);
			//hueRequest[i] = Convert.ToInt32(PICK_FOR_ME);
			spherePixelPosition[i] = new Vector2();
			sphereProjectionPosition[i] = new Vector2();
		}
		
		isButtonSquare = new bool[MAX_MOVE_NUM];
		isButtonCross = new bool[MAX_MOVE_NUM];
		isButtonCircle = new bool[MAX_MOVE_NUM];
		isButtonTriangle = new bool[MAX_MOVE_NUM];
		isButtonMove = new bool[MAX_MOVE_NUM];
		isButtonStart = new bool[MAX_MOVE_NUM];
		isButtonSelect = new bool[MAX_MOVE_NUM];
		
		valueT = new int[MAX_MOVE_NUM];
		
		moveConnected = new bool[MAX_MOVE_NUM];
		
		isTracking = new bool[MAX_MOVE_NUM];
		trackingHue = new int[MAX_MOVE_NUM];
		rumbleLevel = new int[MAX_MOVE_NUM];
		
		spherePixelRadius = new float[MAX_MOVE_NUM];
		sphereDistance = new float[MAX_MOVE_NUM];
		sphereVisible = new bool[MAX_MOVE_NUM];
		sphereRadiusValid = new bool[MAX_MOVE_NUM];
		
		isCheckPressSquare = new bool[MAX_MOVE_NUM];
		isCheckPressCross = new bool[MAX_MOVE_NUM];
		isCheckPressCircle = new bool[MAX_MOVE_NUM];
		isCheckPressTriangle = new bool[MAX_MOVE_NUM];
		isCheckPressMove = new bool[MAX_MOVE_NUM];
		isCheckPressStart = new bool[MAX_MOVE_NUM];
		isCheckPressSelect = new bool[MAX_MOVE_NUM];
		isCheckPressT = new bool[MAX_MOVE_NUM];
		
		isCheckReleaseSquare = new bool[MAX_MOVE_NUM];
		isCheckReleaseCross = new bool[MAX_MOVE_NUM];
		isCheckReleaseCircle = new bool[MAX_MOVE_NUM];
		isCheckReleaseTriangle = new bool[MAX_MOVE_NUM];
		isCheckReleaseMove = new bool[MAX_MOVE_NUM];
		isCheckReleaseStart = new bool[MAX_MOVE_NUM];
		isCheckReleaseSelect = new bool[MAX_MOVE_NUM];
		isCheckReleaseT = new bool[MAX_MOVE_NUM];
		
		isButtonT = new bool[MAX_MOVE_NUM];
		
		//for nav
		navConnected = new bool[MAX_NAV_NUM];
		isNavUp = new bool[MAX_NAV_NUM];
		isNavDown = new bool[MAX_NAV_NUM];
		isNavLeft = new bool[MAX_NAV_NUM];
		isNavRight = new bool[MAX_NAV_NUM];
	
		isNavButtonCross = new bool[MAX_NAV_NUM];
		isNavButtonCircle = new bool[MAX_NAV_NUM];
	
		isNavButtonL1 = new bool[MAX_NAV_NUM];
		isNavButtonL3 = new bool[MAX_NAV_NUM];
	
		valueNavAnalogX = new int[MAX_NAV_NUM];
		valueNavAnalogY = new int[MAX_NAV_NUM];
		valueNavL2 = new int[MAX_NAV_NUM];
		
		isCheckPressNavUp = new bool[MAX_NAV_NUM];
		isCheckPressNavDown = new bool[MAX_NAV_NUM];
		isCheckPressNavLeft = new bool[MAX_NAV_NUM];
		isCheckPressNavRight = new bool[MAX_NAV_NUM];
		isCheckPressNavCross = new bool[MAX_NAV_NUM];
		isCheckPressNavCircle = new bool[MAX_NAV_NUM];
		isCheckPressNavL1 = new bool[MAX_NAV_NUM];
		isCheckPressNavL2 = new bool[MAX_NAV_NUM];
		isCheckPressNavL3 = new bool[MAX_NAV_NUM];
		
		isCheckReleaseNavUp = new bool[MAX_NAV_NUM];
		isCheckReleaseNavDown = new bool[MAX_NAV_NUM];
		isCheckReleaseNavLeft = new bool[MAX_NAV_NUM];
		isCheckReleaseNavRight = new bool[MAX_NAV_NUM];
		isCheckReleaseNavCross = new bool[MAX_NAV_NUM];
		isCheckReleaseNavCircle = new bool[MAX_NAV_NUM];
		isCheckReleaseNavL1 = new bool[MAX_NAV_NUM];
		isCheckReleaseNavL2 = new bool[MAX_NAV_NUM];
		isCheckReleaseNavL3 = new bool[MAX_NAV_NUM];
		
		isNavButtonL2 = new bool[MAX_NAV_NUM];
	}
	
	private void PrepareButtonCheck() {
		dicIsButton[SQUARE] = isButtonSquare;
		dicIsCheckPress[SQUARE] = isCheckPressSquare;
		dicIsCheckRelease[SQUARE] = isCheckReleaseSquare;
		
		dicIsButton[CROSS] = isButtonCross;
		dicIsCheckPress[CROSS] = isCheckPressCross;
		dicIsCheckRelease[CROSS] = isCheckReleaseCross;
				
		dicIsButton[CROSS] = isButtonCross;
		dicIsCheckPress[CROSS] = isCheckPressCross;
		dicIsCheckRelease[CROSS] = isCheckReleaseCross;
				
		dicIsButton[CIRCLE] = isButtonCircle;
		dicIsCheckPress[CIRCLE] = isCheckPressCircle;
		dicIsCheckRelease[CIRCLE] = isCheckReleaseCircle;
		
		dicIsButton[TRIANGLE] = isButtonTriangle;
		dicIsCheckPress[TRIANGLE] = isCheckPressTriangle;
		dicIsCheckRelease[TRIANGLE] = isCheckReleaseTriangle;
		
		dicIsButton[TRIANGLE] = isButtonTriangle;
		dicIsCheckPress[TRIANGLE] = isCheckPressTriangle;
		dicIsCheckRelease[TRIANGLE] = isCheckReleaseTriangle;
		
		dicIsButton[MOVE] = isButtonMove;
		dicIsCheckPress[MOVE] = isCheckPressMove;
		dicIsCheckRelease[MOVE] = isCheckReleaseMove;
		
		dicIsButton[START] = isButtonStart;
		dicIsCheckPress[START] = isCheckPressStart;
		dicIsCheckRelease[START] = isCheckReleaseStart;
		
		dicIsButton[SELECT] = isButtonSelect;
		dicIsCheckPress[SELECT] = isCheckPressSelect;
		dicIsCheckRelease[SELECT] = isCheckReleaseSelect;
		
		dicIsButton[T] = isButtonT;
		dicIsCheckPress[T] = isCheckPressT;
		dicIsCheckRelease[T] = isCheckReleaseT;
		
		dicIsButton[NAV_UP] = isNavUp;
		dicIsCheckPress[NAV_UP] = isCheckPressNavUp;
		dicIsCheckRelease[NAV_UP] = isCheckReleaseNavUp;
	
		dicIsButton[NAV_DOWN] = isNavDown;
		dicIsCheckPress[NAV_DOWN] = isCheckPressNavDown;
		dicIsCheckRelease[NAV_DOWN] = isCheckReleaseNavDown;
		
		dicIsButton[NAV_LEFT] = isNavLeft;
		dicIsCheckPress[NAV_LEFT] = isCheckPressNavLeft;
		dicIsCheckRelease[NAV_LEFT] = isCheckReleaseNavLeft;
		
		dicIsButton[NAV_RIGHT] = isNavRight;
		dicIsCheckPress[NAV_RIGHT] = isCheckPressNavRight;
		dicIsCheckRelease[NAV_RIGHT] = isCheckReleaseNavRight;
		
		dicIsButton[NAV_CROSS] = isNavButtonCross;
		dicIsCheckPress[NAV_CROSS] = isCheckPressNavCross;
		dicIsCheckRelease[NAV_CROSS] = isCheckReleaseNavCross;
		
		dicIsButton[NAV_CIRCLE] = isNavButtonCircle;
		dicIsCheckPress[NAV_CIRCLE] = isCheckPressNavCircle;
		dicIsCheckRelease[NAV_CIRCLE] = isCheckReleaseNavCircle;
		
		dicIsButton[NAV_L1] = isNavButtonL1;
		dicIsCheckPress[NAV_L1] = isCheckPressNavL1;
		dicIsCheckRelease[NAV_L1] = isCheckReleaseNavL1;
		
		dicIsButton[NAV_L2] = isNavButtonL2;
		dicIsCheckPress[NAV_L2] = isCheckPressNavL2;
		dicIsCheckRelease[NAV_L2] = isCheckReleaseNavL2;
		
		dicIsButton[NAV_L3] = isNavButtonL3;
		dicIsCheckPress[NAV_L3] = isCheckPressNavL3;
		dicIsCheckRelease[NAV_L3] = isCheckReleaseNavL3;
		
		
		for(int i = 0; i < MAX_MOVE_NUM; i++) {
			WasReleased(i, SQUARE);
			WasReleased(i, CIRCLE);
			WasReleased(i, CROSS);
			WasReleased(i, TRIANGLE);
			WasReleased(i, MOVE);
			WasReleased(i, START);
			WasReleased(i, SELECT);
			WasReleased(i, T);
		}
		
		for(int i = 0; i < MAX_NAV_NUM; i++) {
			WasReleased(i, NAV_UP);
			WasReleased(i, NAV_DOWN);
			WasReleased(i, NAV_LEFT);
			WasReleased(i, NAV_RIGHT);
			WasReleased(i, NAV_CROSS);
			WasReleased(i, NAV_CIRCLE);
			WasReleased(i, NAV_L1);
			WasReleased(i, NAV_L2);
			WasReleased(i, NAV_L3);
		}
	}
	
	/// <summary>
	/// the same as <c>Connect(ipAdress, port)"</c>.
	/// </summary>
	public void Connect() {		
		Connect(ipAddress, port);
	}
	
	public void Connect(string address, int port) {
		Debug.Log("ipaddress="+address);
		if(!onlineMode) {
			Debug.LogWarning("onlineMode == false, running in offline mode!");
			return;
		}
		PSMoveWrapper.client_connect(address, port);
		isConnected = PSMoveWrapper.client_connected;
		Debug.Log("Connect");		
	}
	
	/// <summary>
	/// Pause camera stream; Reset all move controllers; Disconnect;
	/// If you just want to disconnect, call <c>Disconnect(false)</c>
	/// </summary>
	public void Disconnect() {
		Disconnect(true);
	}
	
	public void Disconnect(bool isCleanUp) {
		if(isCleanUp) {
			CameraFramePause();
			ResetAll();
		}
		PSMoveWrapper.client_disconnect();
		isConnected = false;
		//Debug.Log("Disconnect");
	}
	
	/// <summary>
	/// Resume camera stream and set slice num to 8.
	/// Camera stream is initially paused.
	/// </summary>
	public void CameraFrameResume() {
		CameraFrameResume(8);
	}
	
	/// <summary>
	/// Resume camera stream and set slice num.
	/// Camera stream is initially paused.
	/// </summary>
	/// <param name="sliceNum">
	/// A <see cref="System.Int32"/>, range from 1 to 8
	/// </param>
	public void CameraFrameResume(int sliceNum) {
		if(!isConnected) {
			return;
		}
		sliceNum = Mathf.Clamp(sliceNum, 1, 8);
		PSMoveWrapper.moveClient.CameraFrameResume();
		isCameraResume = true;
		SetCameraFrameSlices(sliceNum);
	}
	
	/// <summary>
	/// Set slice number
	/// </summary>
	/// <param name="sliceNum">
	/// A <see cref="System.Int32"/>, range from 1 to 8
	/// </param>
	public void SetCameraFrameSlices(int sliceNum) {
		if(!isConnected) {
			return;
		}
		PSMoveWrapper.moveClient.CameraFrameSetNumSlices((uint)sliceNum);
		PSMoveWrapper.moveClient.SetNumImageSlices(sliceNum);
	}
	
	public void CameraFramePause() {
		if(!isConnected) {
			return;
		}
		PSMoveWrapper.moveClient.CameraFramePause();
		isCameraResume = false;
	}
	
	/// <summary>
	/// Calibrate the PS Move. The ball will NOT glow after calibration. 
	/// You need to call any one of the "SetColor" methods to make it glow, and the "Track" methods to track.
	/// </summary>
	/// <param name="num">
	/// A <see cref="System.Int32"/>, move controller number
	/// </param>
	public void Calibrate(int num) {
		if(!isConnected) {
			return;
		}
		if (PSMoveWrapper.moveClient != null)
		{
		    PSMoveWrapper.moveClient.CalibrateController(num);
		}
	}
	
	/// <summary>
	/// Set all PS Moves with default color and track with corresponding hue. 
	/// Use this method when you do not care about the color of all PS Moves.
	/// </summary>
	public void TrackAll() {
		if(!isConnected) {
			return;
		}
		if (PSMoveWrapper.moveClient != null)
        {
            PSMoveWrapper.moveClient.TrackAllHues();
        }
	}
	
	/// <summary>
	/// Let PS3 pick color and track the selected PS Move. 
	/// Use this method when you do not care about the color of the ball.
	/// </summary>
	/// <param name="num">
	/// A <see cref="System.Int32"/>, move controller number.
	/// </param>
	public void AutoTrack(int num) {
		if(!isConnected) {
			return;
		}
		trackingHue[num] = Convert.ToInt32(PICK_FOR_ME);
		PSMoveWrapper.moveClient.SendRequestPacket(PSMoveClient.ClientRequest.PSMoveClientRequestTrackHues, 
		                                     Convert.ToUInt32(trackingHue[0]),
		                                     Convert.ToUInt32(trackingHue[1]), 
		                                     Convert.ToUInt32(trackingHue[2]), 
		                                     Convert.ToUInt32(trackingHue[3]));
	}
	
	/// <summary>
	/// Set the color of PS Move's ball.
	/// The PS Eye camera will NOT automatically track after calling this method. 
	/// If you change the color of a tracking PS Move, the tracking will be lost. 
	/// You need to call any one of the "Track" methods to track. 
	/// The minimum step for color is 0.2f for any RGB value, the alpha value is not used.
	/// </summary>
	/// <param name="num">
	/// A <see cref="System.Int32"/>, move controller number.
	/// </param>
	/// <param name="color">
	/// A <see cref="Color"/>
	/// </param>
	public void SetColor(int num, Color color) {
		if(!isConnected) {
			return;
		}
		float r = color.r;
		float g = color.g;
		float b = color.b;
		PSMoveWrapper.moveClient.SendRequestPacket(PSMoveClient.ClientRequest.PSMoveClientRequestForceRGB, Convert.ToUInt32(num), 
		                                     r, g, b);
	}
	
	/// <summary>
	/// Set the tracking hue of PS Move. 
	/// The hue should fit the ball's color to enable tracking.
	/// </summary>
	/// <param name="num">
	/// A <see cref="System.Int32"/>, move controller number.
	/// </param>
	/// <param name="hue">
	/// A <see cref="System.Int32"/>
	/// </param>
	public void SetTrackingHue(int num, int hue) {
		if(!isConnected) {
			return;
		}
		trackingHue[num] = hue;
		PSMoveWrapper.moveClient.SendRequestPacket(PSMoveClient.ClientRequest.PSMoveClientRequestTrackHues, 
		                                     Convert.ToUInt32(trackingHue[0]),
		                                     Convert.ToUInt32(trackingHue[1]), 
		                                     Convert.ToUInt32(trackingHue[2]), 
		                                     Convert.ToUInt32(trackingHue[3]));
	}
	
	
	public void SetColorAndTrack(int num, Color color) {
		SetColor(num, color);
		SetTrackingHue(num, GetHueFromColor(color));
	}
	
	/// <summary>
	/// the same as <c>CalibrateAndTrack(num, 0.8f)</c>
	/// </summary>
	/// <param name="num">
	/// A <see cref="System.Int32"/>
	/// </param>
	public void CalibrateAndTrack(int num) {
		CalibrateAndTrack(num, 0.8f);
	}
	
	/// <summary>
	/// the same as <c>CalibrateAndTrack(num, color, 0.8f)</c>
	/// </summary>
	/// <param name="num">
	/// A <see cref="System.Int32"/>
	/// </param>
	/// <param name="color">
	/// A <see cref="Color"/>
	/// </param>
	public void CalibrateAndTrack(int num, Color color) {
		CalibrateAndTrack(num, color, 0.8f);
	}
	
	/// <summary>
	/// The combination of "Calibrate" and "AutoTrack". 
	/// Since calibration takes time, tracking should be delayed a certain amount of seconds. 
	/// 0.8f seems appropriate after some tests.
	/// </summary>
	/// <param name="num">
	/// A <see cref="System.Int32"/>, move controller number.
	/// </param>
	/// <param name="time">
	/// A <see cref="System.Single"/>, delay time for tracking after calibration.
	/// </param>
	public void CalibrateAndTrack(int num, float time) {
		Calibrate(num);
		StartCoroutine(DelayTrack(num, time));
	}	
	
	/// <summary>
	/// The combination of "Calibrate" and "SetColorAndTrack".
	/// </summary>
	/// <param name="num">
	/// A <see cref="System.Int32"/>, move controller number.
	/// </param>
	/// <param name="color">
	/// A <see cref="Color"/>, the color to set and to track
	/// </param>
	/// <param name="time">
	/// A <see cref="System.Single"/>, delay time for tracking after calibration.
	/// </param>
	public void CalibrateAndTrack(int num, Color color, float time) {
		Calibrate(num);
		StartCoroutine(DelayTrackHue(num, color, time));
	}
	
	private IEnumerator DelayTrack(int num, float time) {
		yield return new WaitForSeconds(time);
		AutoTrack(num);
	}
	
	private IEnumerator DelayTrackHue(int num, Color color, float time) {
		yield return new WaitForSeconds(time);
		SetColorAndTrack(num, color);
	}
	
	public void ResetAll () {
		for(int i =0; i<MAX_MOVE_NUM; i++) {
			Reset(i);
		}
	}
	
	/// <summary>
	/// Reset move controller. 
	/// The ball will not glow and it need to re-calibrate.
	/// </summary>
	/// <param name="num">
	/// A <see cref="System.Int32"/>, move controller number.
	/// </param>
	public void Reset(int num) {
		if(!isConnected) {
			return;
		}
		PSMoveWrapper.moveClient.SendRequestPacket(PSMoveClient.ClientRequest.PSMoveClientRequestControllerReset, Convert.ToUInt32(num));
	}
	
	/// <summary>
	/// Set rumble level. 0 -> not rumble. 19 -> max rumble.
	/// </summary>
	/// <param name="num">
	/// A <see cref="System.Int32"/>, move controller number.
	/// </param>
	/// <param name="level">
	/// A <see cref="System.Int32"/>, rumble level, 0-19.
	/// </param>
	public void SetRumble(int num, int level) {
		if(!isConnected) {
			return;
		}
		level = Mathf.Clamp(level,0,19);
		if(rumbleLevel[num] == level) {
			return;
		}
		rumbleLevel[num] = level;
		//map to actual rumble scale. 0 -> 0; 1-19 -> 70-250.
		if(level!=0) {
			level = level*10 + 60;
		}
		Rumble(num, level);
	}
	
	/// <summary>
	/// send rumble request.
	/// The minimum step of the scale is 10. 
	/// According to the test, 0 is no rumble, 70 is minimum rumble and 250 is maximum rumble. 
	/// Scale 10-60 will not affect anything.
	/// </summary>
	/// <param name="num">
	/// A <see cref="System.Int32"/>
	/// </param>
	/// <param name="rumbleValue">
	/// A <see cref="System.Int32"/>, 0-255
	/// </param>
	private void Rumble(int num, int rumbleValue) {
		//rumbleValue = Mathf.Clamp(rumbleValue, 0, 255);
		PSMoveWrapper.moveClient.SendRequestPacket(PSMoveClient.ClientRequest.PSMoveClientRequestSetRumble, Convert.ToUInt32(num), Convert.ToUInt32(rumbleValue));
	}
	
	/// <summary>
	/// Get camera image.
	/// The image should be 640x480, please check the length before using it.
	/// Also I know it is a weird issue, but after Move.me started, please calibrate at least once to make sure the image stream working smoothly. 
	/// Once you have done this, it will work fine.
	/// </summary>
	/// <returns>
	/// A <see cref="Color32[]"/>, null if not connected or camera stream is paused. 
	/// </returns>
	public Color32[] GetCameraImage() {
		if(!isConnected || !isCameraResume) {
			return null;
		}
		PSMoveSharpState dummyState = new PSMoveSharpState();
		List<byte[]> image = cameraFrameState.GetCameraFrameAndState(ref dummyState);
		finalImage.Clear();
		
		foreach(byte[] slice in image) {
			imageTex.LoadImage(slice);
			finalImage.AddRange(imageTex.GetPixels32());
		}
		
		return finalImage.ToArray();
	}
	
	
	private void UpdateState() {
		
		if (PSMoveWrapper.moveClient == null)
        {
            return;
        }

        state = PSMoveWrapper.moveClient.GetLatestState();
		cameraFrameState = PSMoveWrapper.moveClient.GetLatestCameraFrameState();
		
		moveCount = 0;
		for(int i = 0; i < MAX_MOVE_NUM; i++) {
			
			//sphere state
			UpdateSphereState(i);
			//image state
			UpdateImageState(i);
			//gem state
			UpdateGemState(i);
			//gem status
			moveConnected[i] = (state.gemStatus[i].connected == 1);
			if(moveConnected[i]) {
				moveCount ++;
			}
		}
		navCount = 0;
		for(int i = 0; i < MAX_NAV_NUM; i++) {
			UpdateNavState(i);
			if(navConnected[i]) {
				navCount ++;
			}
		}
	}
	
	private void UpdateGemState(int num)
    {
        PSMoveSharpGemState selected_gem = state.gemStates[num];
		position[num].x = (float)Convert.ToInt32(selected_gem.pos.x)/100;
		position[num].y = (float)Convert.ToInt32(selected_gem.pos.y)/100;
		position[num].z = (float)Convert.ToInt32(selected_gem.pos.z)/100;
		velocity[num].x = (float)Convert.ToInt32(selected_gem.vel.x)/100;
		velocity[num].y = (float)Convert.ToInt32(selected_gem.vel.y)/100;
		velocity[num].z = (float)Convert.ToInt32(selected_gem.vel.z)/100;
		acceleration[num].x = (float)Convert.ToInt32(selected_gem.accel.x)/100;
		acceleration[num].y = (float)Convert.ToInt32(selected_gem.accel.y)/100;
		acceleration[num].z = (float)Convert.ToInt32(selected_gem.accel.z)/100;
		
		Quaternion rotation = new Quaternion(selected_gem.quat.x, selected_gem.quat.y, selected_gem.quat.z, selected_gem.quat.w);
		orientation[num] = rotation.eulerAngles;
		qOrientation[num] = rotation;
		//orientation[num] = Convert.ToInt32((180.0 / Math.PI) * quatToEuler(selected_gem.quat).x);
		angularVelocity[num].x = Convert.ToInt32((180.0 / Math.PI) * selected_gem.angvel.x);
		angularVelocity[num].y = Convert.ToInt32((180.0 / Math.PI) * selected_gem.angvel.y);
		angularVelocity[num].z = Convert.ToInt32((180.0 / Math.PI) * selected_gem.angvel.z);
		angularAcceleration[num].x = Convert.ToInt32((180.0 / Math.PI) * selected_gem.angaccel.x);
		angularAcceleration[num].y = Convert.ToInt32((180.0 / Math.PI) * selected_gem.angaccel.y);
		angularAcceleration[num].z = Convert.ToInt32((180.0 / Math.PI) * selected_gem.angaccel.z);
		
		handlePosition[num].x = (float)Convert.ToInt32(selected_gem.handle_pos.x)/100;
		handlePosition[num].y = (float)Convert.ToInt32(selected_gem.handle_pos.y)/100;
		handlePosition[num].z = (float)Convert.ToInt32(selected_gem.handle_pos.z)/100;
		handleVelocity[num].x = (float)Convert.ToInt32(selected_gem.handle_vel.x)/100;
		handleVelocity[num].y = (float)Convert.ToInt32(selected_gem.handle_vel.y)/100;
		handleVelocity[num].z = (float)Convert.ToInt32(selected_gem.handle_vel.z)/100;
		handleAcceleration[num].x = (float)Convert.ToInt32(selected_gem.handle_accel.x)/100;
		handleAcceleration[num].y = (float)Convert.ToInt32(selected_gem.handle_accel.y)/100;
		handleAcceleration[num].z = (float)Convert.ToInt32(selected_gem.handle_accel.z)/100;
		
		
		isButtonSquare[num] = ((selected_gem.pad.digitalbuttons & PSMoveSharpConstants.ctrlSquare) != 0);
		isButtonCross[num] =  ((selected_gem.pad.digitalbuttons & PSMoveSharpConstants.ctrlCross) != 0);
		isButtonCircle[num] = ((selected_gem.pad.digitalbuttons & PSMoveSharpConstants.ctrlCircle) != 0);
		isButtonTriangle[num] =  ((selected_gem.pad.digitalbuttons & PSMoveSharpConstants.ctrlTriangle) != 0);
		isButtonMove[num] = ((selected_gem.pad.digitalbuttons & PSMoveSharpConstants.ctrlTick) != 0);
	    valueT[num] =  selected_gem.pad.analog_trigger;
		isButtonT[num] = (valueT[num] >= thresholdT);
	    isButtonStart[num] = ((selected_gem.pad.digitalbuttons & PSMoveSharpConstants.ctrlStart) != 0);
	    isButtonSelect[num] = ((selected_gem.pad.digitalbuttons & PSMoveSharpConstants.ctrlSelect) != 0);
    }
	
	private void UpdateSphereState(int num) {
		PSMoveSharpSphereState sphereState = state.sphereStates[num];
		sphereColor[num].r = sphereState.r;
		sphereColor[num].g = sphereState.g;
		sphereColor[num].b = sphereState.b;
		isTracking[num] = (sphereState.tracking == 1);
		trackingHue[num] = (int)sphereState.tracking_hue;		
	}
	
	private void UpdateImageState(int num) {
		PSMoveSharpImageState imageState = state.imageStates[num];
		spherePixelPosition[num].x = imageState.u;
		spherePixelPosition[num].y = imageState.v;
		spherePixelRadius[num] = imageState.r;
		sphereProjectionPosition[num].x = imageState.projectionx;
		sphereProjectionPosition[num].y = imageState.projectiony;
		sphereDistance[num] = imageState.distance/100;
		sphereVisible[num] = (imageState.visible == 1);
		sphereRadiusValid[num] = (imageState.r_valid == 1);
	}
	
	private void UpdateNavState(int num) {
		navConnected[num] = ((state.navInfo.port_status[num] & 0x1)==0x1);
		
		PSMoveSharpNavPadData padData = state.padData[num];
		isNavUp[num] = (padData.button[PSMoveSharpConstants.offsetDigital1] & PSMoveSharpConstants.ctrlUp)!=0;
		isNavDown[num] = (padData.button[PSMoveSharpConstants.offsetDigital1] & PSMoveSharpConstants.ctrlDown)!=0;
		isNavLeft[num] = (padData.button[PSMoveSharpConstants.offsetDigital1] & PSMoveSharpConstants.ctrlLeft)!=0;
		isNavRight[num] = (padData.button[PSMoveSharpConstants.offsetDigital1] & PSMoveSharpConstants.ctrlRight)!=0;
		isNavButtonCross[num] = (padData.button[PSMoveSharpConstants.offsetDigital2] & PSMoveSharpConstants.ctrlCross)!=0;
		isNavButtonCircle[num] = (padData.button[PSMoveSharpConstants.offsetDigital2] & PSMoveSharpConstants.ctrlCircle)!=0;
		isNavButtonL1[num] = (padData.button[PSMoveSharpConstants.offsetDigital2] & PSMoveSharpConstants.ctrlL1)!=0;
		isNavButtonL3[num] = (padData.button[PSMoveSharpConstants.offsetDigital1] & PSMoveSharpConstants.ctrlL3)!=0;
		
		valueNavAnalogX[num] = padData.button[PSMoveSharpConstants.offsetAnalogLeftX] - 128;
		valueNavAnalogY[num] = padData.button[PSMoveSharpConstants.offsetAnalogLeftY] - 128;
		valueNavL2[num] = padData.button[PSMoveSharpConstants.offsetPressL2];
		isNavButtonL2[num] = (valueNavL2[num] >= thresholdNavL2);
	}
	
	/// <summary>
	/// whether a button was pressed after your last call. 
	/// If you call it in every update loop, it will only return true once you pressed a button, and return false while you are holding it.
	/// </summary>
	/// <param name="num">
	/// A <see cref="System.Int32"/>, move controller number or navigation controller number.
	/// </param>
	/// <param name="button">
	/// A <see cref="System.String"/>, example: <c>PSMoveWrapper.SQUARE</c>
	/// </param>
	/// <returns>
	/// A <see cref="System.Boolean"/>
	/// </returns>
	public bool WasPressed(int num, string button) {
		if(!dicIsButton.ContainsKey(button)) {
			return false;
		}
		if(dicIsButton[button][num]) {
			if(dicIsCheckPress[button][num]) {
				return false;
			}
			dicIsCheckPress[button][num] = true;
			//Debug.Log("Pressed " + button);
			return true;
		}
		dicIsCheckPress[button][num] = false;
		return false;
	}
	
	
	/// <summary>
	///  whether a button was released after your last call. 
	/// If you call it in every update loop, it will only return true once you released a button, and return false while its status remains.
	/// </summary>
	/// <param name="num">
	/// A <see cref="System.Int32"/>, move controller number or navigation controller number
	/// </param>
	/// <param name="button">
	/// A <see cref="System.String"/>, example: <c>PSMoveWrapper.SQUARE</c>
	/// </param>
	/// <returns>
	/// A <see cref="System.Boolean"/>
	/// </returns>
	public bool WasReleased(int num, string button) {
		if(!dicIsButton.ContainsKey(button)) {
			return false;
		}
		if(!dicIsButton[button][num]) {
			if(dicIsCheckRelease[button][num]) {
				return false;
			}
			dicIsCheckRelease[button][num] = true;
			//Debug.Log("Released " + button);
			return true;
		}
		dicIsCheckRelease[button][num] = false;
		return false;
	}
	
	
	public void OnApplicationQuit() {
		Disconnect();
	}
	
	
	private void ReadIPAddress() {
		try {
			StreamReader inFile = new StreamReader("IPAddress.txt");
			ipAddress = inFile.ReadLine();
			inFile.Close();
		}
		catch(Exception) {
			Debug.LogWarning("IP address file not found");
		}
	}
	
	public int GetHueFromColor(Color color) {
		float r, g, b;
		int h = 0;
		r = color.r;
		g = color.g;
		b = color.b;
		if(r >= g && g >= b) {
			h = (int)(60 * GetFraction(r,g,b));
		}
		else if(g > r && r >= b) {
			h = (int)(60 * (2-GetFraction(g,r,b)));
		}
		else if(g >= b && b > r) {
			h = (int)(60 * (2+GetFraction(g,b,r)));
		}
		else if(b > g && g > r) {
			h = (int)(60 * (4-GetFraction(b,g,r)));
		}
		else if(b > r && r >= g) {
			h = (int)(60 * (4+GetFraction(b,r,g)));
		}
		else if(r >= b && b > g) {
			h = (int)(60 * (6-GetFraction(r,b,g)));
		}
		return h;
	}
	
	private float GetFraction(float h, float m, float l) {
		if(h == l) {
			return 0;
		}
		return (m-l)/(h-l);
	}
	
	public Float4 quatToEuler(Float4 q)
    {
        Float4 euler;

        euler.y = Convert.ToSingle(Math.Asin(2.0 * ((q.x * q.z) - (q.w * q.y))));

        if (euler.y == 90.0)
        {
            euler.x = Convert.ToSingle(2.0 * Math.Atan2(q.x, q.w));
            euler.z = 0;
        }
        else if (euler.y == -90.0)
        {
            euler.x = Convert.ToSingle(-2.0 * Math.Atan2(q.x, q.w));
            euler.z = 0;
        }
        else
        {
            euler.x = Convert.ToSingle(Math.Atan2(2.0 * ((q.x * q.y) + (q.z * q.w)), 1.0 - (2.0 * ((q.y * q.y) + (q.z * q.z)))));                
            euler.z = Convert.ToSingle(Math.Atan2(2.0 * ((q.x * q.w) + (q.y * q.z)), 1.0 - (2.0 * ((q.z * q.z) + (q.w * q.w)))));
        }

        euler.w = 0;

        return euler;
    }
	
	
	private static void client_connect(String server_address, int server_port)
    {
        moveClient = new PSMoveClientThreadedRead();

        try
        {
            moveClient.Connect(Dns.GetHostAddresses(server_address)[0].ToString(), server_port);
            moveClient.StartThread();
        }
        catch
        {
            
            return;
        }
		
		client_connected = true;

    }
	
	private static void client_disconnect()
    {
        try
        {

            moveClient.StopThread();
            moveClient.Close();
        }
        catch
        {
            return;
        }
		
		client_connected = false;
    }
	
}
