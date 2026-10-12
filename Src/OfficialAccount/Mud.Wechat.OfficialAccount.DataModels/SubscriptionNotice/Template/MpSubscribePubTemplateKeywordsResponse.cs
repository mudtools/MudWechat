// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

namespace Mud.Wechat.OfficialAccount.DataModels.SubscriptionNotice;

/// <summary>获取模板中的关键词（<c>wxaapi/newtmpl/getpubtemplatekeywords</c>）响应。</summary>
/// <remarks>
/// 官方契约（逐页核验 2026-10-07）：<b>GET</b>，Query <c>tid</c>（模板标题 id）；
/// 顶层 <c>count</c> 的官方说明原文为「模版标题列表总数」——系从 titles 接口复制来的文案
/// （本接口返回的是关键词列表），矛盾照录。
/// </remarks>
[HttpJsonSerializable(SerializerClassName = "SubscriptionNotice")]
public class MpSubscribePubTemplateKeywordsResponse : MpResponse
{
    /// <summary>获取或设置关键词列表总数（官方 <c>count</c>；官方说明文案与实际语义不符，照录）。</summary>
    [JsonPropertyName("count")]
    public int Count { get; set; }

    /// <summary>获取或设置关键词列表（官方 <c>data</c>）。</summary>
    [JsonPropertyName("data")]
    public List<MpSubscribePubTemplateKeyword>? Data { get; set; }
}
