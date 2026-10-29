// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

namespace Mud.Wechat.OfficialAccount.DataModels.DataCube;

/// <summary>
/// 数据统计域统一请求体（官方 21 端点请求体均为 <c>{begin_date, end_date}</c> 两字段 ⇒ 共用 DTO，N4 裁决）。
/// </summary>
/// <remarks>
/// <para>
/// 日期格式 yyyy-MM-dd。各端点<b>跨度上限措辞不一（照官方各页原文，勿互相套用）</b>：
/// 用户数据 2 端点「最大跨度7天」；getupstreammsg「与end_date差值小于7天」；消息周/月/分时
/// 「结束日期(必须为同一天)」；消息分布 3 端点「跨度不超过15天」；getinterfacesummary
/// 「最大时间跨度30天」；getinterfacesummaryhour「最大时间跨度为1天」（注意事项）；
/// 新图文 3 端点「日期范围仅支持查询1天。」；getbizsummary「最长支持查询30天。」；
/// 旧图文 6 端点参数表仅「结束日期(最大值为昨日)」（无天数表述）。
/// </para>
/// <para>SDK 不做本地跨度校验（越界由官方 61501 表达）。</para>
/// </remarks>
[HttpJsonSerializable(SerializerClassName = "DataCube")]
public class MpDateRangeRequest
{
    /// <summary>获取或设置起始日期（官方 <c>begin_date</c>，格式 yyyy-MM-dd）。</summary>
    [JsonPropertyName("begin_date")]
    public string BeginDate { get; set; } = string.Empty;

    /// <summary>获取或设置结束日期（官方 <c>end_date</c>，格式 yyyy-MM-dd；各端点跨度上限见类型 remarks）。</summary>
    [JsonPropertyName("end_date")]
    public string EndDate { get; set; } = string.Empty;
}
