// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

namespace Mud.Wechat.OfficialAccount.DataModels.Menu;

/// <summary>
/// 创建个性化菜单响应（<c>addConditionalMenu</c>，<c>POST /cgi-bin/menu/addconditional</c>）。
/// </summary>
/// <remarks>
/// 官方响应：<c>menuid</c>（菜单 ID，<b>字符串</b>，示例 <c>208379533</c>）+ <c>errcode</c>/<c>errmsg</c>。
/// </remarks>
[HttpJsonSerializable(SerializerClassName = "Menu")]
public class MpAddConditionalMenuResponse : MpResponse
{
    /// <summary>个性化菜单 ID（后续删除个性化菜单须回传本值）。</summary>
    [JsonPropertyName("menuid")]
    public string? MenuId { get; set; }
}
