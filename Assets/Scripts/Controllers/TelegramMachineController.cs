using UnityEngine;
using System.Collections;

using Shanghai.Grid;

namespace Shanghai.Controllers {
    public class TelegramMachineController : MonoBehaviour {
        public static readonly string TELEGRAM_PATH = "prefabs/telegram";

        public Source CurrentSource;

        private ShanghaiConfig _Config;
        private GameObject _TelegramPrefab;
        private GameObject _CurrentTelegram;

        public void Awake() {
            _Config = ShanghaiConfig.Instance;

            _TelegramPrefab = Resources.Load(TELEGRAM_PATH) as GameObject;
            if (_TelegramPrefab == null) {
                Debug.Log("Could not load cell from path " + TELEGRAM_PATH);
            }

            Messenger<Source>.AddListener(EventGenerator.EVENT_SOURCE_CREATED, OnSourceCreated);
        }

        public void OnDestroy() {
            Messenger<Source>.RemoveListener(EventGenerator.EVENT_SOURCE_CREATED, OnSourceCreated);
        }

        public void OnSourceCreated(Source source) {
            Destroy(_CurrentTelegram);
            GameObject telegramGO = GameObject.Instantiate(_TelegramPrefab) as GameObject;
            _CurrentTelegram = telegramGO;
            TelegramController telegram = telegramGO.GetComponent<TelegramController>();
            telegram.Source = source;
            telegram.transform.parent = transform;
            telegram.transform.localPosition = Vector2.zero;
            telegram.transform.localScale = Vector2.one;
        }
    }
}
