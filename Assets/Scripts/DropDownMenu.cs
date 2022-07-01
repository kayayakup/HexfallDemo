using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class DropDownMenu : MonoBehaviour
{
    private AudioSource audioSource;
    public AudioClip Musicc;
    public AudioClip Soundd;
    public GameObject Menu;
    private bool music;
    private bool sound;


    //Menu kodlamalarý
    private void Awake()
    {
        audioSource = GameObject.FindObjectOfType<GameManager>().GetComponent<AudioSource>();
    }

    public void setactivate()
    {
        if (Menu.activeInHierarchy == false)
        {
            Menu.SetActive(true);
        }
        else
        {
            Menu.SetActive(false);
        }
    }

    public void Restart()
    {
        SceneManager.LoadScene(0);
    }

    public void MuteMusic()
    {
        audioSource.mute = true;
    }

    public void UnmuteMusic()
    {
        audioSource.mute = false;
    }
}
