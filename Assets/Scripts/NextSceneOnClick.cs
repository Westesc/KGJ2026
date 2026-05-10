using UnityEngine;
using SaintsField;
using UnityEngine.SceneManagement;
using System.Collections;

[RequireComponent(typeof(AudioSource))]
public class NextSceneOnClick : MonoBehaviour
{
    [Scene]
    public string SceneName;

    public AudioClip clicked;

    public void NextScene()
    {
        AudioSource source = GetComponent<AudioSource>();
        source.clip = clicked;
        source.Play();
        StartCoroutine(nameof(WaitForClip));
    }

    public IEnumerator WaitForClip()
    {
        yield return new WaitForSeconds(1);
        SceneManager.LoadScene(SceneName);
    }
}
