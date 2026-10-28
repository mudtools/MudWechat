// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

namespace Mud.Wechat.OfficialAccount.DataModels.Mass;

/// <summary>卡券属性（官方 <c>card_ext</c>，仅 preview 页 wxcard 分支出现）。</summary>
[HttpJsonSerializable(SerializerClassName = "Mass")]
public class MpMassWxCardExt
{
    /// <summary>获取或设置卡券 code（官方 <c>code</c>）。</summary>
    [JsonPropertyName("code")]
    public string? Code { get; set; }

    /// <summary>获取或设置用户 openid（官方 <c>openid</c>）。</summary>
    [JsonPropertyName("openid")]
    public string? OpenId { get; set; }

    /// <summary>获取或设置时间戳（官方 <c>timestamp</c>）。</summary>
    [JsonPropertyName("timestamp")]
    public string? Timestamp { get; set; }

    /// <summary>获取或设置签名（官方 <c>signature</c>）。</summary>
    [JsonPropertyName("signature")]
    public string? Signature { get; set; }
}
