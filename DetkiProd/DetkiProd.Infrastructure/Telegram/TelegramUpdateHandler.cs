using DetkiProd.Application.Interfaces;
using DetkiProd.Domain.Enums;
using DetkiProd.Domain.Exceptions;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System.Text;
using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;
using Telegram.Bot.Types.ReplyMarkups;

namespace DetkiProd.Infrastructure.Telegram;

public interface ITelegramUpdateHandler
{
    Task HandleAsync(Update update, CancellationToken cancellationToken);
}

public class TelegramUpdateHandler : ITelegramUpdateHandler
{
    private const string AccessDenied = "Доступ запрещен.";
    private const string ChooseAction = "✨ Выбери, что будем делать дальше.";
    private const string SendFile = "⬆️ Отправьте файл.";
    private const string FileUploaded = "🎉 Файл успешно загружен!";
    private const string FileDeleted = "🎉 Файл успешно удален!";
    private const string ChooseFileNameForDownload = "Выбери файл, который нужно скачать 🔹";
    private const string ChooseFileNameForDelete = "Выбери файл, который нужно удалить 🔹";
    private const string WriteFileNameForGetUrl = "Выбери файл, ссылку на который нужно сгенерировать 🔹";
    private const string FileNotFound = "Файл с таким названием не найден ❗🔍";
    private const string ProjectNotFound = "Проект с таким 🆔 не найден ❗🔍";
    private const string ProjectDeleted = "🎉 Проект успешно удален!";
    private const string FileEmpty = "Файл не должен быть пустым 😕 Удали его!";
    private const string NotFound = "🕳️ Здесь пока пусто…";
    private const string NotFoundFilesForDownload = "🕳️ Список файлов пуст, поэтому нечего скачать.";
    private const string NotFoundFilesForDelete = "🕳️ Список файлов пуст, поэтому нечего удалять.";
    private const string NotFoundFilesForGetUrl = "🕳️ Список файлов пуст, поэтому ссылку не сформировать.";
    private const string WriteProject = "✨ Напиши проект в формате:\n📘 Название\n📆 Год\n⚙️ Инструменты\n🔗 Ссылка на видео";
    private const string WriteProjectIdForDelete = "Напишите 🆔 проекта, который нужно удалить ✍️📄";
    private const string WrongFormat = $"❌ Ты не следовал формату, попробуй ещё раз:\n{WriteProject}";
    private const string WrongId = "❌ Некорректный 🆔 проекта, проект не найден.";
    private const string ProjectAdded = "🎉 Проект успешно добавлен!";
    private const string ApiFilesDownloadUrlEmpty = $"🔴 Получение ссылки невозможно, в конфигурации не указан ApiFilesDownloadUrl.";
    private const string UnexpectedError = $"🔴 Непредвиденная ошибка, используй {StartCommand}.";

    private const string StartCommand = "/start";
    private const string GetFilesCommand = "/getfiles";
    private const string UploadFileCommand = "/uploadfile";
    private const string DownloadFileCommand = "/downloadfile";
    private const string DeleteFileCommand = "/deletefile";
    private const string GetFileUrlCommand = "/getfileurl";
    private const string UploadMainFileCommand = "/uploadmainfile";
    private const string DownloadMainFileCommand = "/downloadmainfile";
    private const string GetProjectsCommand = "/getprojects";
    private const string AddProjectCommand = "/addproject";
    private const string DeleteProjectCommand = "/deleteproject";

    private const string GetFilesFriendlyCommand = "📂 Файлы";
    private const string UploadFileFriendlyCommand = "⬆️ Отправить файл";
    private const string DownloadFileFriendlyCommand = "⬇️ Получить файл";
    private const string DeleteFileFriendlyCommand = "🗑️ Удалить файл";
    private const string UploadMainFileFriendlyCommand = "⬆️ Отправить главный файл";
    private const string DownloadMainFileFriendlyCommand = "⬆️ Загрузить главный файл";
    private const string GetFileUrlFriendlyCommand = "🔗 Получить ссылку";
    private const string GetProjectsFriendlyCommand = "📁 Проекты";
    private const string AddProjectFriendlyCommand = "➕ Добавить проект";
    private const string DeleteProjectFriendlyCommand = "❌ Удалить проект";

