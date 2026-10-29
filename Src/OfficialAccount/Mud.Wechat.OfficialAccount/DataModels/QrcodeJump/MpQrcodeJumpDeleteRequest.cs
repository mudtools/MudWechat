// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

namespace Mud.Wechat.OfficialAccount.DataModels.QrcodeJump;

/// <summary>
/// 删除已设置的二维码规则（<c>POST /cgi-bin/wxopen/qrcodejumpdelete</c>）请求体。
/// </summary>
/// <remarks>官方契约：<c>prefix</c> 必填；<c>appid</c> 为删除「扫服务号二维码打开小程序」规则时的必传项。</remarks>
[HttpJsonSerializable(SerializerClassName = "QrcodeJump")]
public class MpQrcodeJumpDeleteRequest
{
    /// <summary>获取或设置二维码规则（官方 <c>prefix</c>，必填）。</summary>
    [JsonPropertyName("prefix")]
    public string Prefix { get; set; } = string.Empty;

    /// <summary>获取或设置小程序 appid（官方 <c>appid</c>；删除「扫服务号二维码」规则时需传）。</summary>
    [JsonPropertyName("appid")]
    public string? AppId { get; set; }
}
