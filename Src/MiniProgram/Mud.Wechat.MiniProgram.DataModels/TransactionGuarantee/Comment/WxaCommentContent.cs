// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

namespace Mud.Wechat.MiniProgram.DataModels.TransactionGuarantee;

/// <summary>
/// 评价内容（<c>content</c>，官方原文「视频跟图片只能存在一种，不同时存在；图片可多张，视频只有一个」）。
/// </summary>
/// <remarks>
/// <para>
/// <b>共用裁决</b>：列表侧 <c>commentList[].content</c> 与详情侧 <c>info.content.content</c> 参数表
/// 均为 <c>{ txt, media[] }</c> ⇒ 共用本类型。改评前的旧评价内容是<b>另一套字段</b>（<c>ext</c> 而非 <c>txt</c>），
/// 见 <see cref="WxaCommentOldContent"/>，不得并入本类型。
/// </para>
/// </remarks>
[HttpJsonSerializable(SerializerClassName = "TransactionGuarantee")]
public class WxaCommentContent
{
    /// <summary>评价内容文本（<c>txt</c>，官方字段名为 <c>txt</c> 而非 <c>content</c>）。</summary>
    [JsonPropertyName("txt")]
    public string? Txt { get; set; }

    /// <summary>评价的媒体文件列表（<c>media</c>，图片 / 视频二选一），见 <see cref="WxaCommentMedia"/>。</summary>
    [JsonPropertyName("media")]
    public List<WxaCommentMedia>? Media { get; set; }
}
