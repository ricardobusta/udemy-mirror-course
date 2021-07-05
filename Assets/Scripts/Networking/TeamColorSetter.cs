using Mirror;
using UnityEngine;
using UnityEngine.UI;

namespace Networking
{
    public class TeamColorSetter : NetworkBehaviour
    {
        [SerializeField] private Renderer[] teamColorRenderers;
        [SerializeField] private int[] teamColorMaterialIndexes;
        [SerializeField] private Image HealthBarColor;
        [SerializeField] private Material teamColorMaterialPrefab;

        [SyncVar(hook = nameof(HandleTeamColorUpdate))]
        private Color teamColor;

        public override void OnStartServer()
        {
            var player = connectionToClient.identity.GetComponent<RtsPlayer>();
            teamColor = player.TeamColor;
        }

        private void HandleTeamColorUpdate(Color oldColor, Color newColor)
        {
            for (var i = 0; i < teamColorRenderers.Length; i++)
            {
                teamColorRenderers[i].materials[teamColorMaterialIndexes[i]].SetColor("_EmissionColor", newColor);
            }

            HealthBarColor.color = newColor;
        }
    }
}