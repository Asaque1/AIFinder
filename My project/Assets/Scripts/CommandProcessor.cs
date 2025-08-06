using System;
using System.Collections;
using System.Text;
using UnityEngine;
using UnityEngine.Networking;

public class CommandProcessor : MonoBehaviour
{
    [Header("API 설정")]
    [SerializeField] private string huggingFaceApiKey;

    [Header("AI 캐릭터 컨트롤")]
    [SerializeField] private AICharacterController characterController;

    public void ProcessCommand(string userText)
    {
        StartCoroutine(SendToBlenderBot(userText));
    }

    private IEnumerator SendToBlenderBot(string inputText)
    {
        string url = "https://api-inference.huggingface.co/models/facebook/blenderbot-400M-distill";
        string token = "Bearer " + huggingFaceApiKey;

        // conversational 형식 맞추기: inputs에 text 키 포함
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
                Debug.LogError("BlenderBot 요청 실패: " + request.error);
                yield break;
            }

            string response = request.downloadHandler.text;
            Debug.Log("BlenderBot 응답 (raw): " + response);

            string reply = ExtractGeneratedText(response);
            Debug.Log("AI 응답 텍스트: " + reply);

            InterpretResponse(reply);
        }
    }



    private void InterpretResponse(string responseText)
    {
        string lower = responseText.ToLower();

        if (lower.Contains("앞으로") || lower.Contains("전진") || lower.Contains("가자"))
            characterController.MoveForward();
        else if (lower.Contains("뒤로") || lower.Contains("후진"))
            characterController.MoveBackward();
        else if (lower.Contains("왼쪽"))
            characterController.TurnLeft();
        else if (lower.Contains("오른쪽"))
            characterController.TurnRight();
        else if (lower.Contains("문") && (lower.Contains("열") || lower.Contains("열어")))
            characterController.OpenDoor();
        else
            Debug.LogWarning("알 수 없는 응답: 동작으로 매핑되지 않음.");
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
            Debug.LogError("파싱 실패: " + e.Message);
            return "";
        }
    }

    private string EscapeForJson(string input)
    {
        return input.Replace("\\", "\\\\").Replace("\"", "\\\"");
    }
}
