using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Aiesec_Notification_Manager
{
    public partial class Form2 : Form
    {
        public Form2()
        {
            InitializeComponent();
        }

        private void Form2_Load(object sender, EventArgs e)
        {
            // TODO: данная строка кода позволяет загрузить данные в таблицу "_AppDb_mdfDataSet.Users". При необходимости она может быть перемещена или удалена.
            this.usersTableAdapter.Fill(this._AppDb_mdfDataSet.Users);

        }
    }
}
