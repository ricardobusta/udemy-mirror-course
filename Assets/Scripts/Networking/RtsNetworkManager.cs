using Buildings;
using Mirror;
using UnityEngine;

namespace Networking
{
    public class RtsNetworkManager : NetworkManager
    {
        [SerializeField] private GameObject playerSpawnerPrefab;
        [SerializeField] private GameOverHandler gameOverHandlerPrefab;
        
        public override void OnServerAddPlayer(NetworkConnection conn)
        {
            base.OnServerAddPlayer(conn);

            var rtsPlayer = conn.identity.GetComponent<RtsPlayer>();
            var randomColor = Random.onUnitSphere;
            Debug.Log($"New player color: {randomColor}");
            rtsPlayer.SetTeamColor(new Color(Mathf.Abs(randomColor.x), Mathf.Abs(randomColor.y), Mathf.Abs(randomColor.z)));

            var playerTransform = conn.identity.transform;

            var spawner = Instantiate(playerSpawnerPrefab, playerTransform.position, playerTransform.rotation);
            
            NetworkServer.Spawn(spawner, conn);
        }

        public override void OnServerSceneChanged(string sceneName)
        {
            if (sceneName.EndsWith("Map.unity"))
            {
                var gameOverHandler = Instantiate(gameOverHandlerPrefab);
                
                NetworkServer.Spawn(gameOverHandler.gameObject);
            }
        }
    }
}