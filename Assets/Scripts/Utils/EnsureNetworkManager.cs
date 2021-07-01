using Mirror;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Utils
{
    public class EnsureNetworkManager : MonoBehaviour
    {
        void Start()
        {
            if (FindObjectOfType<NetworkManager>() == null)
            {
                SceneManager.LoadScene(0);
            }
        }
    }
}
