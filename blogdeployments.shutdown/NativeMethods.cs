using System.Runtime.InteropServices;

internal static class NativeMethods
{
    [DllImport( "libc.so.6", SetLastError = true)] // You may need to change this to "libc.so.6" or "libc.so.7" depending on your platform)
    public static extern Int32 reboot(Int32 magic);

    [DllImport( "libc.so.6", SetLastError = true)] // You may need to change this to "libc.so.6" or "libc.so.7" depending on your platform)
    public static extern Int32 sync();

    public const Int32 RB_POWER_OFF = unchecked((int)0x4321fedc);

    public const Int32 EPERM  =  1;
    public const Int32 EFAULT = 14;
    public const Int32 EINVAL = 22;
}