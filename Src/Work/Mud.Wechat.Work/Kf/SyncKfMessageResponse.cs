// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

namespace Mud.Wechat.Work.DataModels.Kf;

/// <summary>
/// 同步消息响应体（<c>/cgi-bin/kf/sync_msg</c>）。
/// </summary>
/// <remarks>
/// <para>
/// 一次调用返回一页消息列表，须按 <see cref="HasMore"/> 与 <see cref="NextCursor"/>
/// 续页拉取，直到 <see cref="HasMore"/> 为 0。
/// </para>
/// <para>
/// 官方业务限制（94670 正文原文）：<b>不能通过判断 msg_list 是否为空来停止拉取</b>——空列表只表示
/// 本页没有消息，翻页终止的唯一判据是 <see cref="HasMore"/> 为 0；官方同时「强烈建议」把
/// <see cref="NextCursor"/> 入库保存，以便异常后从上次位置续拉。
/// </para>
/// </remarks>
[HttpJsonSerializable(SerializerClassName = "Kf")]
public class SyncKfMessageResponse : WechatWorkResponse
{
    /// <summary>
    /// 获取或设置本次同步返回的消息列表。
    /// </summary>
    [JsonPropertyName("msg_list")]
    public List<KfSyncMessage>? MessageList { get; set; }

    /// <summary>
    /// 获取或设置是否仍有未拉取的消息（非 0 时须以 <see cref="NextCursor"/> 继续请求；<b>官方以数字 0/1 承载布尔语义</b>，本仓无数值-布尔转换器，故原样以 <c>int</c> 承载）。
    /// </summary>
    [JsonPropertyName("has_more")]
    public int? HasMore { get; set; }

    /// <summary>
    /// 获取或设置下一页的翻页标记（回填至下一次请求的 cursor）。
    /// </summary>
    [JsonPropertyName("next_cursor")]
    public string? NextCursor { get; set; }
}
