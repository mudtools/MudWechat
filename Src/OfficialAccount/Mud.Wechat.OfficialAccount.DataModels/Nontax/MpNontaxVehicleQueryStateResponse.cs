// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

namespace Mud.Wechat.OfficialAccount.DataModels.Nontax;

/// <summary>
/// 「查询车辆状态」响应。
/// </summary>
[HttpJsonSerializable(SerializerClassName = "Nontax")]
public class MpNontaxVehicleQueryStateResponse : MpResponse
{
    /// <summary>获取或设置用户状态（官方 <c>user_state</c>）。</summary>
    [JsonPropertyName("user_state")]
    public string? UserState { get; set; }

    /// <summary>获取或设置用户 OpenId（官方 <c>openid</c>，可选）。</summary>
    [JsonPropertyName("openid")]
    public string? OpenId { get; set; }

    /// <summary>获取或设置跳转路径（官方 <c>path</c>，可选）。</summary>
    [JsonPropertyName("path")]
    public string? Path { get; set; }

    /// <summary>获取或设置车牌信息列表（官方 <c>plate_number_info</c>，可选）。</summary>
    [JsonPropertyName("plate_number_info")]
    public string[]? PlateNumberInfo { get; set; }

    /// <summary>获取或设置授权信息（官方 <c>authorize_package</c>；键值对形态）。</summary>
    [JsonPropertyName("authorize_package")]
    public Dictionary<string, string>? AuthorizePackage { get; set; }
}
