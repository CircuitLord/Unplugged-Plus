using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Harmony;
using HarmonyLib;
using MelonLoader;
using UnpluggedNoteSmooth;
using UnityEngine;


namespace UnpluggedNoteSmooth {

	
	public class UnpluggedPlus : MelonMod {

		private const string PREF_CATAGORY = "UnpluggedPlus";

		private const string PREF_SMOOTH_CAMERA = "SmoothCamera";
		private const string PREF_SMOOTH_CAMERA_ROT_SPEED = "SmoothCameraRotSpeed";
		private const string PREF_SMOOTH_CAMERA_POS_SPEED = "SmoothCameraPosSpeed";
		private const string PREF_SMOOTH_CAMERA_FOV = "SmoothCameraFOV";
		private const string PREF_FIX_NOTE_JITTER = "FixNoteJitter";


		private Camera cameraSmoothCamera;

		private bool smoothCameraEnabled = false;
		private int smoothCameraFOV = 80;
		private float smoothCameraRotationSpeed = 0.15f;
		private float smoothCameraPositionSpeed = 0.7f;

		private bool noteJitterFixEnabled = true;


		private HarmonyLib.Harmony harmony;
		

		public override void OnApplicationStart() {
			base.OnApplicationStart();
			
			MelonPreferences.Load();

			MelonPreferences.CreateCategory(PREF_CATAGORY, "Settings");
			MelonPreferences.CreateEntry(PREF_CATAGORY, PREF_FIX_NOTE_JITTER, true, "Fix Note Jitter");
			MelonPreferences.CreateEntry(PREF_CATAGORY, PREF_SMOOTH_CAMERA, false, "Smooth Camera");
			MelonPreferences.CreateEntry(PREF_CATAGORY, PREF_SMOOTH_CAMERA_ROT_SPEED, 0.15f, "Smooth Camera Rot Speed");
			MelonPreferences.CreateEntry(PREF_CATAGORY, PREF_SMOOTH_CAMERA_POS_SPEED, 0.7f, "Smooth Camera Pos Speed");
			MelonPreferences.CreateEntry(PREF_CATAGORY, PREF_SMOOTH_CAMERA_FOV, 80, "Smooth Camera FOV");


			noteJitterFixEnabled = MelonPreferences.GetEntryValue<bool>(PREF_CATAGORY, PREF_FIX_NOTE_JITTER);
			
			smoothCameraEnabled = MelonPreferences.GetEntryValue<bool>(PREF_CATAGORY, PREF_SMOOTH_CAMERA);
			smoothCameraRotationSpeed = MelonPreferences.GetEntryValue<float>(PREF_CATAGORY, PREF_SMOOTH_CAMERA_ROT_SPEED);
			smoothCameraPositionSpeed = MelonPreferences.GetEntryValue<float>(PREF_CATAGORY, PREF_SMOOTH_CAMERA_POS_SPEED);
			smoothCameraFOV = MelonPreferences.GetEntryValue<int>(PREF_CATAGORY, PREF_SMOOTH_CAMERA_FOV);

			LoggerInstance.Msg("Unplugged Note Smooth initialized!");

			if (smoothCameraEnabled) {
				LoggerInstance.Msg("Smooth camera is enabled!");
			}

			if (noteJitterFixEnabled) {
				
				LoggerInstance.Msg("Note jitter fix enabled!");
				
				var originalUpdate = HarmonyLib.AccessTools.Method(typeof(Mp3Player), "Update");

				var prefix = HarmonyLib.AccessTools.Method(typeof(Mp3PlayerPatch), "Prefix");
				
				HarmonyInstance.Patch(originalUpdate, new HarmonyLib.HarmonyMethod(prefix));

				

			}



		}


		public override void OnSceneWasLoaded(int buildIndex, string sceneName) {
			base.OnSceneWasLoaded(buildIndex, sceneName);

			if (smoothCameraEnabled) {
				MelonCoroutines.Start(ActivateSmoothCamera());
			}
		}

		private IEnumerator ActivateSmoothCamera() {

			var desktop = GameObject.FindObjectOfType<DesktopCamera>();

			if (desktop) {
				desktop.gameObject.SetActive(false);
			}

			if (cameraSmoothCamera != null) GameObject.Destroy(cameraSmoothCamera.gameObject);

			yield return new WaitForSeconds(3f);

			var cameraGO = new GameObject("SmoothCamera");

			var lerp = cameraGO.AddComponent<TransformLerpFollow>();
			lerp.targetTransform =  Camera.main.transform;
			lerp.useLocalPos = false;
			lerp.useLocalRot = false;
			lerp.followPosStrength = smoothCameraPositionSpeed;
			lerp.followRotStrength = smoothCameraRotationSpeed;

			cameraSmoothCamera = cameraGO.AddComponent<Camera>();


			cameraSmoothCamera.stereoTargetEye = StereoTargetEyeMask.None;
			cameraSmoothCamera.targetDisplay = 0;
			cameraSmoothCamera.clearFlags =   Camera.main.clearFlags;
			cameraSmoothCamera.backgroundColor =   Camera.main.backgroundColor;
			cameraSmoothCamera.nearClipPlane = 0.1f;
			cameraSmoothCamera.farClipPlane =  Camera.main.farClipPlane;
			cameraSmoothCamera.fieldOfView = smoothCameraFOV;
		}

	}

}