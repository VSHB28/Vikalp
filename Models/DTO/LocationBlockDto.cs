
public class LocationBlockDto
{
    public int BlockId { get; set; }
    public string BlockName { get; set; }
    public string BlockCode { get; set; }

    public int RegionId { get; set; }  
    public int DistrictId { get; set; }

    public bool IsActive { get; set; }
    public bool IsAspirational { get; set; }
}
