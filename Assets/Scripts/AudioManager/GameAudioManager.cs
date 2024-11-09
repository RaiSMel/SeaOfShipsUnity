using System.Collections.Generic;
using UnityEngine;

public class GameAudioManager : MonoBehaviour
{
    public AudioSource audioSource;

    public AudioClip taTriste;
    public AudioClip podeNaoParecer;
    public AudioClip pobre;
    public AudioClip naoAcredito;
    public AudioClip gp;
    public AudioClip ganhamos;
    public AudioClip areYou;
    public AudioClip eNoisVelho;
    public AudioClip acertoMizeravi;
    public AudioClip errou;

    public void PlayTaTriste()
    {
        audioSource.clip = taTriste;
        audioSource.Play();
    }

    public void PlayPodeNaoParecer()
    {
        audioSource.clip = podeNaoParecer;
        audioSource.Play();
    }

    public void PlayPobre()
    {
        audioSource.clip = pobre;
        audioSource.Play();
    }

    public void PlayNaoAcredito()
    {
        audioSource.clip = naoAcredito;
        audioSource.Play();
    }

    public void PlayGP()
    {
        audioSource.clip = gp;
        audioSource.Play();
    }

    public void PlayGanhamos()
    {
        audioSource.clip = ganhamos;
        audioSource.Play();
    }

    public void PlayAreYou()
    {
        audioSource.clip = areYou;
        audioSource.Play();
    }

    public void PlayENoisVelho()
    {
        audioSource.clip = eNoisVelho;
        audioSource.Play();
    }
    public void PlayAcertoMizeravi()
    {
        audioSource.clip = acertoMizeravi;
        audioSource.Play();
    }
    public void PlayErrou()
    {
        audioSource.clip = errou;
        audioSource.Play();
    }

    public void PlayRandomAudio(List<AudioClip> selectedClips)
    {
        if (selectedClips == null || selectedClips.Count == 0)
        {
            Debug.LogWarning("Nenhum áudio selecionado para tocar.");
            return;
        }

        int randomIndex = Random.Range(0, selectedClips.Count);
        audioSource.clip = selectedClips[randomIndex];
        audioSource.Play();
    }

}
