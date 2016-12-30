package com.example.anita.walkietalkie;

public enum ServerResponseType {
    SUCCESS(0),
    USER_NOT_EXIST(1),
    WRONG_DETAILS(2),
    ALREADY_CONNECTED(3),
    OTHER(7)//TODO
    ;

    private int value;

    private ServerResponseType(int value) {this.value = value;}

    public int getValue() {
        return value;
    }

}
