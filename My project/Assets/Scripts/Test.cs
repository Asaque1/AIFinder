using System.IO;
using TMPro;
using UnityEngine;
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

            clip = Microphone.Start(null, false, 10, 44100);
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
            var position = Microphone.GetPosition(null);
            Microphone.End(null);

            if (position <= 0)
            {
                text.color = Color.red;
                text.text = "녹음 데이터가 없습니다.";
                startButton.interactable = true;
                stopButton.interactable = false;
                return;
            }

            var samples = new float[position];
            clip.GetData(samples, 0);
            bytes = EncodeAsWAV(samples, clip.frequency, clip.channels);
            recording = false;
            SendRecording();
        }

        private void SendRecording()
        {
            text.color = Color.yellow;
            text.text = "Sending...";
            stopButton.interactable = false;

            HuggingFaceAPI.AutomaticSpeechRecognition(bytes, response => {
                text.color = Color.white;

                string safeResponse = response.Replace("<", "&lt;").Replace(">", "&gt;");
                text.text = safeResponse;

                // 챗봇 커넥터에게 전달
                OnSpeechRecognized?.Invoke(response);

                startButton.interactable = true;
            }, error => {
                text.color = Color.red;
                text.text = error;
                startButton.interactable = true;
            });

        }


        private byte[] EncodeAsWAV(float[] samples, int frequency, int channels)
        {
            using (var memoryStream = new MemoryStream(44 + samples.Length * 2))
            {
                using (var writer = new BinaryWriter(memoryStream))
                {
                    writer.Write("RIFF".ToCharArray());
                    writer.Write(36 + samples.Length * 2);
                    writer.Write("WAVE".ToCharArray());
                    writer.Write("fmt ".ToCharArray());
                    writer.Write(16);
                    writer.Write((ushort)1);
                    writer.Write((ushort)channels);
                    writer.Write(frequency);
                    writer.Write(frequency * channels * 2);
                    writer.Write((ushort)(channels * 2));
                    writer.Write((ushort)16);
                    writer.Write("data".ToCharArray());
                    writer.Write(samples.Length * 2);

                    foreach (var sample in samples)
                    {
                        writer.Write((short)(sample * short.MaxValue));
                    }
                }
                return memoryStream.ToArray();
            }
        }
    }
}