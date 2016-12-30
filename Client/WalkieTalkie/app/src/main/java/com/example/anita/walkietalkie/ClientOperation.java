package com.example.anita.walkietalkie;

public enum ClientOperation {
    SIGNIN((byte)0),
    SIGNUP((byte)1);

    private byte value;

    private ClientOperation(byte value) {
        this.value = value;
    }

    public byte getValue() {
        return value;
    }
}
