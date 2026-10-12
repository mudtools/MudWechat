// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

namespace Mud.Wechat.OfficialAccount.DataModels.Menu;

/// <summary>
/// 查询自定义菜单信息响应（<c>getCurrentSelfmenuInfo</c>，<c>GET /cgi-bin/get_current_selfmenu_info</c>）。
/// </summary>
/// <remarks>
/// <para>
/// <b>与 <see cref="MpGetMenuResponse"/> 的本质差异（官方注意事项原文）</b>：本接口<b>不仅能查 API 设置的菜单</b>，
/// 还能查<b>公众平台官网（mp.weixin.qq.com）中设置</b>的菜单；而菜单查询接口只能查到用 API 设置的菜单。
/// 故第三方平台场景用它检测「公众号官网菜单配置」。
/// </para>
/// <para>
/// <b>权限面更宽</b>：官方原文「认证 / 未认证的服务号 / 公众号，以及接口测试号，均拥有该接口权限」
/// （与菜单域其余端点「仅认证」不同）。
/// </para>
/// <para>
/// 官方响应顶层：<c>is_menu_open</c>（<c>0</c> 未开启 / <c>1</c> 开启）与 <c>selfmenu_info</c>。
/// </para>
/// </remarks>
[HttpJsonSerializable(SerializerClassName = "Menu")]
public class MpGetCurrentSelfMenuInfoResponse : MpResponse
{
    /// <summary>菜单是否开启（<c>0</c> 未开启 / <c>1</c> 开启）。</summary>
    [JsonPropertyName("is_menu_open")]
    public int IsMenuOpen { get; set; }

    /// <summary>菜单信息（未开启时可能缺省）。</summary>
    [JsonPropertyName("selfmenu_info")]
    public MpSelfMenuInfo? SelfMenuInfo { get; set; }
}
