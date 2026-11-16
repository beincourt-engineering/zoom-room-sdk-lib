using System.Reflection;
using System.Runtime.InteropServices;
using PepperDash.ZoomRoom.Sdk;
using Xunit;

namespace ZoomRoomSdk.Tests;

public class StatisticalInfoTests
{
    [Fact]
    public void Native_Struct_Matches_The_C_Layout()
    {
        // ZrcStatisticalInfo in ZrcSdkWrapper_C.h: int32 cpuCoreNumber, float cpuFrequency, then five int32.
        var native = typeof(ZrcSdk).GetNestedType("ZrcStatisticalInfoNative", BindingFlags.NonPublic);
        Assert.NotNull(native);
        Assert.Equal(28, Marshal.SizeOf(native!));

        var offsets = new[]
        {
            ("cpuCoreNumber", 0), ("cpuFrequency", 4), ("cpuZRUsage", 8), ("cpuOverallUsage", 12),
            ("memorySize", 16), ("memoryZRUsage", 20), ("memoryOverallUsage", 24),
        };
        foreach (var (field, offset) in offsets)
            Assert.Equal(offset, (int)Marshal.OffsetOf(native!, field));
    }

    [Fact]
    public void Event_Args_Carry_Every_Figure()
    {
        var args = new StatisticalInfoEventArgs
        {
            CpuCoreCount = 8,
            CpuFrequency = 2.4f,
            CpuZoomRoomsUsage = 12,
            CpuOverallUsage = 31,
            MemorySize = 16384,
            MemoryZoomRoomsUsage = 9,
            MemoryOverallUsage = 47,
        };

        Assert.Equal(8, args.CpuCoreCount);
        Assert.Equal(2.4f, args.CpuFrequency);
        Assert.Equal(12, args.CpuZoomRoomsUsage);
        Assert.Equal(31, args.CpuOverallUsage);
        Assert.Equal(16384, args.MemorySize);
        Assert.Equal(9, args.MemoryZoomRoomsUsage);
        Assert.Equal(47, args.MemoryOverallUsage);
    }
}
