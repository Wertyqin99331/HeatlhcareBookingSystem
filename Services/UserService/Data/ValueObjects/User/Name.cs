using Core_ValueObject = Core.ValueObject;
using ValueObject = Core.ValueObject;
using ValueObjects_ValueObject = Core.ValueObject;

namespace UserService.Data.ValueObjects.User;

public class Name: Core_ValueObject
{
    public const int MAX_LENGTH = 50;
    
    protected Name(string value)
    {
        this.Value = value;
    }
    
    public string Value { get; }

    public static Result<Name, string> Create(string value)
    {
        if (string.IsNullOrEmpty(value))
        {
            return "Name can't be empty or null";
        }

        if (value.Length > MAX_LENGTH)
        {
            return $"Name can't be longer than {MAX_LENGTH} characters";
        }

        return new Name(value);
    }
    
    protected override IEnumerable<object> GetAtomicValues()
    {
        yield return this.Value;
    }
}

public class NameMapping: IRegister {
    public void Register(TypeAdapterConfig config)
    {
        config.NewConfig<Name, string>()
            .MapWith(src => src.Value);
    }
}