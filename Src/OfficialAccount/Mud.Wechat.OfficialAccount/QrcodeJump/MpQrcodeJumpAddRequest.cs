// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

namespace Mud.Wechat.OfficialAccount.DataModels.QrcodeJump;

/// <summary>
/// 增加或修改二维码规则（<c>POST /cgi-bin/wxopen/qrcodejumpadd</c>）请求体。
/// </summary>
/// <remarks>
/// <para>
/// <b>场景分流（官方原文，「扫普通二维码」与「扫服务号二维码」两族参数）</b>：
/// </para>
/// <list type="bullet">
/// <item><b>两场景都需要</b>：<c>prefix</c> / <c>path</c> / <c>is_edit</c>。</item>
/// <item><b>仅「扫服务号二维码」</b>：<c>appid</c>（跳转的小程序 appid）。</item>
/// <item><b>仅「扫普通二维码」</b>：<c>open_version</c> / <c>debug_url</c> / <c>permit_sub_rule</c>。</item>
/// </list>
/// <para>
/// <b>官方文档矛盾（照录）</b>：字段表把 <c>appid</c>/<c>open_version</c>/<c>permit_sub_rule</c> 均标「必填」，
/// 但说明文字限定为上述场景专属，且各场景示例均未携带对方参数 ⇒ SDK 以可空超集承载，
/// 由调用方按场景携带（不本地校验）。
/// </para>
/// <para>
/// 官方原文：<b>「已经发布的规则，不支持修改」</b>（<c>is_edit = 1</c> 仅可改未发布规则）。
/// </para>
/// </remarks>
[HttpJsonSerializable(SerializerClassName = "QrcodeJump")]
public class MpQrcodeJumpAddRequest
{
    /// <summary>获取或设置二维码规则（官方 <c>prefix</c>，必填；如 <c>http://weixin.qq.com/q/kZgfwMTm72Wxxxx</c>）。</summary>
    [JsonPropertyName("prefix")]
    public string Prefix { get; set; } = string.Empty;

    /// <summary>获取或设置小程序 appid（官方 <c>appid</c>；仅「扫服务号二维码」场景需传）。</summary>
    [JsonPropertyName("appid")]
    public string? AppId { get; set; }

    /// <summary>获取或设置小程序功能页面（官方 <c>path</c>，必填；如 <c>pages/index/index</c>）。</summary>
    [JsonPropertyName("path")]
    public string Path { get; set; } = string.Empty;

    /// <summary>获取或设置编辑标志位（官方 <c>is_edit</c>，必填；<c>0</c> 新增 / <c>1</c> 修改已有规则，见 <c>MpQrcodeJumpEditFlags</c>）。</summary>
    [JsonPropertyName("is_edit")]
    public int IsEdit { get; set; }

    /// <summary>获取或设置测试范围（官方 <c>open_version</c>；仅「扫普通二维码」场景需传，取值见 <c>MpQrcodeJumpOpenVersions</c>）。</summary>
    [JsonPropertyName("open_version")]
    public int? OpenVersion { get; set; }

    /// <summary>获取或设置测试链接（官方 <c>debug_url</c>，<b>至多 5 个</b>；仅「扫普通二维码」场景需传）。</summary>
    [JsonPropertyName("debug_url")]
    public List<string>? DebugUrls { get; set; }

    /// <summary>获取或设置是否独占子规则（官方 <c>permit_sub_rule</c>；仅「扫普通二维码」场景需传，
    /// <c>1</c> = 不占用 / <c>2</c> = 占用，见 <c>MpQrcodeJumpSubRuleModes</c>）。</summary>
    [JsonPropertyName("permit_sub_rule")]
    public int? PermitSubRule { get; set; }
}
