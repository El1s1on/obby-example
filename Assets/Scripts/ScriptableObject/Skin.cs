using UnityEngine;

[CreateAssetMenu(fileName = "Skin", menuName = "ScriptableObjects/Skin")]
public class Skin : ScriptableObject
{
    [Header("Clothes")]
    public Material head;
    public Material spine;
    [Space]
    public Material leftArm;
    public Material rightArm;
    [Space]
    public Material leftLeg;
    public Material rightLeg;
    [Header("Info")]
    public int skinID;
    [Space]
    public Sprite skinIcon;
    public int skinPrice;
}
