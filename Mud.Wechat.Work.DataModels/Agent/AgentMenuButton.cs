// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

namespace Mud.Wechat.Work.DataModels.Agent;

/// <summary>
/// 自定义菜单按钮（创建/获取菜单的 <c>button</c> 与 <c>sub_button</c> 数组元素，一级与二级菜单同构）。
/// </summary>
/// <remarks>
/// <para>官方限制：一级菜单个数为 1~3 个；二级菜单个数为 1~5 个；主菜单 name 不超过 16 字节、
/// 子菜单 name 不超过 40 字节；key 不超过 128 字节；url 不超过 1024 字节（建议使用 https）；
/// appid 仅限与企业绑定的小程序。</para>
/// <para>
/// 响应动作类型（type）取值：click（点击推事件，key 必填）/ view（跳转 URL，url 必填）/
/// scancode_push（扫码推事件）/ scancode_waitmsg（扫码推事件且弹出提示）/
/// pic_sysphoto（弹出系统拍照发图）/ pic_photo_or_album（弹出拍照或者相册发图）/
/// pic_weixin（弹出企业微信相册发图器）/ location_select（弹出地理位置选择器）/
/// view_miniprogram（跳转到小程序，appid 与 pagepath 必填）。
/// </para>
/// </remarks>
[HttpJsonSerializable(SerializerClassName = "Agent")]
public class AgentMenuButton
{
    /// <summary>获取或设置菜单的响应动作类型（官方必填），取值见类型 remarks。</summary>
    [JsonPropertyName("type")]
    public string? Type { get; set; }

    /// <summary>获取或设置菜单名（官方必填，不能为空；主菜单不超过 16 字节、子菜单不超过 40 字节）。</summary>
    [JsonPropertyName("name")]
    public string? Name { get; set; }

    /// <summary>获取或设置菜单 KEY 值（click 等点击类型必填，用于消息接口推送，不超过 128 字节）。</summary>
    [JsonPropertyName("key")]
    public string? Key { get; set; }

    /// <summary>获取或设置网页链接（view 类型必填，不超过 1024 字节，建议使用 https）。</summary>
    [JsonPropertyName("url")]
    public string? Url { get; set; }

    /// <summary>获取或设置小程序页面路径（view_miniprogram 类型必填）。</summary>
    [JsonPropertyName("pagepath")]
    public string? Pagepath { get; set; }

    /// <summary>获取或设置小程序 appid（view_miniprogram 类型必填；仅与企业绑定的小程序可配置）。</summary>
    [JsonPropertyName("appid")]
    public string? Appid { get; set; }

    /// <summary>获取或设置二级菜单数组（个数应为 1~5 个），元素与本对象同构。</summary>
    [JsonPropertyName("sub_button")]
    public List<AgentMenuButton>? SubButton { get; set; }
}
