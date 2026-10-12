// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

namespace Mud.Wechat.OfficialAccount.DataModels.Mass;

/// <summary>删除群发消息（<c>message/mass/delete</c>）请求体。</summary>
/// <remarks>
/// <para>
/// 官方契约（逐页核验 2026-10-07）：<c>msg_id</c>/<c>url</c> 必须有一个传值（都有值时仅 msg_id 有效）；
/// <c>article_idx</c> 仅在 msg_id 有值时生效。
/// </para>
/// <para>
/// <b>可删除条件（官方「注意事项」原文）</b>：①「只有通过 api 发送的并且已经发送成功的消息才能删除」；
/// ②「删除消息是将消息的正文内容失效，已经收到的用户，还是能在其本地看到消息卡片」；
/// ③「<b>删除群发消息只能删除文章和视频消息</b>，其他类型的消息一经发送，无法删除」；
/// ④「如果多次群发发送的是一个文章，那么删除其中一次群发，就会删除掉这个内容，导致所有群发都失效」。
/// </para>
/// <para>
/// <b>官方文档矛盾（照录）</b>：<c>article_idx</c> 必填列标「是」但说明写「该字段不填或填 0 会删除全部内容」。
/// </para>
/// </remarks>
[HttpJsonSerializable(SerializerClassName = "Mass")]
public class MpMassDeleteRequest
{
    /// <summary>获取或设置发送出去的消息 ID（官方 <c>msg_id</c>，number 形态；与 url 二选一，都有值时仅 msg_id 有效）。</summary>
    [JsonPropertyName("msg_id")]
    public long? MsgId { get; set; }

    /// <summary>获取或设置要删除的内容位置（官方 <c>article_idx</c>，第一篇编号为 1；不填或填 0 删除全部——官方必填标注矛盾，照录）。</summary>
    [JsonPropertyName("article_idx")]
    public int ArticleIdx { get; set; }

    /// <summary>获取或设置要删除的内容 url（官方 <c>url</c>；仅 msg_id 未指定时生效）。</summary>
    [JsonPropertyName("url")]
    public string? Url { get; set; }
}
