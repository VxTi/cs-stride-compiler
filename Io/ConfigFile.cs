using System.Text.Json;

namespace StrideCompiler.Io;
public class ConfigFile(string path) : AbstractProjectFile(path)
{
    public ProjectConfig ToConfig()
    {
        var content = Content();
        try
        {
            return JsonSerializer.Deserialize<ProjectConfig>(content);
        }
        catch (ArgumentNullException nullEx)
        {
            
        }
        catch (JsonException jsonEx)
        {

        }
        catch (NotSupportedException notSupportedEx)
        {
            
        }
    }
}