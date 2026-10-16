// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

using Mud.Wechat.Pay.DataModels.Common;
using System.Text.Json.Serialization;
using Mud.HttpUtils.Attributes;

namespace Mud.Wechat.Pay.DataModels.BrandStore;

/// <summary>品牌门店创建请求·【门店基础信息】 用于描述门店编码，名称等基本情况。官方页：<see href="https://pay.weixin.qq.com/doc/v3/partner/4015783183"/></summary>
[HttpJsonSerializable(SerializerClassName = "BrandStore")]
public class BrandStoreCreateRequestStoreBasics
{
    /// <summary>【商家门店编号】 商家内部的门店编号，最长32位字符；商家自行保证该编码在商家内部的唯一性。不允许有符号表情。 此字段为免审字段（仅修改免审字段时，将会直接更新门店，无需审核）（官方字段路径 <c>store_basics.store_reference_id</c>，选填，类型 <c>string(32)</c>）。</summary>
    [JsonPropertyName("store_reference_id")]
    public string? StoreReferenceId { get; set; }

    /// <summary>【门店名称】 只需填写纯粹的分店名称，例如："南山店"、"朝阳门店"、"天河城店" 不要包含任何括号、破折号等辅助符号 系统会自动将该名称与品牌名组合展示，最终呈现格式为："品牌名(分店名)"，例如："麦当劳(南山店)"（官方字段路径 <c>store_basics.branch_name</c>，选填，类型 <c>string(50)</c>）。</summary>
    [JsonPropertyName("branch_name")]
    public string? BranchName { get; set; }

}

/// <summary>品牌门店创建请求·【门店地址信息】 用于描述门店地址，经纬度等地理位置相关情况。官方页：<see href="https://pay.weixin.qq.com/doc/v3/partner/4015783183"/></summary>
[HttpJsonSerializable(SerializerClassName = "BrandStore")]
public class BrandStoreCreateRequestStoreAddress
{
    /// <summary>【门店省市编码】 门店所在省市区编码，只能由数字组成；详细参见 微信支付提供的省市对照表（官方字段路径 <c>store_address.address_code</c>，<b>必填</b>，类型 <c>string(20)</c>）。</summary>
    [JsonPropertyName("address_code")]
    public string? AddressCode { get; set; }

    /// <summary>【门店地址】 门店地址为核心重要信息，请准确填写并精确到门牌号，该信息涉及到地址核实、营销活动等业务，说明：不要重复填写省市区信息（官方字段路径 <c>store_address.address_detail</c>，<b>必填</b>，类型 <c>string(200)</c>）。</summary>
    [JsonPropertyName("address_detail")]
    public string? AddressDetail { get; set; }

    /// <summary>【门店地址辅助描述】 门店周围标志性建筑，用于辅助定位（官方字段路径 <c>store_address.address_complements</c>，选填，类型 <c>string(50)</c>）。</summary>
    [JsonPropertyName("address_complements")]
    public string? AddressComplements { get; set; }

    /// <summary>【门店经度】 经度，取值在\[-180,180\]之间的数字，经度长度不能超过32个字符，腾讯地图经纬度查询： https://lbs.qq.com/tool/getpoint/index.html（官方字段路径 <c>store_address.longitude</c>，选填，类型 <c>string(32)</c>）。</summary>
    [JsonPropertyName("longitude")]
    public string? Longitude { get; set; }

    /// <summary>【门店纬度】 纬度，取值在\[-90,90\]之间的数字，纬度长度不能超过32个字符，腾讯地图经纬度查询： https://lbs.qq.com/tool/getpoint/index.html（官方字段路径 <c>store_address.latitude</c>，选填，类型 <c>string(32)</c>）。</summary>
    [JsonPropertyName("latitude")]
    public string? Latitude { get; set; }

}

/// <summary>品牌门店创建请求·【门店经营信息】 用于描述门店联系电话，经营时间等经营状况。官方页：<see href="https://pay.weixin.qq.com/doc/v3/partner/4015783183"/></summary>
[HttpJsonSerializable(SerializerClassName = "BrandStore")]
public class BrandStoreCreateRequestStoreBusiness
{
    /// <summary>【门店服务电话】 支持座机和手机，只支持数字和“-”符号，最多支持两个电话，两个电话间用英文竖线“&amp;#124;”区隔。 此字段为免审字段（仅修改免审字段时，将会直接更新门店，无需审核）（官方字段路径 <c>store_business.service_phone</c>，选填，类型 <c>string(32)</c>）。</summary>
    [JsonPropertyName("service_phone")]
    public string? ServicePhone { get; set; }

    /// <summary>【门店经营时间】 经营时间需要使用指定格式，如下： 最多支持 7 个时间段，时间段间用英文竖线"｜"分隔，严格按 “周一至周五 09:00-20:00&amp;#124;周六至周日 10:00-22:00” 的格式进行填写； 支持填写跨日经营时间，需在终止时间前添加“次日”，严格参照“周一至周五 09:00-次日2:00” 格式填写，跨日周期不得超过 24 小时； 此字段为免审字段（仅修改免审字段时，将会直接更新门店，无需审核）（官方字段路径 <c>store_business.business_hours</c>，选填，类型 <c>string(256)</c>）。</summary>
    [JsonPropertyName("business_hours")]
    public string? BusinessHours { get; set; }

}

