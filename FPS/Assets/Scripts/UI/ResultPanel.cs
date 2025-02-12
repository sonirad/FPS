using UnityEngine;
using TMPro;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class ResultPanel : MonoBehaviour
{
    private TextMeshProUGUI title;
    private TextMeshProUGUI kill;
    private TextMeshProUGUI time;

    private void Awake()
    {
        Transform child = transform.GetChild(0);
        title = child.GetComponent<TextMeshProUGUI>();
        child = transform.GetChild(2);
        kill = child.GetComponent<TextMeshProUGUI>();
        child = transform.GetChild(4);
        time = child.GetComponent<TextMeshProUGUI>();
        child = transform.GetChild(5);
        Button restart = child.GetComponent<Button>();
        restart.onClick.AddListener(() => SceneManager.LoadScene(0));
    }

    /// <summary>
    /// ������ ���� �� ������ â
    /// </summary>
    /// <param name="isClear">t : �ⱸ�� ����. f : ������ ����</param>
    /// <param name="killCount">���� �� ��</param>
    /// <param name="playTime">��ü �÷��� Ÿ��</param>
    public void Open(bool isClear, int killCount, float playTime)
    {
        if (isClear)
        {
            title.text = "Game Clear";
        }
        else
        {
            title.text = "Game Over";
        }

        kill.text = killCount.ToString();
        time.text = playTime.ToString("f1");

        Cursor.lockState = CursorLockMode.None;

        gameObject.SetActive(true);
    }
}
