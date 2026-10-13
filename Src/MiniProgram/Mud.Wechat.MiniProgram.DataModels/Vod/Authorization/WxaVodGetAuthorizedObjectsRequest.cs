// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

namespace Mud.Wechat.MiniProgram.DataModels.Vod;

/// <summary>
/// 「查询被授权信息」请求体（<c>POST /wxa/sec/vod/getauthorizedobjects</c>，站在被授权方视角查我得到的权利）。
/// </summary>
/// <remarks>
/// <para>官方文档：<see href="https://developers.weixin.qq.com/miniprogram/dev/server/API/minidrama/authorizeapp/api_getauthorizedobjects.html"/></para>
/// </remarks>
[HttpJsonSerializable(SerializerClassName = "Vod")]
public class WxaVodGetAuthorizedObjectsRequest
{
    /// <summary>authorizer_appid（官方类型 <c>string</c>）：授权方小程序id。不填表示查询所有授权方。</summary>
    [JsonPropertyName("authorizer_appid")]
    public string? AuthorizerAppid { get; set; }

    /// <summary>offset（官方类型 <c>number</c>）：偏移值，分页请求用。默认0。</summary>
    [JsonPropertyName("offset")]
    public long? Offset { get; set; }

    /// <summary>limit（官方类型 <c>number</c>）：分页大小，默认100，最大值为1000。</summary>
    [JsonPropertyName("limit")]
    public long? Limit { get; set; }
}
