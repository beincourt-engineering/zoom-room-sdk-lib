namespace PepperDash.ZoomRoom.Sdk;

/// <summary>
/// The paired Zoom Room as it describes itself (ZRC SDK ZoomRoomInfo): its name, display name and IP address. The
/// SDK carries no model, serial number, MAC or Zoom Rooms version for the room.
/// </summary>
public sealed class PairedRoomInfo
{
    /// <summary>The room's name in Zoom, or null.</summary>
    public string? RoomName { get; init; }

    /// <summary>The name the room shows, or null.</summary>
    public string? DisplayName { get; init; }

    /// <summary>The room's IP address, or null.</summary>
    public string? RoomAddress { get; init; }

    /// <summary>True when the room sent at least one of the three.</summary>
    public bool Any => RoomName != null || DisplayName != null || RoomAddress != null;

    /// <summary>The three as the wrapper copies them out: blank is none, and each is trimmed.</summary>
    public static PairedRoomInfo From(string? roomName, string? displayName, string? roomAddress) => new()
    {
        RoomName = Clean(roomName),
        DisplayName = Clean(displayName),
        RoomAddress = Clean(roomAddress),
    };

    private static string? Clean(string? value) => string.IsNullOrWhiteSpace(value) ? null : value.Trim();
}
