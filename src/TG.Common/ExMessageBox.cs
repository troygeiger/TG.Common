using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

namespace TG.Common
{
    public partial class ExMessageBox : Form
    {
        string msg = null;

        public ExMessageBox()
        {
            InitializeComponent();
        }

        public static void Show(string message)
        {
            
        }

        public string MessageText
        {
            get
            {
                return msg;
            }
            set
            {
                msg = value;
                OnMessageTextChanged();
            }
        }

        protected virtual void OnMessageTextChanged()
        {

        }
    }
}
