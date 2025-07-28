using UnityEngine;
using TMPro;
using UnityEngine.UI;
using System.IO;
using UnityEditor;
using Mono.Cecil;
using HuggingFace.API;
using UnityEditor.Compilation;
using UnityEditor.PackageManager;

public class Test : MonoBehaviour
{
    [SerializeField]
    private Button startButton;
    [SerializeField]
    private Button stopButton;
    [SerializeField]
    private TextMeshProUGUI text;

    private AudioClip clip;
    private byte[] bytes;
    private bool recording;

    private void Start()
    {
        startButton.onClick.AddListener(StartRecording);
        stopButton.onClick.AddListener(StopRecording);
    }

    private void StartRecording()
    {
        clip = Microphone.Start(null, false, 10, 44100);
        recording = true;
    }

    private void Update()
    {
        if (recording && clip != null && Microphone.GetPosition(null) >= clip.samples)
        {
            StopRecording();
        }
    }

    private void StopRecording()
    {
        var position = Microphone.GetPosition(null);
        Microphone.End(null);
        var samples = new float[position * clip.channels];
        clip.GetData(samples, 0);
        bytes = EncodeAsWav(samples, clip.frequency, clip.channels);
        recording = false;

        SendRecording();
    }

    private void SendRecording()
    {
        HuggingFaceAPI.AutomaticSpeechRecognition(bytes, ResponseFileData =>
        {
            text.text = ResponseFileData;
        }, error => {
            text.text = error;
        });
    }

    private byte[] EncodeAsWav(float[] samples, int frequency, int channels)
    {
        using (var memoryStream = new MemoryStream(44 + samples.Length * 2))
        {
            using (var writer = new BinaryWriter(memoryStream))
            {
                writer.Write("RIFF".ToCharArray());
                writer.Write(36 + samples.Length * 2);
                writer.Write("WAVE".ToCharArray());
                writer.Write("fmt".ToCharArray());
                writer.Write(16);
                writer.Write((ushort)1);
                writer.Write((ushort)channels);
                writer.Write(frequency);
                writer.Write(frequency * channels * 2);
                writer.Write((ushort)(channels * 2));
                writer.Write((ushort)2);
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