/// <summary>品牌门店创建请求。官方页：<see href="https://pay.weixin.qq.com/doc/v3/partner/4015783183"/></summary>
[HttpJsonSerializable(SerializerClassName = "BrandStore")]
public class BrandStoreCreateRequest
{
    /// <summary>【品牌ID】 商家进驻微信支付品牌商家后获得的品牌ID（官方字段路径 <c>brand_id</c>，<b>必填</b>，类型 <c>string</c>）。</summary>
    [JsonPropertyName("brand_id")]
    public string? BrandId { get; set; }

    /// <summary>【门店基础信息】 用于描述门店编码，名称等基本情况（官方字段路径 <c>store_basics</c>，选填，类型 <c>object</c>）。</summary>
    [JsonPropertyName("store_basics")]
    public BrandStoreCreateRequestStoreBasics? StoreBasics { get; set; }

    /// <summary>【门店地址信息】 用于描述门店地址，经纬度等地理位置相关情况（官方字段路径 <c>store_address</c>，<b>必填</b>，类型 <c>object</c>）。</summary>
    [JsonPropertyName("store_address")]
    public BrandStoreCreateRequestStoreAddress? StoreAddress { get; set; }

    /// <summary>【门店经营信息】 用于描述门店联系电话，经营时间等经营状况（官方字段路径 <c>store_business</c>，选填，类型 <c>object</c>）。</summary>
    [JsonPropertyName("store_business")]
    public BrandStoreCreateRequestStoreBusiness? StoreBusiness { get; set; }

}

/// <summary>品牌门店创建响应·【门店基础信息】 用于描述门店编码，名称等基本情况。官方页：<see href="https://pay.weixin.qq.com/doc/v3/partner/4015783183"/></summary>
[HttpJsonSerializable(SerializerClassName = "BrandStore")]
public class BrandStoreResponseStoreBasics
{
    /// <summary>【商家门店编号】 商家内部的门店编号，最长32位字符；商家自行保证该编码在商家内部的唯一性。不允许有符号表情。 此字段为免审字段（仅修改免审字段时，将会直接更新门店，无需审核）（官方字段路径 <c>store_basics.store_reference_id</c>，选填，类型 <c>string(32)</c>）。</summary>
    [JsonPropertyName("store_reference_id")]
    public string? StoreReferenceId { get; set; }

    /// <summary>【门店名称】 只需填写纯粹的分店名称，例如："南山店"、"朝阳门店"、"天河城店" 不要包含任何括号、破折号等辅助符号 系统会自动将该名称与品牌名组合展示，最终呈现格式为："品牌名(分店名)"，例如："麦当劳(南山店)"（官方字段路径 <c>store_basics.branch_name</c>，选填，类型 <c>string(50)</c>）。</summary>
    [JsonPropertyName("branch_name")]
    public string? BranchName { get; set; }

}

/// <summary>品牌门店创建响应·【门店地址信息】 用于描述门店地址，经纬度等地理位置相关情况。官方页：<see href="https://pay.weixin.qq.com/doc/v3/partner/4015783183"/></summary>
[HttpJsonSerializable(SerializerClassName = "BrandStore")]
public class BrandStoreResponseStoreAddress
{
    /// <summary>【门店省市编码】 门店所在省市区编码，只能由数字组成；详细参见 微信支付提供的省市对照表（官方字段路径 <c>store_address.address_code</c>，<b>必填</b>，类型 <c>string(20)</c>）。</summary>
    [JsonPropertyName("address_code")]
    public string? AddressCode { get; set; }

    /// <summary>【门店地址】 门店地址为核心重要信息，请准确填写并精确到门牌号，该信息涉及到地址核实、营销活动等业务，说明：不要重复填写省市区信息（官方字段路径 <c>store_address.address_detail</c>，<b>必填</b>，类型 <c>string(200)</c>）。</summary>
    [JsonPropertyName("address_detail")]
    public string? AddressDetail { get; set; }

    /// <summary>【门店地址辅助描述】 门店周围标志性建筑，用于辅助定位（官方字段路径 <c>store_address.address_complements</c>，选填，类型 <c>string(50)</c>）。</summary>
    [JsonPropertyName("address_complements")]
    public string? AddressComplements { get; set; }

    /// <summary>【门店经度】 经度，取值在\[-180,180\]之间的数字，经度长度不能超过32个字符，腾讯地图经纬度查询： https://lbs.qq.com/tool/getpoint/index.html（官方字段路径 <c>store_address.longitude</c>，选填，类型 <c>string(32)</c>）。</summary>
    [JsonPropertyName("longitude")]
    public string? Longitude { get; set; }

    /// <summary>【门店纬度】 纬度，取值在\[-90,90\]之间的数字，纬度长度不能超过32个字符，腾讯地图经纬度查询： https://lbs.qq.com/tool/getpoint/index.html（官方字段路径 <c>store_address.latitude</c>，选填，类型 <c>string(32)</c>）。</summary>
    [JsonPropertyName("latitude")]
    public string? Latitude { get; set; }

}

/// <summary>品牌门店创建响应·【门店经营信息】 用于描述门店联系电话，经营时间等经营状况。官方页：<see href="https://pay.weixin.qq.com/doc/v3/partner/4015783183"/></summary>
[HttpJsonSerializable(SerializerClassName = "BrandStore")]
public class BrandStoreResponseStoreBusiness
{
    /// <summary>【门店服务电话】 支持座机和手机，只支持数字和“-”符号，最多支持两个电话，两个电话间用英文竖线“&amp;#124;”区隔。 此字段为免审字段（仅修改免审字段时，将会直接更新门店，无需审核）（官方字段路径 <c>store_business.service_phone</c>，选填，类型 <c>string(32)</c>）。</summary>
    [JsonPropertyName("service_phone")]
    public string? ServicePhone { get; set; }

