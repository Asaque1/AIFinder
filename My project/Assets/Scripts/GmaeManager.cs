using UnityEngine;
using HuggingFace.API.Examples;

public class GameManager : MonoBehaviour
{
    [SerializeField] private Test test;
    [SerializeField] private CommandProcessor commandProcessor;

    private void Start()
    {
        test.OnSpeechRecognized += commandProcessor.ProcessCommand;
    }
}
