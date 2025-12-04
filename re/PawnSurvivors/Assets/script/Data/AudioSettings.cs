using UnityEngine;

namespace PawnSurvivors.Data
{
    /// <summary>
    /// 오디오 설정을 관리하는 클래스입니다.
    /// PlayerPrefs를 통해 저장/로드됩니다.
    /// </summary>
    [System.Serializable]
    public class GameAudioSettings
    {
        private const string MASTER_VOLUME_KEY = "MasterVolume";
        private const string BGM_VOLUME_KEY = "BGMVolume";
        private const string SFX_VOLUME_KEY = "SFXVolume";
        
        public float masterVolume = 1f;
        public float bgmVolume = 0.3f;
        public float sfxVolume = 0.7f;
        
        /// <summary>
        /// 실제 BGM 볼륨 (마스터 * BGM)
        /// </summary>
        public float EffectiveBGMVolume => masterVolume * bgmVolume;
        
        /// <summary>
        /// 실제 효과음 볼륨 (마스터 * SFX)
        /// </summary>
        public float EffectiveSFXVolume => masterVolume * sfxVolume;
        
        /// <summary>
        /// PlayerPrefs에서 설정을 로드합니다.
        /// </summary>
        public void Load()
        {
            masterVolume = PlayerPrefs.GetFloat(MASTER_VOLUME_KEY, 1f);
            bgmVolume = PlayerPrefs.GetFloat(BGM_VOLUME_KEY, 0.3f);
            sfxVolume = PlayerPrefs.GetFloat(SFX_VOLUME_KEY, 0.7f);
        }
        
        /// <summary>
        /// PlayerPrefs에 설정을 저장합니다.
        /// </summary>
        public void Save()
        {
            PlayerPrefs.SetFloat(MASTER_VOLUME_KEY, masterVolume);
            PlayerPrefs.SetFloat(BGM_VOLUME_KEY, bgmVolume);
            PlayerPrefs.SetFloat(SFX_VOLUME_KEY, sfxVolume);
            PlayerPrefs.Save();
        }
        
        /// <summary>
        /// 마스터 볼륨을 설정합니다.
        /// </summary>
        public void SetMasterVolume(float volume)
        {
            masterVolume = Mathf.Clamp01(volume);
            Save();
        }
        
        /// <summary>
        /// BGM 볼륨을 설정합니다.
        /// </summary>
        public void SetBGMVolume(float volume)
        {
            bgmVolume = Mathf.Clamp01(volume);
            Save();
        }
        
        /// <summary>
        /// 효과음 볼륨을 설정합니다.
        /// </summary>
        public void SetSFXVolume(float volume)
        {
            sfxVolume = Mathf.Clamp01(volume);
            Save();
        }
    }
}

