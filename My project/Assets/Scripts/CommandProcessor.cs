using System;
using System.Collections;
using System.Text;
using UnityEngine;
using UnityEngine.Networking;

public class CommandProcessor : MonoBehaviour
{
    [Header("API ����")]
    [SerializeField] private string huggingFaceApiKey;

    [Header("AI ĳ���� ��Ʈ��")]
    [SerializeField] private AICharacterController characterController;

    public void ProcessCommand(string userText)
    {
        StartCoroutine(SendToBlenderBot(userText));
    }

    private IEnumerator SendToBlenderBot(string inputText)
    {
        string url = "https://api-inference.huggingface.co/models/facebook/blenderbot-400M-distill";
        string token = "Bearer " + huggingFaceApiKey;

        // conversational ���� ���߱�: inputs�� text Ű ����
        string jsonPayload = "{\"inputs\": {\"text\": \"" + EscapeForJson(inputText) + "\"}}";

        using (UnityWebRequest request = new UnityWebRequest(url, "POST"))
        {
            byte[] bodyRaw = System.Text.Encoding.UTF8.GetBytes(jsonPayload);
            request.uploadHandler = new UploadHandlerRaw(bodyRaw);
            request.downloadHandler = new DownloadHandlerBuffer();
            request.SetRequestHeader("Authorization", token);
            request.SetRequestHeader("Content-Type", "application/json");

            yield return request.SendWebRequest();

            if (request.result != UnityWebRequest.Result.Success)
            {
                Debug.LogError("BlenderBot ��û ����: " + request.error);
                yield break;
            }

            string response = request.downloadHandler.text;
            Debug.Log("BlenderBot ���� (raw): " + response);

            string reply = ExtractGeneratedText(response);
            Debug.Log("AI ���� �ؽ�Ʈ: " + reply);

            InterpretResponse(reply);
        }
    }



    private void InterpretResponse(string responseText)
    {
        string lower = responseText.ToLower();

        if (lower.Contains("������") || lower.Contains("����") || lower.Contains("����"))
            characterController.MoveForward();
        else if (lower.Contains("�ڷ�") || lower.Contains("����"))
            characterController.MoveBackward();
        else if (lower.Contains("����"))
            characterController.TurnLeft();
        else if (lower.Contains("������"))
            characterController.TurnRight();
        else if (lower.Contains("��") && (lower.Contains("��") || lower.Contains("����")))
            characterController.OpenDoor();
        else
            Debug.LogWarning("�� �� ���� ����: �������� ���ε��� ����.");
    }

    private string ExtractGeneratedText(string rawJson)
    {
        try
        {
            int keyIndex = rawJson.IndexOf("\"generated_text\":\"");
            if (keyIndex < 0) return "";

            int start = keyIndex + "\"generated_text\":\"".Length;
            int end = rawJson.IndexOf("\"", start);
            if (end < 0) end = rawJson.Length - 1;

            string result = rawJson.Substring(start, end - start);
            return result.Replace("\\n", "\n").Replace("\\\"", "\"");
        }
        catch (Exception e)
        {
            Debug.LogError("�Ľ� ����: " + e.Message);
            return "";
        }
    }

    private string EscapeForJson(string input)
    {
        return input.Replace("\\", "\\\\").Replace("\"", "\\\"");
    }
}
