namespace Model
{
  /// <summary>
  /// A named location wealth category
  /// </summary>
  public class LocationWealth(LocationWealthIds id)
  {
    ///
    public LocationWealthIds Id { get; } = id;

    ///
    public string Name { get; set; } = "";

    /// <summary>
    /// Alternative name
    /// </summary>
    public string AltName { get; set; } = "";

    /// 
    public List<string> Descriptions { get; set; } = [];

    /// <summary> Make it one worse </summary>
    public static LocationWealthIds DownGrade(LocationWealthIds locationWealthId) => locationWealthId switch
    {
      LocationWealthIds.None => LocationWealthIds.None,
      LocationWealthIds.Destitude => LocationWealthIds.None,
      LocationWealthIds.Poor => LocationWealthIds.Destitude,
      LocationWealthIds.Average => LocationWealthIds.Poor,
      LocationWealthIds.Prospering => LocationWealthIds.Average,
      LocationWealthIds.Opulent => LocationWealthIds.Prospering,
      _ => LocationWealthIds.None,
    };

    /// <summary> Make it one better </summary>
    public static LocationWealthIds UpGrade(LocationWealthIds locationWealthId) => locationWealthId switch
    {
      LocationWealthIds.None => LocationWealthIds.None,
      LocationWealthIds.Destitude => LocationWealthIds.Poor,
      LocationWealthIds.Poor => LocationWealthIds.Average,
      LocationWealthIds.Average => LocationWealthIds.Prospering,
      LocationWealthIds.Prospering => LocationWealthIds.Opulent,
      LocationWealthIds.Opulent => LocationWealthIds.Prospering,
      _ => LocationWealthIds.None,
    };
  }

  /// 
  public enum LocationWealthIds
  {
    /// 
    None = 0,
    ///             
    Destitude = 1,
    ///             
    Poor = 2,
    ///             
    Average = 3,
    ///             
    Prospering = 4,
    ///             
    Opulent = 5,
  }
}