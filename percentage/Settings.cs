using Microsoft.Win32;
using System;
using System.Drawing;
using System.Text.RegularExpressions;
using System.Threading;
using System.Windows.Forms;

namespace percentage
{
    public partial class Settings : Form
    {
        public Settings()
        {
            RegistryKey hklm = Registry.CurrentUser;
            RegistryKey lgn0 = hklm.OpenSubKey(@"Software\BatteryIcon", true);
            if (lgn0 == null)   //初始化
            {
                RegistryKey lgn = hklm.OpenSubKey(@"Software", true).CreateSubKey("BatteryIcon");
                lgn.SetValue("xoffset", "0", RegistryValueKind.String);
                lgn.SetValue("yoffset", "0", RegistryValueKind.String);
                lgn.SetValue("fontsize", "28", RegistryValueKind.String);
                lgn.SetValue("normalColor", "255, 255, 255", RegistryValueKind.String);
                lgn.SetValue("chargingColor", "254, 190, 4", RegistryValueKind.String);
                lgn.SetValue("lowColor", "254, 97, 82", RegistryValueKind.String);
                lgn.SetValue("autoHide", "false", RegistryValueKind.String); // 电池电量充满后是否自动隐藏
            }
            RegistryKey lgn1 = hklm.OpenSubKey(@"Software\BatteryIcon", true);
            // 从注册表获取数据
            string fontsize = lgn1.GetValue("fontsize").ToString();
            string xoffset = lgn1.GetValue("xoffset").ToString();
            string yoffset = lgn1.GetValue("yoffset").ToString();
            string normalColor = lgn1.GetValue("normalColor").ToString();
            string chargingColor = lgn1.GetValue("chargingColor").ToString();
            string lowColor = lgn1.GetValue("lowColor").ToString();

            InitializeComponent(); //绘制控件
            textBox1.Text = fontsize;
            textBox3.Text = xoffset;
            textBox4.Text = yoffset;
            pictureBox1.BackColor = (Color)new ColorConverter().ConvertFromString(normalColor);   // 从字符串获取颜色
            pictureBox2.BackColor = (Color)new ColorConverter().ConvertFromString(chargingColor);
            pictureBox3.BackColor = (Color)new ColorConverter().ConvertFromString(lowColor);

            // 解决textbox1自动聚焦的问题
            textBox2.Visible = false;
            textBox2.Enabled = false;
            ActiveControl = textBox2;
        }

        private void label1_Click(object sender, EventArgs e)
        {

        }

        private void label2_Click(object sender, EventArgs e)
        {

        }

        private void label4_Click(object sender, EventArgs e)
        {

        }

        private void label3_Click(object sender, EventArgs e)
        {

        }

        private void textBox1_TextChanged(object sender, EventArgs e)
        {
            try
            {
                TrayIcon.iconFontSize = Convert.ToInt32(textBox1.Text);
                // 将设置的字号保存到注册表中
                RegistryKey hklm = Registry.CurrentUser;
                RegistryKey lgn = hklm.OpenSubKey(@"Software", true).CreateSubKey("BatteryIcon");
                lgn.SetValue("fontsize", textBox1.Text, RegistryValueKind.String);
            } catch
            {
                // do nothing
            }
        }

        private void textBox3_TextChanged(object sender, EventArgs e)
        {
            try
            {
                TrayIcon.xoffset = Convert.ToInt32(textBox3.Text);
                RegistryKey hklm = Registry.CurrentUser;
                RegistryKey lgn = hklm.OpenSubKey(@"Software", true).CreateSubKey("BatteryIcon");
                lgn.SetValue("xoffset", textBox3.Text, RegistryValueKind.String);
            }
            catch
            {
                // do nothing
            }
        }


        private void textBox4_TextChanged(object sender, EventArgs e)
        {
            try
            {
                TrayIcon.yoffset = Convert.ToInt32(textBox4.Text);
                RegistryKey hklm = Registry.CurrentUser;
                RegistryKey lgn = hklm.OpenSubKey(@"Software", true).CreateSubKey("BatteryIcon");
                lgn.SetValue("yoffset", textBox4.Text, RegistryValueKind.String);
            }
            catch
            {
                // do nothing
            }
        }

