using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.IO;
using UnityEngine.Timeline;
using UnityEditor;
using System.Threading.Tasks;
using Unity.VisualScripting;
using UnityEngine.UI;


#if !UNITY_EDITOR && UNITY_WSA_10_0
using System;
using System.Threading.Tasks;
using Windows.Storage.Pickers;

#endif

public class UIFilePicker : MonoBehaviour
{
	public Task<string> PickFileAsync()
	{
		var tcs = new TaskCompletionSource<string>();

    // If file picker is running in the hololens
	#if !UNITY_EDITOR && UNITY_WSA_10_0
		UnityEngine.WSA.Application.InvokeOnUIThread(async () =>
		{
			var filepicker = new FileOpenPicker();
			// Edit file extension to alter what filetypes show. use '*' for any filetype
			filepicker.FileTypeFilter.Add(".obj");
			var file = await filepicker.PickSingleFileAsync();
			UnityEngine.WSA.Application.InvokeOnAppThread(() =>
			{
				string path = (file != null) ? file.Path.Replace("\\", "/") : "No data";
				Debug.Log("The Path is " + path);
				GlobalVariables.filePath = path;
				Debug.Log("The GlobalVariables Path is " + GlobalVariables.filePath);
				tcs.SetResult(path); 
			}, false);
		}, false);
		return tcs.Task;

    // If file picker is running in unity editor (usually used for testing purposes)
	#elif UNITY_EDITOR
		// Edit file extension to alter what filetypes show. use '*' for any filetype
		string path = EditorUtility.OpenFilePanel("Pick an object file", "", "obj");
		if (!string.IsNullOrEmpty(path))
		{
			tcs.SetResult(path);
		}
		else
		{
			Debug.Log("No file selected.");
			tcs.SetResult(null);
		}
		return tcs.Task;
	#endif
	}
    
    //Async operation ran, path saved in variable for later use
    public async void activatePickFileAsync()
    {

        string filePath = await PickFileAsync();
        Debug.Log("File path received: " + filePath);
    }
}