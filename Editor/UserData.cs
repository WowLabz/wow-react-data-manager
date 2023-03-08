using System;
using System.Collections.Generic;
using Unity.VisualScripting;

[Serializable]
public class UserData
{
	public ulong balance;
	public string _id;
	public string name;
	public string accountId;
	public string avatarUrl;
	public List<int> currentNft = new() { -1, -1 };
	public List<int> selectedLocalNft = new() { -1, -1 };

	public UserData()
	{
		balance = 999000;
		_id = "63a420c65e13ea71d8bfe109";
		name = "John Doe";
		accountId = "5D7sGJu7iCXMNbTdHBDt3irFydbPaaSM5HUPqjiV1RtPSNfx";
		avatarUrl = "https://api.readyplayer.me/v1/avatars/637615ac5764c3e56af9d52e.glb";
		currentNft = new List<int>() { -1, -1 };
		selectedLocalNft = new() { -1, -1 };
	}
	public string GetFormattedBalance()
	{
		string[] displayUnits = { "mS", "S", "KS", "MS", "BS", "TS" };
		var unitIndex = 0;
		var tempBalance = balance;
		while (tempBalance >= 1000 && unitIndex < displayUnits.Length - 1)
		{
			unitIndex++;
			tempBalance /= 1000;
		}
		return tempBalance + " " + displayUnits[unitIndex];
	}
}