    private readonly IUserService _userService;
    private readonly IProjectService _projectService;
    private readonly IFileService _fileService;
    private readonly ICacheService _cacheService;
    private readonly ITelegramBotClient _botClient;
    private readonly IConfiguration _configuration;
    private readonly ILogger<TelegramUpdateHandler> _logger;

    public TelegramUpdateHandler(
        IUserService userService, 
        IProjectService projectService,
        IFileService fileService,
        ICacheService cacheService,
        ITelegramBotClient botClient,
        IConfiguration configuration,
        ILogger<TelegramUpdateHandler> logger)
    {
        _userService = userService ?? throw new ArgumentNullException(nameof(userService));
        _projectService = projectService ?? throw new ArgumentNullException(nameof(projectService));
        _fileService = fileService ?? throw new ArgumentNullException(nameof(fileService));
        _cacheService = cacheService ?? throw new ArgumentNullException(nameof(cacheService));
        _botClient = botClient ?? throw new ArgumentNullException(nameof(botClient));
        _configuration = configuration ?? throw new ArgumentNullException(nameof(configuration));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    public async Task HandleAsync(Update update, CancellationToken cancellationToken)
    {
        var message = update.Message;
        if (message == null)
            return;

        var messageText = message.Text;
        var messageFileId = message.Document?.FileId ?? message.Video?.FileId;
        var messageFileName = message.Document?.FileName ?? message.Video?.FileName;

        if (messageText == null && messageFileId == null)
            return;

        var chatId = message.Chat.Id;
        var telegramUserId = message.From?.Id;

        if (telegramUserId == null)
        {
            await SendTextMessage(chatId, AccessDenied, cancellationToken);
            return;
        }

        var user = await _userService.GetUserByTelegramIdAsync(telegramUserId.Value);
        if (user == null)
        {
            await SendTextMessage(chatId, AccessDenied, cancellationToken);
            return;
        }

        if (messageText == StartCommand)
        {
            await HandleStartCommand(chatId, cancellationToken);
            return;
        }

        var userState = await _cacheService.GetTelegramUserStateAsync(chatId);

        if ((messageText == GetFilesCommand ||
             messageText == GetFilesFriendlyCommand ||
             messageText == UploadFileCommand ||
             messageText == UploadFileFriendlyCommand ||
             messageText == DownloadFileCommand ||
             messageText == DownloadFileFriendlyCommand ||
             messageText == DeleteFileCommand ||
             messageText == DeleteFileFriendlyCommand ||
             messageText == UploadMainFileCommand ||
             messageText == UploadMainFileFriendlyCommand ||
             messageText == DownloadMainFileCommand ||
             messageText == DownloadMainFileFriendlyCommand ||
             messageText == GetFileUrlCommand ||
             messageText == GetFileUrlFriendlyCommand ||
             messageText == GetProjectsCommand ||
             messageText == GetProjectsFriendlyCommand ||
             messageText == AddProjectCommand ||
             messageText == AddProjectFriendlyCommand ||
             messageText == DeleteProjectCommand ||
             messageText == DeleteProjectFriendlyCommand) &&
             userState != TelegramUserState.None)
        {
            userState = TelegramUserState.None;
            await _cacheService.SetTelegramUserStateAsync(chatId, userState);
        }

        if (userState == TelegramUserState.AwaitingFileUpload)
        {
            if (messageFileId == null || messageFileName == null)
            {
                await _cacheService.SetTelegramUserStateAsync(chatId, TelegramUserState.None);
                await SendMainMenuMessage(chatId, cancellationToken, UnexpectedError);
                return;
            }

            var file = await _botClient.GetFile(messageFileId);
            if (file == null || string.IsNullOrWhiteSpace(file.FilePath))
            {
                await _cacheService.SetTelegramUserStateAsync(chatId, TelegramUserState.None);
                await SendMainMenuMessage(chatId, cancellationToken, UnexpectedError);
                return;
            }

            var extendedTelegramBotClient = _botClient as TelegramBotClientExtended;
            if (extendedTelegramBotClient is null)
            {
                await _cacheService.SetTelegramUserStateAsync(chatId, TelegramUserState.None);
                await SendMainMenuMessage(chatId, cancellationToken, UnexpectedError);
                return;
            }
            await using var memoryStream = new MemoryStream();
            await extendedTelegramBotClient.GetFileFromLocalServer(file.FilePath, memoryStream);
            memoryStream.Position = 0;

            await _fileService.UploadAsync(memoryStream, messageFileName);

            await _cacheService.SetTelegramUserStateAsync(chatId, TelegramUserState.None);
            await SendMainMenuMessage(chatId, cancellationToken, FileUploaded);
            return;
        }
        else if (userState == TelegramUserState.AwaitingMainFileUpload)
        {
            if (messageFileId == null || messageFileName == null)
            {
                await _cacheService.SetTelegramUserStateAsync(chatId, TelegramUserState.None);
                await SendMainMenuMessage(chatId, cancellationToken, UnexpectedError);
                return;
            }

            var file = await _botClient.GetFile(messageFileId);
            if (file == null || string.IsNullOrWhiteSpace(file.FilePath))
            {
                await _cacheService.SetTelegramUserStateAsync(chatId, TelegramUserState.None);
                await SendMainMenuMessage(chatId, cancellationToken, UnexpectedError);
                return;
            }

            var extendedTelegramBotClient = _botClient as TelegramBotClientExtended;
            if (extendedTelegramBotClient is null)
            {
                await _cacheService.SetTelegramUserStateAsync(chatId, TelegramUserState.None);
                await SendMainMenuMessage(chatId, cancellationToken, UnexpectedError);
                return;
            }

            await using var memoryStream = new MemoryStream();
            await extendedTelegramBotClient.GetFileFromLocalServer(file.FilePath, memoryStream);
            memoryStream.Position = 0;
            await _fileService.UploadMainAsync(memoryStream, messageFileName);

            await _cacheService.SetTelegramUserStateAsync(chatId, TelegramUserState.None);
            await SendMainMenuMessage(chatId, cancellationToken, FileUploaded);
            return;
        }
        else if (userState == TelegramUserState.AwaitingFileForDownload)
        {
            if (string.IsNullOrWhiteSpace(messageText))
            {
                await _cacheService.SetTelegramUserStateAsync(chatId, TelegramUserState.None);
                await SendMainMenuMessage(chatId, cancellationToken, UnexpectedError);
                return;
            }

            try
            {
                var stream = _fileService.Get(messageText);

                if (stream == null || (stream.CanSeek && stream.Length == 0))
                {
                    _logger.LogError($"Error while downloading file [{messageText}]. File is empty.");
                    await _cacheService.SetTelegramUserStateAsync(chatId, TelegramUserState.None);
                    await SendMainMenuMessage(chatId, cancellationToken, FileEmpty);
                    return;
                }

                await _cacheService.SetTelegramUserStateAsync(chatId, TelegramUserState.None);
                await _botClient.SendDocument(
                      chatId: chatId,
                      document: InputFile.FromStream(stream, messageText));
                return;
            }
            catch(FileNotFoundException)
            {
                await SendMainMenuMessage(chatId, cancellationToken, FileNotFound);
                await _cacheService.SetTelegramUserStateAsync(chatId, TelegramUserState.None);
                return;
            }
            catch(Exception ex)
            {
                _logger.LogError(ex, $"Error while downloading file [{messageText}].");
                await _cacheService.SetTelegramUserStateAsync(chatId, TelegramUserState.None);
                await SendMainMenuMessage(chatId, cancellationToken, UnexpectedError);
                return;
            }
        }
        else if (userState == TelegramUserState.AwaitingFileForDelete)
        {
            if (string.IsNullOrWhiteSpace(messageText))
            {
                await _cacheService.SetTelegramUserStateAsync(chatId, TelegramUserState.None);
                await SendMainMenuMessage(chatId, cancellationToken, UnexpectedError);
                return;
            }

            try
            {
                _fileService.Delete(messageText);

                await _cacheService.SetTelegramUserStateAsync(chatId, TelegramUserState.None);
                await SendMainMenuMessage(chatId, cancellationToken, FileDeleted);
                return;
            }
            catch (FileNotFoundException)
            {
                await SendMainMenuMessage(chatId, cancellationToken, FileNotFound);
                await _cacheService.SetTelegramUserStateAsync(chatId, TelegramUserState.None);
                return;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error while deletingFile file [{messageText}].");
                await _cacheService.SetTelegramUserStateAsync(chatId, TelegramUserState.None);
                await SendMainMenuMessage(chatId, cancellationToken, UnexpectedError);
                return;
            }
        }
        else if (userState == TelegramUserState.AwaitingFileForGetUrl)
        {
            if (string.IsNullOrWhiteSpace(messageText))
            {
                await _cacheService.SetTelegramUserStateAsync(chatId, TelegramUserState.None);
                await SendMainMenuMessage(chatId, cancellationToken, UnexpectedError);
                return;
            }

            var apiFilesDownloadUrl = _configuration["ApiUrls:FilesDownloadUrl"];
            if (string.IsNullOrWhiteSpace(apiFilesDownloadUrl))
            {
                await _cacheService.SetTelegramUserStateAsync(chatId, TelegramUserState.None);
                await SendMainMenuMessage(chatId, cancellationToken, ApiFilesDownloadUrlEmpty);
                return;
            }

            var resultUrl = $"{apiFilesDownloadUrl}{messageText}";
            var resultFormattedUrl = $"<a href=\"{resultUrl}\">{resultUrl}</a>";

            await _cacheService.SetTelegramUserStateAsync(chatId, TelegramUserState.None);
            await SendMainMenuMessage(chatId, cancellationToken, resultFormattedUrl);
            return;
        }
        else if (userState == TelegramUserState.AwaitingProjectInfoForAdd)
        {
            if (string.IsNullOrWhiteSpace(messageText))
            {
                await _cacheService.SetTelegramUserStateAsync(chatId, TelegramUserState.None);
                await SendMainMenuMessage(chatId, cancellationToken, UnexpectedError);
                return;
            }

            var splitted = messageText.Split("\n");

            if (splitted.Length != 4)
            {
                await SendMainMenuMessage(chatId, cancellationToken, WrongFormat);
                return;
            }

            var projectName = splitted[0].Trim();
            var projectYear = splitted[1].Trim();
            var projectTools = splitted[2].Trim();
            var projectVideoUrl = splitted[3].Trim();

            if (string.IsNullOrWhiteSpace(projectName) ||
                string.IsNullOrWhiteSpace(projectYear) ||
                string.IsNullOrWhiteSpace(projectTools) ||
                string.IsNullOrWhiteSpace(projectVideoUrl))
            {
                await SendMainMenuMessage(chatId, cancellationToken, WrongFormat);
                return;
            }

            await _projectService.AddProjectAsync(projectName, projectTools, projectYear, projectVideoUrl);
            await SendMainMenuMessage(chatId, cancellationToken, ProjectAdded);
            return;
        }
        else if (userState == TelegramUserState.AwaitingProjectForDelete)
        {
            if (string.IsNullOrWhiteSpace(messageText))
            {
                await _cacheService.SetTelegramUserStateAsync(chatId, TelegramUserState.None);
                await SendMainMenuMessage(chatId, cancellationToken, UnexpectedError);
                return;
            }

            if (!Guid.TryParse(messageText, out var projectId))
            {
                await _cacheService.SetTelegramUserStateAsync(chatId, TelegramUserState.None);
                await SendMainMenuMessage(chatId, cancellationToken, WrongId);
                return;
            }

            try
            {
                await _projectService.DeleteProjectByIdAsync(projectId);
            }
            catch (ProjectNotFoundException)
            {
                await _cacheService.SetTelegramUserStateAsync(chatId, TelegramUserState.None);
                await SendMainMenuMessage(chatId, cancellationToken, ProjectNotFound);
                return;
            }

            await _cacheService.SetTelegramUserStateAsync(chatId, TelegramUserState.None);
            await SendMainMenuMessage(chatId, cancellationToken, ProjectDeleted);
            return;
        }

        switch (messageText)
        {
            case GetFilesCommand:
            case GetFilesFriendlyCommand:
                await HandleGetFilesCommand(chatId, cancellationToken);
                break;
            case UploadFileCommand:
            case UploadFileFriendlyCommand:
                await HandleUploadFileCommand(chatId, cancellationToken);
                break;
            case DownloadFileCommand:
            case DownloadFileFriendlyCommand:
                await HandleDownloadFileCommand(chatId, cancellationToken);
                break;
            case UploadMainFileCommand:
            case UploadMainFileFriendlyCommand:
                await HandleUploadMainFileCommand(chatId, cancellationToken);
                break;
            case DownloadMainFileCommand:
            case DownloadMainFileFriendlyCommand:
                await HandleDownloadMainFileCommand(chatId, cancellationToken);
                break;
            case DeleteFileCommand:
            case DeleteFileFriendlyCommand:
                await HandleDeleteFileCommand(chatId, cancellationToken);
                break;
            case GetFileUrlCommand:
            case GetFileUrlFriendlyCommand:
                await HandleGetFileUrlCommand(chatId, cancellationToken);
                break;
            case GetProjectsCommand:
            case GetProjectsFriendlyCommand:
                await HandleGetProjectsCommand(chatId, cancellationToken);
                break;
            case AddProjectCommand:
            case AddProjectFriendlyCommand:
                await HandleAddProjectCommand(chatId, cancellationToken);
                break;
            case DeleteProjectCommand:
            case DeleteProjectFriendlyCommand:
                await HandleDeleteProjectCommand(chatId, cancellationToken);
                break;
            default:
                await SendMainMenuMessage(chatId, cancellationToken);
                break;
        }
    }

    private async Task HandleStartCommand(long chatId, CancellationToken cancellationToken)
    {
        await _cacheService.SetTelegramUserStateAsync(chatId, TelegramUserState.None);
        await SendMainMenuMessage(chatId, cancellationToken);
    }

    private async Task HandleGetFilesCommand(long chatId, CancellationToken cancellationToken)
    {
        var files = _fileService.GetFiles();

        var sb = new StringBuilder();
        for (var i = 0; i < files.Length; i++)
        {
            var currentFile = files[i];
            sb.AppendLine($"{i + 1}. {currentFile}\n");
        }

        var filesText = files.Length == 0 ? NotFound : sb.ToString();

        await SendMainMenuMessage(chatId, cancellationToken, filesText);
    }

    private async Task HandleUploadFileCommand(long chatId, CancellationToken cancellationToken)
    {
        await _cacheService.SetTelegramUserStateAsync(chatId, TelegramUserState.AwaitingFileUpload);
        await SendMainMenuMessage(chatId, cancellationToken, SendFile);
    }

    private async Task HandleDownloadFileCommand(long chatId, CancellationToken cancellationToken)
    {
        var files = _fileService.GetFiles();
        if (files == null || files.Length == 0)
        {
            await SendMainMenuMessage(chatId, cancellationToken, NotFoundFilesForDownload);
            return;
        }

        await _cacheService.SetTelegramUserStateAsync(chatId, TelegramUserState.AwaitingFileForDownload);
        await SendChooseFileMenuMessage(files, chatId, cancellationToken, ChooseFileNameForDownload);
    }

    private async Task HandleUploadMainFileCommand(long chatId, CancellationToken cancellationToken)
    {
        await _cacheService.SetTelegramUserStateAsync(chatId, TelegramUserState.AwaitingMainFileUpload);
        await SendMainMenuMessage(chatId, cancellationToken, SendFile);
    }

    private async Task HandleDownloadMainFileCommand(long chatId, CancellationToken cancellationToken)
    {
        try
        {
            var stream = _fileService.GetMain();

            if (stream == null || (stream.CanSeek && stream.Length == 0))
            {
                _logger.LogError($"Error while downloading main file. File is empty.");
                await _cacheService.SetTelegramUserStateAsync(chatId, TelegramUserState.None);
                await SendMainMenuMessage(chatId, cancellationToken, FileEmpty);
                return;
            }

            await _cacheService.SetTelegramUserStateAsync(chatId, TelegramUserState.None);
            await _botClient.SendDocument(
                  chatId: chatId,
                  document: InputFile.FromStream(stream));
            return;
        }
        catch (FileNotFoundException)
        {
            await SendMainMenuMessage(chatId, cancellationToken, FileNotFound);
            await _cacheService.SetTelegramUserStateAsync(chatId, TelegramUserState.None);
            return;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"Error while downloading main file.");
            await _cacheService.SetTelegramUserStateAsync(chatId, TelegramUserState.None);
            await SendMainMenuMessage(chatId, cancellationToken, UnexpectedError);
            return;
        }
    }

