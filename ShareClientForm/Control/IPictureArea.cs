using System.Drawing;

namespace SharedClientForm
{
    public interface IPictureArea
    {
        public void PaintDefault();
        public void PaintPicture(Image img);
    }
}
