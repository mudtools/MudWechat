// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

namespace Mud.Wechat.MiniProgram.DataModels.Express;

/// <summary>查询支持同城配送的城市响应（<c>POST /cgi-bin/express/intracity/getcity</c>）。</summary>
/// <remarks>
/// 官方错误码要点：<c>48001</c>（小程序未获同城配送接口权限，需在管理后台开通）、
/// <c>934003</c>（运力 ID 错误）、<c>934009</c>（不支持该门店所在城市）。
/// </remarks>
[HttpJsonSerializable(SerializerClassName = "Express")]
public class WxaExpressGetCityResponse : WxaResponse
{
    /// <summary>各运力支持的城市信息（<c>support_list</c>），见 <see cref="WxaExpressCitySupport"/>。</summary>
    [JsonPropertyName("support_list")]
    public WxaExpressCitySupport[]? SupportList { get; set; }
}
