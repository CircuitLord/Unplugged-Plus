using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MelonLoader;
using UnpluggedNoteSmooth;
using HarmonyLib;
using UnityEngine;


namespace UnpluggedNoteSmooth {

	
	public class UnpluggedPlus : MelonMod {

		private const string PREF_CATAGORY = "UnpluggedPlus";

		private const string PREF_SMOOTH_CAMERA = "SmoothCamera";
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
			
			harmony = new HarmonyLib.Harmony("com.github.circuitlord");

			MelonPreferences.Load();

			MelonPreferences.CreateCategory(PREF_CATAGORY, "Settings");
			MelonPreferences.CreateEntry(PREF_CATAGORY, PREF_SMOOTH_CAMERA, false, "Smooth Camera");
			MelonPreferences.CreateEntry(PREF_CATAGORY, PREF_FIX_NOTE_JITTER, true, "Fix Note Jitter");

			
			smoothCameraEnabled = MelonPreferences.GetEntryValue<bool>(PREF_CATAGORY, PREF_SMOOTH_CAMERA);
			noteJitterFixEnabled = MelonPreferences.GetEntryValue<bool>(PREF_CATAGORY, PREF_FIX_NOTE_JITTER);

			LoggerInstance.Msg("Unplugged Note Smooth initialized!");

			if (smoothCameraEnabled) {
				LoggerInstance.Msg("Smooth camera is enabled!");
			}

			if (noteJitterFixEnabled) {
				harmony.Patch(
					typeof(Mp3Player).GetMethod("Update"),
					new HarmonyMethod(typeof(Mp3PlayerPatch).GetMethod("Prefix"))
					);
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
			lerp.followPosStrength = 0.8f;
			lerp.followRotStrength = 0.2f;

			cameraSmoothCamera = cameraGO.AddComponent<Camera>();


			cameraSmoothCamera.stereoTargetEye = StereoTargetEyeMask.None;
			cameraSmoothCamera.targetDisplay = 0;
			cameraSmoothCamera.clearFlags =   Camera.main.clearFlags;
			cameraSmoothCamera.backgroundColor =   Camera.main.backgroundColor;
			cameraSmoothCamera.nearClipPlane = 0.1f;
			cameraSmoothCamera.farClipPlane =  Camera.main.farClipPlane;
			cameraSmoothCamera.fieldOfView = 75;
		}

	}

}