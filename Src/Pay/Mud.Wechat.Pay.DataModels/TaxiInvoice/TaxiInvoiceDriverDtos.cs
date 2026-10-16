// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

using Mud.Wechat.Pay.DataModels.Common;
using System.Text.Json.Serialization;
using Mud.HttpUtils.Attributes;

namespace Mud.Wechat.Pay.DataModels.TaxiInvoice;

/// <summary>更新司机信息请求·【司机头像】 司机头像·【司机头像摘要】 司机头像摘要。官方页：<see href="https://pay.weixin.qq.com/doc/v3/partner/4012460171"/></summary>
[HttpJsonSerializable(SerializerClassName = "TaxiInvoice")]
public class TaxiInvoiceDriverUpdateRequestDriverPhotoPhotoDigest
{
    /// <summary>【哈希类型】 哈希类型，MD5：MD5算法，目前仅支持MD5。可选取值：`MD5` MD5算法（官方字段路径 <c>driver_photo.photo_digest.hash_type</c>，选填，类型 <c>string</c>）。</summary>
    [JsonPropertyName("hash_type")]
    public string? HashType { get; set; }

    /// <summary>【哈希值】 哈希值（官方字段路径 <c>driver_photo.photo_digest.hash_value</c>，选填，类型 <c>string(512)</c>）。</summary>
    [JsonPropertyName("hash_value")]
    public string? HashValue { get; set; }

}

/// <summary>更新司机信息请求·【司机头像】 司机头像。官方页：<see href="https://pay.weixin.qq.com/doc/v3/partner/4012460171"/></summary>
[HttpJsonSerializable(SerializerClassName = "TaxiInvoice")]
public class TaxiInvoiceDriverUpdateRequestDriverPhoto
{
    /// <summary>【照片类型】 照片类型，PHOTO_URL：url路径，PHOTO_ICON：icon图像（官方字段路径 <c>driver_photo.photo_type</c>，选填，类型 <c>string(16)</c>）。</summary>
    [JsonPropertyName("photo_type")]
    public string? PhotoType { get; set; }

    /// <summary>【司机头像摘要】 司机头像摘要（官方字段路径 <c>driver_photo.photo_digest</c>，选填，类型 <c>object</c>）。</summary>
    [JsonPropertyName("photo_digest")]
    public TaxiInvoiceDriverUpdateRequestDriverPhotoPhotoDigest? PhotoDigest { get; set; }

    /// <summary>【头像url】 头像url（官方字段路径 <c>driver_photo.photo_url</c>，选填，类型 <c>string(255)</c>）。</summary>
    [JsonPropertyName("photo_url")]
    public string? PhotoUrl { get; set; }

    /// <summary>【头像icon】 头像icon（官方字段路径 <c>driver_photo.photo_icon</c>，选填，类型 <c>string(65000)</c>）。</summary>
    [JsonPropertyName("photo_icon")]
    public string? PhotoIcon { get; set; }

}

/// <summary>更新司机信息请求。官方页：<see href="https://pay.weixin.qq.com/doc/v3/partner/4012460171"/></summary>
[HttpJsonSerializable(SerializerClassName = "TaxiInvoice")]
public class TaxiInvoiceDriverUpdateRequest
{
    /// <summary>【司机姓名】 司机姓名，需要使用微信支付公钥加密（推荐），请参考 获取微信支付公钥ID说明 以及 微信支付公钥加密敏感信息指引，也可以使用微信支付平台证书公钥加密，参考 获取平台证书序列号、 平台证书加密敏感信息指引（官方字段路径 <c>driver_name</c>，<b>必填</b>，类型 <c>string(1024)</c>）。</summary>
    [JsonPropertyName("driver_name")]
    public string? DriverName { get; set; }

    /// <summary>【司机身份证号码】 司机身份证号码，需要使用微信支付公钥加密（推荐），请参考 获取微信支付公钥ID说明 以及 微信支付公钥加密敏感信息指引，也可以使用微信支付平台证书公钥加密，参考 获取平台证书序列号、 平台证书加密敏感信息指引（官方字段路径 <c>id_card_number</c>，<b>必填</b>，类型 <c>string(1024)</c>）。</summary>
    [JsonPropertyName("id_card_number")]
    public string? IdCardNumber { get; set; }

