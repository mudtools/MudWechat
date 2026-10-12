// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

namespace Mud.Wechat.OfficialAccount.DataModels.Template;

/// <summary>设置所属行业（<c>template/api_set_industry</c>）请求体。</summary>
/// <remarks>
/// <para>
/// 官方契约（逐页核验 2026-10-07）：<b>仅 industry_id1/industry_id2 两个必填字段</b>
/// （行业代码 1~41 的字符串形态，官方示例 <c>"1"</c>/<c>"4"</c>；当前页面无 <c>industry_id3</c>，
/// 与旧版资料的 3 参数形态不同——以核验页面为准）。
/// </para>
/// <para>
/// <b>硬约束（官方原文）</b>：「每月可修改行业 1 次，账号仅可使用所属行业中相关的模板」；
/// 「修改行业后，你在原有行业中的模板将会被删除」。
/// </para>
/// </remarks>
[HttpJsonSerializable(SerializerClassName = "Template")]
public class MpSetIndustryRequest
{
    /// <summary>获取或设置公众号模板消息所属行业编号（官方 <c>industry_id1</c>，主行业；代码 1~41）。</summary>
    [JsonPropertyName("industry_id1")]
    public string IndustryId1 { get; set; } = string.Empty;

    /// <summary>获取或设置公众号模板消息所属行业编号（官方 <c>industry_id2</c>，副行业；代码 1~41）。</summary>
    [JsonPropertyName("industry_id2")]
    public string IndustryId2 { get; set; } = string.Empty;
}
