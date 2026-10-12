// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

namespace Mud.Wechat.OfficialAccount.DataModels.Menu;

/// <summary>
/// 测试个性化菜单匹配结果请求体（<c>tryMatchMenu</c>，<c>POST /cgi-bin/menu/trymatch</c>）。
/// </summary>
/// <remarks>
/// 官方字段表：<c>user_id</c>（必填，<b>用户 OpenID 或微信号</b>）。
/// 官方「注意事项」原文：①「每日测试限制 <b>20000</b> 次」；②「<b>包含已废除字段的菜单也将自动失效</b>，
/// 不再被匹配。这一点也将体现在本测试接口中」。
/// </remarks>
[HttpJsonSerializable(SerializerClassName = "Menu")]
public class MpTryMatchMenuRequest
{
    /// <summary>用户 OpenID 或微信号。</summary>
    [JsonPropertyName("user_id")]
    public string UserId { get; set; } = string.Empty;
}
