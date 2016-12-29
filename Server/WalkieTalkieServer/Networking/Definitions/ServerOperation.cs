namespace Common.Networking.Definitions
{
    public enum ServerOperation : byte
    {
        SIGN_IN,
        SIGN_UP
    }

    public enum ResponseType : byte
    {
        SUCCESS,
        USER_DOESNT_EXIST,
        WRONG_DETAILS,
        USER_ALREADY_CONNETED,
        INVALID_USERNAME,
        USERNAME_ALREADY_EXISTS,
        INVALID_PASSWORD,
        OTHER
    }
}
