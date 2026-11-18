using System.Reflection;
using System.Runtime.InteropServices;
using PepperDash.ZoomRoom.Sdk;
using Xunit;

namespace ZoomRoomSdk.Tests;

public class PairedRoomInfoTests
{
    [Fact]
    public void Blank_Fields_Are_None_And_The_Rest_Are_Trimmed()
    {
        var info = PairedRoomInfo.From(" Courtroom 1 ", "", "192.0.2.40");

        Assert.Equal("Courtroom 1", info.RoomName);
        Assert.Null(info.DisplayName);
        Assert.Equal("192.0.2.40", info.RoomAddress);
        Assert.True(info.Any);
        Assert.False(PairedRoomInfo.From(null, " ", "").Any);
    }

    [Fact]
    public void The_Native_Export_Takes_Three_Buffers_With_Their_Sizes()
    {
        // ZrcSdk_GetLastZoomRoomInfo in ZrcSdkWrapper_C.h: handle, then name, display name and address, each a
        // buffer followed by its size.
        var import = typeof(ZrcSdk).GetMethod("ZrcSdk_GetLastZoomRoomInfo", BindingFlags.NonPublic | BindingFlags.Static);
        Assert.NotNull(import);
        Assert.NotNull(import!.GetCustomAttribute<DllImportAttribute>());
        var types = import.GetParameters().Select(p => p.ParameterType.Name).ToArray();
        Assert.Equal(new[] { "IntPtr", "StringBuilder", "Int32", "StringBuilder", "Int32", "StringBuilder", "Int32" }, types);

        var header = File.ReadAllText(Path.Combine(SourceRoot(), "ZrcSdkWrapper", "ZrcSdkWrapper_C.h"));
        Assert.Contains("ZrcSdk_GetLastZoomRoomInfo(ZrcSdkHandle handle,", header);
        Assert.Contains("char* roomName, int roomNameSize, char* displayName, int displayNameSize, char* roomAddress, int roomAddressSize", header);
    }

    private static string SourceRoot()
    {
        var dir = new DirectoryInfo(AppContext.BaseDirectory);
        while (dir != null && !Directory.Exists(Path.Combine(dir.FullName, "ZrcSdkWrapper")))
            dir = dir.Parent;
        Assert.NotNull(dir);
        return dir!.FullName;
    }
}
