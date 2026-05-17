using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MainMenu : MonoBehaviour
{
    [SerializeField] private Button play;
    [SerializeField] private Button exit;
    [SerializeField] private Button howTo;
    [SerializeField] private Button back;
    [SerializeField] private GameObject instructions;
    private void Awake()
    {
        play.onClick.AddListener(() => SceneManager.LoadScene(1));
        exit.onClick.AddListener(Application.Quit);
        howTo.onClick.AddListener(() => instructions.SetActive(true));
        back.onClick.AddListener(() => instructions.SetActive(false));
    }
}
