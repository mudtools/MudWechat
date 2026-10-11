// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

namespace Mud.Wechat.MiniProgram.DataModels.Express;

/// <summary>
/// 物流助手「寄件人 / 收件人」地址信息（<c>sender</c> / <c>receiver</c> 内嵌对象，
/// 商家寄件域 <c>order/add</c> 与面单预览 <c>template/preview</c> 复用）。
/// </summary>
/// <remarks>
/// <para>官方字段以 <c>express/business</c>（商家寄件）各页「Body 对象 Payload」为准（2026-10-11 逐页核验）。</para>
/// <para><c>tel</c> 与 <c>mobile</c> 二选一必填（官方原文：若不填写则必须填写另一个，不超过 32 字节）；姓名不超过 64 字节、地址不超过 512 字节。</para>
/// </remarks>
[HttpJsonSerializable(SerializerClassName = "Express")]
public class WxaExpressContact
{
    /// <summary>发/收件人姓名（<c>name</c>，不超过 64 字节）。</summary>
    [JsonPropertyName("name")]
    public string? Name { get; set; }

    /// <summary>发/收件人座机号码（<c>tel</c>，与 <c>mobile</c> 二选一，不超过 32 字节）。</summary>
    [JsonPropertyName("tel")]
    public string? TelephoneNumber { get; set; }

    /// <summary>发/收件人手机号码（<c>mobile</c>，与 <c>tel</c> 二选一，不超过 32 字节）。</summary>
    [JsonPropertyName("mobile")]
    public string? MobileNumber { get; set; }

    /// <summary>发/收件人公司名称（<c>company</c>，不超过 64 字节）。</summary>
    [JsonPropertyName("company")]
    public string? Company { get; set; }

    /// <summary>发/收件人邮编（<c>post_code</c>，不超过 10 字节）。</summary>
    [JsonPropertyName("post_code")]
    public string? PostCode { get; set; }

    /// <summary>发/收件人国家（<c>country</c>，不超过 64 字节）。</summary>
    [JsonPropertyName("country")]
    public string? Country { get; set; }

    /// <summary>发/收件人省份（<c>province</c>，例如“广东省”，不超过 64 字节）。</summary>
    [JsonPropertyName("province")]
    public string? Province { get; set; }

    /// <summary>发/收件人市/地区（<c>city</c>，例如“广州市”，不超过 64 字节）。</summary>
    [JsonPropertyName("city")]
    public string? City { get; set; }

    /// <summary>发/收件人区/县（<c>area</c>，例如“海珠区”，不超过 64 字节）。</summary>
    [JsonPropertyName("area")]
    public string? Area { get; set; }

    /// <summary>发/收件人详细地址（<c>address</c>，不超过 512 字节）。</summary>
    [JsonPropertyName("address")]
    public string? Address { get; set; }
}

/// <summary>包裹信息（<c>cargo</c>，将传递给快递公司）。</summary>
/// <remarks>官方字段以商家寄件 <c>order/add</c> 的「Body.cargo Object Payload」为准。</remarks>
[HttpJsonSerializable(SerializerClassName = "Express")]
public class WxaExpressCargo
{
    /// <summary>包裹数量（<c>count</c>，必填）。</summary>
    [JsonPropertyName("count")]
    public int Count { get; set; }

    /// <summary>包裹长度（<c>space_x</c>，单位：厘米）。</summary>
    [JsonPropertyName("space_x")]
    public double? Length { get; set; }

    /// <summary>包裹宽度（<c>space_y</c>，单位：厘米）。</summary>
    [JsonPropertyName("space_y")]
    public double? Width { get; set; }

    /// <summary>包裹高度（<c>space_z</c>，单位：厘米）。</summary>
    [JsonPropertyName("space_z")]
    public double? Height { get; set; }

    /// <summary>包裹重量（<c>weight</c>，单位：千克，精确到小数点后 2 位）。</summary>
    [JsonPropertyName("weight")]
    public double? Weight { get; set; }

    /// <summary>包裹中商品详情列表（<c>detail_list</c>）。</summary>
    [JsonPropertyName("detail_list")]
    public List<WxaExpressCargoGood>? GoodsList { get; set; }
}