    /// <summary>【门店经营时间】 经营时间需要使用指定格式，如下： 最多支持 7 个时间段，时间段间用英文竖线"｜"分隔，严格按 “周一至周五 09:00-20:00&amp;#124;周六至周日 10:00-22:00” 的格式进行填写； 支持填写跨日经营时间，需在终止时间前添加“次日”，严格参照“周一至周五 09:00-次日2:00” 格式填写，跨日周期不得超过 24 小时； 此字段为免审字段（仅修改免审字段时，将会直接更新门店，无需审核）（官方字段路径 <c>store_business.business_hours</c>，选填，类型 <c>string(256)</c>）。</summary>
    [JsonPropertyName("business_hours")]
    public string? BusinessHours { get; set; }

}

/// <summary>品牌门店创建响应·【门店收款信息】 门店收款商户列表。官方页：<see href="https://pay.weixin.qq.com/doc/v3/partner/4015783183"/></summary>
[HttpJsonSerializable(SerializerClassName = "BrandStore")]
public class BrandStoreResponseStoreRecipientItem
{
    /// <summary>【门店收款商户号】 门店的收款商户号，仅支持绑定品牌已关联的商户号（官方字段路径 <c>store_recipient.mchid</c>，选填，类型 <c>string(16)</c>）。</summary>
    [JsonPropertyName("mchid")]
    public string? Mchid { get; set; }

    /// <summary>【门店收款主体】 门店收款的主体信息，支持企业，个体户，小微（官方字段路径 <c>store_recipient.company_name</c>，选填，类型 <c>string(256)</c>）。</summary>
    [JsonPropertyName("company_name")]
    public string? CompanyName { get; set; }

    /// <summary>【收款绑定状态】 门店收款商户号的绑定状态。可选取值：`CONFIRMED` 门店已绑定此收款商户号 / `ADMIN_REJECTED` 商户管理员已拒绝 / `CONFIRMING` 商户号管理员确认中 / `TIMEOUT_REJECTED` 由于商户管理员确认超时，系统自动拒绝此绑定（官方字段路径 <c>store_recipient.recipient_state</c>，选填，类型 <c>string</c>）。</summary>
    [JsonPropertyName("recipient_state")]
    public string? RecipientState { get; set; }

}

/// <summary>品牌门店创建响应。官方页：<see href="https://pay.weixin.qq.com/doc/v3/partner/4015783183"/></summary>
[HttpJsonSerializable(SerializerClassName = "BrandStore")]
public class BrandStoreResponse : WechatPayResponse
{
    /// <summary>【品牌ID】 商家进驻微信支付品牌商家后获得的品牌ID（官方字段路径 <c>brand_id</c>，<b>必填</b>，类型 <c>string</c>）。</summary>
    [JsonPropertyName("brand_id")]
    public string? BrandId { get; set; }

    /// <summary>【品牌门店ID】 创建品牌门店后，系统为该门店分配的唯一ID（官方字段路径 <c>store_id</c>，选填，类型 <c>string</c>）。</summary>
    [JsonPropertyName("store_id")]
    public string? StoreId { get; set; }

    /// <summary>【门店状态】 用于描述门店当前状态。可选取值：`OPEN` 门店生效中。门店审核通过创建成功后即为生效中，后续更新门店信息只会影响审核状态，不会改变门店状态，如门店暂停营业可调用“暂停门店营业 API”，如门店关闭可调用“删除品牌门店 API”。门店营业时间不影响门店状态。 / `CREATING` 门店创建中。在创建门店后，门店资料正在审核。审核详情请查看审核状态。 / `CLOSED` 门店停业中。不可用于微信支付生态中的其他业务，可删除该门店，删除后将无法恢复，请谨慎操作。（官方字段路径 <c>store_state</c>，选填，类型 <c>string</c>）。</summary>
    [JsonPropertyName("store_state")]
    public string? StoreState { get; set; }

    /// <summary>【审核状态】 创建、修改门店时，通过此字段可得知当前审核状态。可选取值：`SUCCESS` 门店资料审核通过。 / `PROCESSING` 门店资料审核中，请等待审核结果。 / `REJECTED` 门店资料被驳回，请根据驳回原因进行修改。（官方字段路径 <c>audit_state</c>，选填，类型 <c>string</c>）。</summary>
    [JsonPropertyName("audit_state")]
    public string? AuditState { get; set; }

    /// <summary>【审核失败原因】 门店资料审核失败的原因（官方字段路径 <c>review_reject_reason</c>，选填，类型 <c>string</c>）。</summary>
    [JsonPropertyName("review_reject_reason")]
    public string? ReviewRejectReason { get; set; }

    /// <summary>【门店基础信息】 用于描述门店编码，名称等基本情况（官方字段路径 <c>store_basics</c>，选填，类型 <c>object</c>）。</summary>
    [JsonPropertyName("store_basics")]
    public BrandStoreResponseStoreBasics? StoreBasics { get; set; }

    /// <summary>【门店地址信息】 用于描述门店地址，经纬度等地理位置相关情况（官方字段路径 <c>store_address</c>，选填，类型 <c>object</c>）。</summary>
    [JsonPropertyName("store_address")]
    public BrandStoreResponseStoreAddress? StoreAddress { get; set; }

