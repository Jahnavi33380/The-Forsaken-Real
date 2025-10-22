using UnityEngine;
using TMPro;
using UnityEngine.SceneManagement;


public class HUDController : MonoBehaviour
{
    public TMP_Text healthText;   

    public GameObject deathPanel;
    public Transform player;


    public void SetHealth(int max, int current)
    {
        healthText.text = $"Health: {current} / {max}";
    }

    public void ShowDeathScreen()
    {
        Debug.Log("death screenis shown");
        deathPanel.SetActive(true);
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
        Time.timeScale = 0f;
    }


}
