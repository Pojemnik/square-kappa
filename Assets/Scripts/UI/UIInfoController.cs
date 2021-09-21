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
        public float messageHideDelay;
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
            config.missionEvent.missionEvent += (s, a) => DisplayOnScreenMessage(config);
            config.message.StringChanged += (_) => textController.StopTyping();
        }
        textController.displayEndEvent += (s, a) => imageController.HideImage();
    }

    private void DisplayOnScreenMessage(UIInfoConfigType config)
    {
        textController.TypeText(config.message.GetLocalizedString(), config.messageHideDelay);
        imageController.DisplayImage(config.image.LoadAsset());
    }
}
