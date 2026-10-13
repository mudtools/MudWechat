// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

namespace Mud.Wechat.MiniProgram.DataModels.Live;

/// <summary>
/// 获取长期订阅用户请求体（<c>POST /wxa/business/get_wxa_followers</c>）。
/// </summary>
/// <remarks>
/// <para>
/// 官方文档：<see href="https://developers.weixin.qq.com/miniprogram/dev/server/API/livebroadcast/subscribe-management/api_getfollowers.html"/>。
/// </para>
/// <para>
/// <b>翻页为游标而非页码</b>：官方原文「page_break 翻页标记，获取第一页时不带，第二页开始需带上上一页返回结果中的 page_break」
/// ⇒ 不得用 offset/limit 递增模拟分页；<c>limit</c> 默认 200、<b>最大 2000</b>。
/// </para>
/// </remarks>
[HttpJsonSerializable(SerializerClassName = "Live")]
public class WxaLiveFollowersRequest
{
    /// <summary>获取长期订阅用户的个数限制（<c>limit</c>，选填；官方原文默认 200，最大 2000）。</summary>
    [JsonPropertyName("limit")]
    public long? Limit { get; set; }

    /// <summary>翻页标记（<c>page_break</c>，选填；首页不带，次页起带上一页返回值）。</summary>
    [JsonPropertyName("page_break")]
    public long? PageBreak { get; set; }
}
