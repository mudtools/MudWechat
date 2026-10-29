// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

namespace Mud.Wechat.OfficialAccount.DataModels.Store;

/// <summary>
/// 区域对象（官方 <c>result</c> 内层元素）。
/// </summary>
/// <remarks>
/// 官方文档缺陷（照录）：① 返回示例的第三层（区级）仅有 <c>id</c>/<c>fullname</c>/<c>location</c>，
/// 缺少字段表声明的 <c>name</c>/<c>pinyin</c>/<c>cidx</c>；② <c>cidx</c> 说明为「下属地区所有 id」，
/// 但示例省级仅 2 个值，与「所有」表述不符。
/// </remarks>
[HttpJsonSerializable(SerializerClassName = "Store")]
public class MpDistrictInfo
{
    /// <summary>获取或设置区域 id（官方 <c>id</c>，也叫 districtid）。</summary>
    [JsonPropertyName("id")]
    public string? Id { get; set; }

    /// <summary>获取或设置省市区简要名称（官方 <c>name</c>）。</summary>
    [JsonPropertyName("name")]
    public string? Name { get; set; }

    /// <summary>获取或设置省市区完整名称（官方 <c>fullname</c>）。</summary>
    [JsonPropertyName("fullname")]
    public string? FullName { get; set; }

    /// <summary>获取或设置省市区拼音列表（官方 <c>pinyin</c>）。</summary>
    [JsonPropertyName("pinyin")]
    public List<string>? Pinyin { get; set; }

    /// <summary>获取或设置坐标（官方 <c>location</c>）。</summary>
    [JsonPropertyName("location")]
    public MpDistrictLocation? Location { get; set; }

    /// <summary>获取或设置下属地区 id 列表（官方 <c>cidx</c>；可通过此 id 获取下属地区）。</summary>
    [JsonPropertyName("cidx")]
    public List<int>? ChildIndexes { get; set; }
}
