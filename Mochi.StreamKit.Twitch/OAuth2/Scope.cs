using System.Text;

namespace Mochi.StreamKit.Twitch.OAuth2;

public class Scope
{
    private static readonly List<Scope> _scopes = new();
    private static readonly Lazy<List<Scope>> _usableScopes = new(() => ResolveAllUsableScopes().ToList());

    public static readonly Scope ChatEdit = Builtin("chat:edit");
    public static readonly Scope ChatRead = Builtin("chat:read");
    
    private static Scope Builtin(string path) =>
        GetByPath(path) ?? throw new Exception($"Builtin scope \"{path}\" not initialized");
    
    static Scope()
    {
        foreach (var path in new[]
        {
            "analytics:read:extensions", "analytics:read:games", "bits:read", "channel:manage:broadcast",
            "channel:read:charity", "channel:edit:commercial", "channel:read:editors", "channel:manage:extensions",
            "channel:read:goals", "channel:read:guest_star", "channel:manage:guest_star", "channel:read:hype_train",
            "channel:manage:moderators", "channel:read:polls", "channel:manage:polls", "channel:read:predictions",
            "channel:manage:predictions", "channel:manage:raids", "channel:read:redemptions",
            "channel:manage:redemptions", "channel:manage:schedule", "channel:read:stream_key",
            "channel:read:subscriptions", "channel:manage:videos", "channel:read:vips", "channel:manage:vips",
            "clips:edit", "moderation:read", "moderator:manage:announcements", "moderator:manage:automod",
            "moderator:read:automod_settings", "moderator:manage:automod_settings", "moderator:manage:banned_users",
            "moderator:read:blocked_terms", "moderator:manage:blocked_terms", "moderator:manage:chat_messages",
            "moderator:read:chat_settings", "moderator:manage:chat_settings", "moderator:read:chatters",
            "moderator:read:followers", "moderator:read:guest_star", "moderator:manage:guest_star",
            "moderator:read:shield_mode", "moderator:manage:shield_mode", "moderator:read:shoutouts",
            "moderator:manage:shoutouts", "user:edit", "user:edit:follows", "user:read:blocked_users",
            "user:manage:blocked_users", "user:read:broadcast", "user:manage:chat_color", "user:read:email",
            "user:read:follows", "user:read:subscriptions", "user:manage:whispers"
        })
        {
            RegisterPath(path);
        }
    }
    
    private static IEnumerable<Scope> ResolveAllUsableScopes() => _scopes.SelectMany(x => x.GetUsableChildren());

    public static List<Scope> AllUsableScopes => _usableScopes.Value;

    public Scope? Parent { get; private set; }
    public string Name { get; }
    public List<Scope> Children { get; } = new();
    public bool CanUse { get; private set; }

    public Scope? this[string name] => Children.FirstOrDefault(x => x.Name == name);
    
    public Scope? GetChildByPath(string path)
    {
        var names = path.Split(':', 2);
        var first = names.First();
        if (string.IsNullOrEmpty(first)) return this;

        var child = this[first];
        if (child == null) return null;

        return names.Length == 2 ? child.GetChildByPath(names[1]) : child;
    }
    
    public static Scope? GetByPath(string path)
    {
        var names = path.Split(':', 2);
        var first = names.First();
        if (string.IsNullOrEmpty(first))
        {
            throw new ArgumentException("Invalid empty path");
        }

        var child = _scopes.FirstOrDefault(x => x.Name == first);
        if (child == null) return null;

        return names.Length == 2 ? child.GetChildByPath(names[1]) : child;
    }

    public string FullPath
    {
        get
        {
            var sb = new StringBuilder();
            if (Parent != null)
            {
                sb.Append(Parent.FullPath);
                sb.Append(':');
            }

            sb.Append(Name);
            return sb.ToString();
        }
    }

    public IEnumerable<Scope> GetUsableChildren()
    {
        var direct = Children.Where(x => x.CanUse).ToList();
        return direct.Concat(direct.SelectMany(x => x.Children.Where(c => c.CanUse)));
    }

    public Scope SetCanUse()
    {
        CanUse = true;
        return this;
    }
    
    public Scope(string name, Scope? parent = null)
    {
        if (name.Contains(':'))
            throw new ArgumentException($"Invalid scope node with name: \"{name}\"");
        
        Name = name;
        Parent = parent;
    }

    public Scope Then(Scope scope)
    {
        var existing = Children.FirstOrDefault(x => x.Name == scope.Name);
        if (existing != null)
        {
            existing.MergeChildrenWith(scope);
        }
        else
        {
            scope.Parent = this;
            Children.Add(scope);
        }
        return this;
    }

    private void MergeChildrenWith(Scope other)
    {
        foreach (var child in other.Children)
        {
            Then(child);
        }
    }

    private static void Register(Scope scope)
    {
        _scopes.Add(scope);
    }

    private static Scope GetOrRegister(string name)
    {
        var existing = _scopes.FirstOrDefault(x => x.Name == name);
        if (existing == null)
        {
            existing = new Scope(name);
            Register(existing);
        }

        return existing;
    }

    private static void RegisterPath(string fullName)
    {
        var names = fullName.Split(':');
        var scope = GetOrRegister(names.First());

        foreach (var name in names.Skip(1))
        {
            var child = new Scope(name);
            scope.Then(child);
            scope = child;
        }

        scope.SetCanUse();
    }
}