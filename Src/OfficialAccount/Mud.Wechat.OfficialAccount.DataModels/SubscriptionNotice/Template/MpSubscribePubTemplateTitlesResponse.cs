// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

namespace Mud.Wechat.OfficialAccount.DataModels.SubscriptionNotice;

/// <summary>获取类目下的公共模板（<c>wxaapi/newtmpl/getpubtemplatetitles</c>）响应。</summary>
/// <remarks>
/// <para>
/// 官方契约（逐页核验 2026-10-07）：<b>GET</b>，Query <c>ids</c>（类目 id，多个用逗号隔开）/
/// <c>start</c>（分页，从 0 开始）/<c>limit</c>（单页条数，<b>最大 30</b>）。
/// </para>
/// <para>
/// <b>官方文档矛盾（照录）</b>：<c>categoryId</c> 类型表标 number、返回示例为字符串 "616"
/// ⇒ 按示例字符串形态建模（数字形态反序列化由调用方经原始响应处理——官方示例为可信形态）；
/// 请求示例 URL 中 ids 带字面双引号（官方页面原文如此）。
/// </para>
/// </remarks>
[HttpJsonSerializable(SerializerClassName = "SubscriptionNotice")]
public class MpSubscribePubTemplateTitlesResponse : MpResponse
{
    /// <summary>获取或设置模版标题列表总数（官方 <c>count</c>）。</summary>
    [JsonPropertyName("count")]
    public int Count { get; set; }

    /// <summary>获取或设置模板标题列表（官方 <c>data</c>）。</summary>
    [JsonPropertyName("data")]
    public List<MpSubscribePubTemplateTitle>? Data { get; set; }
}