    /// <summary>【门店经营信息】 用于描述门店联系电话，经营时间等经营状况（官方字段路径 <c>store_business</c>，选填，类型 <c>object</c>）。</summary>
    [JsonPropertyName("store_business")]
    public BrandStoreResponseStoreBusiness? StoreBusiness { get; set; }

    /// <summary>【门店收款信息】 门店收款商户列表（官方字段路径 <c>store_recipient</c>，选填，类型 <c>array[object]</c>）。</summary>
    [JsonPropertyName("store_recipient")]
    public List<BrandStoreResponseStoreRecipientItem>? StoreRecipient { get; set; }

}

/// <summary>品牌门店列表响应·【品牌门店列表】 查询到的门店列表·【门店基础信息】 用于描述门店编码，名称等基本情况。官方页：<see href="https://pay.weixin.qq.com/doc/v3/partner/4016756674"/></summary>
[HttpJsonSerializable(SerializerClassName = "BrandStore")]
public class BrandStoreListResponseDataItemStoreBasics
{
    /// <summary>【商家门店编号】 商家内部的门店编号，最长32位字符；商家自行保证该编码在商家内部的唯一性。不允许有符号表情。 此字段为免审字段（仅修改免审字段时，将会直接更新门店，无需审核）（官方字段路径 <c>data.store_basics.store_reference_id</c>，选填，类型 <c>string(32)</c>）。</summary>
    [JsonPropertyName("store_reference_id")]
    public string? StoreReferenceId { get; set; }

    /// <summary>【门店名称】 只需填写纯粹的分店名称，例如："南山店"、"朝阳门店"、"天河城店" 不要包含任何括号、破折号等辅助符号 系统会自动将该名称与品牌名组合展示，最终呈现格式为："品牌名(分店名)"，例如："麦当劳(南山店)"（官方字段路径 <c>data.store_basics.branch_name</c>，选填，类型 <c>string(50)</c>）。</summary>
    [JsonPropertyName("branch_name")]
    public string? BranchName { get; set; }

}

/// <summary>品牌门店列表响应·【品牌门店列表】 查询到的门店列表·【门店地址信息】 用于描述门店地址，经纬度等地理位置相关情况。官方页：<see href="https://pay.weixin.qq.com/doc/v3/partner/4016756674"/></summary>
[HttpJsonSerializable(SerializerClassName = "BrandStore")]
public class BrandStoreListResponseDataItemStoreAddress
{
    /// <summary>【门店省市编码】 门店所在省市区编码，只能由数字组成；详细参见 微信支付提供的省市对照表（官方字段路径 <c>data.store_address.address_code</c>，<b>必填</b>，类型 <c>string(20)</c>）。</summary>
    [JsonPropertyName("address_code")]
    public string? AddressCode { get; set; }

    /// <summary>【门店地址】 门店地址为核心重要信息，请准确填写并精确到门牌号，该信息涉及到地址核实、营销活动等业务，说明：不要重复填写省市区信息（官方字段路径 <c>data.store_address.address_detail</c>，<b>必填</b>，类型 <c>string(200)</c>）。</summary>
    [JsonPropertyName("address_detail")]
    public string? AddressDetail { get; set; }

    /// <summary>【门店地址辅助描述】 门店周围标志性建筑，用于辅助定位（官方字段路径 <c>data.store_address.address_complements</c>，选填，类型 <c>string(50)</c>）。</summary>
    [JsonPropertyName("address_complements")]
    public string? AddressComplements { get; set; }

    /// <summary>【门店经度】 经度，取值在\[-180,180\]之间的数字，经度长度不能超过32个字符，腾讯地图经纬度查询： https://lbs.qq.com/tool/getpoint/index.html（官方字段路径 <c>data.store_address.longitude</c>，选填，类型 <c>string(32)</c>）。</summary>
    [JsonPropertyName("longitude")]
    public string? Longitude { get; set; }

    /// <summary>【门店纬度】 纬度，取值在\[-90,90\]之间的数字，纬度长度不能超过32个字符，腾讯地图经纬度查询： https://lbs.qq.com/tool/getpoint/index.html（官方字段路径 <c>data.store_address.latitude</c>，选填，类型 <c>string(32)</c>）。</summary>
    [JsonPropertyName("latitude")]
    public string? Latitude { get; set; }

}

/// <summary>品牌门店列表响应·【品牌门店列表】 查询到的门店列表·【门店经营信息】 用于描述门店联系电话，经营时间等经营状况。官方页：<see href="https://pay.weixin.qq.com/doc/v3/partner/4016756674"/></summary>
[HttpJsonSerializable(SerializerClassName = "BrandStore")]
public class BrandStoreListResponseDataItemStoreBusiness
{
    /// <summary>【门店服务电话】 支持座机和手机，只支持数字和“-”符号，最多支持两个电话，两个电话间用英文竖线“&amp;#124;”区隔。 此字段为免审字段（仅修改免审字段时，将会直接更新门店，无需审核）（官方字段路径 <c>data.store_business.service_phone</c>，选填，类型 <c>string(32)</c>）。</summary>
    [JsonPropertyName("service_phone")]
    public string? ServicePhone { get; set; }

