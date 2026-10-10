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

/// <summary>文本内容安全识别应答（<c>POST /wxa/msg_sec_check</c>）。</summary>
/// <remarks>
/// <b>勿据 <c>errcode</c> 判合规</b>：<c>errcode = 0</c> 只表示调用成功，违规与否在
/// <see cref="Result"/>（<c>suggest</c>）与 <see cref="Detail"/> 中。
/// </remarks>
[HttpJsonSerializable(SerializerClassName = "Security")]
public class WxaMsgSecCheckResponse : WxaResponse
{
    /// <summary>详细命中信息（<c>detail</c>，数组），见 <see cref="WxaSecCheckDetail"/>。</summary>
    [JsonPropertyName("detail")]
    public List<WxaSecCheckDetail>? Detail { get; set; }

    /// <summary>唯一请求标识（<c>trace_id</c>，排障与官方工单用）。</summary>
    [JsonPropertyName("trace_id")]
    public string? TraceId { get; set; }

    /// <summary>综合结论（<c>result</c>），见 <see cref="WxaSecCheckResult"/>。</summary>
    [JsonPropertyName("result")]
    public WxaSecCheckResult? Result { get; set; }
}

/// <summary>内容安全综合结论（<c>result</c>）。</summary>
[HttpJsonSerializable(SerializerClassName = "Security")]
public class WxaSecCheckResult
{
    /// <summary>建议（<c>suggest</c>）：<c>risky</c>（违规）/ <c>pass</c>（通过）/ <c>review</c>（需人工复核）。</summary>
    [JsonPropertyName("suggest")]
    public string? Suggest { get; set; }

    /// <summary>命中标签（<c>label</c>，风险分类枚举值）。</summary>
    [JsonPropertyName("label")]
    public long? Label { get; set; }
}

/// <summary>命中明细（<c>detail[]</c>）。</summary>
[HttpJsonSerializable(SerializerClassName = "Security")]
public class WxaSecCheckDetail
{
    /// <summary>策略（<c>strategy</c>）：命中策略的名称（errcode 为 87014 时给出具体命中策略与子标签）。</summary>
    [JsonPropertyName("strategy")]
    public string? Strategy { get; set; }

    /// <summary>策略下错误码（<c>errcode</c>）。</summary>
    [JsonPropertyName("errcode")]
    public long? ErrorCode { get; set; }

    /// <summary>建议（<c>suggest</c>）。</summary>
    [JsonPropertyName("suggest")]
    public string? Suggest { get; set; }

    /// <summary>命中标签（<c>label</c>）。</summary>
    [JsonPropertyName("label")]
    public long? Label { get; set; }

    /// <summary>命中关键词（<c>keyword</c>）。</summary>
    [JsonPropertyName("keyword")]
    public string? Keyword { get; set; }

    /// <summary>置信度（<c>prob</c>）。</summary>
    [JsonPropertyName("prob")]
    public double? Probability { get; set; }
}

/// <summary>音视频内容安全识别请求体（<c>POST /wxa/media_check_async</c>）。</summary>
/// <remarks><c>media_url</c> 须为<b>公网可访问</b>地址（官方回源拉取）。</remarks>
[HttpJsonSerializable(SerializerClassName = "Security")]
public class WxaMediaCheckAsyncRequest
{
    /// <summary>要检测的媒体地址（<c>media_url</c>，必填；公网可访问、无需鉴权）。</summary>
    [JsonPropertyName("media_url")]
    public string? MediaUrl { get; set; }

    /// <summary>媒体类型（<c>media_type</c>，必填）：<c>1</c> 音频 / <c>2</c> 图片（官方枚举）。</summary>
    [JsonPropertyName("media_type")]
    public long? MediaType { get; set; }

    /// <summary>接口版本（<c>version</c>，选填；当前为 <c>2</c>）。</summary>
    [JsonPropertyName("version")]
    public long? Version { get; set; }

    /// <summary>场景值（<c>scene</c>，选填，取值同文本审核）。</summary>
    [JsonPropertyName("scene")]
    public long? Scene { get; set; }

    /// <summary>用户 <c>openid</c>（<c>openid</c>，选填）。</summary>
    [JsonPropertyName("openid")]
    public string? OpenId { get; set; }
}

/// <summary>音视频内容安全识别应答（<b>仅受理，不含结论</b>）。</summary>
[HttpJsonSerializable(SerializerClassName = "Security")]
public class WxaMediaCheckAsyncResponse : WxaResponse
{
    /// <summary>唯一请求标识（<c>trace_id</c>）—— 结论经官方回调送达时的对账凭据。</summary>
    [JsonPropertyName("trace_id")]
    public string? TraceId { get; set; }
}
