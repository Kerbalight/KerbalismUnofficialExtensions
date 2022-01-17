using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace KerbalismUnofficialExtensions.Utils
{
	public static class Lib
	{
		public enum LogLevel
		{
			Message,
			Warning,
			Error
		}

		private static void Log(MethodBase method, string message, LogLevel level)
		{
			switch (level)
			{
				default:
					UnityEngine.Debug.Log(string.Format("[KerbalismUE] {0}.{1} {2}", method.ReflectedType.Name, method.Name, message));
					return;
				case LogLevel.Warning:
					UnityEngine.Debug.LogWarning(string.Format("[KerbalismUE] {0}.{1} {2}", method.ReflectedType.Name, method.Name, message));
					return;
				case LogLevel.Error:
					UnityEngine.Debug.LogError(string.Format("[KerbalismUE] {0}.{1} {2}", method.ReflectedType.Name, method.Name, message));
					return;
			}
		}

		///<summary>write a message to the log</summary>
		public static void Log(string message, LogLevel level = LogLevel.Message, params object[] param)
		{
			StackTrace stackTrace = new StackTrace();
			Log(stackTrace.GetFrame(1).GetMethod(), string.Format(message, param), level);
		}

		///<summary>write a message and the call stack to the log</summary>
		public static void LogStack(string message, LogLevel level = LogLevel.Message, params object[] param)
		{
			StackTrace stackTrace = new StackTrace();
			Log(stackTrace.GetFrame(1).GetMethod(), string.Format(message, param), level);
			UnityEngine.Debug.Log(stackTrace);
		}

		///<summary>write a message to the log, only on DEBUG and DEVBUILD builds</summary>
		[Conditional("DEBUG"), Conditional("DEVBUILD")]
		public static void LogDebug(string message, LogLevel level = LogLevel.Message, params object[] param)
		{
			StackTrace stackTrace = new StackTrace();
			Log(stackTrace.GetFrame(1).GetMethod(), string.Format(message, param), level);
		}

		///<summary>write a message and the full call stack to the log, only on DEBUG and DEVBUILD builds</summary>
		[Conditional("DEBUG"), Conditional("DEVBUILD")]
		public static void LogDebugStack(string message, LogLevel level = LogLevel.Message, params object[] param)
		{
			StackTrace stackTrace = new StackTrace();
			Log(stackTrace.GetFrame(1).GetMethod(), string.Format(message, param), level);
			UnityEngine.Debug.Log(stackTrace);
		}

	}
}
