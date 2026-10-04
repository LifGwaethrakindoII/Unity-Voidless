#if UNITY_EDITOR
using UnityEditor.Recorder;
using UnityEditor;
#endif

namespace Voidless
{
	public static class VRecorder
	{
		private static RecorderWindow _window;

		/// <summary>Gets window property.</summary>
		public static RecorderWindow window
		{
			get
			{
				if(_window == null) _window = (RecorderWindow)EditorWindow.GetWindow(typeof(RecorderWindow));
				return _window;
			}
		}

		/// <summary>Used to Start the recording with current settings. If not already the case, the Editor will also switch to PlayMode.</summary>
		public static void StartRecording()
		{
#if UNITY_EDITOR
			if(window != null && !window.IsRecording())
			window.StartRecording();
#endif
		}

		/// <summary>Used to Stop current recordings if any. Exiting PlayMode while the Recorder is recording will automatically Stop the recorder.</summary>
		public static void StopRecording()
		{
#if UNITY_EDITOR
	        if(window != null && window.IsRecording())
	        window.StopRecording();
#endif
		}
	}
}