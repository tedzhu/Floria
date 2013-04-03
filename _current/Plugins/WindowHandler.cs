/* A simple script that handles all of the window operations we'll want to perform for Unity.
 * 
 * You should be able to place this in your Scripts folder, but if you run into compiling issues, place it
 * inside of Plugins.
 * 
 * Props to Jake Jeffery and Lilian Chan for the Cave Demo this was based off of.
 * 
 * Dave Bennett, July 2011
 */

using UnityEngine;
using System.Collections;
using System.Diagnostics;
using System;
using System.Runtime.InteropServices;


public class WindowHandler : MonoBehaviour
{
	#region Win32 variables
	const uint SWP_NOMOVE = 0X2;
    const uint SWP_NOSIZE = 1;
    const uint SWP_NOZORDER = 0X4;
    const uint SWP_SHOWWINDOW = 0x0040; 
	const uint SWP_HIDEWINDOW = 0x0080;
	const uint SWP_FRAMECHANGED = 0x0020;
	const long SWP_WSVISIBLE = 0x10000000L;
	const long SWP_BORDER = 0x00800000L;
	
	const int SW_MAXIMIZE = 3;
	const int SW_MINIMIZE = 6;
	const int SW_RESTORE = 9;
	#endregion
	
	#region Win32 import
	[DllImport("user32.dll")]
	static extern bool SetWindowPos(IntPtr hWnd, int hWndInsertAfter, int X, int Y, int cx, int cy, uint uFlags);
	[DllImport("user32.dll")]
	public static extern IntPtr GetForegroundWindow();
	[DllImport("user32.dll")]
	public static extern bool SetForegroundWindow(IntPtr hWnd);
	[DllImport("user32.dll")]
	public static extern bool ShowWindow(IntPtr hWnd, int command);
	[DllImport("user32.dll")]
	public static extern IntPtr SetActiveWindow(IntPtr hWnd);
	[DllImport("user32.dll")]
	static extern long	SetWindowLong(IntPtr hWnd, int nIndex, long dwNewLong);
	[DllImport("user32.dll", EntryPoint = "SetWindowText")]
    public static extern bool SetWindowText(System.IntPtr hwnd, System.String lpString);
	[DllImport("user32.dll")]
	public static extern bool MoveWindow(IntPtr hWnd, int x, int y, int width, int height, bool repaint);
	
	#endregion
	
	// Grabs the foreground window, undecorates it, and then places it at the location specified.
	static public void UndecorateAndPlace(int xPos, int yPos, int width, int height)
	{
		SetWindowLong(GetForegroundWindow(),-16,SWP_WSVISIBLE|SWP_BORDER);
		SetWindowPos(GetForegroundWindow(),0,xPos,yPos,width,height,SWP_NOZORDER | SWP_SHOWWINDOW | SWP_FRAMECHANGED);
	}
	
	// Function that simply places the window without undecorating.
	static public void PlaceWindow(int xPos, int yPos, int width, int height)
	{
		SetWindowPos(GetForegroundWindow(),0,xPos,yPos,width,height,SWP_NOZORDER | SWP_SHOWWINDOW | SWP_FRAMECHANGED);
	}
	
	// Function that minimizes the window to the task bar.
	static public bool MinWindow(IntPtr windowID)
	{
		return ShowWindow(windowID, SW_MINIMIZE);
	}
	
	// Function taht restores the window to original size / position, returning focus.
	static public bool RestoreWindow(IntPtr windowID)
	{
		return ShowWindow(windowID, SW_RESTORE);
	}
	
	// Function that lets you rename the window
	static public void RenameWindow(IntPtr windowID, string windowName)
	{
		SetWindowText(windowID, windowName);
	}
}


