namespace EQueueLib
{
    public enum Command
    {
        ServerState,
        NewClient,
        NextClient,
        FreeWindow,
        TakeClient,
        ChangeService,
        CompleteClientService,
        Disconnect
    }
}
