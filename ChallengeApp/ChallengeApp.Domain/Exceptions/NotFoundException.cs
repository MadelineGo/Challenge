namespace ChallengeApp.Domain.Exceptions;

public class NotFoundException(string message) : Exception(message);