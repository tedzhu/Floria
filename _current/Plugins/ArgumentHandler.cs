/* A script that takes care of handling the arguments that may be passed in via command line on runtime.
 * It grabs the passed argument, stores them in an array, and then hands them off.
 * 
 * Place this inside of the Plugins folder within the Assets folder.
 * 
 * Currently, the way the monitor setup works, your batch files should look something like
 * this if you want to run the game with multiple windows:
 * 
 * programName.exe true
 * 
 * The argument after the program name is the true/false statement that checks whether it should launch itself.
 * 
 * The placement of the windows themselves is taken care of within Unity.
 * 
 * Dave Bennett, July 2011
 */

using System;

public class ArgumentHandler {
	
	// Array to hold our command line arguments
	static string[] commArguments = Environment.GetCommandLineArgs();
	
	// Return the array of arguments for use in the monitor script.
	public static string[] GetCommandLineArguments()
	{
		return commArguments;
	}
}
