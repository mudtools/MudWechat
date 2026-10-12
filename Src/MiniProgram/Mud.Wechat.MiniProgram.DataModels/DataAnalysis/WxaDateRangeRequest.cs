// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

namespace Mud.Wechat.MiniProgram.DataModels.DataAnalysis;

/// <summary>
/// 数据分析日期区间请求体（<c>begin_date</c> / <c>end_date</c>；9 端点共用同形）。
/// </summary>
/// <remarks>
/// <para>格式 <c>yyyymmdd</c>；<b>上界恒为昨日</b>。跨度按端点族不同（1 天 / 对应周期 / 0·6·29 天），
/// 见各端点 remarks —— SDK <b>不做本地校验</b>，越界由官方错误码表达。</para>
/// <para>保持 <see cref="string"/> 而非日期类型：官方为定长压缩格式（<c>20170313</c>），
/// 转成 <c>DateTime</c> 再格式化只会引入文化/时区漂移风险。</para>
/// </remarks>
[HttpJsonSerializable(SerializerClassName = "DataAnalysis")]
public class WxaDateRangeRequest
{
    /// <summary>开始日期（<c>begin_date</c>，必填，格式 <c>yyyymmdd</c>）。</summary>
    [JsonPropertyName("begin_date")]
    public string? BeginDate { get; set; }

    /// <summary>结束日期（<c>end_date</c>，必填，格式 <c>yyyymmdd</c>；最大值 ≤ 昨日）。</summary>
    [JsonPropertyName("end_date")]
    public string? EndDate { get; set; }
}
