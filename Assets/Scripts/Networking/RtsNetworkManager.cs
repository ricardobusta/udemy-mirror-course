using Mirror;
using UnityEngine;

namespace Networking
{
    public class RtsNetworkManager : NetworkManager
    {
        [SerializeField] private GameObject playerSpawnerPrefab;
        
        public override void OnServerAddPlayer(NetworkConnection conn)
        {
            base.OnServerAddPlayer(conn);

            var playerTransform = conn.identity.transform;

            var spawner = Instantiate(playerSpawnerPrefab, playerTransform.position, playerTransform.rotation);
            
            NetworkServer.Spawn(spawner, conn);
        }
    }
}