    /// <summary>【门店经营时间】 经营时间需要使用指定格式，如下： 最多支持 7 个时间段，时间段间用英文竖线"｜"分隔，严格按 “周一至周五 09:00-20:00&amp;#124;周六至周日 10:00-22:00” 的格式进行填写； 支持填写跨日经营时间，需在终止时间前添加“次日”，严格参照“周一至周五 09:00-次日2:00” 格式填写，跨日周期不得超过 24 小时； 此字段为免审字段（仅修改免审字段时，将会直接更新门店，无需审核）（官方字段路径 <c>data.store_business.business_hours</c>，选填，类型 <c>string(256)</c>）。</summary>
    [JsonPropertyName("business_hours")]
    public string? BusinessHours { get; set; }

}

/// <summary>品牌门店列表响应·【品牌门店列表】 查询到的门店列表·【门店收款信息】 门店收款商户列表。官方页：<see href="https://pay.weixin.qq.com/doc/v3/partner/4016756674"/></summary>
[HttpJsonSerializable(SerializerClassName = "BrandStore")]
public class BrandStoreListResponseDataItemStoreRecipientItem
{
    /// <summary>【门店收款商户号】 门店的收款商户号，仅支持绑定品牌已关联的商户号（官方字段路径 <c>data.store_recipient.mchid</c>，选填，类型 <c>string(16)</c>）。</summary>
    [JsonPropertyName("mchid")]
    public string? Mchid { get; set; }

    /// <summary>【门店收款主体】 门店收款的主体信息，支持企业，个体户，小微（官方字段路径 <c>data.store_recipient.company_name</c>，选填，类型 <c>string(256)</c>）。</summary>
    [JsonPropertyName("company_name")]
    public string? CompanyName { get; set; }

    /// <summary>【收款绑定状态】 门店收款商户号的绑定状态。可选取值：`CONFIRMED` 门店已绑定此收款商户号 / `ADMIN_REJECTED` 商户管理员已拒绝 / `CONFIRMING` 商户号管理员确认中 / `TIMEOUT_REJECTED` 由于商户管理员确认超时，系统自动拒绝此绑定（官方字段路径 <c>data.store_recipient.recipient_state</c>，选填，类型 <c>string</c>）。</summary>
    [JsonPropertyName("recipient_state")]
    public string? RecipientState { get; set; }

}

/// <summary>品牌门店列表响应·【品牌门店列表】 查询到的门店列表。官方页：<see href="https://pay.weixin.qq.com/doc/v3/partner/4016756674"/></summary>
[HttpJsonSerializable(SerializerClassName = "BrandStore")]
public class BrandStoreListResponseDataItem
{
    /// <summary>【品牌ID】 商家进驻微信支付品牌商家后获得的品牌ID（官方字段路径 <c>data.brand_id</c>，<b>必填</b>，类型 <c>string</c>）。</summary>
    [JsonPropertyName("brand_id")]
    public string? BrandId { get; set; }

    /// <summary>【品牌门店ID】 创建品牌门店后，系统为该门店分配的唯一ID（官方字段路径 <c>data.store_id</c>，选填，类型 <c>string</c>）。</summary>
    [JsonPropertyName("store_id")]
    public string? StoreId { get; set; }

    /// <summary>【门店状态】 用于描述门店当前状态。可选取值：`OPEN` 门店生效中。门店审核通过创建成功后即为生效中，后续更新门店信息只会影响审核状态，不会改变门店状态，如门店暂停营业可调用“暂停门店营业 API”，如门店关闭可调用“删除品牌门店 API”。门店营业时间不影响门店状态。 / `CREATING` 门店创建中。在创建门店后，门店资料正在审核。审核详情请查看审核状态。 / `CLOSED` 门店停业中。不可用于微信支付生态中的其他业务，可删除该门店，删除后将无法恢复，请谨慎操作。（官方字段路径 <c>data.store_state</c>，选填，类型 <c>string</c>）。</summary>
    [JsonPropertyName("store_state")]
    public string? StoreState { get; set; }

    /// <summary>【审核状态】 创建、修改门店时，通过此字段可得知当前审核状态。可选取值：`SUCCESS` 门店资料审核通过。 / `PROCESSING` 门店资料审核中，请等待审核结果。 / `REJECTED` 门店资料被驳回，请根据驳回原因进行修改。（官方字段路径 <c>data.audit_state</c>，选填，类型 <c>string</c>）。</summary>
    [JsonPropertyName("audit_state")]
    public string? AuditState { get; set; }

    /// <summary>【审核失败原因】 门店资料审核失败的原因（官方字段路径 <c>data.review_reject_reason</c>，选填，类型 <c>string</c>）。</summary>
    [JsonPropertyName("review_reject_reason")]
    public string? ReviewRejectReason { get; set; }

    /// <summary>【门店基础信息】 用于描述门店编码，名称等基本情况（官方字段路径 <c>data.store_basics</c>，选填，类型 <c>object</c>）。</summary>
    [JsonPropertyName("store_basics")]
    public BrandStoreListResponseDataItemStoreBasics? StoreBasics { get; set; }

    /// <summary>【门店地址信息】 用于描述门店地址，经纬度等地理位置相关情况（官方字段路径 <c>data.store_address</c>，选填，类型 <c>object</c>）。</summary>
    [JsonPropertyName("store_address")]
    public BrandStoreListResponseDataItemStoreAddress? StoreAddress { get; set; }

    /// <summary>【门店经营信息】 用于描述门店联系电话，经营时间等经营状况（官方字段路径 <c>data.store_business</c>，选填，类型 <c>object</c>）。</summary>
    [JsonPropertyName("store_business")]
    public BrandStoreListResponseDataItemStoreBusiness? StoreBusiness { get; set; }

