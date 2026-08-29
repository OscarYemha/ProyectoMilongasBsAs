using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Milongas.App
{
    public partial class FormPrincipal : Form
    {

        private FormMilongas? formMilongas;
        public FormPrincipal()
        {
            InitializeComponent();

            PicMilongas.LoadAsync(
        "https://www.hoy-milonga.com/img/nav-milongas.jpeg");
        }

        private void PnlMilongas_Click(
            object sender,
            EventArgs e)
        {
            if (formMilongas is null ||
                formMilongas.IsDisposed)
            {
                formMilongas =
                    new FormMilongas();
            }

            formMilongas.ShowDialog();
        }

    }
}
