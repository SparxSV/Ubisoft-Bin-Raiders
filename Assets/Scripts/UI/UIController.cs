using UnityEngine;
using UnityEngine.SceneManagement;

public class UIController : MonoBehaviour
{
	public void ChangeScene(string _sceneToChangeTo) => SceneManager.LoadSceneAsync(_sceneToChangeTo, LoadSceneMode.Single);

	public void Quit() => Application.Quit();
}
