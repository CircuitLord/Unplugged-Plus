using HarmonyLib;
using MelonLoader;

namespace UnpluggedNoteSmooth {
	
	[HarmonyPatch(typeof(Mp3Player), "Update")]
	public static class Mp3PlayerPatch {

		public static bool Prefix(Mp3Player __instance) {

			if (!GameManager.IsPlaying || GameManager.I.StartSongPercent > 0f) {

				__instance.CurrentTime = __instance.MusicAudioSource.time;

				return false;
			}


			__instance.SongTimeDelayCounter += UnityEngine.Time.deltaTime;

			if (GameManager.I.StartDelay > __instance.SongTimeDelayCounter) {
				__instance.CurrentTime = __instance.SongTimeDelayCounter;
				return false;
			}

			if (__instance.MusicAudioSource.isPlaying) {
				__instance.CurrentTime += UnityEngine.Time.deltaTime * __instance.MusicAudioSource.pitch;
			}
			else {
				__instance.CurrentTime = __instance.MusicAudioSource.time;
			}


			return false;
		}

	}

}