using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;


public class StartUpManager : MonoBehaviour
{
    [SerializeField] InputField locationInput = null;
    [SerializeField] bool testMode = false;
    [SerializeField] Text errorText = null;


    private void Start()
    {
        if (!UnityEngine.Android.Permission.HasUserAuthorizedPermission(UnityEngine.Android.Permission.CoarseLocation))
        {
            UnityEngine.Android.Permission.RequestUserPermission(UnityEngine.Android.Permission.CoarseLocation);
        }

        Input.location.Start();

        if (PlayerPrefs.GetInt("errogps", 0) == 1)
        {
            errorText.text = "Erro GPS";
            PlayerPrefs.SetInt("errogps", 0);
            PlayerPrefs.SetInt("erroparagem", 0);
            PlayerPrefs.SetInt("serveroffline", 0);
            return;
        }

        if (PlayerPrefs.GetInt("erroparagem", 0) == 1)
        {
            errorText.text = "Paragem inexistente";
            PlayerPrefs.SetInt("errogps", 0);
            PlayerPrefs.SetInt("erroparagem", 0);
            PlayerPrefs.SetInt("serveroffline", 0);
            return;
        }

        if(PlayerPrefs.GetInt("serveroffline", 0) == 1)
        {
            errorText.text = "Servidor offline";
            PlayerPrefs.SetInt("errogps", 0);
            PlayerPrefs.SetInt("erroparagem", 0);
            PlayerPrefs.SetInt("serveroffline", 0);
            return;
        }
    }


    public void GoGPS()
    {
        PlayerPrefs.SetInt("testmode", 0);
        if (Input.location.isEnabledByUser)
        {
            PlayerPrefs.SetInt("usinggps", 1);
            SceneManager.LoadScene("MainMap");
        }
        else if (testMode)
        {
            PlayerPrefs.SetInt("testmode", 1);
            PlayerPrefs.SetInt("usinggps", 1);
            SceneManager.LoadScene("MainMap");
        }
        else errorText.text = "GPS desativado";
    }


    public void GoLocation()
    {
        string nomeParagem = locationInput.text;
        if (nomeParagem.Length == 0)
        {
            errorText.text = "Insira uma paragem";
            return;
        }
        PlayerPrefs.SetInt("usinggps", 0);
        PlayerPrefs.SetString("paragem", nomeParagem);
        SceneManager.LoadScene("MainMap");
    }
}
