namespace AutoFindBot.Invariants;

public class CaptchaInvariants
{
    public const string ErrorCaptchaSolveMessage = "Запрос заблокирован. Не удалось пройти капчу.";
    
    public const string SolvingError = "Произошла ошибка при решении капчи: :errorMessage";

    public const string MessageServerConnection = "Не удалось подключиться к серверу: :errorMessage";
    
    public const string ImageLoadingError = "Произошла ошибка при загрузке изображения: :errorMessage";

    public const string DetectionErrorInHtml = "HTML не содержит капчу: :html";
}