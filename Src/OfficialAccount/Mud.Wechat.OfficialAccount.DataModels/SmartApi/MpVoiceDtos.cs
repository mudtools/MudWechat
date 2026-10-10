// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

namespace Mud.Wechat.OfficialAccount.DataModels.SmartApi;

/// <summary>
/// 获取语音识别结果（<c>POST /cgi-bin/media/voice/queryrecoresultfortext</c>）响应。
/// </summary>
/// <remarks>
/// <para>
/// 官方字段表（逐页核验 2026-10-07）：仅 <c>result</c>（识别结果）。
/// </para>
/// <para>
/// <b>异步轮询语义（官方注意事项原文，SDK 不编排）</b>：「<b>添加完文件之后 10s 内调用这个接口</b>」
/// ——上传语音（<c>addvoicetorecofortext</c>）后须在 10 秒内以同一 <c>voice_id</c> 查询；
/// 轮询次数与退避策略归宿主（「SDK 不做业务编排」红线）。
/// </para>
/// <para>官方错误码：<c>-1</c> / <c>40001</c>（本页错误码表仅此两行，照录）。</para>
/// </remarks>
[HttpJsonSerializable(SerializerClassName = "SmartApi")]
public class MpVoiceRecoResultResponse : MpResponse
{
    /// <summary>获取或设置识别结果（官方 <c>result</c>）。</summary>
    [JsonPropertyName("result")]
    public string? Result { get; set; }
}

/// <summary>
/// 微信翻译（<c>POST /cgi-bin/media/voice/translatecontent</c>）请求体。
/// </summary>
/// <remarks>
/// <para>
/// 官方契约（逐页核验 2026-10-07）：<c>lfrom</c>/<c>lto</c> 为 <b>Query</b> 参数（非请求体），
/// 请求体仅 <c>content</c>（源内容，utf8 格式，<b>最大 600Byte</b>）。
/// </para>
/// <para>
/// <b>路径语义异常（官方现状，照录）</b>：本接口为「文本内容翻译」，但路径位于
/// <c>/cgi-bin/media/voice/</c> 下（voice 段语义与文本翻译不符）——SDK 照抄官方路径，不做「纠正」。
/// </para>
/// <para>
/// <b>语言枚举仅两值</b>：<c>zh_CN</c> / <c>en_US</c>（见 <c>MpVoiceLanguages</c>）。
/// </para>
/// </remarks>
[HttpJsonSerializable(SerializerClassName = "SmartApi")]
public class MpVoiceTranslateRequest
{
    /// <summary>获取或设置源内容（官方 <c>content</c>；utf8 格式，最大 600Byte；SDK 不做本地长度拦截）。</summary>
    [JsonPropertyName("content")]
    public string Content { get; set; } = string.Empty;
}

/// <summary>
/// 微信翻译（<c>POST /cgi-bin/media/voice/translatecontent</c>）响应。
/// </summary>
/// <remarks>
/// <para>
/// 官方字段表（逐页核验 2026-10-07）：<c>from_content</c>（原文内容）/ <c>to_content</c>（译文内容）
/// ——响应<b>不含 errcode/errmsg</b>（失败时官方以 <c>errcode</c> 表达，故仍继承 <see cref="MpResponse"/>
/// 以兼容错误形态）。
/// </para>
/// <para>官方错误码：<c>40001</c> / <c>40035</c>（invalid args size，不合法的参数）。</para>
/// </remarks>
[HttpJsonSerializable(SerializerClassName = "SmartApi")]
public class MpVoiceTranslateResponse : MpResponse
{
    /// <summary>获取或设置原文内容（官方 <c>from_content</c>）。</summary>
    [JsonPropertyName("from_content")]
    public string? FromContent { get; set; }

    /// <summary>获取或设置译文内容（官方 <c>to_content</c>）。</summary>
    [JsonPropertyName("to_content")]
    public string? ToContent { get; set; }
}
