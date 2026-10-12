// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

namespace Mud.Wechat.OfficialAccount.DataModels.SmartApi;

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
