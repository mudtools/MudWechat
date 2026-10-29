// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

namespace Mud.Wechat.OfficialAccount.DataModels.Menu;

/// <summary>
/// 获取自定义菜单配置响应（<c>getMenu</c>，<c>GET /cgi-bin/menu/get</c>）。
/// </summary>
/// <remarks>
/// <para>
/// 官方响应：<c>menu</c>（默认菜单）/ <c>conditionalmenu</c>（个性化菜单列表，元素含 <c>button</c> 与
/// <c>matchrule</c>）。
/// </para>
/// <para>
/// 官方「注意事项」原文：「在设置了个性化菜单后，使用本自定义菜单查询接口可以获取<b>默认菜单和全部个性化菜单</b>信息」。
/// </para>
/// </remarks>
[HttpJsonSerializable(SerializerClassName = "Menu")]
public class MpGetMenuResponse : MpResponse
{
    /// <summary>默认菜单（未创建默认菜单时可能缺省）。</summary>
    [JsonPropertyName("menu")]
    public MpMenu? Menu { get; set; }

    /// <summary>个性化菜单列表。</summary>
    [JsonPropertyName("conditionalmenu")]
    public List<MpConditionalMenu>? ConditionalMenu { get; set; }
}
