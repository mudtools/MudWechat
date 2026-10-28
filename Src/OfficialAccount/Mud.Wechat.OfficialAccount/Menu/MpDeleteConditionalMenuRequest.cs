// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

namespace Mud.Wechat.OfficialAccount.DataModels.Menu;

/// <summary>
/// 删除个性化菜单请求体（<c>deleteConditionalMenu</c>，<c>POST /cgi-bin/menu/delconditional</c>）。
/// </summary>
/// <remarks>官方字段表：<c>menuid</c>（必填，菜单 ID，由创建个性化菜单响应下发）。</remarks>
[HttpJsonSerializable(SerializerClassName = "Menu")]
public class MpDeleteConditionalMenuRequest
{
    /// <summary>菜单 ID（创建个性化菜单响应中的 <c>menuid</c>）。</summary>
    [JsonPropertyName("menuid")]
    public string MenuId { get; set; } = string.Empty;
}
