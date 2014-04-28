using UnityEngine;
using System.Collections;
using System.Collections.Generic;

using Shanghai.Entities;

namespace Shanghai.Controllers {
    public class TargetsController : MonoBehaviour {
        public static readonly string TARGET_PATH = "prefabs/target";

        private UITable _Table;

        public void Awake() {
            Messenger<Target>.AddListener(Target.EVENT_TARGET_UPDATED, OnTargetUpdated);
        }

        public void OnDestroy() {
            Messenger<Target>.RemoveListener(Target.EVENT_TARGET_UPDATED, OnTargetUpdated);
        }

        private void OnTargetUpdated(Target target) {
            Transform targetTrans = transform.Find(string.Format("{0}", target.Key));
            if (targetTrans != null) {
                TargetController targetCtr = targetTrans.GetComponent<TargetController>();
                if (targetCtr != null) {
                    targetCtr.UpdateTarget(target);
                }
            }
        }

        public void CreateTable(Dictionary<string,Target> targets) {
            _Table = gameObject.GetComponent<UITable>();
            if (_Table == null) {
                Debug.Log("UITable not initialized ");
                return;
            }

            GameObject targetPrefab = Resources.Load(TARGET_PATH) as GameObject;
            if (targetPrefab == null) {
                Debug.Log("Could not load target from path " + TARGET_PATH);
            }

            ShanghaiUtils.RemoveAllChildren(transform);

            foreach (KeyValuePair<string, Target> targetPair in targets) {
                Target target = targetPair.Value;
                GameObject targetGO = GameObject.Instantiate(targetPrefab) as GameObject;

                TargetController targetCtr = targetGO.GetComponent<TargetController>();
                targetCtr.Key = target.Key;

                targetGO.name = target.Key;
                targetGO.transform.parent = transform;
                targetGO.transform.localPosition = Vector3.zero;
                targetGO.transform.localScale = Vector3.one;
            }

            _Table.repositionNow = true;
        }
    }
}
