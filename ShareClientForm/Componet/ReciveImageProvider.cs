using ShareClient.Component;
using System.Collections.Concurrent;
using System.Drawing;
using System.IO;

namespace SharedClientForm.Component
{
    public class ReciveImageProvider : IReceiveDataProvider
    {
        public int Capacity { get; set; } = 100;

        private readonly ConcurrentQueue<Image> imgQueue = new();

        public ReciveImageProvider() { }

        public Image GetImage()
        {
            if (imgQueue.IsEmpty)
            {
                return null;
            }

            imgQueue.TryDequeue(out var img);
            return img;
        }

        public void Dispose()
        {
            if (!imgQueue.IsEmpty)
            {
                foreach (var img in imgQueue)
                {
                    img.Dispose();
                }
                imgQueue.Clear();
            }
        }

        public void Receive(byte[] data)
        {
            var img = Image.FromStream(new MemoryStream(data));
            imgQueue.Enqueue(img);
        }
    }
}
