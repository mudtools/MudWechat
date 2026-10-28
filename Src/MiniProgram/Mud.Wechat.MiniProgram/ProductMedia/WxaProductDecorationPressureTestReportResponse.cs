// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

namespace Mud.Wechat.MiniProgram.DataModels.ProductMedia;

/// <summary>
/// 压测报告回包（<c>POST /product/decoration/wxaproduct_decoration_get_pressure_test_report</c>）。
/// </summary>
/// <remarks>
/// <para><b>官方字段拼写错误照录</b>：<c>blankpage_pencent</c> 为官方原文（应为 percent）
/// ⇒ 本仓不改名，避免反序列化失配。</para>
/// <para><b>官方回包示例 JSON 畸形</b>：示例首行弯引号 + 缺逗号，且同一示例同时包含业务字段与
/// <c>errcode</c> ⇒ 按参数表建模；<c>pressure_id</c> 在回包参数表中出现但示例未列。</para>
/// <para>错误码 10007 压测 id 不存在 / 10003 压测报告获取失败（压测未结束）。</para>
/// </remarks>
[HttpJsonSerializable(SerializerClassName = "ProductMedia")]
public class WxaProductDecorationPressureTestReportResponse : WxaResponse
{
    /// <summary>白页率（官方拼写 pencent）。</summary>
    [JsonPropertyName("blankpage_pencent")]
    public int? BlankpagePencent { get; set; }

    /// <summary>打开页面平均耗时，单位 ms。</summary>
    [JsonPropertyName("aver_time_cost")]
    public long? AverTimeCost { get; set; }

    /// <summary>打开页面最大耗时，单位 ms。</summary>
    [JsonPropertyName("max_time_cost")]
    public long? MaxTimeCost { get; set; }

    /// <summary>页面打开次数。</summary>
    [JsonPropertyName("total_launch_cnt")]
    public long? TotalLaunchCnt { get; set; }

    /// <summary>网络请求总次数。</summary>
    [JsonPropertyName("total_request_cnt")]
    public long? TotalRequestCnt { get; set; }

    /// <summary>各网络请求统计。</summary>
    [JsonPropertyName("network_list")]
    public List<WxaProductDecorationPressureTestReportNetwork>? NetworkList { get; set; }

    /// <summary>小程序 appid。</summary>
    [JsonPropertyName("appid")]
    public string? AppId { get; set; }

    /// <summary>压测时长，单位 s。</summary>
    [JsonPropertyName("run_time")]
    public long? RunTime { get; set; }

    /// <summary>压测 ID。</summary>
    [JsonPropertyName("pressure_id")]
    public long? PressureId { get; set; }

    /// <summary>压测状态，见 <see cref="WxaProductPressureTestStatuses"/>。</summary>
    [JsonPropertyName("status")]
    public int? Status { get; set; }
}
