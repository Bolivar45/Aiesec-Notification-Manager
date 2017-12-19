using Aiesec_Notification_Manager.Models;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Data;
using System.Data.Entity;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using VkNet;
using VkNet.Enums.Filters;
using VkNet.Model;

namespace Aiesec_Notification_Manager
{

    public partial class Form1 : Form
    {
        VkBot vkbot;
        TgBot tgBot;
        DataContext db = new DataContext();
        public Form1()
        {
            InitializeComponent();
        }
        private void button1_Click(object sender, EventArgs e)
        {
            vkbot = new VkBot();
            vkbot.BotNotify += LogsHandler;
            if (vkbot.Auth_Start())
            {
                button2.Enabled = true;
                button8.Enabled = true;
            }
        }
        private void button3_Click(object sender, EventArgs e)
        {

            foreach (string name in richTextBox1.Lines)
            {
                if (name != "")
                {
                    Models.User Users = null;
                    string[] s = name.Split(' ');
                    if (s.Length > 1)
                    {
                        string fname = s[0];
                        string lname = s[1];
                        Users = db.Users.Where(x => x.FirstName == fname && x.LastName == lname).FirstOrDefault();
                    }
                    if (Users != null)
                    {
                        if (vkbot.SendMessageWithPhoto(richTextBox3.Text, Users.VkUserId))
                            ShowColorStatus(name, richTextBox1, Color.LightGreen);
                        else
                            ShowColorStatus(name, richTextBox1, Color.LightPink);
                    }
                    else
                    {
                        if (vkbot.SendMessageWithPhoto(richTextBox3.Text, name))
                            ShowColorStatus(name, richTextBox1, Color.LightGreen);
                        else
                            ShowColorStatus(name, richTextBox1, Color.LightPink);
                    }
                }
            }
        }
        private void LogsHandler(string msg, Color color)        //обработчик логов от вкбота
        {
            richTextBox2.AppendText(msg + Environment.NewLine, color);
        }
        void LogsHandlerTheardsSave(string msg, Color color)
        {
            if (richTextBox2.InvokeRequired)
                richTextBox2.Invoke(new Action(() => LogsHandler(msg, color)));
            else LogsHandler(msg, color);
        }

        private void button2_Click(object sender, EventArgs e)
        {
            label7.Text = "000";
            label11.Text = "000";
            foreach (string name in richTextBox1.Lines)
            {
                if (name != "")
                {
                    Models.User Users = null;
                    string[] s = name.Split(' ');
                    if (s.Length > 1)
                    {
                        string fname = s[0];
                        string lname = s[1];
                        Users = db.Users.Where(x => x.FirstName == fname && x.LastName == lname).FirstOrDefault();
                    }

                    //Models.User Users = db.Users.Where(x => x.FirstName == fname && x.LastName == lname).FirstOrDefault();
                    if (Users != null)
                    {
                        if (vkbot.SendMessage(richTextBox3.Text, Users.VkUserId))
                            ShowColorStatus(name, richTextBox1, Color.LightGreen);
                        else
                            ShowColorStatus(name, richTextBox1, Color.LightPink);
                    }
                    else
                    {
                        if (vkbot.SendMessage(richTextBox3.Text, name))
                            ShowColorStatus(name, richTextBox1, Color.LightGreen);
                        else
                            ShowColorStatus(name, richTextBox1, Color.LightPink);
                    }

                }
            }
        }

        public void ShowColorStatus(string cell, RichTextBox rtbox, Color color)
        {
            rtbox.Find(cell);
            rtbox.SelectionBackColor = color;
            if (rtbox == richTextBox1 && color == Color.LightGreen)
                label7.Text = Convert.ToString(Convert.ToInt32(label7.Text) + 1);
            if (rtbox == richTextBox4 && color == Color.LightGreen)
                label9.Text = Convert.ToString(Convert.ToInt32(label9.Text) + 1);
            if (rtbox == richTextBox1 && color == Color.LightPink)
                label11.Text = Convert.ToString(Convert.ToInt32(label11.Text) + 1);
            if (rtbox == richTextBox4 && color == Color.LightPink)
                label12.Text = Convert.ToString(Convert.ToInt32(label12.Text) + 1);
        }

