namespace facepunchsteamworkstest.core.domain.steam_multiplayer;

public enum MessageType: byte {
    NetIdHandshake = 0,
    GameMessage = 1 << 0
}