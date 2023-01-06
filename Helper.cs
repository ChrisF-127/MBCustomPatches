using HarmonyLib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TaleWorlds.Library;

namespace CustomPatches
{
	internal static class Helper
	{
		internal static void Message(string s, bool stacktrace = true, Color? color = null)
		{
			FileLog.Log(s + (stacktrace ? $"\n{Environment.StackTrace}" : ""));

			try
			{
				InformationManager.DisplayMessage(new InformationMessage(s, color ?? new Color(1f, 0f, 0f)));
			}
			catch
			{
			}
		}
	}
}
