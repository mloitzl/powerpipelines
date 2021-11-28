using System.Runtime.InteropServices;
using static NativeMethods;

static void Shutdown()
{
    sync();
    // Int32 ret = reboot(RB_POWER_OFF); // too hard, doesn't run systemd scripts
    Int32 ret = system("shutdown -h 0");


    // todo: check if these are correct for 'system(...)' syscall
    if (ret == 0) throw new InvalidOperationException("reboot(LINUX_REBOOT_CMD_POWER_OFF) returned 0.");

    if (ret != -1) throw new InvalidOperationException("Unexpected reboot() return value: " + ret);

    Int32 errno = Marshal.GetLastWin32Error();
    switch (errno)
    {
        case EPERM:
            throw new UnauthorizedAccessException("You do not have permission to call reboot()");

        case EINVAL:
            throw new ArgumentException("Bad magic numbers (stray cosmic-ray?)");

        case EFAULT:
        default:
            throw new InvalidOperationException("Could not call reboot():" + errno.ToString());
    }
}

Console.WriteLine("Shutting down");
Shutdown();
