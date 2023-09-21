using System;
using UnityEngine;
using YG;

public class SkinManager : MonoBehaviour
{
    [SerializeField] private bool isRagdoll;
    private Skin[] skins;

    private void OnEnable()
    {
        SkinItem.OnEquip += UpdateSkin;
        YandexGame.GetDataEvent += UpdateSkin;
    }
    private void OnDisable()
    {
        SkinItem.OnEquip -= UpdateSkin;
        YandexGame.GetDataEvent -= UpdateSkin;
    }

    private void Start()
    {
        skins = Resources.LoadAll<Skin>("ScriptableObjects/Skins");

        if(YandexGame.SDKEnabled)
        {
            UpdateSkin();
        }
    }

    private void UpdateSkin()
    {
        int skinId = YandexGame.savesData.equippedSkinId;
        if (isRagdoll) SetRagdollSkin(skinId); else SetPlayerSkin(skinId);
    }

    private void SetRagdollSkin(int ID)
    {
        MeshRenderer[] partsRenderer = GetComponentsInChildren<MeshRenderer>();

        foreach(Skin skin in skins)
        {
            if (skin.skinID == ID)
            {
                foreach(MeshRenderer mesh in partsRenderer)
                {
                    switch(mesh.name)
                    {
                        case "Head":
                            mesh.material = skin.head;
                            break;
                        case "RightHand":
                            mesh.material = skin.rightArm;
                            break;
                        case "LeftHand":
                            mesh.material = skin.leftArm;
                            break;
                        case "RightLeg":
                            mesh.material = skin.rightLeg;
                            break;
                        case "LeftLeg":
                            mesh.material = skin.leftLeg;
                            break;
                        case "Torso":
                            mesh.material = skin.spine;
                            break;
                    }
                }
            }
        }
    }

    public void SetPlayerSkin(int ID)
    {
        SkinnedMeshRenderer meshRenderer = GetComponentInChildren<SkinnedMeshRenderer>();
        if (meshRenderer == null)
            throw new Exception("SkinnedMeshRenderer not found in child.");

        foreach (Skin skin in skins)
        {
            if (skin.skinID == ID)
            {
                Material[] mats = meshRenderer.materials;

                mats[0] = skin.head;
                mats[1] = skin.rightLeg;
                mats[2] = skin.leftArm;
                mats[3] = skin.rightArm;
                mats[4] = skin.leftLeg;
                mats[5] = skin.spine;

                meshRenderer.materials = mats;
            }
        }
    }
}
