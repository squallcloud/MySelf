Add-Type @"
using System;
using System.Runtime.InteropServices;

public class WinAPI {
    public delegate bool EnumWindowsProc(IntPtr hWnd, IntPtr lParam);

    [DllImport("user32.dll")]
    public static extern bool EnumWindows(EnumWindowsProc lpEnumFunc, IntPtr lParam);
    
    [DllImport("user32.dll")]
    public static extern IntPtr FindWindow(string lpClassName, string lpWindowName);
    
    [DllImport("user32.dll")]
    public static extern bool SetWindowPos(IntPtr hWnd, IntPtr hWndInsertAfter, int X, int Y, int cx, int cy, uint uFlags);

    [DllImport("user32.dll")]
    public static extern IntPtr GetForegroundWindow();

    [DllImport("user32.dll")]
    public static extern IntPtr GetWindowThreadProcessId(IntPtr hWnd, out uint lpdwProcessId);

    [DllImport("user32.dll")]
    public static extern bool GetWindowRect(IntPtr hWnd, out RECT lpRect);

    [StructLayout(LayoutKind.Sequential)]
    public struct RECT {
        public int Left;
        public int Top;
        public int Right;
        public int Bottom;
    }

    public static IntPtr GetWindowHandleFromProcessId(uint processId) {
        IntPtr targetHWnd = IntPtr.Zero;
        
        EnumWindows((hWnd, lParam) => {
            uint windowProcessId;
            GetWindowThreadProcessId(hWnd, out windowProcessId);
            if (windowProcessId == processId) {
                targetHWnd = hWnd;
                return false; // 見つけたら列挙を停止
            }
            return true;
        }, IntPtr.Zero);

        return targetHWnd;
    }

    public static readonly IntPtr HWND_TOP = IntPtr.Zero;
    public static readonly uint SWP_NOSIZE = 0x0001;
    public static readonly uint SWP_NOZORDER = 0x0004;
}

"@

# 操作するプロセス名（例：notepad.exe）
$processName = "notepad"

# プロセスを取得
$process = Get-Process | Where-Object { $_.ProcessName -eq $processName }

if ($process) {
    # ウィンドウハンドルを取得
    $hWnd = [WinAPI]::GetWindowHandleFromProcessId($process.Id)

    if ($hWnd -ne [IntPtr]::Zero) {
        Write-Output "取得したウィンドウハンドル: $hWnd"

        $rect = New-Object WinAPI+RECT
        if ([WinAPI]::GetWindowRect($hWnd, [ref]$rect)) {
            $cur_wid = $rect.Right - $rect.Left
            $cur_hig = $rect.Bottom - $rect.Top

            $cur_x = $rect.Left
            $cur_y = $rect.Top

            Write-Output "ウィンドウ位置: X=$($rect.Left), Y=$($rect.Top)"
            Write-Output "ウィンドウサイズ: 幅=$cur_wid, 高さ=$cur_hig"

            $is_transform = $false
            $new_wid = $cur_wid
            $new_hig = $cur_hig
            if ($cur_wid -gt 1000) {
                $new_wid = 1000
                $is_transform = $true
            }
            if ($cur_hig -gt 600) {
                $new_hig = 600
                $is_transform = $true
            }

            $new_x = $cur_x
            $new_y = $cur_y
            if ($cur_x -gt 0) {
                $new_x = 0
                $is_transform = $true
            }
            if ($cur_y -gt 0) {
                $new_y = 0
                $is_transform = $true
            }

            if ($is_transform) {
                [WinAPI]::SetWindowPos($hWnd, [WinAPI]::HWND_TOP, $new_x, $new_y, $new_wid, $new_hig, [WinAPI]::SWP_NOZORDER)
                Write-Output "ウィンドウのサイズを変更しました。"
            }
        } else {
            Write-Output "ウィンドウの位置を取得できませんでした。"
        }

    } else {
        Write-Output "指定したプロセスに紐づくウィンドウが見つかりません。"
    }

} else {
    Write-Output "指定したプロセスが見つかりません。"
}
