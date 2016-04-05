using System;
using System.Collections;
using System.Windows.Forms;

namespace instaLolSkill
{
    public partial class FormSelect : Form
    {
        private Functions functions;

        public FormSelect()
        {
            InitializeComponent();
            Visible = false;
            WindowState = FormWindowState.Minimized;
            notifyIcon.Visible = true;
            functions = new Functions();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            UsersFinder usersFinder = new UsersFinder();
            ArrayList foundUsers = usersFinder.getCurrentUsers();

            if(foundUsers != null)
                foreach(string user in foundUsers)
                    usersFoundCombo.Items.Add(user);
            else
            {
                //No hay usuarios almacenados. Habilito un textbox para que lo ingrese manualmente y oculto el combo.
                usernameBox.Visible = true;
                usersFoundCombo.Visible = false;
            }

            //Checkeo si inicia o no con el sistema
            checkBox1.CheckState = (Properties.Settings.Default.startup) ? CheckState.Checked : CheckState.Unchecked;

            //Me fijo si hay un usuario ya setteado sino muestro el form
            if(Properties.Settings.Default.actual_username != null && Properties.Settings.Default.region != null)
            {
                usersFoundCombo.Text = Properties.Settings.Default.actual_username;
                regionCombo.Text = functions.reverseRegion(Properties.Settings.Default.region);
                functions.startLooking();
                notifyIcon.Visible = true;
                notifyIcon.ShowBalloonTip(500);
                Hide();
            }
            else
            {
                WindowState = FormWindowState.Normal;
                Visible = true;
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            notifyIcon.Visible = true;
            notifyIcon.ShowBalloonTip(500);
            Properties.Settings.Default.Save();
            Hide();
        }

        private void notifyIcon_MouseDoubleClick(object sender, MouseEventArgs e)
        {
            notifyIcon.Visible = false;
            Show();
        }

        private void cerrarToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Application.Exit();
        }

        private void opcionesToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Show();
        }

        private void checkBox1_CheckedChanged(object sender, EventArgs e)
        {
            if(checkBox1.Checked)
            {
                functions.setStartup(true);
                Properties.Settings.Default.startup = true;
            }
            else
            {
                functions.setStartup(false);
                Properties.Settings.Default.startup = false;
            }
        }

        private void comboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            Properties.Settings.Default.region = functions.convertRegion(regionCombo.SelectedItem.ToString());
            Properties.Settings.Default.Save();
        }

        private void usersFoundCombo_SelectedIndexChanged(object sender, EventArgs e)
        {
            Properties.Settings.Default.actual_username = usersFoundCombo.SelectedItem.ToString();
            Properties.Settings.Default.Save();
        }

        private void linkLabel1_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            functions.openGithub();
        }

        private void usernameBox_TextChanged(object sender, EventArgs e)
        {
            Properties.Settings.Default.actual_username = usernameBox.Text;
            Properties.Settings.Default.Save();
        }

        private void button2_Click(object sender, EventArgs e)
        {
            if(usernameBox.Visible)
            {
                usersFoundCombo.Visible = true;
                usernameBox.Visible = false;
                button2.Text = "Ingresar usuario";
            }
            else
            {
                usersFoundCombo.Visible = false;
                usernameBox.Visible = true;
                button2.Text = "Cambiar a lista";
            }
        }
    }
}
