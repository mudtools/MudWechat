// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

namespace Mud.Wechat.OfficialAccount.DataModels.Menu;

/// <summary>
/// 创建自定义菜单请求体（<c>createCustomMenu</c>，<c>POST /cgi-bin/menu/create</c>）。
/// </summary>
/// <remarks>
/// 官方「注意事项」原文（**硬约束**）：①自定义菜单<b>最多 3 个一级菜单</b>，每个一级菜单<b>最多 5 个二级菜单</b>；
/// ②一级菜单最多 4 个汉字、二级最多 8 个汉字，超出以 <c>...</c> 代替；③创建后客户端菜单刷新策略为
/// 「用户进入会话页/profile 页时，若上次拉取菜单已超过 5 分钟则重新拉取，有更新才刷新」。
/// </remarks>
[HttpJsonSerializable(SerializerClassName = "Menu")]
public class MpCreateMenuRequest
{
    /// <summary>一级菜单数组（官方：<b>1~3 个</b>；每个一级最多含 5 个二级）。</summary>
    [JsonPropertyName("button")]
    public List<MpMenuButton> Button { get; set; } = new();
}