    /// <summary>【门店收款信息】 门店收款商户列表（官方字段路径 <c>data.store_recipient</c>，选填，类型 <c>array[object]</c>）。</summary>
    [JsonPropertyName("store_recipient")]
    public List<BrandStoreListResponseDataItemStoreRecipientItem>? StoreRecipient { get; set; }

}

/// <summary>品牌门店列表响应。官方页：<see href="https://pay.weixin.qq.com/doc/v3/partner/4016756674"/></summary>
[HttpJsonSerializable(SerializerClassName = "BrandStore")]
public class BrandStoreListResponse : WechatPayResponse
{
    /// <summary>【品牌ID】 商家进驻微信支付品牌商家后获得的品牌ID（官方字段路径 <c>brand_id</c>，<b>必填</b>，类型 <c>string</c>）。</summary>
    [JsonPropertyName("brand_id")]
    public string? BrandId { get; set; }

    /// <summary>【品牌门店列表】 查询到的门店列表（官方字段路径 <c>data</c>，选填，类型 <c>array[object]</c>）。</summary>
    [JsonPropertyName("data")]
    public List<BrandStoreListResponseDataItem>? Data { get; set; }

    /// <summary>【分页起始位置】 默认从0开始查询（官方字段路径 <c>offset</c>，选填，类型 <c>integer</c>）。</summary>
    [JsonPropertyName("offset")]
    public long? Offset { get; set; }

    /// <summary>【分页条数】 查询的门店数量（官方字段路径 <c>limit</c>，选填，类型 <c>integer</c>）。</summary>
    [JsonPropertyName("limit")]
    public long? Limit { get; set; }

    /// <summary>【门店总数】 符合条件的门店总数（官方字段路径 <c>total_count</c>，选填，类型 <c>integer</c>）。</summary>
    [JsonPropertyName("total_count")]
    public long? TotalCount { get; set; }

}

/// <summary>品牌门店更新请求·【门店基础信息】 用于描述门店编码，名称等基本情况。官方页：<see href="https://pay.weixin.qq.com/doc/v3/partner/4015783158"/></summary>
[HttpJsonSerializable(SerializerClassName = "BrandStore")]
public class BrandStoreUpdateRequestStoreBasics
{
    /// <summary>【商家门店编号】 商家内部的门店编号，最长32位字符；商家自行保证该编码在商家内部的唯一性。不允许有符号表情。 此字段为免审字段（仅修改免审字段时，将会直接更新门店，无需审核）（官方字段路径 <c>store_basics.store_reference_id</c>，选填，类型 <c>string(32)</c>）。</summary>
    [JsonPropertyName("store_reference_id")]
    public string? StoreReferenceId { get; set; }

    /// <summary>【门店名称】 只需填写纯粹的分店名称，例如："南山店"、"朝阳门店"、"天河城店" 不要包含任何括号、破折号等辅助符号 系统会自动将该名称与品牌名组合展示，最终呈现格式为："品牌名(分店名)"，例如："麦当劳(南山店)"（官方字段路径 <c>store_basics.branch_name</c>，选填，类型 <c>string(50)</c>）。</summary>
    [JsonPropertyName("branch_name")]
    public string? BranchName { get; set; }

}

/// <summary>品牌门店更新请求·【门店地址信息】 用于描述门店地址，经纬度等地理位置相关情况。官方页：<see href="https://pay.weixin.qq.com/doc/v3/partner/4015783158"/></summary>
[HttpJsonSerializable(SerializerClassName = "BrandStore")]
public class BrandStoreUpdateRequestStoreAddress
{
    /// <summary>【门店省市编码】 门店所在省市区编码，只能由数字组成；详细参见 微信支付提供的省市对照表（官方字段路径 <c>store_address.address_code</c>，<b>必填</b>，类型 <c>string(20)</c>）。</summary>
    [JsonPropertyName("address_code")]
    public string? AddressCode { get; set; }

    /// <summary>【门店地址】 门店地址为核心重要信息，请准确填写并精确到门牌号，该信息涉及到地址核实、营销活动等业务，说明：不要重复填写省市区信息（官方字段路径 <c>store_address.address_detail</c>，<b>必填</b>，类型 <c>string(200)</c>）。</summary>
    [JsonPropertyName("address_detail")]
    public string? AddressDetail { get; set; }

    /// <summary>【门店地址辅助描述】 门店周围标志性建筑，用于辅助定位（官方字段路径 <c>store_address.address_complements</c>，选填，类型 <c>string(50)</c>）。</summary>
    [JsonPropertyName("address_complements")]
    public string? AddressComplements { get; set; }

    /// <summary>【门店经度】 经度，取值在\[-180,180\]之间的数字，经度长度不能超过32个字符，腾讯地图经纬度查询： https://lbs.qq.com/tool/getpoint/index.html（官方字段路径 <c>store_address.longitude</c>，选填，类型 <c>string(32)</c>）。</summary>
    [JsonPropertyName("longitude")]
    public string? Longitude { get; set; }

    /// <summary>【门店纬度】 纬度，取值在\[-90,90\]之间的数字，纬度长度不能超过32个字符，腾讯地图经纬度查询： https://lbs.qq.com/tool/getpoint/index.html（官方字段路径 <c>store_address.latitude</c>，选填，类型 <c>string(32)</c>）。</summary>
    [JsonPropertyName("latitude")]
    public string? Latitude { get; set; }

}

