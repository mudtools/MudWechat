// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

namespace Mud.Wechat.OfficialAccount.DataModels.Basic;

/// <summary>
/// 网络通信检测响应（<c>POST /cgi-bin/callback/check</c>）。
/// </summary>
/// <remarks>
/// 错误码：<c>40201</c>（未设置回调 URL）/ <c>40202</c>（非法 action）/ <c>40203</c>（非法 check_operator）。
/// 官方「注意事项」章节原文为「本接口无特殊注意事项」。
/// </remarks>
[HttpJsonSerializable(SerializerClassName = "Basic")]
public class MpCallbackCheckResponse : MpResponse
{
    /// <summary>DNS 解析结果列表。</summary>
    [JsonPropertyName("dns")]
    public List<MpCallbackCheckDns>? Dns { get; set; }

    /// <summary>PING 检测结果列表。</summary>
    [JsonPropertyName("ping")]
    public List<MpCallbackCheckPing>? Ping { get; set; }
}

/// <summary>网络通信检测——DNS 解析结果项。</summary>
[HttpJsonSerializable(SerializerClassName = "Basic")]
public class MpCallbackCheckDns
{
    /// <summary>解析出来的 IP。</summary>
    [JsonPropertyName("ip")]
    public string? Ip { get; set; }

    /// <summary>IP 对应的运营商。</summary>
    [JsonPropertyName("real_operator")]
    public string? RealOperator { get; set; }
}

/// <summary>网络通信检测——PING 检测结果项。</summary>
[HttpJsonSerializable(SerializerClassName = "Basic")]
public class MpCallbackCheckPing
{
    /// <summary>ping 的 IP（官方执行命令：<c>ping ip –c 1 -w 1 -q</c>）。</summary>
    [JsonPropertyName("ip")]
    public string? Ip { get; set; }

    /// <summary>ping 源头的运营商，由请求中的 <c>check_operator</c> 控制。</summary>
    [JsonPropertyName("from_operator")]
    public string? FromOperator { get; set; }

    /// <summary>
    /// ping 的丢包率。
    /// </summary>
    /// <remarks>官方仅发送一个 ping 包，故取值只有 <c>0%</c> 与 <c>100%</c> 两种，不宜作为精细质量指标。</remarks>
    [JsonPropertyName("package_loss")]
    public string? PackageLoss { get; set; }

    /// <summary>
    /// ping 耗时（示例形如 <c>23.079ms</c>）。
    /// </summary>
    /// <remarks>
    /// <b>官方参数表未定义本字段</b>（仅出现在官方返回示例中）；建模为可空字符串以免官方补录时破坏反序列化。
    /// </remarks>
    [JsonPropertyName("time")]
    public string? Time { get; set; }
}
