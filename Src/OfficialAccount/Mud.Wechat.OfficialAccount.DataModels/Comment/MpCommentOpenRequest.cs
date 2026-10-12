// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

namespace Mud.Wechat.OfficialAccount.DataModels.Comment;

/// <summary>
/// 打开 / 关闭文章评论的请求体（<c>comment/open</c> 与 <c>comment/close</c> 请求体字段集一致 ⇒ 共用）。
/// </summary>
/// <remarks>
/// <para>
/// <b>共用裁决（N4：字段集一致才共用，由守卫双向锁定）</b>：open 与 close 两页请求体均为
/// <c>{msg_data_id, index}</c>（index 否——多图文时指定第几篇，从 0 开始，不带默认第一篇）。
/// </para>
/// <para><b>前置条件</b>：公众号需具备<b>留言功能权限</b>（官方原文；无权限返回 88000）。</para>
/// </remarks>
[HttpJsonSerializable(SerializerClassName = "Comment")]
public class MpCommentOpenRequest
{
    /// <summary>获取或设置群发返回的 msg_data_id（官方 <c>msg_data_id</c>，必填）。</summary>
    [JsonPropertyName("msg_data_id")]
    public long MsgDataId { get; set; }

    /// <summary>获取或设置多图文序号（官方 <c>index</c>，从 0 开始；不带默认操作第一篇）。</summary>
    [JsonPropertyName("index")]
    public int? Index { get; set; }
}
