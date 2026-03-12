using UnityEngine;

public class AudioPlayerFromAnim : MonoBehaviour
{
    public void PlayAudio(AudioController.AudioType audioType)
    {
        AudioController.Instance.PlayAudio(audioType);
    }
}