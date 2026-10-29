// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

namespace Mud.Wechat.MiniProgram.DataModels.Express;

/// <summary>绑定 / 解绑快递账号请求体（<c>POST /cgi-bin/express/business/account/bind</c>）。</summary>
/// <remarks>
/// <para>官方文档：<see href="https://developers.weixin.qq.com/miniprogram/dev/server/API/express/express-by-business/api_bindaccount"/>。</para>
/// <para><c>type=bind</c> 绑定（须传入快递公司签约的 <c>password</c>）、<c>type=unbind</c> 解绑。绑定为<b>异步</b>流程：返回结果仅表示受理，最终状态经回调/查询确认。</para>
/// </remarks>
[HttpJsonSerializable(SerializerClassName = "Express")]
public class WxaExpressBindAccountRequest
{
    /// <summary>操作类型（<c>type</c>，必填；bind 绑定 / unbind 解绑）。</summary>
    [JsonPropertyName("type")]
    public string? Action { get; set; }

    /// <summary>快递公司客户编码（<c>biz_id</c>，必填）。</summary>
    [JsonPropertyName("biz_id")]
    public string? BusinessId { get; set; }

    /// <summary>快递公司 ID（<c>delivery_id</c>，必填）。</summary>
    [JsonPropertyName("delivery_id")]
    public string? DeliveryId { get; set; }

    /// <summary>快递公司签约密码（<c>password</c>，绑定必填；解绑不填）。</summary>
    [JsonPropertyName("password")]
    public string? Password { get; set; }

    /// <summary>备注内容（<c>remark_content</c>，可选；子商户模式用，以区分入驻方）。</summary>
    [JsonPropertyName("remark_content")]
    public string? RemarkContent { get; set; }
}
