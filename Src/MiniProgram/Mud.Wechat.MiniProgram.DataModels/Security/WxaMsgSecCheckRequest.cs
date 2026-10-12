// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

namespace Mud.Wechat.MiniProgram.DataModels.Security;

/// <summary>文本内容安全识别请求体（<c>POST /wxa/msg_sec_check</c>）。</summary>
[HttpJsonSerializable(SerializerClassName = "Security")]
public class WxaMsgSecCheckRequest
{
    /// <summary>待检测文本（<c>content</c>，必填；长度上限以官方页面为准，SDK 不做本地校验）。</summary>
    [JsonPropertyName("content")]
    public string? Content { get; set; }

    /// <summary>接口版本（<c>version</c>，选填；当前为 <c>2</c>）。</summary>
    [JsonPropertyName("version")]
    public long? Version { get; set; }

    /// <summary>
    /// 场景值（<c>scene</c>，选填）：<c>1</c> 资料 / <c>2</c> 评论 / <c>3</c> 论坛 / <c>4</c> 社交日志。
    /// </summary>
    [JsonPropertyName("scene")]
    public long? Scene { get; set; }

    /// <summary>用户 <c>openid</c>（<c>openid</c>，选填；同用户累积违规会触发封禁策略）。</summary>
    [JsonPropertyName("openid")]
    public string? OpenId { get; set; }
}
