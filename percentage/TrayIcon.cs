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

        private const string iconFont = "Segoe UI";
        private const int iconFontSize = 26;    // 带渲染文本的字体大小

        private string batteryPercentage;
        private NotifyIcon notifyIcon;

        private Color batteryColor = Color.Green;   // 电池数字颜色            

        public TrayIcon()
        {
            ContextMenu contextMenu = new ContextMenu();
            MenuItem menuItem = new MenuItem();

            notifyIcon = new NotifyIcon();

            // 初始化上下文菜单
            contextMenu.MenuItems.AddRange(new MenuItem[] { menuItem });

            // 初始化上下文菜单项
            menuItem.Index = 0;
            menuItem.Text = "退出";
            menuItem.Click += new EventHandler(menuItem_Click);     // 注册上下文菜单点击事件

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

            // 如果电量充满则不显示
            if(batteryPercentage == "100")
            {
                notifyIcon.Visible = false;
            } else
            {
                notifyIcon.Visible = true;
            }

            // 如果电池正在充电,则将数字颜色改为金黄色
            if (powerStatus.BatteryChargeStatus.ToString().Contains(BatteryChargeStatus.Charging.ToString()))
            {
                batteryColor = Color.FromArgb(254,190,4);
            } else
            {
                if (powerStatus.BatteryChargeStatus.ToString().Contains(BatteryChargeStatus.Low.ToString()))
                {
                    batteryColor = Color.FromArgb(254, 97, 82);
                } else
                {
                    batteryColor = Color.FromArgb(7, 216, 47);
                }
            }
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
         * 右键菜单退出按钮点击事件
         */
        private void menuItem_Click(object sender, EventArgs e)
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
                    graphics.TextRenderingHint = System.Drawing.Text.TextRenderingHint.SystemDefault;
                    graphics.DrawString(text, font, textBrush, 6, 0);   //渲染文本的偏移
                    graphics.Save();
                }
            }

            return image;
        }

        private static SizeF GetImageSize(string text, Font font)
        {
            using (Image image = new Bitmap(10, 10))
            using (Graphics graphics = Graphics.FromImage(image))
                return graphics.MeasureString(text, font);
        }
    }
}
