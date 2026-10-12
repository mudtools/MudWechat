// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

namespace Mud.Wechat.OfficialAccount.DataModels.Template;

/// <summary>查询拦截的模板消息（<c>wxa/sec/queryblocktmplmsg</c>）请求体。</summary>
/// <remarks>
/// 官方契约（逐页核验 2026-10-07）：<c>tmpl_msg_id</c>/<c>largest_id</c>/<c>limit</c> 三字段均必填；
/// <c>largest_id</c> 为上一页查询结果最大的 id（翻页游标，第一次传 0）；<c>limit</c> 单页最大 <b>100</b>。
/// </remarks>
[HttpJsonSerializable(SerializerClassName = "Template")]
public class MpQueryBlockTmplMsgRequest
{
    /// <summary>获取或设置被拦截的模板消息 id（官方 <c>tmpl_msg_id</c>，必填）。</summary>
    [JsonPropertyName("tmpl_msg_id")]
    public string TmplMsgId { get; set; } = string.Empty;

    /// <summary>获取或设置翻页游标（官方 <c>largest_id</c>，上一页查询结果最大的 id；第一次传 0）。</summary>
    [JsonPropertyName("largest_id")]
    public long LargestId { get; set; }

    /// <summary>获取或设置单页查询大小（官方 <c>limit</c>，最大 100）。</summary>
    [JsonPropertyName("limit")]
    public int Limit { get; set; }
}
