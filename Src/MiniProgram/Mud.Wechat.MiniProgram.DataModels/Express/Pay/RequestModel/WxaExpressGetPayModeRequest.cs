// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

namespace Mud.Wechat.MiniProgram.DataModels.Express;

/// <summary>查询扣费主体请求体（<c>POST /cgi-bin/express/intracity/getpaymode</c>）。</summary>
/// <remarks>
/// <para>
/// 官方文档：<see href="https://developers.weixin.qq.com/miniprogram/dev/server/API/weixin-express/same_city_distribution/api_intracity_getpaymode"/>
/// （接口英文名 <c>intracity_getpaymode</c>）。
/// </para>
/// <para><b>官方原文约束</b>：<c>appid</c> <b>必须</b>与当前 <c>access_token</c> 匹配，否则返回 <c>934012</c>（appid 和 access_token 不匹配）。</para>
/// </remarks>
[HttpJsonSerializable(SerializerClassName = "Express")]
public class WxaExpressGetPayModeRequest
{
    /// <summary>小程序 AppId（<c>appid</c>，必填；必须与 access_token 匹配）。</summary>
    [JsonPropertyName("appid")]
    public string? AppId { get; set; }
}
