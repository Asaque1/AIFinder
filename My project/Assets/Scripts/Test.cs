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
        [SerializeField] private Button startButton;
        [SerializeField] private Button stopButton;
        [SerializeField] private TextMeshProUGUI text;
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
            stopButton.interactable = false;
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
                text.text = "마이크 장치가 없습니다.";
                return;
            }

            foreach (var device in Microphone.devices)
                Debug.Log("Detected mic: " + device);

            text.color = Color.white;
            text.text = "Recording...";
            startButton.interactable = false;
            stopButton.interactable = true;

            clip = Microphone.Start(null, false, maxDuration, sampleRate);
            if (clip == null)
            {
                Debug.LogError("Microphone.Start returned null.");
                text.text = "녹음 실패: 마이크 시작 불가";
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
                text.color = Color.red;
                text.text = "녹음 데이터가 없습니다.";
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
            text.color = Color.yellow;
            text.text = "Sending...";
            stopButton.interactable = false;

            string url = "https://api-inference.huggingface.co/models/openai/whisper-large-v3";
            string token = "Bearer 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"; // << Bearer 남겨둬야함 꼭, 그 후 API 키 복붙 ㄱ

            UnityWebRequest request = new UnityWebRequest(url, "POST");
            request.uploadHandler = new UploadHandlerRaw(audioData);
            request.downloadHandler = new DownloadHandlerBuffer();
            request.SetRequestHeader("Authorization", token);
            request.SetRequestHeader("Content-Type", "audio/wav");

            yield return request.SendWebRequest();

            if (request.result != UnityWebRequest.Result.Success)
            {
                Debug.LogError($"HuggingFace API error: {request.error}");
                text.color = Color.red;
                text.text = $"에러 발생: {request.error}";
            }
            else
            {
                string responseText = request.downloadHandler.text;
                Debug.Log("API Raw Response: " + responseText);

                WhisperResponse result = JsonUtility.FromJson<WhisperResponse>(responseText);
                string recognizedText = result.text?.Trim();

                text.color = Color.white;
                text.text = recognizedText;

                OnSpeechRecognized?.Invoke(recognizedText);
            }

            startButton.interactable = true;
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
