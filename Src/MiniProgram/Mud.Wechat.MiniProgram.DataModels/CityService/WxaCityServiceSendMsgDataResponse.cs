// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

namespace Mud.Wechat.MiniProgram.DataModels.CityService;

/// <summary>
/// 城市服务消息通路发消息回包（<c>POST /cityservice/sendmsgdata</c>）。
/// </summary>
/// <remarks>
/// <para><b><c>result_page_url</c> 的语义</b>：需跳转至该 URL 以替代原有服务结果页面；
/// 未传 <c>result_page_style_id</c> 时返回为空。</para>
/// <para>错误码：40097 参数错误或 openid 不来自有消息通路权限的公众号、48001 未授权、
/// 82020~82026（未关注/频次/order_no/URL/服务下线等业务限制）。</para>
/// </remarks>
[HttpJsonSerializable(SerializerClassName = "CityService")]
public class WxaCityServiceSendMsgDataResponse : WxaResponse
{
    /// <summary>需跳转的结果页 URL（未传 result_page_style_id 时为空）。</summary>
    [JsonPropertyName("result_page_url")]
    public string? ResultPageUrl { get; set; }
}
