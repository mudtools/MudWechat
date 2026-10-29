// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

namespace Mud.Wechat.MiniProgram.DataModels.Express;

/// <summary>服务商更新订单审核结果请求体（<c>POST /cgi-bin/express/delivery/service/business/update</c>）。</summary>
/// <remarks>
/// <para>官方文档：<see href="https://developers.weixin.qq.com/miniprogram/dev/server/API/express/express-by-provider/api_updatebusiness"/>。</para>
/// <para>服务商承接商户绑定时异步审核（账号绑定为异步流程），审核完成后调用本接口把结果回传微信侧。</para>
/// </remarks>
[HttpJsonSerializable(SerializerClassName = "Express")]
public class WxaExpressUpdateBusinessRequest
{
    /// <summary>委托绑定的小程序 AppId（<c>shop_app_id</c>，必填）。</summary>
    [JsonPropertyName("shop_app_id")]
    public string? ShopAppId { get; set; }

    /// <summary>物流服务商侧商户编号（<c>biz_id</c>，必填）。</summary>
    [JsonPropertyName("biz_id")]
    public string? BusinessId { get; set; }

    /// <summary>审核结果（<c>result_code</c>，必填；0 成功，2 已开通账号，5 审核驳回，6 重复绑定）。</summary>
    [JsonPropertyName("result_code")]
    public int ResultCode { get; set; }

    /// <summary>审核结果说明（<c>result_msg</c>，驳回时必填）。</summary>
    [JsonPropertyName("result_msg")]
    public string? ResultMessage { get; set; }
}
