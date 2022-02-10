namespace web_spa_vue
{
  public class ClientAppSettings
  {
    public string Path { get; set; }
  }

  public class AppSettings
  {
    public ClientAppSettings ClientApp { get; set; } = new();
  }
}
