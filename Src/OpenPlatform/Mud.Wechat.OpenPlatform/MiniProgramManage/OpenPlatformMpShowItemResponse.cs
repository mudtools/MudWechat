// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

namespace Mud.Wechat.OpenPlatform.DataModels.MiniProgramManage;

/// <summary>
/// 「获取小程序被展示组件状态」应答（<c>getshowwxaitem</c>，官方为 GET）。
/// 官方文档：<see href="https://developers.weixin.qq.com/doc/oplatform/openApi/OpenApiDoc/miniprogram-management/subscribe-component/getShowItem.html"/>。
/// </summary>
/// <remarks>官方页面把 <c>can_open</c>/<c>is_open</c> 呈现为开关标志；原页未展开其取值语义，本仓以 <c>long?</c> 承载官方 number，不做布尔臆断。</remarks>
[HttpJsonSerializable(SerializerClassName = "MiniProgramManage")]
public class OpenPlatformMpShowItemResponse : OpenPlatformResponse
{
    /// <summary>是否可开启被展示（官方 <c>can_open</c>，number）。</summary>
    [JsonPropertyName("can_open")]
    public long? CanOpen { get; set; }

    /// <summary>是否已开启被展示（官方 <c>is_open</c>，number）。</summary>
    [JsonPropertyName("is_open")]
    public long? IsOpen { get; set; }

    /// <summary>当前展示关联的小程序 AppID（官方 <c>appid</c>，string）。</summary>
    [JsonPropertyName("appid")]
    public string? AppId { get; set; }

    /// <summary>名称（官方 <c>nickname</c>，string）。</summary>
    [JsonPropertyName("nickname")]
    public string? Nickname { get; set; }

    /// <summary>头像（官方 <c>headimg</c>，string）。</summary>
    [JsonPropertyName("headimg")]
    public string? HeadImg { get; set; }
}
