// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

namespace Mud.Wechat.MiniProgram.DataModels.Express;

/// <summary>同城配送询价 / 下单共用请求体（<c>preaddorder</c> 询价 + <c>addorder</c> 下单）。</summary>
/// <remarks>
/// <para>询价 <see href="https://developers.weixin.qq.com/miniprogram/dev/server/API/express/express-intracity/api_preaddorder"/>：调用后返回预估费用与服务信息，商家可据此展示给用户。</para>
/// <para>下单 <see href="https://developers.weixin.qq.com/miniprogram/dev/server/API/express/express-intracity/api_addorder"/>：询价后确认下单，凭 <c>store_order_id</c> 幂等（重复下单返回同一配送单）。</para>
/// </remarks>
[HttpJsonSerializable(SerializerClassName = "Express")]
public class WxaExpressIntracityOrderBaseRequest
{
    /// <summary>微信门店编号（<c>wx_store_id</c>，必填）。</summary>
    [JsonPropertyName("wx_store_id")]
    public string? StoreId { get; set; }

    /// <summary>用户姓名（<c>user_name</c>，必填）。</summary>
    [JsonPropertyName("user_name")]
    public string? UserName { get; set; }

    /// <summary>用户手机号（<c>user_phone</c>，必填）。</summary>
    [JsonPropertyName("user_phone")]
    public string? UserPhoneNumber { get; set; }

    /// <summary>用户位置经度（<c>user_lng</c>，必填）。</summary>
    [JsonPropertyName("user_lng")]
    public double UserLongitude { get; set; }

    /// <summary>用户位置纬度（<c>user_lat</c>，必填）。</summary>
    [JsonPropertyName("user_lat")]
    public double UserLatitude { get; set; }

    /// <summary>用户详细地址（<c>user_address</c>，必填）。</summary>
    [JsonPropertyName("user_address")]
    public string? UserAddress { get; set; }

    /// <summary>货品信息（<c>cargo</c>，必填），见 <see cref="WxaExpressIntracityCargo"/>。</summary>
    [JsonPropertyName("cargo")]
    public WxaExpressIntracityCargo? Cargo { get; set; }

    /// <summary>是否使用沙箱（<c>use_sandbox</c>，可选；true 走测试环境不真实计费）。</summary>
    [JsonPropertyName("use_sandbox")]
    public bool? IsSandbox { get; set; }
}
