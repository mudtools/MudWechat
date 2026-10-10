// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

namespace Mud.Wechat.Work.DataModels.Kf;

/// <summary>
/// 微信客服菜单消息体（msgmenu 消息，<c>/cgi-bin/kf/send_msg</c> 与
/// <c>/cgi-bin/kf/send_msg_on_event</c> 共用；用户点击 click 菜单后会自动回复一条文本消息并附带菜单 ID）。
/// </summary>
[HttpJsonSerializable(SerializerClassName = "Kf")]
public class KfMenuMsgBody
{
    /// <summary>
    /// 获取或设置起始文本（不多于 1024 个字节）。
    /// </summary>
    [JsonPropertyName("head_content")]
    public string? HeadContent { get; set; }

    /// <summary>
    /// 获取或设置菜单项数组。
    /// <para>
    /// 通过 <c>/cgi-bin/kf/send_msg</c> 发送时不超过 50 个（其中 click / view / miniprogram 合计不超过 10 个）；
    /// 通过 <c>/cgi-bin/kf/send_msg_on_event</c> 发送时不超过 10 个。
    /// </para>
    /// </summary>
    [JsonPropertyName("list")]
    public List<KfMenuMsgListItem>? List { get; set; }

    /// <summary>
    /// 获取或设置结束文本（不多于 1024 个字节）。
    /// </summary>
    [JsonPropertyName("tail_content")]
    public string? TailContent { get; set; }
}

/// <summary>
/// 微信客服菜单消息的菜单项（按 <see cref="Type"/> 判别，填充对应的子消息体）。
/// </summary>
[HttpJsonSerializable(SerializerClassName = "Kf")]
public class KfMenuMsgListItem
{
    /// <summary>
    /// 获取或设置菜单类型（官方必填）：click - 点击回复，view - 跳转链接，miniprogram - 跳转小程序，text - 文本。
    /// </summary>
    [JsonPropertyName("type")]
    public string? Type { get; set; }

    /// <summary>
    /// 获取或设置点击回复型菜单体（<see cref="Type"/> 为 click 时填充）。
    /// </summary>
    [JsonPropertyName("click")]
    public KfMenuClickItem? Click { get; set; }

    /// <summary>
    /// 获取或设置跳转链接型菜单体（<see cref="Type"/> 为 view 时填充）。
    /// </summary>
    [JsonPropertyName("view")]
    public KfMenuViewItem? View { get; set; }

    /// <summary>
    /// 获取或设置跳转小程序型菜单体（<see cref="Type"/> 为 miniprogram 时填充）。
    /// </summary>
    [JsonPropertyName("miniprogram")]
    public KfMenuMiniProgramItem? MiniProgram { get; set; }

    /// <summary>
    /// 获取或设置文本型菜单体（<see cref="Type"/> 为 text 时填充）。
    /// </summary>
    [JsonPropertyName("text")]
    public KfMenuTextItem? Text { get; set; }
}

/// <summary>
/// 微信客服菜单消息的点击回复型菜单项（click）。
/// </summary>
[HttpJsonSerializable(SerializerClassName = "Kf")]
public class KfMenuClickItem
{
    /// <summary>
    /// 获取或设置菜单 ID（不多于 128 个字节，建议仅使用 a-z、A-Z、0-9、_；不建议使用 #，可能被截断）。
    /// </summary>
    [JsonPropertyName("id")]
    public string? Id { get; set; }

    /// <summary>
    /// 获取或设置菜单显示内容（官方必填，1~128 个字节）。
    /// </summary>
    [JsonPropertyName("content")]
    public string? Content { get; set; }
}

/// <summary>
/// 微信客服菜单消息的跳转链接型菜单项（view）。
/// </summary>
[HttpJsonSerializable(SerializerClassName = "Kf")]
public class KfMenuViewItem
{
    /// <summary>
    /// 获取或设置点击后跳转的链接（官方必填，1~2048 个字节）。
    /// </summary>
    [JsonPropertyName("url")]
    public string? Url { get; set; }

    /// <summary>
    /// 获取或设置菜单显示内容（官方必填，1~1024 个字节）。
    /// </summary>
    [JsonPropertyName("content")]
    public string? Content { get; set; }
}

/// <summary>
/// 微信客服菜单消息的跳转小程序型菜单项（miniprogram）。
/// </summary>
[HttpJsonSerializable(SerializerClassName = "Kf")]
public class KfMenuMiniProgramItem
{
    /// <summary>
    /// 获取或设置小程序 appid（官方必填，1~32 个字节）。
    /// </summary>
    [JsonPropertyName("appid")]
    public string? AppId { get; set; }

    /// <summary>
    /// 获取或设置小程序消息的页面路径（官方必填，1~1024 个字节）。
    /// </summary>
    [JsonPropertyName("pagepath")]
    public string? PagePath { get; set; }

    /// <summary>
    /// 获取或设置菜单显示内容（不多于 1024 个字节；通过 <c>/cgi-bin/kf/send_msg_on_event</c> 发送时官方必填）。
    /// </summary>
    [JsonPropertyName("content")]
    public string? Content { get; set; }
}

/// <summary>
/// 微信客服菜单消息的文本型菜单项（text）。
/// </summary>
[HttpJsonSerializable(SerializerClassName = "Kf")]
public class KfMenuTextItem
{
    /// <summary>
    /// 获取或设置菜单显示内容（官方必填，1~256 个字节，支持 <c>\n</c> 换行）。
    /// </summary>
    [JsonPropertyName("content")]
    public string? Content { get; set; }

    /// <summary>
    /// 获取或设置是否换行：0 - 换行（默认），1 - 不换行。
    /// </summary>
    [JsonPropertyName("no_newline")]
    public int? NoNewline { get; set; }
}
