namespace blogdeployments.agent.Native;

using System.Runtime.InteropServices;

internal static class NativeMethods
{
    [DllImport( "libc.so.6", SetLastError = true)] // You may need to change this to "libc.so.6" or "libc.so.7" depending on your platform)
    public static extern Int32 reboot(Int32 magic);

    [DllImport( "libc.so.6", SetLastError = true)] // You may need to change this to "libc.so.6" or "libc.so.7" depending on your platform)
    public static extern Int32 sync();

    [DllImport( "libc.so.6", SetLastError = true)] // You may need to change this to "libc.so.6" or "libc.so.7" depending on your platform)
    public static extern Int32 system([MarshalAs(UnmanagedType.LPStr)] string cmd);

    public const Int32 RB_POWER_OFF = unchecked((int)0x4321fedc);

    public const Int32 LINUX_REBOOT_MAGIC1 = unchecked((int)0xfee1dead);
    public const Int32 LINUX_REBOOT_MAGIC2 = 672274793;
    public const Int32 LINUX_REBOOT_MAGIC2A = 85072278;
    public const Int32 LINUX_REBOOT_MAGIC2B = 369367448;
    public const Int32 LINUX_REBOOT_MAGIC2C = 537993216;


    public const Int32 LINUX_REBOOT_CMD_RESTART = 0x01234567;
    public const Int32 LINUX_REBOOT_CMD_HALT = unchecked((int)0xCDEF0123);
    public const Int32 LINUX_REBOOT_CMD_CAD_ON = unchecked((int)0x89ABCDEF);
    public const Int32 LINUX_REBOOT_CMD_CAD_OFF = 0x00000000;
    public const Int32 LINUX_REBOOT_CMD_POWER_OFF = 0x4321FEDC;
    public const Int32 LINUX_REBOOT_CMD_RESTART2 = unchecked((int)0xA1B2C3D4);
    public const Int32 LINUX_REBOOT_CMD_SW_SUSPEND = unchecked((int)0xD000FCE2);
    public const Int32 LINUX_REBOOT_CMD_KEXEC = 0x45584543;

    public const Int32 EPERM  =  1;
    public const Int32 EFAULT = 14;
    public const Int32 EINVAL = 22;
}