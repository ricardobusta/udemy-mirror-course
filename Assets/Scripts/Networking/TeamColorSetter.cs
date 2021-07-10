using System;
using Mirror;
using UnityEngine;
using UnityEngine.UI;

namespace Networking
{
    public class TeamColorSetter : NetworkBehaviour
    {
        [SerializeField] private TeamColorConfigEntry[] teamColorConfigs;
        [SerializeField] private Image healthBarColor;
        [SerializeField] private SpriteRenderer miniMapColor;

        [SyncVar(hook = nameof(HandleTeamColorUpdate))]
        private Color teamColor;

        private static readonly int EmissionColor = Shader.PropertyToID("_EmissionColor");
        private static readonly int BaseColor = Shader.PropertyToID("_BaseColor");

        public override void OnStartServer()
        {
            var player = connectionToClient.identity.GetComponent<RtsPlayer>();
            teamColor = player.TeamColor;
        }

        private void HandleTeamColorUpdate(Color oldColor, Color newColor)
        {
            foreach (var teamColorConfigEntry in teamColorConfigs)
            {
                teamColorConfigEntry.teamColorRenderer.materials[teamColorConfigEntry.teamColorMaterialIndex]
                    .SetColor(teamColorConfigEntry.emission?EmissionColor:BaseColor, newColor);
            }

            miniMapColor.color = newColor;
            healthBarColor.color = newColor;
        }
    }
    
    [Serializable]
    public class TeamColorConfigEntry
    {
        public Renderer teamColorRenderer;
        public int teamColorMaterialIndex;
        public bool emission;
    }
}