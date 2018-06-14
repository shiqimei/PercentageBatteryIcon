using Microsoft.Win32;
using System;
using System.Drawing;
using System.Runtime.InteropServices;
using System.Windows.Forms;

namespace percentage
{
    class TrayIcon
    {
        [DllImport("user32.dll", CharSet = CharSet.Auto)]
        static extern bool DestroyIcon(IntPtr handle);

        private const string iconFont = "Microsoft Yahei";

        //全局变量
        public static int iconFontSize = 28;    // 带渲染文本的字体大小(全局变量)
        public static int xoffset = 0;
        public static int yoffset = 0;
        public static Color normalColor = Color.FromArgb(255, 255, 255); // 主颜色
        public static Color chargingColor = Color.FromArgb(254, 190, 4);   // 充电时颜色
        public static Color lowColor = Color.FromArgb(254, 97, 82);   // 低电量时颜色
        public static int isHideIcon = 1;
        public static int isShowPercent = 0;
        public static string autoHide;

        private string batteryPercentage;
        private NotifyIcon notifyIcon;

        private Color batteryColor = Color.Green;   // 电池数字颜色            

        public TrayIcon()
        {
            // 从注册表加载颜色
            RegistryKey hklm = Registry.CurrentUser;
            RegistryKey lgn0 = hklm.OpenSubKey(@"Software\BatteryIcon", true);
            if (lgn0 != null)   //当该注册表存在则应用注册表中的值
            { 
                RegistryKey lgn = hklm.OpenSubKey(@"Software\BatteryIcon", true);
                string fontsize = lgn.GetValue("fontsize").ToString();
                string xoff = lgn.GetValue("xoffset").ToString();
                string yoff = lgn.GetValue("yoffset").ToString();
                string normal = lgn.GetValue("normalColor").ToString();
                string charging = lgn.GetValue("chargingColor").ToString();
                string low = lgn.GetValue("lowColor").ToString();
                autoHide = lgn.GetValue("autoHide").ToString();
                try
                {
                    iconFontSize = Convert.ToInt32(fontsize);   // 从字符串获取字体大小
                    xoffset = Convert.ToInt32(xoff);
                    yoffset = Convert.ToInt32(yoff);
                } catch
                {
                    // do nothing
                }
                normalColor = (Color)new ColorConverter().ConvertFromString(normal);   // 从字符串获取颜色
                chargingColor = (Color)new ColorConverter().ConvertFromString(charging);   // 从字符串获取颜色
                lowColor = (Color)new ColorConverter().ConvertFromString(low);   // 从字符串获取颜色
            }

            ContextMenu contextMenu = new ContextMenu();
            MenuItem menuItem1 = new MenuItem();    //设置按钮
            MenuItem menuItem2 = new MenuItem();    //退出按钮

            notifyIcon = new NotifyIcon();

            // 初始化上下文菜单
            contextMenu.MenuItems.AddRange(new MenuItem[] { menuItem1, menuItem2 });

            // 初始化上下文菜单项
            // 初始化上下文菜单项
            menuItem1.Index = 0;
            menuItem1.Text = "设置";
            menuItem1.Click += new EventHandler(settingButton_Click);     // 注册上下文菜单点击事件

            menuItem2.Index = 1;
            menuItem2.Text = "退出";
            menuItem2.Click += new EventHandler(exitButton_Click);     // 注册上下文菜单点击事件

            notifyIcon.ContextMenu = contextMenu;

            batteryPercentage = "?";

            notifyIcon.Visible = true;

            // 设定计时器
            Timer timer = new Timer();
            timer.Tick += new EventHandler(timer_Tick); // 注册计时器事件(用于更新电池百分比数字)
            timer.Interval = 1000; // 计时器频率：1s (也就是电池百分比的更新频率)
            timer.Start();
        }

        /**
         * 电池百分比更新事件
         */
        private void timer_Tick(object sender, EventArgs e)
        {
            PowerStatus powerStatus = SystemInformation.PowerStatus;
            batteryPercentage = (powerStatus.BatteryLifePercent * 100).ToString();

            // 从注册表加载偏移信息
            RegistryKey hklm = Registry.CurrentUser;
            RegistryKey lgn = hklm.OpenSubKey(@"Software\BatteryIcon", true);
            iconFontSize = Convert.ToInt32(lgn.GetValue("fontsize").ToString()); // 更新信息
            xoffset = Convert.ToInt32(lgn.GetValue("xoffset").ToString());
            yoffset = Convert.ToInt32(lgn.GetValue("yoffset").ToString());

            // 如果电量充满则不显示
            if (batteryPercentage == "100" && autoHide == "true")
            {
                    notifyIcon.Visible = false;
            } else
            {
                if (batteryPercentage == "100")
                {
                    iconFontSize = 23;
                    xoffset = -10;
                    yoffset = 5;
                    batteryPercentage = "100";
                }
                notifyIcon.Visible = true;
            }

            // 如果电池正在充电,则将数字颜色改为金黄色
            if (powerStatus.BatteryChargeStatus.ToString().Contains(BatteryChargeStatus.Charging.ToString()))
            {
                batteryColor = chargingColor;
            } else
            {
                if (powerStatus.BatteryChargeStatus.ToString().Contains(BatteryChargeStatus.Low.ToString()))
                {
                    batteryColor = lowColor;
                } else
                {
                    batteryColor = normalColor;
                }
            }                                         // 渲染字体内容
            using (Bitmap bitmap = new Bitmap(DrawText(batteryPercentage, new Font(iconFont, iconFontSize), batteryColor, Color.Transparent)))   // 背景色透明
            {
                IntPtr intPtr = bitmap.GetHicon();
                try
                {
                    using (Icon icon = Icon.FromHandle(intPtr))
                    {
                        notifyIcon.Icon = icon;
                        notifyIcon.Text = batteryPercentage + "%";
                    }
                }
                finally
                {
                    DestroyIcon(intPtr);
                }
            }
        }

        /**
         * 右键菜单设置按钮点击事件
         */
        private void settingButton_Click(object sender, EventArgs e)
        {
            new Settings().ShowDialog();
        }

        /**
         * 右键菜单退出按钮点击事件
         */
        private void exitButton_Click(object sender, EventArgs e)
        {
            notifyIcon.Visible = false;
            notifyIcon.Dispose();
            Application.Exit();
        }

        private Image DrawText(String text, Font font, Color textColor, Color backColor)
        {
            var textSize = GetImageSize(text, font);
            Image image = new Bitmap(48, 45);       // 位图大小
            using (Graphics graphics = Graphics.FromImage(image))
            {
                // 绘制背景
                graphics.Clear(backColor);

                // 文本笔刷
                using (Brush textBrush = new SolidBrush(textColor))
                {
                    graphics.TextRenderingHint = System.Drawing.Text.TextRenderingHint.SystemDefault;   //字体渲染方案
                    graphics.DrawString(text, font, textBrush, xoffset, yoffset);   //渲染文本的偏移
                    graphics.Save();
                }
            }

            return image;
        }

        private static SizeF GetImageSize(string text, Font font)
        {
            using (Image image = new Bitmap(32, 32))
            using (Graphics graphics = Graphics.FromImage(image))
                return graphics.MeasureString(text, font);
        }
    }
}