    /// <summary>【公司名称(全称)】 公司名称(全称)。首次更新时，从【新增出租车公司信息】返回报文中获取（官方字段路径 <c>company_name</c>，<b>必填</b>，类型 <c>string(1024)</c>）。</summary>
    [JsonPropertyName("company_name")]
    public string? CompanyName { get; set; }

    /// <summary>【公司商户号】 公司商户号（官方字段路径 <c>mchid</c>，<b>必填</b>，类型 <c>string(20)</c>）。</summary>
    [JsonPropertyName("mchid")]
    public string? Mchid { get; set; }

    /// <summary>【营运资格证号】 营运资格证号（官方字段路径 <c>driver_license</c>，<b>必填</b>，类型 <c>string(128)</c>）。</summary>
    [JsonPropertyName("driver_license")]
    public string? DriverLicense { get; set; }

    /// <summary>【司机类别】 司机类别：MAIN:主班，DEPUTY:副班，FLEXIBLE:机动司机，UNKNOWN:未知（官方字段路径 <c>driver_category</c>，<b>必填</b>，类型 <c>string(20)</c>）。</summary>
    [JsonPropertyName("driver_category")]
    public string? DriverCategory { get; set; }

    /// <summary>【岗位状态】 岗位状态：ON_DUTY:在岗 OFF_DUTY:离岗（官方字段路径 <c>driver_status</c>，<b>必填</b>，类型 <c>string(20)</c>）。</summary>
    [JsonPropertyName("driver_status")]
    public string? DriverStatus { get; set; }

    /// <summary>【司机头像】 司机头像（官方字段路径 <c>driver_photo</c>，选填，类型 <c>object</c>）。</summary>
    [JsonPropertyName("driver_photo")]
    public TaxiInvoiceDriverUpdateRequestDriverPhoto? DriverPhoto { get; set; }

    /// <summary>【行政区划ID】 行政区划ID（官方字段路径 <c>region_id</c>，<b>必填</b>，类型 <c>integer</c>）。</summary>
    [JsonPropertyName("region_id")]
    public long? RegionId { get; set; }

}

/// <summary>更新司机信息响应·【司机头像】 司机头像·【司机头像摘要】 司机头像摘要。官方页：<see href="https://pay.weixin.qq.com/doc/v3/partner/4012460171"/></summary>
[HttpJsonSerializable(SerializerClassName = "TaxiInvoice")]
public class TaxiInvoiceDriverUpdateResponseDriverPhotoPhotoDigest
{
    /// <summary>【哈希类型】 哈希类型，MD5：MD5算法，目前仅支持MD5。可选取值：`MD5` MD5算法（官方字段路径 <c>driver_photo.photo_digest.hash_type</c>，选填，类型 <c>string</c>）。</summary>
    [JsonPropertyName("hash_type")]
    public string? HashType { get; set; }

    /// <summary>【哈希值】 哈希值（官方字段路径 <c>driver_photo.photo_digest.hash_value</c>，选填，类型 <c>string(512)</c>）。</summary>
    [JsonPropertyName("hash_value")]
    public string? HashValue { get; set; }

}

/// <summary>更新司机信息响应·【司机头像】 司机头像。官方页：<see href="https://pay.weixin.qq.com/doc/v3/partner/4012460171"/></summary>
[HttpJsonSerializable(SerializerClassName = "TaxiInvoice")]
public class TaxiInvoiceDriverUpdateResponseDriverPhoto
{
    /// <summary>【照片类型】 照片类型，PHOTO_URL：url路径，PHOTO_ICON：icon图像（官方字段路径 <c>driver_photo.photo_type</c>，选填，类型 <c>string(16)</c>）。</summary>
    [JsonPropertyName("photo_type")]
    public string? PhotoType { get; set; }

    /// <summary>【司机头像摘要】 司机头像摘要（官方字段路径 <c>driver_photo.photo_digest</c>，选填，类型 <c>object</c>）。</summary>
    [JsonPropertyName("photo_digest")]
    public TaxiInvoiceDriverUpdateResponseDriverPhotoPhotoDigest? PhotoDigest { get; set; }

    /// <summary>【头像url】 头像url（官方字段路径 <c>driver_photo.photo_url</c>，选填，类型 <c>string(255)</c>）。</summary>
    [JsonPropertyName("photo_url")]
    public string? PhotoUrl { get; set; }

