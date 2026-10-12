// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

namespace Mud.Wechat.OfficialAccount.DataModels.QrcodeJump;

/// <summary>
/// 二维码规则条目（官方 <c>rule_list[]</c>）。
/// </summary>
/// <remarks>
/// <para>
/// <b>条件返回（官方原文）</b>：<c>open_version</c> 与 <c>debug_url</c>
/// <b>仅「扫普通二维码打开小程序」场景返回</b>；服务号场景不返回 ⇒ 两者可空。
/// </para>
/// <para>
/// <b>官方文档矛盾（照录）</b>：<c>prefix</c> 字段说明写「服务号二维码规则已过滤不展示」，
/// 但服务号返回示例中实际返回了 <c>prefix</c> ⇒ SDK 保留该字段。
/// </para>
/// </remarks>
[HttpJsonSerializable(SerializerClassName = "QrcodeJump")]
public class MpQrcodeJumpRule
{
    /// <summary>获取或设置二维码规则（官方 <c>prefix</c>；普通二维码为规则前缀，服务号为带参二维码 url）。</summary>
    [JsonPropertyName("prefix")]
    public string? Prefix { get; set; }

    /// <summary>获取或设置小程序功能页面（官方 <c>path</c>）。</summary>
    [JsonPropertyName("path")]
    public string? Path { get; set; }

    /// <summary>获取或设置发布标志位（官方 <c>state</c>，<c>1</c> = 未发布 / <c>2</c> = 已发布；见 <c>MpQrcodeJumpStates</c>）。</summary>
    [JsonPropertyName("state")]
    public int? State { get; set; }

    /// <summary>获取或设置测试范围（官方 <c>open_version</c>；<b>仅普通二维码场景返回</b>，取值见 <c>MpQrcodeJumpOpenVersions</c>）。</summary>
    [JsonPropertyName("open_version")]
    public int? OpenVersion { get; set; }

    /// <summary>获取或设置测试链接（官方 <c>debug_url</c>，<b>最多 5 个</b>；<b>仅普通二维码场景返回</b>）。</summary>
    [JsonPropertyName("debug_url")]
    public List<string>? DebugUrls { get; set; }
}
