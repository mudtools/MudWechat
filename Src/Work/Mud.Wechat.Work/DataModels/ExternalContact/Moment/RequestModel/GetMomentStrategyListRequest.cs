// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

namespace Mud.Wechat.Work.DataModels.ExternalContact.Moment;

/// <summary>
/// 获取客户朋友圈规则组列表请求体（<c>/cgi-bin/externalcontact/moment_strategy/list</c>）。
/// <para>应用仅能获取和管理由本应用创建的规则组。</para>
/// </summary>
[HttpJsonSerializable(SerializerClassName = "Moment")]
public class GetMomentStrategyListRequest
{
    /// <summary>
    /// 获取或设置用于分页查询的游标，由上一次调用返回，首次调用可不填。
    /// </summary>
    [JsonPropertyName("cursor")]
    public string? Cursor { get; set; }

    /// <summary>
    /// 获取或设置分页大小（默认为 1000，最大值为 1000）。
    /// </summary>
    [JsonPropertyName("limit")]
    public int? Limit { get; set; }
}
