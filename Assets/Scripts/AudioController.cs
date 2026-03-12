namespace Blast.Core
{
    using System.Collections.Generic;
    using UnityEngine;

    public class AudioController : MonoBehaviour
    {
        public static AudioController Instance;

        public enum AudioType
        {
            Win,
            Lose,
            Click,
            StartMerge,
            EndMerge,
            Attack,
            CoinCollect,
        }

        [SerializeField] private List<AudioSource> _audioSource;

        [SerializeField] private AudioClip _audioClipWin;
        [SerializeField] private AudioClip _audioClipLose;
        [SerializeField] private AudioClip _audioClipClick;
        [SerializeField] private AudioClip _audioClipMerge;
        [SerializeField] private AudioClip _audioClipEndMerge;
        [SerializeField] private AudioClip _audioClipAttack;
        [SerializeField] private AudioClip _audioClipCoinCollect;

        private int _nextSourceIndex;

        private void Awake()
        {
            Instance = this;
        }

        public void PlayAudio(AudioType audioType)
        {
            AudioSource currentSource = GetNextAudioSource();

            switch (audioType)
            {
                case AudioType.Win:
                    currentSource.PlayOneShot(_audioClipWin);
                    break;
                case AudioType.Lose:
                    currentSource.PlayOneShot(_audioClipLose);
                    break;
                case AudioType.Click:
                    _audioSource[0].clip = _audioClipClick;
                    _audioSource[0].Play();
                    break;
                case AudioType.StartMerge:
                    currentSource.PlayOneShot(_audioClipMerge);
                    break;
                case AudioType.Attack:
                    currentSource.PlayOneShot(_audioClipAttack);
                    break;
                case AudioType.CoinCollect:
                    currentSource.PlayOneShot(_audioClipCoinCollect);
                    break;
                case AudioType.EndMerge:
                    currentSource.PlayOneShot(_audioClipEndMerge);
                    break;
            }
        }

        private AudioSource GetNextAudioSource()
        {
            if (_nextSourceIndex >= _audioSource.Count)
            {
                _nextSourceIndex = 0;
            }

            AudioSource source = _audioSource[_nextSourceIndex];
            _nextSourceIndex++;
            return source;
        }
    }
}