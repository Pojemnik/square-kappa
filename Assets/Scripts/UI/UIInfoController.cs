using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Localization;

public class UIInfoController : MonoBehaviour
{
    [System.Serializable]
    private class UIInfoConfigType
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
    [SerializeField]
    private UnityEngine.UI.Image panel;

    private Dictionary<UIInfoConfigType, System.EventHandler> missionEventHandlers;
    private Dictionary<UIInfoConfigType, LocalizedString.ChangeHandler> stringChangedEventHandlers;
    private System.EventHandler displayEndEventHandler;

    private void Awake()
    {
        missionEventHandlers = new Dictionary<UIInfoConfigType, System.EventHandler>();
        stringChangedEventHandlers = new Dictionary<UIInfoConfigType, LocalizedString.ChangeHandler>();
        foreach (UIInfoConfigType config in UIInfoConfig)
        {
            System.EventHandler handler = (s, a) => DisplayOnScreenMessage(config);
            missionEventHandlers.Add(config, handler);
            config.missionEvent.missionEvent += handler;

            LocalizedString.ChangeHandler stringHandler = (_) => textController.StopTyping();
            stringChangedEventHandlers.Add(config, stringHandler);
            config.message.StringChanged += stringHandler;
        }
        displayEndEventHandler = (s, a) => { imageController.HideImage(); panel.enabled = false; };
        textController.displayEndEvent += displayEndEventHandler;
    }

    private void OnDestroy()
    {
        foreach (UIInfoConfigType config in UIInfoConfig)
        {
            config.missionEvent.missionEvent -= missionEventHandlers[config];
            config.message.StringChanged -= stringChangedEventHandlers[config];
        }
        textController.displayEndEvent -= displayEndEventHandler;
    }

    private void DisplayOnScreenMessage(UIInfoConfigType config)
    {
        textController.TypeText(config.message.GetLocalizedString(), config.messageHideDelay);
        imageController.DisplayImage(config.image.LoadAsset());
        panel.enabled = true;
    }
}