        private void pictureBox1_Click_1(object sender, EventArgs e)
        {
            ColorDialog loColorForm = new ColorDialog();
            if (loColorForm.ShowDialog() == DialogResult.OK)
            {
                pictureBox1.BackColor = loColorForm.Color;
                TrayIcon.normalColor = loColorForm.Color;
                // 将设置的颜色保存到注册表中
                RegistryKey hklm = Registry.CurrentUser;
                RegistryKey lgn = hklm.OpenSubKey(@"Software", true).CreateSubKey("BatteryIcon");
                lgn.SetValue("normalColor", new ColorConverter().ConvertToString(TrayIcon.normalColor), RegistryValueKind.String);
            }
        }

        private void pictureBox2_Click(object sender, EventArgs e)
        {
            ColorDialog loColorForm = new ColorDialog();
            if (loColorForm.ShowDialog() == DialogResult.OK)
            {
                pictureBox2.BackColor = loColorForm.Color;
                TrayIcon.chargingColor = loColorForm.Color;
                // 将设置的颜色保存到注册表中
                RegistryKey hklm = Registry.CurrentUser;
                RegistryKey lgn = hklm.OpenSubKey(@"Software", true).CreateSubKey("BatteryIcon");
                lgn.SetValue("chargingColor", new ColorConverter().ConvertToString(TrayIcon.chargingColor), RegistryValueKind.String);
            }
        }

        private void pictureBox3_Click(object sender, EventArgs e)
        {
            ColorDialog loColorForm = new ColorDialog();
            if (loColorForm.ShowDialog() == DialogResult.OK)
            {
                pictureBox3.BackColor = loColorForm.Color;
                TrayIcon.lowColor = loColorForm.Color;          // 更新颜色
                // 将设置的颜色保存到注册表中
                RegistryKey hklm = Registry.CurrentUser;
                RegistryKey lgn = hklm.OpenSubKey(@"Software", true).CreateSubKey("BatteryIcon");
                lgn.SetValue("lowColor", new ColorConverter().ConvertToString(TrayIcon.lowColor), RegistryValueKind.String);
            }
        }

        // 限制文本框只能输入数字
        private void textBox1_KeyPress(object sender, KeyPressEventArgs e)
        {
            if(!Char.IsNumber(e.KeyChar) && e.KeyChar != (char)8)
            {
                e.Handled = true;
            }
        }

        private void textBox3_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (!Char.IsNumber(e.KeyChar) && e.KeyChar != (char)8 && e.KeyChar != '-')
            {
                e.Handled = true;
            }
        }

        private void textBox4_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (!Char.IsNumber(e.KeyChar) && e.KeyChar != (char)8 && e.KeyChar != '-')
            {
                e.Handled = true;
            }
        }

        private void pictureBox4_Click(object sender, EventArgs e)
        {

        }

        private void label7_Click(object sender, EventArgs e)
        {
            RegistryKey key = Registry.ClassesRoot.OpenSubKey(@"http\shell\open\command\");
            string s = key.GetValue("").ToString();

            Regex reg = new Regex("\"([^\"]+)\"");
            MatchCollection matchs = reg.Matches(s);

            string filename = "";
            if (matchs.Count > 0)
            {
                filename = matchs[0].Groups[1].Value;
                System.Diagnostics.Process.Start(filename, "https://github.com/loliMay/PercentageBatteryIcon");
            }
        }

        private void label8_Click(object sender, EventArgs e)
        {

        }

        private void label9_Click(object sender, EventArgs e)
        {

        }

        private void Settings_Load(object sender, EventArgs e)
        {

        }

        // 电池电量充满后是否自动隐藏电池图标
        private void checkBox1_CheckedChanged(object sender, EventArgs e)
        {
            RegistryKey hklm = Registry.CurrentUser;
            RegistryKey lgn = hklm.OpenSubKey(@"Software\BatteryIcon", true);
            if (checkBox1.Checked == true)
            {
                TrayIcon.autoHide = "true";
                lgn.SetValue("autoHide", "true", RegistryValueKind.String);
            } else
            {
                TrayIcon.autoHide = "false";
                lgn.SetValue("autoHide", "false", RegistryValueKind.String);
            }
        }
    }
}
