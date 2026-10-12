// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

using System.Text.Json;

namespace Mud.Wechat.MiniProgram.DataModels.Operation;

/// <summary>获取用户反馈列表请求体（<c>POST /wxaapi/feedback/list</c>）。</summary>
/// <remarks>官方文档：<c>operation/api_getfeedback.html</c>。<c>type</c> 选填（不传则查全部）。</remarks>
[HttpJsonSerializable(SerializerClassName = "Operation")]
public class WxaFeedbackListRequest
{
    /// <summary>起始位置（<c>page</c>，从 <c>0</c> 开始，必填）。</summary>
    [JsonPropertyName("page")]
    public long? Page { get; set; }

    /// <summary>返回条数（<c>limit</c>，必填；上限以官方页面为准）。</summary>
    [JsonPropertyName("limit")]
    public long? Limit { get; set; }

    /// <summary>反馈类型（<c>type</c>，选填）：<c>1</c> 无法打开 / <c>2</c> 一直闪退 / <c>3</c> 一直卡死 / <c>4</c> 其他问题。</summary>
    [JsonPropertyName("type")]
    public long? Type { get; set; }
}
