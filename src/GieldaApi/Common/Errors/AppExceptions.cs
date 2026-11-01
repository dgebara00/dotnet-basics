namespace GieldaApi.Common.Errors;

// Serwisy rzucają te wyjątki, a GlobalExceptionHandler zamienia je na odpowiedni kod HTTP.
// Dzięki temu kontrolery nie mają w sobie if-ów "czy znaleziono".

public sealed class NotFoundException(string message) : Exception(message);

public sealed class ConflictException(string message) : Exception(message);
