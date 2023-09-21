using UnityEngine;

namespace YG
{
    [System.Serializable]
    public class SavesYG
    {
        public int idSave;
        public bool isFirstSession = true;
        public string language = "ru";
        public bool promptDone;

        public Vector3 spawnPointPosition = new Vector3(55.9f, 1.32f, -534.8f);
        public int robux;

        //Skins
        public int[] purchasedSkinsId = { 0 };
        public int equippedSkinId = 0;

        // Вы можете выполнить какие то действия при загрузке сохранений
        public SavesYG()
        {
        }
    }
}