        private void button6_Click(object sender, EventArgs e)
        {
            tgBot = new TgBot();
            tgBot.BotNotify += LogsHandlerTheardsSave;
            tgBot.StartBot();
            button5.Enabled = true;
            button4.Enabled = true;
        }

        private void button5_ClickAsync(object sender, EventArgs e)
        {
            label12.Text = "000";
            label9.Text = "000";
            TgSendMessageAsync();
        }

        private async Task TgSendMessageAsync()
        {

            foreach (string name in richTextBox4.Lines)
            {
                if (name != "")
                {
                    string[] s = name.Split(' ');
                    string fname = s[0];
                    string lname = s[1];
                    Models.User Users = db.Users.Where(x => x.FirstName == fname && x.LastName == lname).FirstOrDefault();
                    if (await tgBot.SendMessageAsync(richTextBox3.Text, Convert.ToInt32(Users.TgChatId)))
                        ShowColorStatus(name, richTextBox4, Color.LightGreen);
                    else
                        ShowColorStatus(name, richTextBox4, Color.LightPink);
                }
            }
        }
        private async Task TgSendPhotoAsync()
        {
            if (openFileDialog1.ShowDialog() == DialogResult.Cancel)
                return;
            string filename = openFileDialog1.FileName;

            foreach (string name in richTextBox4.Lines)
            {
                if (name != "")
                {
                    string[] s = name.Split(' ');
                    string fname = s[0];
                    string lname = s[1];
                    Models.User Users = db.Users.Where(x => x.FirstName == fname && x.LastName == lname).FirstOrDefault();
                    if (await tgBot.SendMessageAsync(richTextBox3.Text, Convert.ToInt32(Users.TgChatId)) && await tgBot.SendPhotoAsync(filename, Convert.ToInt32(Users.TgChatId)))
                        ShowColorStatus(name, richTextBox4, Color.LightGreen);
                    else
                        ShowColorStatus(name, richTextBox4, Color.LightPink);
                }
            }
        }

        private void button4_Click(object sender, EventArgs e)
        {
            label9.Text = "000";
            TgSendPhotoAsync();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            comboBox1.SelectedIndex = 5;
        }

        private void comboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            string selectedState = comboBox1.SelectedItem.ToString();

            TgDepatmentListUdate(selectedState);
        }
        private void TgDepatmentListUdate(string department)
        {
            IQueryable<Models.User> Users;
            richTextBox4.Clear();
            richTextBox1.Clear();
            if (department == "All Departments")
            {
                Users = db.Users.Where(x => x.Depatment != "");
                foreach (Models.User i in Users)
                {
                    richTextBox4.AppendText(i.FirstName + " " + i.LastName + Environment.NewLine);
                    richTextBox1.AppendText(i.FirstName + " " + i.LastName + Environment.NewLine);
                }
            }
            else
            {
                Users = db.Users.Where(x => x.Depatment == department);
                foreach (Models.User i in Users)
                {
                    richTextBox4.AppendText(i.FirstName + " " + i.LastName + Environment.NewLine);
                    richTextBox1.AppendText(i.FirstName + " " + i.LastName + Environment.NewLine);
                }
            }
        }

        private void button7_Click(object sender, EventArgs e)
        {
            Form f2 = new Form2();
            f2.Show();
        }

        private void button8_Click(object sender, EventArgs e)
        {
            if (openFileDialog1.ShowDialog() == DialogResult.Cancel)
                return;
            string filename = openFileDialog1.FileName;
            VkBot.SavePhotoToServer(filename);
            button3.Enabled = true;
        }
    }
}
