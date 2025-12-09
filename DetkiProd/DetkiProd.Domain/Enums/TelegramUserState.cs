namespace DetkiProd.Domain.Enums;

public enum TelegramUserState
{
    None,
    AwaitingFileUpload,
    AwaitingFileForDownload,
    AwaitingMainFileUpload,
    AwaitingFileForDelete,
    AwaitingFileForGetUrl,
    AwaitingProjectInfoForAdd,
    AwaitingProjectForDelete
}
