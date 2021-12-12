using System.Runtime.InteropServices;

namespace blogdeployments.agent.Native;

internal static class NativeMethods
{
    public const int RB_POWER_OFF = 0x4321fedc;

    public const int LINUX_REBOOT_MAGIC1 = unchecked((int) 0xfee1dead);
    public const int LINUX_REBOOT_MAGIC2 = 672274793;
    public const int LINUX_REBOOT_MAGIC2A = 85072278;
    public const int LINUX_REBOOT_MAGIC2B = 369367448;
    public const int LINUX_REBOOT_MAGIC2C = 537993216;


    public const int LINUX_REBOOT_CMD_RESTART = 0x01234567;
    public const int LINUX_REBOOT_CMD_HALT = unchecked((int) 0xCDEF0123);
    public const int LINUX_REBOOT_CMD_CAD_ON = unchecked((int) 0x89ABCDEF);
    public const int LINUX_REBOOT_CMD_CAD_OFF = 0x00000000;
    public const int LINUX_REBOOT_CMD_POWER_OFF = 0x4321FEDC;
    public const int LINUX_REBOOT_CMD_RESTART2 = unchecked((int) 0xA1B2C3D4);
    public const int LINUX_REBOOT_CMD_SW_SUSPEND = unchecked((int) 0xD000FCE2);
    public const int LINUX_REBOOT_CMD_KEXEC = 0x45584543;

    public const int EPERM = 1;
    public const int EFAULT = 14;
    public const int EINVAL = 22;

    [DllImport("libc.so.6",
        SetLastError = true)] // You may need to change this to "libc.so.6" or "libc.so.7" depending on your platform)
    public static extern int reboot(int magic);

    [DllImport("libc.so.6",
        SetLastError = true)] // You may need to change this to "libc.so.6" or "libc.so.7" depending on your platform)
    public static extern int sync();

    [DllImport("libc.so.6",
        SetLastError = true)] // You may need to change this to "libc.so.6" or "libc.so.7" depending on your platform)
    public static extern int system([MarshalAs(UnmanagedType.LPStr)] string cmd);
}