    /// <summary>【头像icon】 头像icon（官方字段路径 <c>driver_photo.photo_icon</c>，选填，类型 <c>string(65000)</c>）。</summary>
    [JsonPropertyName("photo_icon")]
    public string? PhotoIcon { get; set; }

}

/// <summary>更新司机信息响应。官方页：<see href="https://pay.weixin.qq.com/doc/v3/partner/4012460171"/></summary>
[HttpJsonSerializable(SerializerClassName = "TaxiInvoice")]
public class TaxiInvoiceDriverUpdateResponse : WechatPayResponse
{
    /// <summary>【司机姓名】 司机姓名，需要 使用API证书私钥解密敏感字段（官方字段路径 <c>driver_name</c>，选填，类型 <c>string(1024)</c>）。</summary>
    [JsonPropertyName("driver_name")]
    public string? DriverName { get; set; }

    /// <summary>【司机身份证号码】 司机身份证号码，需要 使用API证书私钥解密敏感字段（官方字段路径 <c>id_card_number</c>，选填，类型 <c>string(1024)</c>）。</summary>
    [JsonPropertyName("id_card_number")]
    public string? IdCardNumber { get; set; }

    /// <summary>【公司名称(全称)】 公司名称(全称)（官方字段路径 <c>company_name</c>，<b>必填</b>，类型 <c>string(1024)</c>）。</summary>
    [JsonPropertyName("company_name")]
    public string? CompanyName { get; set; }

    /// <summary>【公司商户号】 公司商户号（官方字段路径 <c>mchid</c>，<b>必填</b>，类型 <c>string(20)</c>）。</summary>
    [JsonPropertyName("mchid")]
    public string? Mchid { get; set; }

    /// <summary>【营运资格证号】 营运资格证号（官方字段路径 <c>driver_license</c>，<b>必填</b>，类型 <c>string(128)</c>）。</summary>
    [JsonPropertyName("driver_license")]
    public string? DriverLicense { get; set; }

    /// <summary>【司机类别】 司机类别：MAIN:主班 DEPUTY:副班 FLEXIBLE:机动司机（官方字段路径 <c>driver_category</c>，<b>必填</b>，类型 <c>string(16)</c>）。</summary>
    [JsonPropertyName("driver_category")]
    public string? DriverCategory { get; set; }

    /// <summary>【岗位状态】 岗位状态：ON_DUTY:在岗 OFF_DUTY:离岗（官方字段路径 <c>driver_status</c>，<b>必填</b>，类型 <c>string(16)</c>）。</summary>
    [JsonPropertyName("driver_status")]
    public string? DriverStatus { get; set; }

    /// <summary>【司机头像】 司机头像（官方字段路径 <c>driver_photo</c>，选填，类型 <c>object</c>）。</summary>
    [JsonPropertyName("driver_photo")]
    public TaxiInvoiceDriverUpdateResponseDriverPhoto? DriverPhoto { get; set; }

    /// <summary>【行政区划ID】 行政区划ID（官方字段路径 <c>region_id</c>，<b>必填</b>，类型 <c>integer</c>）。</summary>
    [JsonPropertyName("region_id")]
    public long? RegionId { get; set; }

}

/// <summary>获取司机信息响应·【司机照片】 司机照片·【司机照片摘要】 司机照片摘要。官方页：<see href="https://pay.weixin.qq.com/doc/v3/partner/4012461309"/></summary>
[HttpJsonSerializable(SerializerClassName = "TaxiInvoice")]
public class TaxiInvoiceDriverInfoResponseDriverPhotoPhotoDigest
{
    /// <summary>【哈希类型】 哈希类型，MD5：MD5算法，目前仅支持MD5。可选取值：`MD5` MD5算法（官方字段路径 <c>driver_photo.photo_digest.hash_type</c>，选填，类型 <c>string</c>）。</summary>
    [JsonPropertyName("hash_type")]
    public string? HashType { get; set; }

    /// <summary>【哈希值】 哈希值（官方字段路径 <c>driver_photo.photo_digest.hash_value</c>，选填，类型 <c>string(512)</c>）。</summary>
    [JsonPropertyName("hash_value")]
    public string? HashValue { get; set; }

}

