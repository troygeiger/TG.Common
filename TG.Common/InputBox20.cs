using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

namespace TG.Common
{
    public partial class InputBox : Form
    {
        Type searchForm = null;

        /// <summary>
        /// Creates a new instance of <see cref="InputBox"/>.
        /// </summary>
        public InputBox()
        {
            InitializeComponent();
            this.Load += InputBox_Load; 
        }

        private void InputBox_Load(object sender, EventArgs e)
        {
            txtValue.Select();
        }

        /// <summary>
        /// Creates a new instance of <see cref="InputBox"/>.
        /// </summary>
        /// <param name="title">The title of the input box.</param>
        /// <param name="description">A brief description of the information being requested.</param>
        /// <param name="value">A value to initialize the <see cref="InputBox"/> with.</param>
        public InputBox(string title, string description, string value) : this()
        {
            this.Text = title;
            Description = description;
            Value = value;
        }

        /// <summary>
        /// Creates a new instance of <see cref="InputBox"/>.
        /// </summary>
        /// <param name="title">The title of the input box.</param>
        /// <param name="description">A brief description of the information being requested.</param>
        /// <param name="value">A value to initialize the <see cref="InputBox"/> with.</param>
        /// <param name="searchFormType">A type that is a base type of <see cref="SearchFormBase"/> that an instance will be created when the search button is clicked.</param>
        public InputBox(string title, string description, string value, Type searchFormType): this(title, description, value)
        {
            this.SearchForm = searchFormType;
        }

        /// <summary>
        /// The title of the <see cref="InputBox"/>.
        /// </summary>
        public string Title
        {
            get { return this.Text; }
            set { this.Text = value; }
        }

        /// <summary>
        /// A brief description of the information being requested.
        /// </summary>
        public string Description
        {
            get { return lblDescription.Text; }
            set { lblDescription.Text = value; }
        }

        /// <summary>
        /// The value of the text box.
        /// </summary>
        public string Value
        {
            get { return txtValue.Text; }
            set { txtValue.Text = value; }
        }

        /// <summary>
        /// A type that is a base type of <see cref="SearchFormBase"/> that an instance will be created when the search button is clicked.
        /// </summary>
        public Type SearchForm
        {
            get
            {
                return searchForm;
            }
            set
            {
                if (value != null && value.BaseType != typeof(SearchFormBase))
                    throw new Exception("Type must be a base type of SearchFormBase.");
                searchForm = value;
                btnSearch.Visible = value != null;
            }
        }

        public bool IsPassword
        {
            get { return txtValue.UseSystemPasswordChar; }
            set { txtValue.UseSystemPasswordChar = value; }
        }

        private void txtValue_TextChanged(object sender, EventArgs e)
        {
            btnOk.Enabled = txtValue.TextLength > 0;
        }

        private void btnSearch_Click(object sender, EventArgs e)
        {
            try
            {
                if (searchForm == null)
                    return;
                SearchFormBase form = Activator.CreateInstance(searchForm) as SearchFormBase;
                if (form.ShowDialog() == DialogResult.OK)
                {
                    switch (form.ValueReplaceOption)
                    {
                        case ValueOptions.Replace:
                            txtValue.Text = form.ResultValue;
                            break;
                        case ValueOptions.Append:
                            txtValue.Text += form.ResultValue;
                            break;
                        case ValueOptions.AppendSemiColonSeparated:
                            if (!string.IsNullOrEmpty(txtValue.Text))
                                txtValue.Text = form.ResultValue;
                            else
                                txtValue.Text += (";" + form.ResultValue);
                            break;
                        default:
                            break;
                    }
                    
                }
            }
            catch (Exception)
            {
                
            }
        }

        /// <summary>
        /// Gets or sets whether or not the Ok button is enabled.
        /// </summary>
        public bool OkButtonEnabled
        {
            get { return btnOk.Enabled; }
            set { btnOk.Enabled = value; }
        }

        /// <summary>
        /// Gets or sets whether or not the Cancel button is enabled.
        /// </summary>
        public bool CancelButtonEnabled
        {
            get { return btnCancel.Enabled; }
            set { btnCancel.Enabled = value; }
        }
    }
}
