using Mirror;
using UnityEngine;
using UnityEngine.UI;

namespace Networking
{
    public class TeamColorSetter : NetworkBehaviour
    {
        [SerializeField] private Renderer[] teamColorRenderers;
        [SerializeField] private Image healthBarColor;
        [SerializeField] private SpriteRenderer miniMapColor;
        [SerializeField] private int[] teamColorMaterialIndexes;
        [SerializeField] private Material teamColorMaterialPrefab;

        [SyncVar(hook = nameof(HandleTeamColorUpdate))]
        private Color teamColor;

        private static readonly int EmissionColor = Shader.PropertyToID("_EmissionColor");

        public override void OnStartServer()
        {
            var player = connectionToClient.identity.GetComponent<RtsPlayer>();
            teamColor = player.TeamColor;
        }

        private void HandleTeamColorUpdate(Color oldColor, Color newColor)
        {
            for (var i = 0; i < teamColorRenderers.Length; i++)
            {
                teamColorRenderers[i].materials[teamColorMaterialIndexes[i]].SetColor(EmissionColor, newColor);
            }

            miniMapColor.color = newColor;
            healthBarColor.color = newColor;
        }
    }
}