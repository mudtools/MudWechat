// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

using Mud.Wechat.Pay.DataModels.Common;
using System.Text.Json.Serialization;
using Mud.HttpUtils.Attributes;

namespace Mud.Wechat.Pay.DataModels.IndustryCoupon;

/// <summary>出行券快捷切卡组件 Token请求·【出行券列表】 用户最近领取的出行券列表。 校验规则： 1、多组券ID不能有重复。官方页：<see href="https://pay.weixin.qq.com/doc/v3/merchant/4012553563"/></summary>
[HttpJsonSerializable(SerializerClassName = "IndustryCoupon")]
public class IndustryCouponTransitCouponTokenRequestCouponListItem
{
    /// <summary>【批次号】 出行券的批次号，唯一定义出行代金券的标识（官方字段路径 <c>coupon_list.stock_id</c>，<b>必填</b>，类型 <c>string(64)</c>）。</summary>
    [JsonPropertyName("stock_id")]
    public string? StockId { get; set; }

    /// <summary>【券ID】 用户领券后唯一标识这张券的ID（官方字段路径 <c>coupon_list.coupon_id</c>，<b>必填</b>，类型 <c>string(64)</c>）。</summary>
    [JsonPropertyName("coupon_id")]
    public string? CouponId { get; set; }

}

/// <summary>出行券快捷切卡组件 Token请求。官方页：<see href="https://pay.weixin.qq.com/doc/v3/merchant/4012553563"/></summary>
[HttpJsonSerializable(SerializerClassName = "IndustryCoupon")]
public class IndustryCouponTransitCouponTokenRequest
{
    /// <summary>【用户标识】 用户在商户AppID下的唯一标识，该用户为后续拉起切卡组件的用户（官方字段路径 <c>open_id</c>，<b>必填</b>，类型 <c>string(64)</c>）。</summary>
    [JsonPropertyName("open_id")]
    public string? OpenId { get; set; }

    /// <summary>【出行券列表】 用户最近领取的出行券列表。 校验规则： 1、多组券ID不能有重复； 2、多组券只能是同一批次下的； 3、由于是领券后拉起切卡组件，所以券列表中的券需要是用户最近5分钟内领取的（官方字段路径 <c>coupon_list</c>，<b>必填</b>，类型 <c>array[TransitCouponEntity]</c>）。</summary>
    [JsonPropertyName("coupon_list")]
    public List<IndustryCouponTransitCouponTokenRequestCouponListItem>? CouponList { get; set; }

}

/// <summary>出行券快捷切卡组件 Token响应。官方页：<see href="https://pay.weixin.qq.com/doc/v3/merchant/4012553563"/></summary>
[HttpJsonSerializable(SerializerClassName = "IndustryCoupon")]
public class IndustryCouponTransitCouponTokenResponse : WechatPayResponse
{
    /// <summary>【切卡组件token值】 切卡组件预下单token，随后用于拉起快捷切卡小程序（官方字段路径 <c>token</c>，选填，类型 <c>string(1024)</c>）。</summary>
    [JsonPropertyName("token")]
    public string? Token { get; set; }

    /// <summary>【token过期时间】 token过期时间，遵循RFC3339标准格式，格式为YYYY-MM-DDTHH:mm:ss+TIMEZONE，YYYY-MM-DD表示年月日，T出现在字符串中，表示time元素的开头，HH:mm:ss表示时分秒，TIMEZONE表示时区（+08:00表示东八区时间，领先UTC 8小时，即北京时间）（官方字段路径 <c>expires_time</c>，选填，类型 <c>string(32)</c>）。</summary>
    [JsonPropertyName("expires_time")]
    public string? ExpiresTime { get; set; }

}
