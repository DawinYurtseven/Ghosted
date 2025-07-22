using UnityEngine;
using UnityEngine.SceneManagement;
public class HomeManager : MonoBehaviour
{
    // Start is called before the first frame update
    [SerializeField] private FadeOut fadeOut;

    // Update is called once per frame
    public void Finish()
    {
        fadeOut.Fade(true, () => {
            SceneManager.LoadScene("Birthday_ThirdPlayable");
        });
    }
}
