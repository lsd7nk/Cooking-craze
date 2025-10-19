using System;
using System.Diagnostics;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Text;
using UnityEditor;
using UnityEngine;
using UnityToolbarExtender;
using Debug = UnityEngine.Debug;

namespace CookingPrototype.Utils {
	[InitializeOnLoad]
	public static class RightToolbarButtons {
		static RightToolbarButtons() {
			ToolbarExtender.RightToolbarGUI.Add(OnToolbarGUI);
		}

		static void OnToolbarGUI() {
			GUILayout.FlexibleSpace();

			if ( GUILayout.Button(new GUIContent("Open Project in Fork Git", "Open Fork Git in this project's location"), EditorStyles.toolbarButton) ) {
				try {
					OpenFork();
				}
				finally {
					EditorUtility.ClearProgressBar();
				}
			}
			
			GUILayout.Space(5);
			
			if ( GUILayout.Button(new GUIContent("Pack Project", "Pack zip archive with the project to send to Matryoshka Games"), EditorStyles.toolbarButton) ) {
				try {
					EditorUtility.DisplayProgressBar("Packing project", "Packing project to zip archive...", 0.5f);
					PackArchive();
				}
				finally {
					EditorUtility.ClearProgressBar();
				}
			}
			
		}

		static void OpenFork() {
			
#if UNITY_EDITOR_WIN || UNITY_STANDALONE_WIN
			
			var userFolder = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData);
			var appPath = Path.Combine(userFolder, @"Fork\current\Fork.exe");
			Debug.Log($"Opening fork at: {appPath}");
			Process.Start(new ProcessStartInfo {
				FileName = appPath,
				ArgumentList = { "./" }
			});
#else 
			Debug.LogError("Opening Fork was not implemented for this platform");
#endif
		}

		static void PackArchive() {
			var projectPath = Directory.GetParent(Application.dataPath)?.FullName ?? string.Empty; 
			var outputPath = Path.Combine(projectPath, "TestDeveloper.zip");

			var ignoreDirectories = new[] {
				"Library",
				"Temp",
				"Obj",
				"obj",
				".idea",
				"Logs"
			};
			
			var fullPathIgnoreDirectories = ignoreDirectories
				.Select(x => Path.Combine(projectPath, x))
				.ToArray();

			if ( File.Exists(outputPath) ) {
				File.Delete(outputPath);
			}
			
			using var archive = ZipFile.Open(outputPath, ZipArchiveMode.Create);
			
			foreach (var filePath in Directory.GetFiles(projectPath, "*", SearchOption.AllDirectories)) {
				var anyIgnoredDirectory = fullPathIgnoreDirectories.Any(x => filePath.StartsWith(x));
				if ( anyIgnoredDirectory ) {
					// ignore file
					continue;
				}
				// Skip the output zip file itself
				if (Path.GetFullPath(filePath) == outputPath) {
					continue;
				}
				
				var relativePath = Path.GetRelativePath(projectPath, filePath);
				archive.CreateEntryFromFile(filePath, relativePath);
			}
			
			
			TryOpenDirectory(projectPath);
		}

		static void TryOpenDirectory(string path) {
			try {
				if (string.IsNullOrEmpty(path)) {
					throw new ArgumentException("Path cannot be null or empty", nameof(path));
				}

#if UNITY_EDITOR_WIN || UNITY_STANDALONE_WIN
				Process.Start("explorer.exe", path.Replace("/", "\\"));
#elif UNITY_EDITOR_OSX || UNITY_STANDALONE_OSX
    Process.Start("open", path);
#else
    Debug.LogError("Opening directories is not supported on this platform.");
#endif
			} catch (Exception) {
				// Do nothing
			}
		}
	}
}