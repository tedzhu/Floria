//private var remoteIp : String = "127.0.0.1";	// "127.0.0.1" = LOCAL "255.255.255.255" = BROADCAST
var remoteIp : String = "255.255.255.255";	// "127.0.0.1" = LOCAL "255.255.255.255" = BROADCAST
var sendToPort : int = 8000;
var listenerPort : int = 15308;

var addrs : String[];			// what "/name" message addresses to listen for
var notifyGO : GameObject[];	// what object to notify - the object must have a ReceivedOSCmessage(data:String) function

private var oscHandler : Osc;
private var sendNeeded : boolean;	// Scooping issue - handler has no access to game objects
private var sendToIndex : int;
private var data : String;

function OnDisable () {
	// close OSC UDP socket
    Debug.Log("closing OSC UDP socket in OnDisable");
    oscHandler.Cancel();
    oscHandler = null;
}

function Start() {
        
    var udp : UDPPacketIO = GetComponent("UDPPacketIO");
    udp.init(remoteIp, sendToPort, listenerPort);
        
    oscHandler = GetComponent("Osc");
    oscHandler.init(udp);
        
    // setup handlers
	for (var addr in addrs) {
		oscHandler.SetAddressHandler(addr, RecMessage);
	}
	
	sendNeeded = false;
	sendToIndex = 0;
	data = "";
}

function Update () {
    
    // send a message to the notify object that data has been received
    if ( sendNeeded ) {
    	notifyGO[sendToIndex].SendMessage("ReceivedOSCmessage", data);
    	sendNeeded = false;
    }
}

function SendOSCMessage(data : String) {
	// data is a string with the addr followed by the message parms seperated by " "'s
	// example: "/test1 TRUE 23 0.501 bla" 
	Debug.Log("sending: " + data); 
	var oscM : OscMessage = Osc.StringToOscMessage(data);
    oscHandler.Send(oscM);
}

function RecMessage(m : OscMessage) {

    //Debug.Log("--------------> OSC message received: ("+m+")");
	Debug.Log("--------------> OSC message received > " + Osc.OscMessageToString(m));

	// need the addr index to tell what object needs to be notified
	var i : int;
	for (var a = 0; a < addrs.Length; a ++) {
		if (addrs[a] == m.Address) i = a;  
	}
	
	// save the index and the data, notify Update that a message needs to be sent out
	sendNeeded = true;
	sendToIndex = i;
	data = Osc.OscMessageToString(m);
	
	// >>>> TO DO: Messages could come in so fast that data could be lost - use lists
} 
