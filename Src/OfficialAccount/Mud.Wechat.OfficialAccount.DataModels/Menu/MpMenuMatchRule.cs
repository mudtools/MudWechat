// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

namespace Mud.Wechat.OfficialAccount.DataModels.Menu;

/// <summary>
/// 个性化菜单匹配规则（<c>matchrule</c>）；官方要求<b>至少一个非空字段</b>。
/// </summary>
/// <remarks>
/// 官方「注意事项」：个性化菜单支持用户标签，当用户身上的标签<b>超过 1 个</b>时，
/// <b>以最后打上的标签</b>为匹配依据；匹配规则包含隐私字段会被拒（错误码 <c>65320 match rule violates privacy</c>）。
/// </remarks>
[HttpJsonSerializable(SerializerClassName = "Menu")]
public class MpMenuMatchRule
{
    /// <summary>用户标签的 id（可通过用户标签管理接口获取）。</summary>
    [JsonPropertyName("tag_id")]
    public string? TagId { get; set; }

    /// <summary>客户端版本，当前只具体到系统型号：<c>IOS(1)</c> / <c>Android(2)</c> / <c>Others(3)</c>；不填则不做匹配。</summary>
    [JsonPropertyName("client_platform_type")]
    public string? ClientPlatformType { get; set; }
}
