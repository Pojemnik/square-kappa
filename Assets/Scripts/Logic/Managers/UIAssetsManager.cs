using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIAssetsManager : Singleton<UIAssetsManager>
{
    [SerializeField]
    private SerializableDictionary<WeaponConfig.WeaponType, Sprite> weaponSprites;
    [SerializeField]
    private Sprite defaultSprite;
    [SerializeField]
    private Sprite emptySprite;

    public Sprite GetEmptySprite()
    {
        return emptySprite;
    }

    public Sprite GetWeaponSprite(WeaponConfig.WeaponType weaponType)
    {
        if(weaponSprites.ContainsKey(weaponType))
        {
            return weaponSprites[weaponType];
        }
        Debug.LogErrorFormat("No sprite available for weapon type {0}. Used default", weaponType);
        return defaultSprite;
    }
}
