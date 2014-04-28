using UnityEngine;
using System.Collections;
using System.Collections.Generic;

using Shanghai.Entities;

namespace Shanghai.Controllers {
    public class ClientsController : MonoBehaviour {
        public static readonly string CLIENT_PATH = "prefabs/client";

        private UITable _Table;

        public void Awake() {
            Messenger<Client>.AddListener(Client.EVENT_CLIENT_UPDATED, OnClientUpdated);
        }

        public void OnDestroy() {
            Messenger<Client>.RemoveListener(Client.EVENT_CLIENT_UPDATED, OnClientUpdated);
        }

        private void OnClientUpdated(Client client) {
            Transform clientTrans = transform.Find(string.Format("{0}", client.Key));
            if (clientTrans != null) {
                ClientController clientCtr = clientTrans.GetComponent<ClientController>();
                if (clientCtr != null) {
                    clientCtr.UpdateClient(client);
                }
            }
        }

        public void CreateTable(Dictionary<string,Client> clients) {
            _Table = gameObject.GetComponent<UITable>();
            if (_Table == null) {
                Debug.Log("UITable not initialized ");
                return;
            }

            GameObject clientPrefab = Resources.Load(CLIENT_PATH) as GameObject;
            if (clientPrefab == null) {
                Debug.Log("Could not load client from path " + CLIENT_PATH);
            }

            ShanghaiUtils.RemoveAllChildren(transform);

            foreach (KeyValuePair<string, Client> clientPair in clients) {
                Client client = clientPair.Value;
                GameObject clientGO = GameObject.Instantiate(clientPrefab) as GameObject;

                ClientController clientCtr = clientGO.GetComponent<ClientController>();
                clientCtr.Key = client.Key;

                clientGO.name = client.Key;
                clientGO.transform.parent = transform;
                clientGO.transform.localPosition = Vector3.zero;
                clientGO.transform.localScale = Vector3.one;
            }

            _Table.repositionNow = true;
        }
    }
}
