using UnityEngine;

namespace PawnSurvivors.Utilities
{
    /// <summary>
    /// 오디오 재생 관련 유틸리티 함수 모음입니다.
    /// Resources/Audio 폴더의 사운드를 재생합니다.
    /// </summary>
    public static class AudioHelper
    {
        /// <summary>
        /// 사운드를 재생합니다.
        /// </summary>
        /// <param name="audioPath">Resources 폴더 기준 경로 (예: "Audio/Hit0")</param>
        /// <param name="position">재생 위치 (3D 사운드)</param>
        /// <param name="volume">볼륨 (0~1, 실제 볼륨은 효과음 설정 * 이 값)</param>
        public static void PlaySound(string audioPath, Vector3 position = default, float volume = 1f)
        {
            if (string.IsNullOrEmpty(audioPath)) return;
            
            AudioClip clip = Resources.Load<AudioClip>(audioPath);
            if (clip == null)
            {
                Debug.LogWarning($"[AudioHelper] AudioClip '{audioPath}' not found in Resources.");
                return;
            }
            
            // 효과음 설정 적용
            float effectiveVolume = volume;
            if (GameManager.Instance?.AudioSettings != null)
            {
                effectiveVolume *= GameManager.Instance.AudioSettings.EffectiveSFXVolume;
            }
            
            AudioSource.PlayClipAtPoint(clip, position, effectiveVolume);
        }
        
        /// <summary>
        /// 여러 사운드 중 랜덤으로 하나를 재생합니다.
        /// </summary>
        /// <param name="audioPaths">사운드 경로 배열</param>
        /// <param name="position">재생 위치</param>
        /// <param name="volume">볼륨 (0~1, 실제 볼륨은 효과음 설정 * 이 값)</param>
        public static void PlayRandomSound(string[] audioPaths, Vector3 position = default, float volume = 1f)
        {
            if (audioPaths == null || audioPaths.Length == 0) return;
            
            int randomIndex = Random.Range(0, audioPaths.Length);
            PlaySound(audioPaths[randomIndex], position, volume);
        }
        
        /// <summary>
        /// UI 사운드를 재생합니다. (위치 없음)
        /// </summary>
        /// <param name="audioPath">Resources 폴더 기준 경로</param>
        /// <param name="volume">볼륨</param>
        public static void PlayUISound(string audioPath, float volume = 1f)
        {
            PlaySound(audioPath, Vector3.zero, volume);
        }
    }
}

