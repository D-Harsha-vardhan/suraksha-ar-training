using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.XR.ARFoundation;
using UnityEngine.XR.ARSubsystems;

namespace ARS.App
{
    public sealed class ARTrainingPlacement : MonoBehaviour
    {
        private ARRaycastManager raycastManager;
        private readonly List<ARRaycastHit> hits = new();
        private GameObject scenario;
        public System.Action<bool> OnPlacementChanged;

        private void Awake()
        {
            raycastManager = FindFirstObjectByType<ARRaycastManager>();
            if (raycastManager == null)
            {
                var managers = new GameObject("AR Placement Managers");
                managers.AddComponent<ARPlaneManager>();
                raycastManager = managers.AddComponent<ARRaycastManager>();
            }

            var arCamera = Camera.main ?? FindFirstObjectByType<Camera>();
            if (arCamera != null)
            {
                if (arCamera.GetComponent<ARCameraManager>() == null) arCamera.gameObject.AddComponent<ARCameraManager>();
                if (arCamera.GetComponent<ARCameraBackground>() == null) arCamera.gameObject.AddComponent<ARCameraBackground>();
            }
        }

        private void Update()
        {
            if (!TryGetTap(out var position)) return;
            if (scenario == null) scenario = CreateScenario();
            if (raycastManager != null && raycastManager.Raycast(position, hits, TrackableType.PlaneWithinPolygon))
                scenario.transform.SetPositionAndRotation(hits[0].pose.position, hits[0].pose.rotation);
            else
            {
                var camera = Camera.main ?? FindFirstObjectByType<Camera>();
                if (camera == null) return;
                scenario.transform.SetPositionAndRotation(camera.transform.position + camera.transform.forward * 1.2f, Quaternion.LookRotation(-camera.transform.forward, Vector3.up));
            }
            scenario.SetActive(true);
            OnPlacementChanged?.Invoke(true);
        }

        private static bool TryGetTap(out Vector2 position)
        {
            if (Touchscreen.current != null && Touchscreen.current.primaryTouch.press.wasPressedThisFrame)
            {
                position = Touchscreen.current.primaryTouch.position.ReadValue();
                return true;
            }
            if (Input.touchCount > 0 && Input.GetTouch(0).phase == UnityEngine.TouchPhase.Began)
            {
                position = Input.GetTouch(0).position;
                return true;
            }
            position = default;
            return false;
        }

        private static GameObject CreateScenario()
        {
            var root = new GameObject("Fire Safety Training Scenario");
            var baseObject = GameObject.CreatePrimitive(PrimitiveType.Cylinder); baseObject.transform.SetParent(root.transform); baseObject.transform.localScale = new Vector3(.45f,.05f,.45f); baseObject.GetComponent<Renderer>().material.color = new Color(.12f,.2f,.3f);
            var extinguisher = GameObject.CreatePrimitive(PrimitiveType.Cylinder); extinguisher.transform.SetParent(root.transform); extinguisher.transform.localPosition = new Vector3(0,.45f,0); extinguisher.transform.localScale = new Vector3(.16f,.55f,.16f); extinguisher.GetComponent<Renderer>().material.color = Color.red;
            var handle = GameObject.CreatePrimitive(PrimitiveType.Cube); handle.transform.SetParent(root.transform); handle.transform.localPosition = new Vector3(0,.98f,0); handle.transform.localScale = new Vector3(.22f,.05f,.12f); handle.GetComponent<Renderer>().material.color = Color.black;
            var marker = GameObject.CreatePrimitive(PrimitiveType.Sphere); marker.transform.SetParent(root.transform); marker.transform.localPosition = new Vector3(.35f,.16f,0); marker.transform.localScale = Vector3.one*.18f; marker.GetComponent<Renderer>().material.color = new Color(1f,.45f,0f);
            return root;
        }
    }
}
