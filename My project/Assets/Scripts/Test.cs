using System.Collections;
using System.IO;
using TMPro;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;

namespace HuggingFace.API.Examples
{
    public class Test : MonoBehaviour
    {
        [Header("버튼 연결")]
        [SerializeField] private Button startButton;
        [SerializeField] private Button stopButton;
        [SerializeField] private Button sendButton;

        [Header("UI 연결")]
        [SerializeField] private TMP_InputField inputField;

        [Header("HuggingFace API 키")]
        [SerializeField] private string Key;

        public System.Action<string> OnSpeechRecognized;

        private AudioClip clip;
        private byte[] bytes;
        private bool recording;

        private const int sampleRate = 44100;
        private const int maxDuration = 10;

        private void Start()
        {
            startButton.onClick.AddListener(StartRecording);
            stopButton.onClick.AddListener(StopRecording);
            sendButton.onClick.AddListener(OnSendButtonClicked);

            // ?? 텍스트 변경 이벤트 등록
            inputField.onValueChanged.AddListener(OnInputFieldChanged);

            stopButton.interactable = false;
            sendButton.interactable = false;
        }

        private void OnInputFieldChanged(string input)
        {
            // 입력 필드에 텍스트가 존재하면 전송 버튼 활성화
            sendButton.interactable = !string.IsNullOrWhiteSpace(input);
        }

        private void Update()
        {
            if (recording && Microphone.GetPosition(null) >= clip.samples)
            {
                StopRecording();
            }
        }

        private void StartRecording()
        {
            Debug.Log("StartRecording called.");

            if (Microphone.devices.Length == 0)
            {
                Debug.LogError("No microphone devices found.");
                inputField.text = "마이크 장치가 없습니다.";
                return;
            }

            foreach (var device in Microphone.devices)
                Debug.Log("Detected mic: " + device);

            inputField.text = "녹음 중...";
            startButton.interactable = false;
            stopButton.interactable = true;
            sendButton.interactable = false;

            clip = Microphone.Start(null, false, maxDuration, sampleRate);
            if (clip == null)
            {
                Debug.LogError("Microphone.Start returned null.");
                inputField.text = "녹음 실패: 마이크 시작 불가";
                return;
            }

            recording = true;
        }

        private void StopRecording()
        {
            int position = Microphone.GetPosition(null);
            Microphone.End(null);

            if (position <= 0 || clip == null)
            {
                Debug.LogWarning("녹음된 길이가 너무 짧거나 클립이 유효하지 않음.");
                inputField.text = "녹음 데이터가 없습니다.";
                startButton.interactable = true;
                stopButton.interactable = false;
                return;
            }

            float[] samples = new float[position * clip.channels];
            clip.GetData(samples, 0);

            bytes = EncodeAsWAV(samples, clip.frequency, clip.channels);
            recording = false;

            StartCoroutine(SendToHuggingFace(bytes));
        }

        private IEnumerator SendToHuggingFace(byte[] audioData)
        {
            inputField.text = "인식 중...";
            stopButton.interactable = false;

            string url = "https://api-inference.huggingface.co/models/openai/whisper-large-v3";
            string token = "Bearer " + Key; // Bearer 옆 띄어쓰기 없애면 진짜 엄마 찢음

            UnityWebRequest request = new UnityWebRequest(url, "POST");
            request.uploadHandler = new UploadHandlerRaw(audioData);
            request.downloadHandler = new DownloadHandlerBuffer();
            request.SetRequestHeader("Authorization", token);
            request.SetRequestHeader("Content-Type", "audio/wav");

            yield return request.SendWebRequest();

            if (request.result != UnityWebRequest.Result.Success)
            {
                Debug.LogError($"HuggingFace API error: {request.error}");
                inputField.text = $"에러 발생: {request.error}";
            }
            else
            {
                string responseText = request.downloadHandler.text;
                Debug.Log("API Raw Response: " + responseText);

                WhisperResponse result = JsonUtility.FromJson<WhisperResponse>(responseText);
                string recognizedText = result.text?.Trim();

                inputField.text = recognizedText;
                sendButton.interactable = !string.IsNullOrWhiteSpace(recognizedText); // 텍스트 있으면 버튼 활성화
            }

            startButton.interactable = true;
        }

        private void OnSendButtonClicked()
        {
            string finalText = inputField.text;
            if (!string.IsNullOrWhiteSpace(finalText))
            {
                OnSpeechRecognized?.Invoke(finalText); // AI 명령 실행
                sendButton.interactable = false; // 중복 방지
            }
        }

        private byte[] EncodeAsWAV(float[] samples, int frequency, int channels)
        {
            using (var memoryStream = new MemoryStream(44 + samples.Length * 2))
            {
                using (var writer = new BinaryWriter(memoryStream))
                {
                    int byteRate = frequency * channels * 2;

                    writer.Write("RIFF".ToCharArray());
                    writer.Write(36 + samples.Length * 2);
                    writer.Write("WAVE".ToCharArray());
                    writer.Write("fmt ".ToCharArray());
                    writer.Write(16);
                    writer.Write((ushort)1);
                    writer.Write((ushort)channels);
                    writer.Write(frequency);
                    writer.Write(byteRate);
                    writer.Write((ushort)(channels * 2));
                    writer.Write((ushort)16);
                    writer.Write("data".ToCharArray());
                    writer.Write(samples.Length * 2);

                    foreach (float sample in samples)
                    {
                        short intSample = (short)(Mathf.Clamp(sample, -1f, 1f) * short.MaxValue);
                        writer.Write(intSample);
                    }
                }

                return memoryStream.ToArray();
            }
        }

        [System.Serializable]
        public class WhisperResponse
        {
            public string text;
        }
    }
}
