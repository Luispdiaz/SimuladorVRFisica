using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneReloader : MonoBehaviour
{
    public void ReloadScene()
    {
        // Obtener el nombre de la escena actual
        string sceneName = SceneManager.GetActiveScene().name;
        // Reiniciar la escena actual
        SceneManager.LoadScene(sceneName);
    }
}
