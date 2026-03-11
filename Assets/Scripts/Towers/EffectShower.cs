using System.Collections;
using UnityEngine;

public class EffectShower : MonoBehaviour
{
    [SerializeField] private ParticleSystem _particleSystem;

    public void ShowEffect()
    {
        StartCoroutine(ShowEffectCoroutine());
    }

    public IEnumerator ShowEffectCoroutine()
    {
        _particleSystem.Play();
        yield return new WaitForSeconds(_particleSystem.main.duration);
        gameObject.SetActive(false);
        TowerBase.Instance.ReleaseEffectShower(this);
    }
}