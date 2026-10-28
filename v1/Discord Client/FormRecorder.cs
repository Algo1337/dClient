using System;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

public class FormRecorder : IDisposable
{
    private readonly Form form;
    private readonly string tempDir;
    private CancellationTokenSource cts;
    private Task recordTask;
    private int frame;

    public int FPS { get; set; } = 30;

    public FormRecorder(Form form)
    {
        this.form = form;
        tempDir = Path.Combine(Path.GetTempPath(), "FormRecording_" + Guid.NewGuid());
        Directory.CreateDirectory(tempDir);
    }

    public void Start()
    {
        frame = 0;
        cts = new CancellationTokenSource();

        recordTask = Task.Run(async () =>
        {
            int delay = 1000 / FPS;

            while (!cts.IsCancellationRequested)
            {
                CaptureFrame();
                await Task.Delay(delay);
            }
        });
    }

    public async Task StopAsync(string outputFile)
    {
        cts.Cancel();
        await recordTask;

        Process ffmpeg = new Process();
        ffmpeg.StartInfo.FileName = "ffmpeg.exe";
        ffmpeg.StartInfo.Arguments =
            $"-y -framerate {FPS} -i \"{tempDir}\\frame_%06d.png\" " +
            "-c:v libx264 -pix_fmt yuv420p " +
            $"\"{outputFile}\"";
        ffmpeg.StartInfo.CreateNoWindow = true;
        ffmpeg.StartInfo.UseShellExecute = false;

        ffmpeg.Start();
        ffmpeg.WaitForExit();

        Directory.Delete(tempDir, true);
    }

    private void CaptureFrame()
    {
        if (form.InvokeRequired)
        {
            form.Invoke((Action)CaptureFrame);
            return;
        }

        using (Bitmap bmp = new Bitmap(form.Width, form.Height))
        {
            form.DrawToBitmap(bmp, new Rectangle(0, 0, bmp.Width, bmp.Height));

            string path = Path.Combine(tempDir, string.Format("frame_{0:D6}.png", frame++));
            bmp.Save(path, ImageFormat.Png);
        }
    }

    public void Dispose()
    {
        cts?.Cancel();
    }
}