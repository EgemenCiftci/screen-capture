using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Windows.Input;

namespace ScreenCapture;

public static class KeyboardHook
{
    private const int WH_KEYBOARD_LL = 13;
    private const int WM_KEYDOWN = 0x0100;
    private const int WM_KEYUP = 0x0101;
    private const int WM_SYSKEYDOWN = 0x0104;
    private const int WM_SYSKEYUP = 0x0105;
    private static readonly LowLevelKeyboardProc _proc = HookCallback;
    private static IntPtr hookID = IntPtr.Zero;
    private static Action<string>? action;

    private static bool ctrlPressed = false;
    private static bool altPressed = false;
    private static bool shiftPressed = false;

    private static readonly List<string> keys = [];

    [DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = true)]
    private static extern IntPtr SetWindowsHookEx(int idHook, LowLevelKeyboardProc lpfn, IntPtr hMod, uint dwThreadId);

    [DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = true)]
    [return: MarshalAs(UnmanagedType.Bool)]
    private static extern bool UnhookWindowsHookEx(IntPtr hhk);

    [DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = true)]
    private static extern IntPtr CallNextHookEx(IntPtr hhk, int nCode, IntPtr wParam, IntPtr lParam);

    [DllImport("kernel32.dll", CharSet = CharSet.Auto, SetLastError = true)]
    private static extern IntPtr GetModuleHandle(string lpModuleName);

    private delegate IntPtr LowLevelKeyboardProc(int nCode, IntPtr wParam, IntPtr lParam);

    public static void SetHook(Action<string> action)
    {
        KeyboardHook.action = action;
        hookID = SetHook(_proc);
    }

    private static IntPtr SetHook(LowLevelKeyboardProc proc)
    {
        using Process curProcess = Process.GetCurrentProcess();
        using ProcessModule? curModule = curProcess.MainModule;

        return curModule == null
            ? throw new ArgumentNullException(nameof(curModule))
            : SetWindowsHookEx(WH_KEYBOARD_LL, proc, GetModuleHandle(curModule.ModuleName), 0);
    }

    private static IntPtr HookCallback(int nCode, IntPtr wParam, IntPtr lParam)
    {
        if (nCode >= 0)
        {
            int vkCode = Marshal.ReadInt32(lParam);
            Key key = KeyInterop.KeyFromVirtualKey(vkCode);
            bool isKeyDown = wParam == WM_KEYDOWN;
            bool isSystemKeyDown = wParam == WM_SYSKEYDOWN;

            //Serilog.Log.Information($"Key:{key} KD:{isKeyDown} SKD:{isSystemKeyDown}");

            switch (key)
            {
                case Key.LeftShift:
                case Key.RightShift:
                    shiftPressed = isKeyDown || isSystemKeyDown;
                    break;
                case Key.LeftCtrl:
                case Key.RightCtrl:
                    ctrlPressed = isKeyDown || isSystemKeyDown;
                    break;
                case Key.LeftAlt:
                case Key.RightAlt:
                    altPressed = isKeyDown || isSystemKeyDown;
                    break;
                case Key.None:
                case Key.Cancel:
                case Key.Back:
                case Key.Tab:
                case Key.LineFeed:
                case Key.Clear:
                case Key.Enter:
                case Key.Pause:
                case Key.CapsLock:
                case Key.HangulMode:
                case Key.JunjaMode:
                case Key.FinalMode:
                case Key.KanjiMode:
                case Key.Escape:
                case Key.ImeConvert:
                case Key.ImeNonConvert:
                case Key.ImeAccept:
                case Key.ImeModeChange:
                case Key.Space:
                case Key.PageUp:
                case Key.PageDown:
                case Key.End:
                case Key.Home:
                case Key.Left:
                case Key.Up:
                case Key.Right:
                case Key.Down:
                case Key.Select:
                case Key.Print:
                case Key.Execute:
                case Key.PrintScreen:
                case Key.Insert:
                case Key.Delete:
                case Key.Help:
                case Key.D0:
                case Key.D1:
                case Key.D2:
                case Key.D3:
                case Key.D4:
                case Key.D5:
                case Key.D6:
                case Key.D7:
                case Key.D8:
                case Key.D9:
                case Key.A:
                case Key.B:
                case Key.C:
                case Key.D:
                case Key.E:
                case Key.F:
                case Key.G:
                case Key.H:
                case Key.I:
                case Key.J:
                case Key.K:
                case Key.L:
                case Key.M:
                case Key.N:
                case Key.O:
                case Key.P:
                case Key.Q:
                case Key.R:
                case Key.S:
                case Key.T:
                case Key.U:
                case Key.V:
                case Key.W:
                case Key.X:
                case Key.Y:
                case Key.Z:
                case Key.LWin:
                case Key.RWin:
                case Key.Apps:
                case Key.Sleep:
                case Key.NumPad0:
                case Key.NumPad1:
                case Key.NumPad2:
                case Key.NumPad3:
                case Key.NumPad4:
                case Key.NumPad5:
                case Key.NumPad6:
                case Key.NumPad7:
                case Key.NumPad8:
                case Key.NumPad9:
                case Key.Multiply:
                case Key.Add:
                case Key.Separator:
                case Key.Subtract:
                case Key.Decimal:
                case Key.Divide:
                case Key.F1:
                case Key.F2:
                case Key.F3:
                case Key.F4:
                case Key.F5:
                case Key.F6:
                case Key.F7:
                case Key.F8:
                case Key.F9:
                case Key.F10:
                case Key.F11:
                case Key.F12:
                case Key.F13:
                case Key.F14:
                case Key.F15:
                case Key.F16:
                case Key.F17:
                case Key.F18:
                case Key.F19:
                case Key.F20:
                case Key.F21:
                case Key.F22:
                case Key.F23:
                case Key.F24:
                case Key.NumLock:
                case Key.Scroll:
                case Key.BrowserBack:
                case Key.BrowserForward:
                case Key.BrowserRefresh:
                case Key.BrowserStop:
                case Key.BrowserSearch:
                case Key.BrowserFavorites:
                case Key.BrowserHome:
                case Key.VolumeMute:
                case Key.VolumeDown:
                case Key.VolumeUp:
                case Key.MediaNextTrack:
                case Key.MediaPreviousTrack:
                case Key.MediaStop:
                case Key.MediaPlayPause:
                case Key.LaunchMail:
                case Key.SelectMedia:
                case Key.LaunchApplication1:
                case Key.LaunchApplication2:
                case Key.OemSemicolon:
                case Key.OemPlus:
                case Key.OemComma:
                case Key.OemMinus:
                case Key.OemPeriod:
                case Key.OemQuestion:
                case Key.OemTilde:
                case Key.AbntC1:
                case Key.AbntC2:
                case Key.OemOpenBrackets:
                case Key.OemPipe:
                case Key.OemCloseBrackets:
                case Key.OemQuotes:
                case Key.Oem8:
                case Key.OemBackslash:
                case Key.ImeProcessed:
                case Key.System:
                case Key.OemAttn:
                case Key.OemFinish:
                case Key.OemCopy:
                case Key.OemAuto:
                case Key.OemEnlw:
                case Key.OemBackTab:
                case Key.Attn:
                case Key.CrSel:
                case Key.ExSel:
                case Key.EraseEof:
                case Key.Play:
                case Key.Zoom:
                case Key.NoName:
                case Key.Pa1:
                case Key.OemClear:
                case Key.DeadCharProcessed:
                default:
                    if (isKeyDown || isSystemKeyDown)
                    {
                        string keyString = key.ToString();

                        if (!string.IsNullOrEmpty(keyString))
                        {
                            keys.Clear();

                            if (ctrlPressed)
                            {
                                keys.Add("CTRL");
                            }

                            if (altPressed)
                            {
                                keys.Add("ALT");
                            }

                            if (shiftPressed)
                            {
                                keys.Add("SHIFT");
                            }

                            keys.Add(keyString);

                            action?.Invoke(string.Join('+', keys).ToUpperInvariant());
                        }
                    }
                    break;
            }
        }

        return CallNextHookEx(hookID, nCode, wParam, lParam);
    }

    public static void UnhookWindowsHookEx()
    {
        _ = UnhookWindowsHookEx(hookID);
    }
}

