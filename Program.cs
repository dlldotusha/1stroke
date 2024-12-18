using System;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;

class Program
{
    [DllImport("user32.dll", CharSet = CharSet.Auto)]
    public static extern IntPtr GetClipboardData(uint uFormat);

    [DllImport("user32.dll")]
    public static extern bool OpenClipboard(IntPtr hWndNewOwner);

    [DllImport("user32.dll")]
    public static extern bool CloseClipboard();

    [DllImport("kernel32.dll", CharSet = CharSet.Auto)]
    public static extern IntPtr GlobalLock(IntPtr handle);

    [DllImport("kernel32.dll")]
    public static extern bool GlobalUnlock(IntPtr handle);

    [DllImport("kernel32.dll")]
    public static extern IntPtr GlobalAlloc(uint uFlags, uint dwBytes);

    [DllImport("user32.dll")]
    public static extern bool SetClipboardData(uint uFormat, IntPtr data);

    [DllImport("user32.dll", CharSet = CharSet.Auto)]
    public static extern uint RegisterClipboardFormat(string lpszFormat);

    const uint CF_UNICODETEXT = 13;

    static void Main()
    {
        string lastClipboardText = string.Empty;

        Console.WriteLine("Программа запущена. Она будет проверять буфер обмена каждую секунду.");

        while (true)
        {
            try
            {
                string clipboardText = GetClipboardText();

                if (clipboardText != lastClipboardText && clipboardText.Contains(";"))
                {
                    string formattedCode = FormatPythonCode(clipboardText);

                    if (!string.IsNullOrWhiteSpace(formattedCode))
                    {
                        SetClipboardText(formattedCode);
                        Console.WriteLine($"Исправленный код:\n{formattedCode}");
                    }

                    lastClipboardText = formattedCode;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Ошибка: {ex.Message}");
            }

            Thread.Sleep(1000); // Пауза в 1 секунду перед следующей проверкой
        }
    }

    static string GetClipboardText()
    {
        OpenClipboard(IntPtr.Zero);
        IntPtr handle = GetClipboardData(CF_UNICODETEXT);
        IntPtr pointer = GlobalLock(handle);
        string text = Marshal.PtrToStringUni(pointer);
        GlobalUnlock(handle);
        CloseClipboard();
        return text;
    }

    static void SetClipboardText(string text)
    {
        IntPtr handle = Marshal.StringToHGlobalUni(text);
        OpenClipboard(IntPtr.Zero);
        SetClipboardData(CF_UNICODETEXT, handle);
        CloseClipboard();
    }

    static string FormatPythonCode(string code)
    {
        string[] commands = code.Split(';');
        return string.Join(Environment.NewLine, commands).Trim();
    }
}
