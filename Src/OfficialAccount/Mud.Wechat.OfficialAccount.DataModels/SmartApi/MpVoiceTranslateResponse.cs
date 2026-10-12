// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

namespace Mud.Wechat.OfficialAccount.DataModels.SmartApi;

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
