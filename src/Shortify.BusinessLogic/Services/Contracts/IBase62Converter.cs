namespace Shortify.BusinessLogic.Services.Contracts;

public interface IBase62Converter
{
    string Encode(int value, int length);
}