using System.Windows;

namespace WPFSample
{
    /// <summary>
    /// WPF 示例应用程序的主窗口
    /// </summary>
    /// <remarks>
    /// 这个窗口包含一个 CartesianChart 控件，用于展示 LiveCharts2 图表功能
    /// </remarks>
    public partial class MainWindow : Window
    {
        /// <summary>
        /// 初始化 <see cref="MainWindow"/> 类的新实例
        /// </summary>
        /// <remarks>
        /// 构造函数会调用 InitializeComponent 方法来加载 XAML 中定义的界面
        /// </remarks>
        public MainWindow()
        {
            InitializeComponent();
        }
    }
}