/// <summary>品牌门店更新请求·【门店经营信息】 用于描述门店联系电话，经营时间等经营状况。官方页：<see href="https://pay.weixin.qq.com/doc/v3/partner/4015783158"/></summary>
[HttpJsonSerializable(SerializerClassName = "BrandStore")]
public class BrandStoreUpdateRequestStoreBusiness
{
    /// <summary>【门店服务电话】 支持座机和手机，只支持数字和“-”符号，最多支持两个电话，两个电话间用英文竖线“&amp;#124;”区隔。 此字段为免审字段（仅修改免审字段时，将会直接更新门店，无需审核）（官方字段路径 <c>store_business.service_phone</c>，选填，类型 <c>string(32)</c>）。</summary>
    [JsonPropertyName("service_phone")]
    public string? ServicePhone { get; set; }

    /// <summary>【门店经营时间】 经营时间需要使用指定格式，如下： 最多支持 7 个时间段，时间段间用英文竖线"｜"分隔，严格按 “周一至周五 09:00-20:00&amp;#124;周六至周日 10:00-22:00” 的格式进行填写； 支持填写跨日经营时间，需在终止时间前添加“次日”，严格参照“周一至周五 09:00-次日2:00” 格式填写，跨日周期不得超过 24 小时； 此字段为免审字段（仅修改免审字段时，将会直接更新门店，无需审核）（官方字段路径 <c>store_business.business_hours</c>，选填，类型 <c>string(256)</c>）。</summary>
    [JsonPropertyName("business_hours")]
    public string? BusinessHours { get; set; }

}

/// <summary>品牌门店更新请求。官方页：<see href="https://pay.weixin.qq.com/doc/v3/partner/4015783158"/></summary>
[HttpJsonSerializable(SerializerClassName = "BrandStore")]
public class BrandStoreUpdateRequest
{
    /// <summary>【品牌ID】 商家进驻微信支付品牌商家后获得的品牌ID（官方字段路径 <c>brand_id</c>，<b>必填</b>，类型 <c>string</c>）。</summary>
    [JsonPropertyName("brand_id")]
    public string? BrandId { get; set; }

    /// <summary>【门店基础信息】 用于描述门店编码，名称等基本情况（官方字段路径 <c>store_basics</c>，选填，类型 <c>object</c>）。</summary>
    [JsonPropertyName("store_basics")]
    public BrandStoreUpdateRequestStoreBasics? StoreBasics { get; set; }

    /// <summary>【门店地址信息】 用于描述门店地址，经纬度等地理位置相关情况（官方字段路径 <c>store_address</c>，选填，类型 <c>object</c>）。</summary>
    [JsonPropertyName("store_address")]
    public BrandStoreUpdateRequestStoreAddress? StoreAddress { get; set; }

    /// <summary>【门店经营信息】 用于描述门店联系电话，经营时间等经营状况（官方字段路径 <c>store_business</c>，选填，类型 <c>object</c>）。</summary>
    [JsonPropertyName("store_business")]
    public BrandStoreUpdateRequestStoreBusiness? StoreBusiness { get; set; }

}

/// <summary>品牌门店删除请求。官方页：<see href="https://pay.weixin.qq.com/doc/v3/partner/4015783113"/></summary>
[HttpJsonSerializable(SerializerClassName = "BrandStore")]
public class BrandStoreDeleteRequest
{
    /// <summary>【品牌ID】 商家进驻微信支付品牌商家后获得的品牌ID（官方字段路径 <c>brand_id</c>，<b>必填</b>，类型 <c>string</c>）。</summary>
    [JsonPropertyName("brand_id")]
    public string? BrandId { get; set; }

}

/// <summary>门店收款商户号绑定请求。官方页：<see href="https://pay.weixin.qq.com/doc/v3/partner/4015783177"/></summary>
[HttpJsonSerializable(SerializerClassName = "BrandStore")]
public class BrandStoreBindRecipientRequest
{
    /// <summary>【品牌ID】 商家进驻微信支付品牌商家后获得的品牌ID（官方字段路径 <c>brand_id</c>，<b>必填</b>，类型 <c>string</c>）。</summary>
    [JsonPropertyName("brand_id")]
    public string? BrandId { get; set; }

    /// <summary>【门店收款商户号】 门店的收款商户号，仅允许填入品牌已关联的商户号，若未关联请先前往“品牌经营平台-账号管理-收款商户号管理”添加商户号（官方字段路径 <c>mchid</c>，<b>必填</b>，类型 <c>string(16)</c>）。</summary>
    [JsonPropertyName("mchid")]
    public string? Mchid { get; set; }

    /// <summary>【门店收款主体】 门店收款的主体信息，支持企业，个体户，小微（官方字段路径 <c>company_name</c>，<b>必填</b>，类型 <c>string(256)</c>）。</summary>
    [JsonPropertyName("company_name")]
    public string? CompanyName { get; set; }

}

/// <summary>门店收款商户号绑定响应。官方页：<see href="https://pay.weixin.qq.com/doc/v3/partner/4015783177"/></summary>
[HttpJsonSerializable(SerializerClassName = "BrandStore")]
public class BrandStoreBindRecipientResponse : WechatPayResponse
{
    /// <summary>【品牌ID】 商家进驻微信支付品牌商家后获得的品牌ID（官方字段路径 <c>brand_id</c>，选填，类型 <c>string</c>）。</summary>
    [JsonPropertyName("brand_id")]
    public string? BrandId { get; set; }

    /// <summary>【品牌门店ID】 创建品牌门店后，系统为该门店分配的唯一ID（官方字段路径 <c>store_id</c>，选填，类型 <c>string</c>）。</summary>
    [JsonPropertyName("store_id")]
    public string? StoreId { get; set; }

