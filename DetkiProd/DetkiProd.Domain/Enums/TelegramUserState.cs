namespace DetkiProd.Domain.Enums;

public enum TelegramUserState
{
    None,
    AwaitingFileUpload,
    AwaitingFileForDownload,
    AwaitingFileForDelete,
    AwaitingFileForGetUrl,
    AwaitingProjectInfoForAdd,
    AwaitingProjectForDelete
}