/// <summary>获取司机信息响应·【司机照片】 司机照片。官方页：<see href="https://pay.weixin.qq.com/doc/v3/partner/4012461309"/></summary>
[HttpJsonSerializable(SerializerClassName = "TaxiInvoice")]
public class TaxiInvoiceDriverInfoResponseDriverPhoto
{
    /// <summary>【照片类型】 照片类型。可选取值：`PHOTO_URL` 图片URL / `PHOTO_DATA` 图片数据流，经过Base64编码（官方字段路径 <c>driver_photo.photo_type</c>，选填，类型 <c>string</c>）。</summary>
    [JsonPropertyName("photo_type")]
    public string? PhotoType { get; set; }

    /// <summary>【司机照片摘要】 司机照片摘要（官方字段路径 <c>driver_photo.photo_digest</c>，选填，类型 <c>object</c>）。</summary>
    [JsonPropertyName("photo_digest")]
    public TaxiInvoiceDriverInfoResponseDriverPhotoPhotoDigest? PhotoDigest { get; set; }

    /// <summary>【照片URL】 头像url（官方字段路径 <c>driver_photo.photo_url</c>，选填，类型 <c>string(1024)</c>）。</summary>
    [JsonPropertyName("photo_url")]
    public string? PhotoUrl { get; set; }

    /// <summary>【照片数据】 头像数据（官方字段路径 <c>driver_photo.photo_data</c>，选填，类型 <c>string(65000)</c>）。</summary>
    [JsonPropertyName("photo_data")]
    public string? PhotoData { get; set; }

}

/// <summary>获取司机信息响应。官方页：<see href="https://pay.weixin.qq.com/doc/v3/partner/4012461309"/></summary>
[HttpJsonSerializable(SerializerClassName = "TaxiInvoice")]
public class TaxiInvoiceDriverInfoResponse : WechatPayResponse
{
    /// <summary>【司机姓名】 司机姓名，需要用微信支付平台证书中的公钥，对其进行加密（官方字段路径 <c>driver_name</c>，<b>必填</b>，类型 <c>string(1024)</c>）。</summary>
    [JsonPropertyName("driver_name")]
    public string? DriverName { get; set; }

    /// <summary>【公司名称(全称)】 公司名称(全称)（官方字段路径 <c>company_name</c>，<b>必填</b>，类型 <c>string(64)</c>）。</summary>
    [JsonPropertyName("company_name")]
    public string? CompanyName { get; set; }

    /// <summary>【公司商户号】 公司商户号（官方字段路径 <c>company_mchid</c>，<b>必填</b>，类型 <c>string(20)</c>）。</summary>
    [JsonPropertyName("company_mchid")]
    public string? CompanyMchid { get; set; }

    /// <summary>【营运资格证号】 营运资格证号（官方字段路径 <c>driver_license</c>，<b>必填</b>，类型 <c>string(128)</c>）。</summary>
    [JsonPropertyName("driver_license")]
    public string? DriverLicense { get; set; }

    /// <summary>【司机类别】 司机类别。可选取值：`MAIN` 主班司机，一般是出租车的承包人 / `DEPUTY` 副班司机 / `FLEXIBLE` 机动司机（官方字段路径 <c>driver_category</c>，<b>必填</b>，类型 <c>string</c>）。</summary>
    [JsonPropertyName("driver_category")]
    public string? DriverCategory { get; set; }

    /// <summary>【司机照片】 司机照片（官方字段路径 <c>driver_photo</c>，选填，类型 <c>object</c>）。</summary>
    [JsonPropertyName("driver_photo")]
    public TaxiInvoiceDriverInfoResponseDriverPhoto? DriverPhoto { get; set; }

    /// <summary>【行政区划代码】 行政区划代码（官方字段路径 <c>region_id</c>，<b>必填</b>，类型 <c>integer</c>）。</summary>
    [JsonPropertyName("region_id")]
    public long? RegionId { get; set; }

    /// <summary>【司机状态】 司机状态。可选取值：`ACTIVE` 表示司机在职 / `INACTIVE` 表示司机已离职（官方字段路径 <c>driver_state</c>，<b>必填</b>，类型 <c>string</c>）。</summary>
    [JsonPropertyName("driver_state")]
    public string? DriverState { get; set; }

}
