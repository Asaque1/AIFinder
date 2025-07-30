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
        StartCoroutine(SendToDialoGPT(userText));
    }

    private IEnumerator SendToDialoGPT(string inputText)
    {
        string url = "https://api-inference.huggingface.co/models/microsoft/DialoGPT-medium"; // /generate ����
        string token = "Bearer " + huggingFaceApiKey;

        string jsonPayload = "{\"inputs\": \"" + EscapeForJson(inputText) + "\"}";

        using (UnityWebRequest request = new UnityWebRequest(url, "POST"))
        {
            byte[] bodyRaw = Encoding.UTF8.GetBytes(jsonPayload);
            request.uploadHandler = new UploadHandlerRaw(bodyRaw);
            request.downloadHandler = new DownloadHandlerBuffer();
            request.SetRequestHeader("Authorization", token);
            request.SetRequestHeader("Content-Type", "application/json");

            yield return request.SendWebRequest();

            if (request.result != UnityWebRequest.Result.Success)
            {
                Debug.LogError("DialoGPT ��û ����: " + request.error);
                yield break;
            }

            string response = request.downloadHandler.text;
            Debug.Log("DialoGPT ����: " + response);

            // ���� ����: [{ "generated_text": "..." }]
            string reply = ExtractGeneratedText(response);
            Debug.Log("AI ���� �ؽ�Ʈ: " + reply);

            InterpretResponse(reply);
        }
    }


    private void InterpretResponse(string responseText)
    {
        string lower = responseText.ToLower();

        if (lower.Contains("������") || lower.Contains("����") || lower.Contains("����"))
        {
            characterController.MoveForward();
        }
        else if (lower.Contains("�ڷ�") || lower.Contains("����"))
        {
            characterController.MoveBackward();
        }
        else if (lower.Contains("����"))
        {
            characterController.TurnLeft();
        }
        else if (lower.Contains("������"))
        {
            characterController.TurnRight();
        }
        else if (lower.Contains("��") && (lower.Contains("��") || lower.Contains("����")))
        {
            characterController.OpenDoor();
        }
        else
        {
            Debug.LogWarning("�� �� ���� ����: �������� ���ε��� ����.");
        }
    }

    // ������ ���� �Ľ� - ���� ���� JSON�� �°� �ʿ� �� �����ϼ���
    private string ExtractGeneratedText(string rawJson)
    {
        // �켱 ���� ���
        Debug.Log("Raw JSON: " + rawJson);

        // ���� JSON ���ð� ������ ���ٰ� �����մϴ�:
        // { "generated_text": "AI�� ������ �ؽ�Ʈ" }
        // ���� �����ϰ� "generated_text" �ʵ� ���� �Ľ��ϵ��� ����

        string key = "\"generated_text\":\"";
        int start = rawJson.IndexOf(key);
        if (start < 0) return "";

        start += key.Length;
        int end = rawJson.IndexOf("\"", start);
        if (end < 0) return "";

        string extracted = rawJson.Substring(start, end - start);
        return extracted;
    }

    // JSON ���ڿ� �̽������� ó��
    private string EscapeForJson(string input)
    {
        return input.Replace("\\", "\\\\").Replace("\"", "\\\"");
    }
}
