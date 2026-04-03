namespace Mud.Wechat.Work.DataModels;

/// <summary>
/// 应用权限信息
/// </summary>
public class Privilege
{
    /// <summary>
    /// 权限等级。
    /// 1:通讯录基本信息只读
    /// 2:通讯录全部信息只读
    /// 3:通讯录全部信息读写
    /// 4:单个基本信息只读
    /// 5:通讯录全部信息只写
    /// </summary>
    [JsonPropertyName("level")]
    public int Level { get; set; }

    /// <summary>
    /// 应用可见范围（部门）
    /// </summary>
    [JsonPropertyName("allow_party")]
    public List<int> AllowParty { get; set; } = [];

    /// <summary>
    /// 应用可见范围（成员）
    /// </summary>
    [JsonPropertyName("allow_user")]
    public List<string?> AllowUser { get; set; } = [];

    /// <summary>
    /// 应用可见范围（标签）
    /// </summary>
    [JsonPropertyName("allow_tag")]
    public List<int> AllowTag { get; set; } = [];

    /// <summary>
    /// 额外通讯录（部门）
    /// </summary>
    [JsonPropertyName("extra_party")]
    public List<int> ExtraParty { get; set; } = [];

    /// <summary>
    /// 额外通讯录（成员）
    /// </summary>
    [JsonPropertyName("extra_user")]
    public List<string?> ExtraUser { get; set; } = [];

    /// <summary>
    /// 额外通讯录（标签）
    /// </summary>
    [JsonPropertyName("extra_tag")]
    public List<int> ExtraTag { get; set; } = [];
}