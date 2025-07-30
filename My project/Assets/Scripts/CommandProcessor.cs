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
        StartCoroutine(SendToDialoGPT(userText));
    }

    private IEnumerator SendToDialoGPT(string inputText)
    {
        string url = "https://api-inference.huggingface.co/models/microsoft/DialoGPT-medium"; // /generate 빼기
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
                Debug.LogError("DialoGPT 요청 실패: " + request.error);
                yield break;
            }

            string response = request.downloadHandler.text;
            Debug.Log("DialoGPT 응답: " + response);

            // 응답 예시: [{ "generated_text": "..." }]
            string reply = ExtractGeneratedText(response);
            Debug.Log("AI 응답 텍스트: " + reply);

            InterpretResponse(reply);
        }
    }


    private void InterpretResponse(string responseText)
    {
        string lower = responseText.ToLower();

        if (lower.Contains("앞으로") || lower.Contains("전진") || lower.Contains("가자"))
        {
            characterController.MoveForward();
        }
        else if (lower.Contains("뒤로") || lower.Contains("후진"))
        {
            characterController.MoveBackward();
        }
        else if (lower.Contains("왼쪽"))
        {
            characterController.TurnLeft();
        }
        else if (lower.Contains("오른쪽"))
        {
            characterController.TurnRight();
        }
        else if (lower.Contains("문") && (lower.Contains("열") || lower.Contains("열게")))
        {
            characterController.OpenDoor();
        }
        else
        {
            Debug.LogWarning("알 수 없는 응답: 동작으로 매핑되지 않음.");
        }
    }

    // 간단한 응답 파싱 - 실제 응답 JSON에 맞게 필요 시 조정하세요
    private string ExtractGeneratedText(string rawJson)
    {
        // 우선 원본 출력
        Debug.Log("Raw JSON: " + rawJson);

        // 응답 JSON 예시가 다음과 같다고 가정합니다:
        // { "generated_text": "AI가 생성한 텍스트" }
        // 따라서 간단하게 "generated_text" 필드 값을 파싱하도록 구현

        string key = "\"generated_text\":\"";
        int start = rawJson.IndexOf(key);
        if (start < 0) return "";

        start += key.Length;
        int end = rawJson.IndexOf("\"", start);
        if (end < 0) return "";

        string extracted = rawJson.Substring(start, end - start);
        return extracted;
    }

    // JSON 문자열 이스케이프 처리
    private string EscapeForJson(string input)
    {
        return input.Replace("\\", "\\\\").Replace("\"", "\\\"");
    }
}
