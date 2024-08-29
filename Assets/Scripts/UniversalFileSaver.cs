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


public class SaveModel : MonoBehaviour
{
    public Task<string> SaveFileAsync()
	{
		var tcs = new TaskCompletionSource<string>();

    // If file picker is running in the hololens
	#if !UNITY_EDITOR && UNITY_WSA_10_0
		UnityEngine.WSA.Application.InvokeOnUIThread(async () =>
		{
			var filepicker = new FileSavePicker();
			// Edit file extension to alter what filetypes show. use '*' for any filetype
            filepicker.FileTypeChoices.Add("Obj Files", new List<string>() { ".obj" });
			var file = await filepicker.PickSaveFileAsync();
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
        string path = EditorUtility.SaveFilePanel("Pick a save path", "", "AnnotatedObj", "obj");
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

    public async void activateSaveFileAsync()
    {
        string filePath = await SaveFileAsync();
        Debug.Log("File path received: " + filePath);
    }
}
