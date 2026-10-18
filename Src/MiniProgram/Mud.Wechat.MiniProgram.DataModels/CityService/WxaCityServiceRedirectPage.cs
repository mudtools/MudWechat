// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

namespace Mud.Wechat.MiniProgram.DataModels.CityService;

/// <summary>
/// 医院跳转页面信息（<c>redirect_page</c> / <c>elder_redirect_page</c> 共用结构）。
/// </summary>
/// <remarks>
/// <para><b>条件必填</b>：需要跳转时 <c>page_type</c> 必填；<c>url</c> 在 web 类型必填；
/// <c>app_id</c> 与 <c>fullpath</c> 在小程序类型必填，且小程序须与当前账号关联主体。</para>
/// </remarks>
[HttpJsonSerializable(SerializerClassName = "CityService")]
public class WxaCityServiceRedirectPage
{
    /// <summary>页面路径类型，见 <see cref="WxaCityServiceRedirectPageTypes"/>。</summary>
    [JsonPropertyName("page_type")]
    public string? PageType { get; set; }

    /// <summary>网页网址，page_type 为 <c>web</c> 时必填（须为业务域名）。</summary>
    [JsonPropertyName("url")]
    public string? Url { get; set; }

    /// <summary>小程序 appid，page_type 为 <c>mini_program</c> 时必填。</summary>
    [JsonPropertyName("app_id")]
    public string? AppId { get; set; }

    /// <summary>小程序路径，page_type 为 <c>mini_program</c> 时必填。</summary>
    [JsonPropertyName("fullpath")]
    public string? FullPath { get; set; }
}
