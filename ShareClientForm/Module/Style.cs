using System;
using System.Collections.Generic;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

namespace ShareClientForm.Module
{
    public class Style
    {
        public ContorlStyle ButtonStyle = new ContorlStyle(typeof(Button), (c) =>
         {
             var b = (Button)c;
             b.FlatStyle = FlatStyle.Flat;
             b.FlatAppearance.BorderSize = 0;
             b.Font = new Font("Yu Gothic UI", 12F, FontStyle.Bold, GraphicsUnit.Point);
         });
    }
}
