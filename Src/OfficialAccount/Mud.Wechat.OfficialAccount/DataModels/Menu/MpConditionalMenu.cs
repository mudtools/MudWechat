// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

namespace Mud.Wechat.OfficialAccount.DataModels.Menu;

/// <summary>
/// 个性化菜单（<c>button</c> + <c>matchrule</c>）。
/// </summary>
/// <remarks>
/// <para>
/// <b>为何同一类型既作请求体又作响应元素</b>：官方「创建个性化菜单」请求体与「获取自定义菜单配置」的
/// <c>conditionalmenu</c> 元素<b>字段表逐字段一致</b>（仅 <c>button</c> + <c>matchrule</c>），
/// 拆成两个同构类属纯冗余。
/// </para>
/// <para>
/// <b>不含 <c>menuid</c></b>：官方「获取自定义菜单配置」的 <c>conditionalmenu</c> 字段表<b>未列</b>
/// <c>menuid</c>（该字段仅由「创建个性化菜单」响应下发）⇒ 建模为两个类型：本类型（查询用）与
/// <see cref="MpAddConditionalMenuResponse"/>（创建响应）。
/// </para>
/// </remarks>
[HttpJsonSerializable(SerializerClassName = "Menu")]
public class MpConditionalMenu
{
    /// <summary>一级菜单数组（官方：<b>1~3 个</b>）。</summary>
    [JsonPropertyName("button")]
    public List<MpMenuButton>? Button { get; set; }

    /// <summary>菜单匹配规则（至少一个非空字段）。</summary>
    [JsonPropertyName("matchrule")]
    public MpMenuMatchRule? MatchRule { get; set; }
}
