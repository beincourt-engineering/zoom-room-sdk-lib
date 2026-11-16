using System.Runtime.InteropServices;

namespace PepperDash.ZoomRoom.Sdk;

// Settings service: the Room PC's statistics (CPU and memory use). Volume and camera selection are in
// ZrcSdk.Audio.cs and ZrcSdk.Camera.cs.
public partial class ZrcSdk
{
    [DllImport(DllName, CallingConvention = CallingConvention.Cdecl)]
    private static extern int ZrcSdk_EnableStatisticalInfo(IntPtr handle, int enable);
    [DllImport(DllName, CallingConvention = CallingConvention.Cdecl)]
    private static extern void ZrcSdk_SetStatisticalInfoCallback(IntPtr handle, ZrcStatisticalInfoCallbackDelegate? cb, IntPtr userData);

    // Matches ZrcStatisticalInfo in ZrcSdkWrapper_C.h: six int32 and one float, 28 bytes.
    [StructLayout(LayoutKind.Sequential)]
    private struct ZrcStatisticalInfoNative
    {
        public int cpuCoreNumber;
        public float cpuFrequency;
        public int cpuZRUsage;
        public int cpuOverallUsage;
        public int memorySize;
        public int memoryZRUsage;
        public int memoryOverallUsage;
    }

    [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
    private delegate void ZrcStatisticalInfoCallbackDelegate(IntPtr infoPtr, IntPtr userData);

    private ZrcStatisticalInfoCallbackDelegate? _statisticalInfoCallbackDelegate;

    /// <summary>
    /// The Room PC's statistics, sent while <see cref="EnableStatisticalInfo"/> is on. See
    /// <see cref="StatisticalInfoEventArgs"/>. Raised on the SDK's thread.
    /// </summary>
    public event EventHandler<StatisticalInfoEventArgs>? StatisticalInfoChanged;

    partial void InitializeSettingsCallbacks()
    {
        _statisticalInfoCallbackDelegate = OnStatisticalInfoCallback;
        ZrcSdk_SetStatisticalInfoCallback(_handle, _statisticalInfoCallbackDelegate, IntPtr.Zero);
    }

    /// <summary>
    /// Turns the Zoom Room's statistics notification on or off (ZRC SDK 6.0.0+). While on, the room sends
    /// <see cref="StatisticalInfoChanged"/>.
    /// </summary>
    /// <returns><see langword="true"/> on success.</returns>
    public bool EnableStatisticalInfo(bool enable)
    {
        ThrowIfDisposed();
        return ZrcSdk_EnableStatisticalInfo(_handle, enable ? 1 : 0) == 0;
    }

    private void OnStatisticalInfoCallback(IntPtr infoPtr, IntPtr userData)
    {
        if (infoPtr == IntPtr.Zero) return;
        var n = Marshal.PtrToStructure<ZrcStatisticalInfoNative>(infoPtr);
        StatisticalInfoChanged?.Invoke(this, new StatisticalInfoEventArgs
        {
            CpuCoreCount        = n.cpuCoreNumber,
            CpuFrequency        = n.cpuFrequency,
            CpuZoomRoomsUsage   = n.cpuZRUsage,
            CpuOverallUsage     = n.cpuOverallUsage,
            MemorySize          = n.memorySize,
            MemoryZoomRoomsUsage = n.memoryZRUsage,
            MemoryOverallUsage  = n.memoryOverallUsage,
        });
    }
}

/// <summary>
/// Event args for <see cref="ZrcSdk.StatisticalInfoChanged"/>: the SDK's StatisticalOverallInfo, as it
/// reports it. The SDK documents no units; usage figures are expected to be percentages.
/// </summary>
public sealed class StatisticalInfoEventArgs : System.EventArgs
{
    /// <summary>CPU cores in the Room PC.</summary>
    public int CpuCoreCount { get; init; }
    /// <summary>CPU frequency.</summary>
    public float CpuFrequency { get; init; }
    /// <summary>CPU used by the Zoom Rooms app.</summary>
    public int CpuZoomRoomsUsage { get; init; }
    /// <summary>CPU used by the whole Room PC.</summary>
    public int CpuOverallUsage { get; init; }
    /// <summary>Memory size.</summary>
    public int MemorySize { get; init; }
    /// <summary>Memory used by the Zoom Rooms app.</summary>
    public int MemoryZoomRoomsUsage { get; init; }
    /// <summary>Memory used by the whole Room PC.</summary>
    public int MemoryOverallUsage { get; init; }
}
