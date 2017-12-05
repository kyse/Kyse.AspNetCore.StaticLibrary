//Created by Jared Fisher (kyse@kyse.us)

namespace Kyse.AspNetCore.StaticLibrary
{
    public interface ILibrary
    {
        string Name { get; set; }
        string Path { get; set; }
    }
}