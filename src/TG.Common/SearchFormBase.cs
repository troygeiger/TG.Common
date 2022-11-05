using System;
using System.Windows.Forms;

namespace TG.Common
{
    public partial class SearchFormBase : Form
    {
        public SearchFormBase()
        {
            InitializeComponent();
        }

        protected override void OnSizeChanged(EventArgs e)
        {
            base.OnSizeChanged(e);
            tableLayoutPanel1.Location =
                new System.Drawing.Point(
                    this.ClientSize.Width - tableLayoutPanel1.Width - tableLayoutPanel1.Margin.Right
             , this.ClientSize.Height - tableLayoutPanel1.Height - tableLayoutPanel1.Margin.Bottom);

        }

        public virtual ValueOptions ValueReplaceOption { get; set; }

        public virtual string InitialValue { get; set; }

        public virtual string ResultValue { get; set; }
    }
}
