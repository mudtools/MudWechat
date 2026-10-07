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

/// <summary>
/// 测试个性化菜单匹配结果响应（<c>tryMatchMenu</c>）。
/// </summary>
/// <remarks>
/// 官方响应仅有 <c>button</c>（菜单结构体数组，字段与创建菜单的按钮一致）——
/// <b>官方字段表未提供 <c>matchrule</c> 与 <c>menuid</c></b>，故 SDK 不建模它们（不据其他页「补全」）。
/// </remarks>
[HttpJsonSerializable(SerializerClassName = "Menu")]
public class MpTryMatchMenuResponse : MpResponse
{
    /// <summary>本次匹配命中的菜单按钮数组。</summary>
    [JsonPropertyName("button")]
    public List<MpMenuButton>? Button { get; set; }
}
