// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

namespace Mud.Wechat.Work.DataModels.PayTool;

/// <summary>
/// 获取发票列表请求体（<c>/cgi-bin/paytool/get_invoice_list</c>）。
/// </summary>
/// <remarks>
/// <para>官方权限口径：服务商需有在收银台完成商户号注册。</para>
/// <para>无需签名：官方本端点参数表不含 nonce_str / ts / sig（与收款工具族不同）。</para>
/// </remarks>
[HttpJsonSerializable(SerializerClassName = "PayTool")]
public class GetPayToolInvoiceListRequest
{
    /// <summary>
    /// 获取或设置开始时间（申请时间，unix 时间戳，官方可选）。
    /// <para>官方限制：不能单独指定该字段，start_time 跟 end_time 必须同时指定。</para>
    /// </summary>
    [JsonPropertyName("start_time")]
    public long? StartTime { get; set; }

    /// <summary>
    /// 获取或设置结束时间（申请时间，unix 时间戳，官方可选）。
    /// <para>官方限制：不能单独指定该字段，start_time 跟 end_time 必须同时指定。</para>
    /// </summary>
    [JsonPropertyName("end_time")]
    public long? EndTime { get; set; }

    /// <summary>
    /// 获取或设置分页游标（官方可选，字符串类型，由上一次调用返回，首次调用可不填）。
    /// </summary>
    [JsonPropertyName("cursor")]
    public string? Cursor { get; set; }

    /// <summary>
    /// 获取或设置返回的最大记录数（官方可选，整型，最大值 100，默认值 50）。
    /// </summary>
    [JsonPropertyName("limit")]
    public int? Limit { get; set; }
}
