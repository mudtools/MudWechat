// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

namespace Mud.Wechat.Work.DataModels.School;

/// <summary>
/// 家长信息（读取学生或家长响应的 <c>parent</c> 对象 / <c>student.parents[]</c> 元素 /
/// 获取部门家长详情响应的 <c>parents[]</c> 元素）。
/// <para>
/// 官方业务限制：<see cref="Mobile"/> 第三方应用不可获取，代开发应用需管理员授权手机号权限才返回；
/// <see cref="ExternalUserid"/> 仅当家长已关注「学校通知」才返回，对同一个服务商来说，
/// 同一个家长微信在不同学校下返回的 external_userid 是一样的。
/// </para>
/// </summary>
[HttpJsonSerializable(SerializerClassName = "School")]
public class SchoolParentInfo
{
    /// <summary>
    /// 获取或设置家长的 userid。
    /// </summary>
    [JsonPropertyName("parent_userid")]
    public string? ParentUserid { get; set; }

    /// <summary>
    /// 获取或设置学生与家长的关系（仅读取学生或家长响应的 <c>parent</c> 对象返回）。
    /// </summary>
    [JsonPropertyName("relation")]
    public string? Relation { get; set; }

    /// <summary>
    /// 获取或设置家长手机号。第三方应用不可获取；代开发应用需管理员授权手机号权限才返回。
    /// </summary>
    [JsonPropertyName("mobile")]
    public string? Mobile { get; set; }

    /// <summary>
    /// 获取或设置家长是否关注了「学校通知」（0-未关注，1-已关注）。
    /// </summary>
    [JsonPropertyName("is_subscribe")]
    public int? IsSubscribe { get; set; }

    /// <summary>
    /// 获取或设置家长的 external_userid。仅当家长已关注「学校通知」才返回；
    /// 对同一个服务商来说，同一个家长微信在不同学校下返回的 external_userid 是一样的。
    /// </summary>
    [JsonPropertyName("external_userid")]
    public string? ExternalUserid { get; set; }

    /// <summary>
    /// 获取或设置家长孩子列表（仅读取学生或家长响应的 <c>parent</c> 对象与
    /// 获取部门家长详情响应返回）。
    /// </summary>
    [JsonPropertyName("children")]
    public List<SchoolChildInfo>? Children { get; set; }
}
