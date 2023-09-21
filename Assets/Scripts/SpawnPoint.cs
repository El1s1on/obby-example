using UnityEngine;
using YG;

public class SpawnPoint : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        if(other.CompareTag("Player"))
        {
            if(YandexGame.SDKEnabled)
            {
                Vector3 savePos = new Vector3(transform.position.x, other.transform.position.y, transform.position.z);

                if (YandexGame.savesData.spawnPointPosition.x == transform.position.x && YandexGame.savesData.spawnPointPosition.z == transform.position.z) return;
                Debug.Log($"{Game.instance.RoundVector(YandexGame.savesData.spawnPointPosition)}; {Game.instance.RoundVector(savePos)}");
                YandexGame.savesData.spawnPointPosition = savePos;
                YandexGame.SaveProgress();

                GetComponent<AudioSource>().Play();
            }
        }
    }
}
