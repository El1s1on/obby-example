using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using YG;

public class Portal : MonoBehaviour
{
    [SerializeField] private Transform teleportPoint;
    [SerializeField] private AudioClip teleportSFX;

    private void OnTriggerEnter(Collider other)
    {
        if(other.CompareTag("Player"))
        {
            other.GetComponent<Player>().Teleport(teleportPoint.position);
            other.GetComponent<Animator>().SetTrigger("Teleport");
            GetComponent<AudioSource>().PlayOneShot(teleportSFX);

            YandexGame.savesData.spawnPointPosition = teleportPoint.position;
            YandexGame.SaveProgress();
        }
    }
}