/// <summary>包裹商品详情项（<c>cargo.detail_list</c> 元素）。</summary>
[HttpJsonSerializable(SerializerClassName = "Express")]
public class WxaExpressCargoGood
{
    /// <summary>商品名（<c>name</c>）。</summary>
    [JsonPropertyName("name")]
    public string? Name { get; set; }

    /// <summary>商品数量（<c>count</c>）。</summary>
    [JsonPropertyName("count")]
    public int Count { get; set; }
}

/// <summary>商品信息（<c>shop</c>，会展示到物流服务通知和电子面单中）。</summary>
/// <remarks>官方字段以商家寄件 <c>order/add</c> 的「Body.shop Object Payload」为准。</remarks>
[HttpJsonSerializable(SerializerClassName = "Express")]
public class WxaExpressShop
{
    /// <summary>商家小程序的路径（<c>wxa_path</c>，必填）。</summary>
    [JsonPropertyName("wxa_path")]
    public string? MiniProgramPath { get; set; }

    /// <summary>商品缩略图 URL（<c>img_url</c>）。</summary>
    [JsonPropertyName("img_url")]
    public string? ImageUrl { get; set; }

    /// <summary>商品名称（<c>goods_name</c>）。</summary>
    [JsonPropertyName("goods_name")]
    public string? GoodsName { get; set; }

    /// <summary>商品数量（<c>goods_count</c>）。</summary>
    [JsonPropertyName("goods_count")]
    public int? GoodsCount { get; set; }

    /// <summary>商品详情列表（<c>detail_list</c>）。</summary>
    [JsonPropertyName("detail_list")]
    public List<WxaExpressShopGood>? GoodsList { get; set; }
}

/// <summary>商品详情项（<c>shop.detail_list</c> 元素）。</summary>
[HttpJsonSerializable(SerializerClassName = "Express")]
public class WxaExpressShopGood
{
    /// <summary>商品名（<c>goods_name</c>）。</summary>
    [JsonPropertyName("goods_name")]
    public string? Name { get; set; }

    /// <summary>商品缩略图 URL（<c>goods_img_url</c>）。</summary>
    [JsonPropertyName("goods_img_url")]
    public string? ImageUrl { get; set; }

    /// <summary>商品详情描述（<c>goods_desc</c>）。</summary>
    [JsonPropertyName("goods_desc")]
    public string? Description { get; set; }
}

/// <summary>保价信息（<c>insured</c>）。</summary>
/// <remarks>官方字段以商家寄件 <c>order/add</c> 的「Body.insured Object Payload」为准。</remarks>
[HttpJsonSerializable(SerializerClassName = "Express")]
public class WxaExpressInsured
{
    /// <summary>是否保价（<c>use_insured</c>，必填；值为 1 时承运方须按 <see cref="InsuredValue"/> 进行保价）。</summary>
    [JsonPropertyName("use_insured")]
    public bool IsUseInsured { get; set; }

    /// <summary>保价金额（<c>insured_value</c>，单位：分；<c>use_insured</c> 为 1 时必填）。</summary>
    [JsonPropertyName("insured_value")]
    public int InsuredValue { get; set; }
}

/// <summary>服务类型（<c>service</c>，顺丰必填，其它快递公司选填）。</summary>
/// <remarks>官方字段以商家寄件 <c>order/add</c> 的「Body.service Object Payload」为准。</remarks>
[HttpJsonSerializable(SerializerClassName = "Express")]
public class WxaExpressService
{
    /// <summary>服务类型 ID（<c>service_type</c>，必填；取值见各快递公司面单服务类型）。</summary>
    [JsonPropertyName("service_type")]
    public int ServiceType { get; set; }

    /// <summary>服务类型名称（<c>service_name</c>，必填；仅展示，例如“标准快递”）。</summary>
    [JsonPropertyName("service_name")]
    public string? ServiceName { get; set; }
}

/// <summary>面单数据项（<c>waybill_data</c> 数组元素，键值对）。</summary>
/// <remarks>键值集合由快递公司定义（各运力不一），SDK 不做本地校验，原样透传。</remarks>
[HttpJsonSerializable(SerializerClassName = "Express")]
public class WxaExpressWaybillDataItem
{
    /// <summary>面单数据键（<c>key</c>）。</summary>
    [JsonPropertyName("key")]
    public string? Key { get; set; }

    /// <summary>面单数据值（<c>value</c>）。</summary>
    [JsonPropertyName("value")]
    public string? Value { get; set; }
}