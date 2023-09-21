using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.SceneManagement;
using YG;

[System.Serializable]
public class Reward
{
    public int id;
    public UnityEvent onReward;
}

public class Game : MonoBehaviour
{
    public static Game instance { get; private set; }
    [SerializeField] private Reward[] rewards;

    private void OnEnable()
    {
        YandexGame.GetDataEvent += GetData;
        YandexGame.RewardVideoEvent += Rewarded;
    }

    private void OnDisable()
    {
        YandexGame.GetDataEvent -= GetData;
        YandexGame.RewardVideoEvent -= Rewarded;
    }

    private void Awake()
    {
        instance = this;
        UpdateRobuxText();
    }

    private void Rewarded(int id)
    {
        foreach (Reward reward in rewards)
        {
            if(reward.id == id)
            {
                reward.onReward.Invoke();
                return;
            }
        }
    }

    public Vector3 RoundVector(Vector3 vector)
    {
        int roundedX = Mathf.RoundToInt(vector.x);
        int roundedY = Mathf.RoundToInt(vector.y);
        int roundedZ = Mathf.RoundToInt(vector.z);
        return new Vector3(roundedX, roundedY, roundedZ);
    }

    public void LoadScene(int scene)
    {
        StartCoroutine(LevelTransition(scene));
    }

    public IEnumerator LevelTransition(int scene)
    {
        GetComponentInChildren<Animator>().SetTrigger("Transition");
        yield return new WaitForSeconds(1.25f);
        SceneManager.LoadScene(scene);
    }

    private void GetData()
    {
        UpdateRobuxText();
    }

    public void Robux(int amount)
    {
        YandexGame.savesData.robux += amount;
        YandexGame.SaveProgress();
        Debug.Log(YandexGame.savesData.robux);

        UpdateRobuxText();
    }

    private void UpdateRobuxText()
    {
        if(YandexGame.SDKEnabled)
        {
            TextMeshProUGUI robuxText = GameObject.FindGameObjectWithTag("RobuxText").GetComponent<TextMeshProUGUI>();
            robuxText.text = YandexGame.savesData.robux.ToString();
        }
    }
}