    /// <summary>【门店收款商户号】 门店的收款商户号，仅支持绑定品牌已关联的商户号（官方字段路径 <c>mchid</c>，选填，类型 <c>string(16)</c>）。</summary>
    [JsonPropertyName("mchid")]
    public string? Mchid { get; set; }

    /// <summary>【门店收款主体】 门店收款的主体信息，支持企业，个体户，小微（官方字段路径 <c>company_name</c>，选填，类型 <c>string(256)</c>）。</summary>
    [JsonPropertyName("company_name")]
    public string? CompanyName { get; set; }

    /// <summary>【收款绑定状态】 门店收款商户号的绑定状态。可选取值：`CONFIRMED` 门店已绑定此收款商户号 / `ADMIN_REJECTED` 商户管理员已拒绝 / `CONFIRMING` 商户号管理员确认中 / `TIMEOUT_REJECTED` 由于商户管理员确认超时，系统自动拒绝此绑定（官方字段路径 <c>recipient_state</c>，选填，类型 <c>string</c>）。</summary>
    [JsonPropertyName("recipient_state")]
    public string? RecipientState { get; set; }

}

/// <summary>门店收款商户号解绑请求。官方页：<see href="https://pay.weixin.qq.com/doc/v3/partner/4015783188"/></summary>
[HttpJsonSerializable(SerializerClassName = "BrandStore")]
public class BrandStoreUnbindRecipientRequest
{
    /// <summary>【品牌ID】 商家进驻微信支付品牌商家后获得的品牌ID（官方字段路径 <c>brand_id</c>，<b>必填</b>，类型 <c>string</c>）。</summary>
    [JsonPropertyName("brand_id")]
    public string? BrandId { get; set; }

    /// <summary>【门店收款商户号】 希望解绑的门店收款商户号，仅支持解绑收款绑定状态为“已绑定”的商户号（官方字段路径 <c>mchid</c>，<b>必填</b>，类型 <c>string(16)</c>）。</summary>
    [JsonPropertyName("mchid")]
    public string? Mchid { get; set; }

}

/// <summary>门店收款商户号解绑响应。官方页：<see href="https://pay.weixin.qq.com/doc/v3/partner/4015783188"/></summary>
[HttpJsonSerializable(SerializerClassName = "BrandStore")]
public class BrandStoreUnbindRecipientResponse : WechatPayResponse
{
    /// <summary>【解绑失败原因】 解绑失败原因（官方字段路径 <c>failed_reason</c>，选填，类型 <c>string</c>）。</summary>
    [JsonPropertyName("failed_reason")]
    public string? FailedReason { get; set; }

}

/// <summary>暂停门店营业请求。官方页：<see href="https://pay.weixin.qq.com/doc/v3/partner/4016756671"/></summary>
[HttpJsonSerializable(SerializerClassName = "BrandStore")]
public class BrandStoreCloseRequest
{
    /// <summary>【品牌ID】 商家进驻微信支付品牌商家后获得的品牌ID（官方字段路径 <c>brand_id</c>，<b>必填</b>，类型 <c>string</c>）。</summary>
    [JsonPropertyName("brand_id")]
    public string? BrandId { get; set; }

}

/// <summary>暂停门店营业响应。官方页：<see href="https://pay.weixin.qq.com/doc/v3/partner/4016756671"/></summary>
[HttpJsonSerializable(SerializerClassName = "BrandStore")]
public class BrandStoreStateResponse : WechatPayResponse
{
    /// <summary>【品牌ID】 商家进驻微信支付品牌商家后获得的品牌ID（官方字段路径 <c>brand_id</c>，<b>必填</b>，类型 <c>string</c>）。</summary>
    [JsonPropertyName("brand_id")]
    public string? BrandId { get; set; }

    /// <summary>【品牌门店ID】 创建品牌门店后，系统为该门店分配的唯一ID（官方字段路径 <c>store_id</c>，选填，类型 <c>string</c>）。</summary>
    [JsonPropertyName("store_id")]
    public string? StoreId { get; set; }

    /// <summary>【门店状态】 用于描述门店当前状态。可选取值：`OPEN` 门店生效中。门店审核通过创建成功后即为生效中，后续更新门店信息只会影响审核状态，不会改变门店状态，如门店暂停营业可调用“暂停门店营业 API”，如门店关闭可调用“删除品牌门店 API”。门店营业时间不影响门店状态。 / `CREATING` 门店创建中。在创建门店后，门店资料正在审核。审核详情请查看审核状态。 / `CLOSED` 门店停业中。不可用于微信支付生态中的其他业务，可删除该门店，删除后将无法恢复，请谨慎操作。（官方字段路径 <c>store_state</c>，选填，类型 <c>string</c>）。</summary>
    [JsonPropertyName("store_state")]
    public string? StoreState { get; set; }

}

/// <summary>恢复门店营业请求。官方页：<see href="https://pay.weixin.qq.com/doc/v3/partner/4016756672"/></summary>
[HttpJsonSerializable(SerializerClassName = "BrandStore")]
public class BrandStoreResumeRequest
{
    /// <summary>【品牌ID】 商家进驻微信支付品牌商家后获得的品牌ID（官方字段路径 <c>brand_id</c>，<b>必填</b>，类型 <c>string</c>）。</summary>
    [JsonPropertyName("brand_id")]
    public string? BrandId { get; set; }

}
