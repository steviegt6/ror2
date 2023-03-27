namespace Void.Optimizer.Core;

public enum AllowanceType {
    Whitelist,
    Blacklist,
    None,
}

public readonly struct Whitelist {
    public AllowanceType AllowanceType { get; }

    public string[] Items { get; }

    public Whitelist(AllowanceType allowanceType, params string[] items) {
        AllowanceType = allowanceType;
        Items = items;
    }
}
