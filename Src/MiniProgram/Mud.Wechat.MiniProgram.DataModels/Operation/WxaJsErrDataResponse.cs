// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

using System.Text.Json;

namespace Mud.Wechat.MiniProgram.DataModels.Operation;

/// <summary>JS 错误详情 / 列表应答基底（<c>data</c> 原始透传）。</summary>
/// <remarks>官方 <c>jserr_detail</c> / <c>jserr_list</c> 的 <c>data</c> 结构随版本演进，SDK 不猜测字段名、以原始键值对透传。</remarks>
[HttpJsonSerializable(SerializerClassName = "Operation")]
public class WxaJsErrDataResponse : WxaResponse
{
    /// <summary>错误数据（<c>data</c>，官方原始键值对）。</summary>
    [JsonPropertyName("data")]
    public Dictionary<string, JsonElement>? Data { get; set; }
}
