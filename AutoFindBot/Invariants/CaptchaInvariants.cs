namespace AutoFindBot.Invariants;

public class CaptchaInvariants
{
    public const string NeededSolveMessage = "Запрос заблокирован. Требуется решить каптчу.";
    
    public const string SolvingError = "Произошла ошибка при решении капчи: :errorMessage";

    public const string PageFetchingError = "Произошла ошибка получении страницы с капчей: :errorMessage";
    
    public const string MessageServerConnection = "Не удалось подключиться к серверу: :errorMessage";
    
    public const string ImageLoadingError = "Произошла ошибка при загрузке изображения: :errorMessage";
    
    public const string PathUndefinedError = "Не удалось определить путь к капче.";
    
    public const string DetectionErrorInHtml = "HTML не содержит капчу";
    
    public const string RetryLimitExceeded = "Превышено количество повторных попыток :retryCount.";
    
    public const string RetryExecutionError = $"Повторная попытка :i не удалась: :errorMessage.";
}