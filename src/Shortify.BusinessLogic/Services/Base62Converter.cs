using System.Text;
using Shortify.BusinessLogic.Services.Contracts;

namespace Shortify.BusinessLogic.Services;

public class Base62Converter : IBase62Converter
{
    private const string Base62Characters = "0123456789ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz";
    private const int Base = 62;
    private const int Secret = 123;

    public string Encode(int value, int length)
    {
        return EncodeBase62(GenerateUniqueId(value, length));
    }

    private static int GenerateUniqueId(int id, int length)
    {
        var minValue = Math.Pow(Base, length - 1);

        return (int)((id ^ Secret) + minValue);
    }
    
    private static string EncodeBase62(int value)
    {
        var sb = new StringBuilder();
        
        while (value > 0)
        {
            sb.Append(Base62Characters[value % Base]);
            value /= Base;
        }

        return sb.ToString();
    }
}