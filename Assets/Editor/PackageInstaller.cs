using UnityEditor;
using UnityEditor.PackageManager;
using UnityEditor.PackageManager.Requests;
using UnityEngine;

namespace ARS.Editor
{
    public static class PackageInstaller
    {
        private static AddAndRemoveRequest request;
        private static double deadline;

        public static void InstallModelImporter()
        {
            request = Client.AddAndRemove(new[] { "com.unity.cloud.gltfast" }, null);
            deadline = EditorApplication.timeSinceStartup + 600;
            EditorApplication.update += Poll;
        }

        private static void Poll()
        {
            if (request == null || !request.IsCompleted)
            {
                if (EditorApplication.timeSinceStartup > deadline) { Debug.LogError("glTFast installation timed out."); EditorApplication.Exit(2); }
                return;
            }
            EditorApplication.update -= Poll;
            if (request.Status == StatusCode.Success) { Debug.Log("glTFast installed."); EditorApplication.Exit(0); }
            Debug.LogError("glTFast installation failed: " + request.Error?.message);
            EditorApplication.Exit(1);
        }
    }
}