    private async Task HandleDeleteFileCommand(long chatId, CancellationToken cancellationToken)
    {
        var files = _fileService.GetFiles();
        if (files == null || files.Length == 0)
        {
            await SendMainMenuMessage(chatId, cancellationToken, NotFoundFilesForDelete);
            return;
        }

        await _cacheService.SetTelegramUserStateAsync(chatId, TelegramUserState.AwaitingFileForDelete);
        await SendChooseFileMenuMessage(files, chatId, cancellationToken, ChooseFileNameForDelete);
    }

    private async Task HandleGetFileUrlCommand(long chatId, CancellationToken cancellationToken)
    {
        var files = _fileService.GetFiles();
        if (files == null || files.Length == 0)
        {
            await SendMainMenuMessage(chatId, cancellationToken, NotFoundFilesForGetUrl);
            return;
        }

        await _cacheService.SetTelegramUserStateAsync(chatId, TelegramUserState.AwaitingFileForGetUrl);
        await SendChooseFileMenuMessage(files, chatId, cancellationToken, WriteFileNameForGetUrl);
    }

    private async Task HandleGetProjectsCommand(long chatId, CancellationToken cancellationToken)
    {
        var projects = await _projectService.GetProjectsAsync();

        var sb = new StringBuilder();
        for (var i = 1; i <= projects.Count; i++)
        {
            var currentProject = projects[i - 1];
            var formattedUrl = $"<a href=\"{currentProject.VideoUrl}\">{currentProject.VideoUrl}</a>";

            sb.AppendLine(
                $"<b>{i}. {currentProject.Name}</b>\n" +
                $"\n" +
                $"🆔: <b>{currentProject.Id}</b>\n" +
                $"📆 Год: <b>{currentProject.Year}</b>\n" +
                $"⚙️ Инструменты:<b>{currentProject.Tools}</b>\n" +
                $"\n" +
                $"🔗 Видео: {formattedUrl}\n" +
                $"──────────────");
        }

        var resultText = projects.Count == 0 ? NotFound : sb.ToString();
        
        await SendMainMenuMessage(chatId, cancellationToken, resultText);
    }

