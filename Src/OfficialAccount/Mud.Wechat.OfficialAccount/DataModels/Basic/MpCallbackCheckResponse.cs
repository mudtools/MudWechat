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
