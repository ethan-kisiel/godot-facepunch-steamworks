namespace facepunchsteamworkstest.core.domain.steam_multiplayer;

public enum MessageTypeEnum: byte {
    NetIdHandshake = 0,
    GameMessage = 1 << 0
}