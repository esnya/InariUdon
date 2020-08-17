
using UdonSharp;
using UnityEngine;
using VRC.SDKBase;
using VRC.Udon;

public class UpdateFPSVisualizer : UdonSharpBehaviour
{
    public UnityEngine.UI.Text text;
    public ParticleSystem particle;
    public int df = 60;

    float prevTime = 0;
    int nextFrame = 0;
    float avgFps = 0;
    float prevFrameTime = 0;

    void Update() {
        var now = Time.time;
        var fps = 1.0 / (now - prevTime);
        prevTime = now;

        var frame = Time.frameCount;
        if (frame >= nextFrame) {
            avgFps = df / (now - prevFrameTime);
            prevFrameTime = now;
            nextFrame = frame + df;
        }

        if (text != null) {
            text.text = $"Update FPS:\t{fps.ToString("n1")}\n{df}F Average:\t{avgFps.ToString("n1")}";
        }

        var value = (float)(fps / 90.0f);

        if (particle != null) {
            var p = particle.transform.localPosition;
            p.y = value;
            particle.transform.localPosition = p;

            var main = particle.main;
            main.startColor = Color.HSVToRGB(value, 1.0f, 1.0f);
            particle.Emit(1);
        }
    }
}
