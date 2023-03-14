using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;

namespace SharedClientForm.Controls
{
    public partial class SpeedMeter : UserControl
    {
        public IEnumerable<Parameter> SelectedValues => listView.SelectedItems.Cast<ListViewItem>().Select(i => i.Tag).Cast<Parameter>();

        public SpeedMeter()
        {
            InitializeComponent();
        }

        public Parameter Parameter(string name)
        {
            var item = new ListViewItem(name);
            _ = item.SubItems.Add(new ListViewItem.ListViewSubItem());
            var param = new Parameter(name, item);
            item.Tag = param;
            return param;
        }

        public void Add(Parameter parameter)
        {
            _ = listView.Items.Add(parameter.Item);
        }
    }

    public class Parameter
    {
        public string Name { get; }
        public ListViewItem Item { get; }

        public Parameter(string name, ListViewItem item)
        {
            Name = name;
            Item = item;
        }

        public void SetSpeed(string speed)
        {
            Item.SubItems[1].Text = speed;
        }
    }
}
