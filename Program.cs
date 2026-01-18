using System;
using System.Runtime.InteropServices;

class Program
{
    // Window Messages
    private const int WM_DESTROY = 0x0002;

    // Window Styles
    private const uint WS_POPUP = 0x80000000;
    private const uint WS_EX_TOOLWINDOW = 0x00000080;

    // ShowWindow Commands
    private const int SW_SHOW = 5;

    // Structures
    [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Auto)]
    private struct WNDCLASS
    {
        public uint style;
        public WndProc lpfnWndProc;
        public int cbClsExtra;
        public int cbWndExtra;
        public IntPtr hInstance;
        public IntPtr hIcon;
        public IntPtr hCursor;
        public IntPtr hbrBackground;

        [MarshalAs(UnmanagedType.LPTStr)]
        public string lpszMenuName;

        [MarshalAs(UnmanagedType.LPTStr)]
        public string lpszClassName;
    }

    [StructLayout(LayoutKind.Sequential)]
    private struct MSG
    {
        public IntPtr hwnd;
        public uint message;
        public IntPtr wParam;
        public IntPtr lParam;
        public uint time;
        public POINT pt;
    }

    [StructLayout(LayoutKind.Sequential)]
    private struct POINT
    {
        public int x;
        public int y;
    }

    // Delegates
    private delegate IntPtr WndProc(IntPtr hWnd, uint msg, IntPtr wParam, IntPtr lParam);

    // P/Invoke declarations
    [DllImport("user32.dll", SetLastError = true, CharSet = CharSet.Auto)]
    private static extern ushort RegisterClass([In] ref WNDCLASS lpWndClass);

    [DllImport("user32.dll", SetLastError = true, CharSet = CharSet.Auto)]
    private static extern IntPtr CreateWindowEx(
        uint dwExStyle,
        string lpClassName,
        string lpWindowName,
        uint dwStyle,
        int x,
        int y,
        int nWidth,
        int nHeight,
        IntPtr hWndParent,
        IntPtr hMenu,
        IntPtr hInstance,
        IntPtr lpParam
    );

    [DllImport("user32.dll")]
    private static extern bool ShowWindow(IntPtr hWnd, int nCmdShow);

    [DllImport("user32.dll")]
    private static extern IntPtr DefWindowProc(
        IntPtr hWnd,
        uint uMsg,
        IntPtr wParam,
        IntPtr lParam
    );

    [DllImport("user32.dll")]
    private static extern bool GetMessage(
        out MSG lpMsg,
        IntPtr hWnd,
        uint wMsgFilterMin,
        uint wMsgFilterMax
    );

    [DllImport("user32.dll")]
    private static extern bool TranslateMessage([In] ref MSG lpMsg);

    [DllImport("user32.dll")]
    private static extern IntPtr DispatchMessage([In] ref MSG lpMsg);

    [DllImport("user32.dll")]
    private static extern void PostQuitMessage(int nExitCode);

    [DllImport("kernel32.dll", CharSet = CharSet.Auto)]
    private static extern IntPtr GetModuleHandle(string lpModuleName);

    // Window Procedure
    private static IntPtr WindowProc(IntPtr hwnd, uint uMsg, IntPtr wParam, IntPtr lParam)
    {
        if (uMsg == WM_DESTROY)
        {
            PostQuitMessage(0);
            return IntPtr.Zero;
        }
        return DefWindowProc(hwnd, uMsg, wParam, lParam);
    }

    [STAThread]
    static void Main(string[] args)
    {
        IntPtr hInstance = GetModuleHandle(null);

        WNDCLASS wc = new WNDCLASS
        {
            lpfnWndProc = WindowProc,
            hInstance = hInstance,
            lpszClassName = "DiscordQuestWindow",
        };

        RegisterClass(ref wc);

        IntPtr hwnd = CreateWindowEx(
            WS_EX_TOOLWINDOW,
            "DiscordQuestWindow",
            "",
            WS_POPUP,
            -10000,
            -10000,
            1,
            1,
            IntPtr.Zero,
            IntPtr.Zero,
            hInstance,
            IntPtr.Zero
        );

        ShowWindow(hwnd, SW_SHOW);

        MSG msg;
        while (GetMessage(out msg, IntPtr.Zero, 0, 0))
        {
            TranslateMessage(ref msg);
            DispatchMessage(ref msg);
        }
    }
}
