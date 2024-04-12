using System;

using Unity.Collections;

using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

public class UIController : MonoBehaviour
{
	[SerializeField] private GameObject pauseMenu;
	[SerializeField] private InputActionReference pauseMenuReference;
	
	[SerializeField, ReadOnly] private bool isPaused;
	
	public void ChangeScene(string _sceneToChangeTo) => SceneManager.LoadSceneAsync(_sceneToChangeTo, LoadSceneMode.Single);

	public void Quit() => Application.Quit();

	private void Awake() => pauseMenuReference.action.performed += Paused;

	private void Start()
	{
		if(pauseMenu == null) return;
		if(pauseMenuReference == null) return;
		
		isPaused = false;
	}

	private void Update()
	{
		if(isPaused)
		{
			pauseMenu.gameObject.SetActive(true);
			Time.timeScale = 0;
		}
		else
		{
			Time.timeScale = 1;
			pauseMenu.gameObject.SetActive(false);
		}
	}

	private void Paused(InputAction.CallbackContext obj) => isPaused = !isPaused;

	public void Resume() => isPaused = false;

	private void OnEnable() => pauseMenuReference.action.Enable();

	private void OnDisable() => pauseMenuReference.action.Disable();
}