// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

namespace Mud.Wechat.MiniProgram.DataModels.Express;

/// <summary>绑定 / 解绑打印员请求体（<c>POST /cgi-bin/express/business/printer/update</c>）。</summary>
/// <remarks>
/// <para>官方文档：<see href="https://developers.weixin.qq.com/miniprogram/dev/server/API/express/express-by-business/api_updateprinter"/>。</para>
/// <para><c>update_type=bind</c> 绑定 / <c>unbind</c> 解绑；绑定后商家端下单可指定打印员（快递员）接收面单打印通知。</para>
/// </remarks>
[HttpJsonSerializable(SerializerClassName = "Express")]
public class WxaExpressUpdatePrinterRequest
{
    /// <summary>操作类型（<c>update_type</c>，必填；bind 绑定 / unbind 解绑）。</summary>
    [JsonPropertyName("update_type")]
    public string? Action { get; set; }

    /// <summary>打印员 OpenId（<c>openid</c>，必填）。</summary>
    [JsonPropertyName("openid")]
    public string? PrinterOpenId { get; set; }

    /// <summary>标签 ID 列表（<c>tagid_list</c>，可选；平台型小程序区分入驻方）。</summary>
    [JsonPropertyName("tagid_list")]
    public List<string>? TagIdList { get; set; }
}
