// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

namespace Mud.Wechat.MiniProgram.DataModels.DataAnalysis;

/// <summary>用户访问小程序数据概况应答（<c>POST /datacube/getweanalysisappiddailysummarytrend</c>）。</summary>
/// <remarks>
/// <para>官方文档：<c>data-analysis/others/api_getdailysummary.html</c>。</para>
/// <para><b>与「日趋势」差异（勿混淆）</b>：趋势返回打开 / 访问 / 停留等<b>过程指标</b>；概况返回
/// 累计用户数 / 转发次数 / 转发人数等<b>结果指标</b>。</para>
/// </remarks>
[HttpJsonSerializable(SerializerClassName = "DataAnalysis")]
public class WxaDailySummaryResponse : WxaResponse
{
    /// <summary>概况数据列表（<c>list</c>），见 <see cref="WxaDailySummaryItem"/>。</summary>
    [JsonPropertyName("list")]
    public List<WxaDailySummaryItem>? List { get; set; }
}
