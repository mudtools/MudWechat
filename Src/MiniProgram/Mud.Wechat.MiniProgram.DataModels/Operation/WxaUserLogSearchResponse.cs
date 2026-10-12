// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

using System.Text.Json;

namespace Mud.Wechat.MiniProgram.DataModels.Operation;

/// <summary>查询实时日志应答（<c>numbers</c> + <c>datas</c>）。</summary>
/// <remarks>官方文档：<c>operation/api_realtimelogsearch.html</c>；<c>datas</c> 为原始日志行数组。</remarks>
[HttpJsonSerializable(SerializerClassName = "Operation")]
public class WxaUserLogSearchResponse : WxaResponse
{
    /// <summary>本次返回的数据条数（<c>numbers</c>）。</summary>
    [JsonPropertyName("numbers")]
    public long? Numbers { get; set; }

    /// <summary>日志数据（<c>datas</c>，原始日志行数组）。</summary>
    [JsonPropertyName("datas")]
    public List<string>? Datas { get; set; }
}
