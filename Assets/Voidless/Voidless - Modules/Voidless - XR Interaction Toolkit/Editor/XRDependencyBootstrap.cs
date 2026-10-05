using System;
using UnityEditor;
using UnityEditor.PackageManager;
using UnityEditor.PackageManager.Requests;
using UnityEngine;

namespace Voidless.XR.Editor
{
    [InitializeOnLoad]
    public static class XRDependencyBootstrap
    {
        private static bool _hasCheckedDependencies;
        private static AddRequest request;

        static XRDependencyBootstrap()
        {
            // Prevent it from running multiple times in the same session
            if (_hasCheckedDependencies) 
            {
                return;
            }
            
            _hasCheckedDependencies = true;
            CheckAndInstallDependencies();

            //EditorApplication.update += OnUpdate;
        }

        private static void OnUpdate()
        {
            if(request == null) return;

            StatusCode status = request.Status;
            Debug.Log(string.Concat("[XRDependencyBootstrap] Request Operation Status: ", status.ToString()));

            if(status == StatusCode.Success || status == StatusCode.Failure) EditorApplication.update -= OnUpdate;
        }

        private static void CheckAndInstallDependencies()
        {
            // Look for a core class from the XR Interaction Toolkit
            Type xrToolkitType = Type.GetType("UnityEngine.XR.Interaction.Toolkit.XRInteractionManager, Unity.XR.Interaction.Toolkit");
            
            if (xrToolkitType == null)
            {
                Debug.Log("Voidless XR: XR Interaction Toolkit missing. Auto-installing...");
                request = Client.Add("com.unity.xr.interaction.toolkit@3.0.5");
            }
            else
            {
                Debug.Log("Voidless XR: Dependencies are up to date.");
            }
        }
    }
}