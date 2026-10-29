// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

namespace Mud.Wechat.MiniProgram.DataModels.Express;

/// <summary>获取打印员列表应答（<c>GET /cgi-bin/express/business/printer/getall</c>）。</summary>
/// <remarks>官方文档：<see href="https://developers.weixin.qq.com/miniprogram/dev/server/API/express/express-by-business/api_getprinter"/>。<b>GET</b> 请求（无请求体）。</remarks>
[HttpJsonSerializable(SerializerClassName = "Express")]
public class WxaExpressGetPrinterResponse : WxaResponse
{
    /// <summary>打印员数量（<c>count</c>）。</summary>
    [JsonPropertyName("count")]
    public int Count { get; set; }

    /// <summary>打印员 OpenId 列表（<c>openid</c>）。</summary>
    [JsonPropertyName("openid")]
    public List<string>? PrinterOpenIdList { get; set; }

    /// <summary>标签 ID 列表（<c>tagid_list</c>；平台型小程序区分入驻方）。</summary>
    [JsonPropertyName("tagid_list")]
    public List<string>? TagIdList { get; set; }
}
