// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

using System.Text.Json;

namespace Mud.Wechat.MiniProgram.DataModels.Operation;

/// <summary>
/// 运维日志类应答基底（<c>data</c> 为<b>逐项键值对</b>列表透传）。
/// </summary>
/// <remarks>
/// <para>承载性能（<c>get_performance</c>）、访问来源（<c>get_scene</c>）、客户端版本（<c>get_client_version</c>）
/// 三类端点的 <c>data</c> 列表 —— 官方各「项」字段随版本演进且互不一致，SDK 不猜测字段名，
/// 以原始键值对透传（元素即官方数据项，键名照官方原文）。</para>
/// </remarks>
[HttpJsonSerializable(SerializerClassName = "Operation")]
public class WxaLogDataListResponse : WxaResponse
{
    /// <summary>数据项列表（<c>data</c>），每项为官方原始键值对。</summary>
    [JsonPropertyName("data")]
    public List<Dictionary<string, JsonElement>>? Data { get; set; }
}
