using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Localization;

public class UIInfoController : MonoBehaviour
{
    [System.Serializable]
    private struct UIInfoConfigType
    {
        public LocalizedString message;
        public LocalizedSprite image;
        public MissionEvent missionEvent;
    }

    [SerializeField]
    private InfoTextController textController;
    [SerializeField]
    private InfoImageController imageController;
    [SerializeField]
    private List<UIInfoConfigType> UIInfoConfig;

    private void Awake()
    {
        foreach (UIInfoConfigType config in UIInfoConfig)
        {
            config.missionEvent.missionEvent += delegate { DisplayOnScreenMessage(config.message, config.image); };
        }
    }

    public void DisplayOnScreenMessage(LocalizedString content, LocalizedSprite image)
    {
        textController.TypeText(content.GetLocalizedString());
        imageController.DisplayImage(image.LoadAsset());
        textController.displayEndEvent += delegate { imageController.HideImage(); };
    }
}
