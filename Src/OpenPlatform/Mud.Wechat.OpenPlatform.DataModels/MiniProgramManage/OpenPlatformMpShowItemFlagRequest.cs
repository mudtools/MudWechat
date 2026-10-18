// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

namespace Mud.Wechat.OpenPlatform.DataModels.MiniProgramManage;

/// <summary>
/// 「设置小程序被展示组件」请求（<c>updateshowwxaitem</c>）。
/// 官方文档：<see href="https://developers.weixin.qq.com/doc/oplatform/openApi/OpenApiDoc/miniprogram-management/subscribe-component/setShowItem.html"/>。
/// </summary>
/// <remarks><c>appid</c> 取自 <c>getwxamplinkforshow</c> 返回值；官方页面把 <c>wxa_subscribe_biz_flag</c> 标注为 number 必填但未展开各取值语义，本仓照原文以 <c>long?</c> 承载，不做三态臆断（追踪号 MP-Subscribe-Flag-Semantic-Pending）。</remarks>
[HttpJsonSerializable(SerializerClassName = "MiniProgramManage")]
public class OpenPlatformMpShowItemFlagRequest
{
    /// <summary>被展示开关标志（官方必填 <c>wxa_subscribe_biz_flag</c>，number）。</summary>
    [JsonPropertyName("wxa_subscribe_biz_flag")]
    public long? WxaSubscribeBizFlag { get; set; }

    /// <summary>关联小程序 AppID（官方选填 <c>appid</c>，string，值来自 getwxamplinkforshow）。</summary>
    [JsonPropertyName("appid")]
    public string? AppId { get; set; }
}
