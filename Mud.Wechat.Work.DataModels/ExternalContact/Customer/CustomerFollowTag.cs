// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

namespace Mud.Wechat.Work.DataModels.ExternalContact.Customer;

/// <summary>
/// 跟进人所打客户标签（获取客户详情 <c>follow_user[].tags[]</c> 元素）。
/// </summary>
[HttpJsonSerializable(SerializerClassName = "Customer")]
public class CustomerFollowTag
{
    /// <summary>
    /// 获取或设置所打标签的分组名称（标签功能需要企业微信升级到 2.7.5 及以上版本）。
    /// </summary>
    [JsonPropertyName("group_name")]
    public string? GroupName { get; set; }

    /// <summary>
    /// 获取或设置所打标签名称。
    /// </summary>
    [JsonPropertyName("tag_name")]
    public string? TagName { get; set; }

    /// <summary>
    /// 获取或设置所打企业标签的 id（用户自定义类型标签 type = 2 不返回）。
    /// </summary>
    [JsonPropertyName("tag_id")]
    public string? TagId { get; set; }

    /// <summary>
    /// 获取或设置所打标签类型：1-企业设置，2-用户自定义，3-规则组标签。
    /// </summary>
    [JsonPropertyName("type")]
    public int? Type { get; set; }
}
