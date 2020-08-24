using UnityEngine;
using UnityEngine.SceneManagement;

namespace CatImitation
{
    public class MainMenu : MonoBehaviour
    {
        public void Play()
        {
            SceneManager.LoadScene(1);
        }

        public void Quit()
        {
            Application.Quit();
        }

        public void ToMainMenu()
        {
            SceneManager.LoadScene(0);
        }
    }
}