    private async Task HandleAddProjectCommand(long chatId, CancellationToken cancellationToken)
    {
        await _cacheService.SetTelegramUserStateAsync(chatId, TelegramUserState.AwaitingProjectInfoForAdd);
        await SendMainMenuMessage(chatId, cancellationToken, WriteProject);
    }

    private async Task HandleDeleteProjectCommand(long chatId, CancellationToken cancellationToken)
    {
        await _cacheService.SetTelegramUserStateAsync(chatId, TelegramUserState.AwaitingProjectForDelete);
        await SendMainMenuMessage(chatId, cancellationToken, WriteProjectIdForDelete);
    }

    private async Task SendChooseFileMenuMessage(string[] files, long chatId, CancellationToken cancellationToken, string? message = null)
    {
        var keyboard = new ReplyKeyboardMarkup(files.Select(file => new[] { new KeyboardButton(file) }));

        keyboard.ResizeKeyboard = true;
        keyboard.OneTimeKeyboard = true;

        await _botClient.SendMessage(
            chatId: chatId,
            text: message ?? ChooseAction,
            replyMarkup: keyboard,
            parseMode: ParseMode.Html,
            cancellationToken: cancellationToken);
    }

    private async Task SendMainMenuMessage(long chatId, CancellationToken cancellationToken, string? message = null)
    {
        var keyboard = new ReplyKeyboardMarkup(
        [
           [new KeyboardButton(GetFilesFriendlyCommand), new KeyboardButton(GetProjectsFriendlyCommand)],
           [new KeyboardButton(UploadFileFriendlyCommand), new KeyboardButton(AddProjectFriendlyCommand)],
           [new KeyboardButton(DeleteFileFriendlyCommand), new KeyboardButton(DeleteProjectFriendlyCommand)],
           [new KeyboardButton(GetFileUrlFriendlyCommand)]
        ]);

        keyboard.ResizeKeyboard = true;
        keyboard.OneTimeKeyboard = true;

        await _botClient.SendMessage(
            chatId: chatId,
            text: message ?? ChooseAction,
            replyMarkup: keyboard,
            parseMode: ParseMode.Html,
            cancellationToken: cancellationToken);
    }

    private async Task SendTextMessage(long chatId, string message, CancellationToken cancellationToken)
    {
        await _botClient.SendMessage(
               chatId: chatId,
               text: message,
               cancellationToken: cancellationToken);
    }
}

