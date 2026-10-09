// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

using Mud.Wechat.Pay.DataModels.Common;

namespace Mud.Wechat.Pay.DataModels.Fapiao;

/// <summary>
/// 开具电子发票（<c>POST /v3/new-tax-control-fapiao/fapiao-applications</c>）。
/// </summary>
/// <remarks>
/// <para>
/// <b>官方文档</b>：<see href="https://pay.weixin.qq.com/doc/v3/merchant/4012538301"/>
/// （2026-10-09 逐字段核验；更新时间 2025.09.26）。<b>支持商户：【普通商户】</b>。
/// </para>
/// <para>
/// <b>⚠️ 更正一处方案记录</b>：设计方案 §2.5 曾记「普通商户文档中心<b>无</b>独立入口」——
/// 实测<b>不成立</b>：普通商户侧有完整电子发票文档（本页即其一）；服务商侧另有
/// 「开具<b>通用行业</b>电子发票」走 <c>/fapiao-applications/<b>issue-general</b></c>（<b>不同路由</b>）。
/// </para>
/// <para>
/// <b>无应答包体</b>：官方标注应答状态 <b>202 Accepted</b> 且<b>无任何应答字段</b> ⇒ 接口方法返回 <c>Task</c>。
/// 官方原文：本接口成功返回<b>仅代表开票请求已被受理</b>，开票完成须经回调通知或「查询电子发票」获取。
/// </para>
/// </remarks>
[HttpJsonSerializable(SerializerClassName = "NewTaxControlFapiao")]
public class FapiaoIssueRequest
{
    /// <summary>场景（<c>scene</c>，必填 string）：开票场景标识（值域未在本轮核验）。</summary>
    [JsonPropertyName("scene")]
    public string? Scene { get; set; }

    /// <summary>发票申请单号（<c>fapiao_apply_id</c>，必填 string(32)）：<b>业务幂等键</b>（查询接口即以此为 path）。</summary>
    [JsonPropertyName("fapiao_apply_id")]
    public string? FapiaoApplyId { get; set; }

    /// <summary>购买方信息（<c>buyer_information</c>，必填），见 <see cref="FapiaoBuyerInformation"/>。</summary>
    [JsonPropertyName("buyer_information")]
    public FapiaoBuyerInformation? BuyerInformation { get; set; }

    /// <summary>发票信息（<c>fapiao_information</c>，必填数组），见 <see cref="FapiaoIssueInformation"/>。</summary>
    [JsonPropertyName("fapiao_information")]
    public List<FapiaoIssueInformation>? FapiaoInformation { get; set; }
}

/// <summary>
/// 购买方信息（<c>buyer_information</c>）—— 开具请求与查询应答<b>字段表一致 ⇒ 共用</b>。
/// </summary>
[HttpJsonSerializable(SerializerClassName = "NewTaxControlFapiao")]
public class FapiaoBuyerInformation
{
    /// <summary>购买方类型（<c>type</c>）：如个人 / 企业（值域未在本轮核验）。</summary>
    [JsonPropertyName("type")]
    public string? Type { get; set; }

    /// <summary>购买方名称（<c>name</c>，string(256)）。</summary>
    [JsonPropertyName("name")]
    public string? Name { get; set; }

    /// <summary>纳税人识别号（<c>taxpayer_id</c>，选填 string(32)）。</summary>
    [JsonPropertyName("taxpayer_id")]
    public string? TaxpayerId { get; set; }

    /// <summary>地址（<c>address</c>，选填 string(128)）。</summary>
    [JsonPropertyName("address")]
    public string? Address { get; set; }

    /// <summary>电话（<c>telephone</c>，选填 string(32)）。</summary>
    [JsonPropertyName("telephone")]
    public string? Telephone { get; set; }

    /// <summary>开户行（<c>bank_name</c>，选填 string(128)）。</summary>
    [JsonPropertyName("bank_name")]
    public string? BankName { get; set; }

    /// <summary>银行账号（<c>bank_account</c>，选填 string(32)）。</summary>
    [JsonPropertyName("bank_account")]
    public string? BankAccount { get; set; }

    /// <summary>手机号（<c>phone</c>，选填）：用于发票交付。</summary>
    [JsonPropertyName("phone")]
    public string? Phone { get; set; }

    /// <summary>邮箱（<c>email</c>，选填）：用于发票交付。</summary>
    [JsonPropertyName("email")]
    public string? Email { get; set; }
}

/// <summary>开具请求的单张发票信息（<c>fapiao_information[]</c>）。</summary>
[HttpJsonSerializable(SerializerClassName = "NewTaxControlFapiao")]
public class FapiaoIssueInformation
{
    /// <summary>商户发票单号（<c>fapiao_id</c>，必填 string(32)）：<b>同一申请单下的发票唯一标识</b>（查询接口可选按它过滤）。</summary>
    [JsonPropertyName("fapiao_id")]
    public string? FapiaoId { get; set; }

    /// <summary>发票总金额（<c>total_amount</c>，必填 integer，单位分）。</summary>
    [JsonPropertyName("total_amount")]
    public long? TotalAmount { get; set; }

    /// <summary>是否要明细（<c>need_list</c>，选填 bool）。</summary>
    [JsonPropertyName("need_list")]
    public bool? NeedList { get; set; }

    /// <summary>备注（<c>remark</c>，选填 string(200)）。</summary>
    [JsonPropertyName("remark")]
    public string? Remark { get; set; }

    /// <summary>发票明细（<c>items</c>，必填数组），见 <see cref="FapiaoIssueItem"/>。</summary>
    [JsonPropertyName("items")]
    public List<FapiaoIssueItem>? Items { get; set; }
}

/// <summary>开具请求的发票明细项（<c>fapiao_information[].items[]</c>）。</summary>
[HttpJsonSerializable(SerializerClassName = "NewTaxControlFapiao")]
public class FapiaoIssueItem
{
    /// <summary>税收分类编码（<c>tax_code</c>，必填 string(32)）。</summary>
    [JsonPropertyName("tax_code")]
    public string? TaxCode { get; set; }

    /// <summary>商品分类（<c>goods_category</c>，选填 string(128)）。</summary>
    [JsonPropertyName("goods_category")]
    public string? GoodsCategory { get; set; }

    /// <summary>商品名称（<c>goods_name</c>，选填 string(128)）。</summary>
    [JsonPropertyName("goods_name")]
    public string? GoodsName { get; set; }

    /// <summary>商品编码（<c>goods_id</c>，选填 integer）。</summary>
    [JsonPropertyName("goods_id")]
    public long? GoodsId { get; set; }

    /// <summary>规格型号（<c>specification</c>，选填 string(20)）。</summary>
    [JsonPropertyName("specification")]
    public string? Specification { get; set; }

    /// <summary>单位（<c>unit</c>，选填 string(20)）。</summary>
    [JsonPropertyName("unit")]
    public string? Unit { get; set; }

    /// <summary>数量（<c>quantity</c>，必填 integer）。</summary>
    [JsonPropertyName("quantity")]
    public long? Quantity { get; set; }

    /// <summary>金额（<c>total_amount</c>，必填 integer，单位分）。</summary>
    [JsonPropertyName("total_amount")]
    public long? TotalAmount { get; set; }

    /// <summary>税率（<c>tax_rate</c>，选填 integer）。</summary>
    [JsonPropertyName("tax_rate")]
    public long? TaxRate { get; set; }

    /// <summary>税收优惠标识（<c>tax_prefer_mark</c>，选填 string）。</summary>
    [JsonPropertyName("tax_prefer_mark")]
    public string? TaxPreferMark { get; set; }

    /// <summary>是否优惠（<c>discount</c>，<b>必填</b> bool，官方本页标为必填）。</summary>
    [JsonPropertyName("discount")]
    public bool? Discount { get; set; }
}

/// <summary>
/// 查询电子发票应答（<c>GET /v3/new-tax-control-fapiao/fapiao-applications/{fapiao_apply_id}</c>）。
/// </summary>
/// <remarks>
/// <b>官方文档</b>：<see href="https://pay.weixin.qq.com/docs/merchant/apis/fapiao/fapiao-applications/get-fapiao-applications.html"/>
/// （2026-10-09 逐字段核验；更新时间 2025.09.26；支持商户：普通商户）。官方原文建议：
/// 「【将电子发票插入微信用户卡包】接口成功后，应调用本接口查询电子发票开票结果」。
/// </remarks>
[HttpJsonSerializable(SerializerClassName = "NewTaxControlFapiao")]
public class FapiaoQueryResponse : WechatPayResponse
{
    /// <summary>发票总数（<c>total_count</c>）。</summary>
    [JsonPropertyName("total_count")]
    public long? TotalCount { get; set; }

    /// <summary>发票信息列表（<c>fapiao_information</c>），见 <see cref="FapiaoQueryInformation"/>。</summary>
    [JsonPropertyName("fapiao_information")]
    public List<FapiaoQueryInformation>? FapiaoInformation { get; set; }
}

/// <summary>查询应答的单张发票信息（<c>fapiao_information[]</c>）。</summary>
[HttpJsonSerializable(SerializerClassName = "NewTaxControlFapiao")]
public class FapiaoQueryInformation
{
    /// <summary>商户发票单号（<c>fapiao_id</c>）。</summary>
    [JsonPropertyName("fapiao_id")]
    public string? FapiaoId { get; set; }

    /// <summary>发票状态（<c>status</c>）：<b>值域未在本轮核验</b>，勿臆造取值判定。</summary>
    [JsonPropertyName("status")]
    public string? Status { get; set; }

    /// <summary>蓝票信息（<c>blue_fapiao</c>），见 <see cref="FapiaoBlueRedInformation"/>。</summary>
    [JsonPropertyName("blue_fapiao")]
    public FapiaoBlueRedInformation? BlueFapiao { get; set; }

    /// <summary>红票信息（<c>red_fapiao</c>），见 <see cref="FapiaoBlueRedInformation"/>。</summary>
    [JsonPropertyName("red_fapiao")]
    public FapiaoBlueRedInformation? RedFapiao { get; set; }

    /// <summary>卡包信息（<c>card_information</c>），见 <see cref="FapiaoCardInformation"/>。</summary>
    [JsonPropertyName("card_information")]
    public FapiaoCardInformation? CardInformation { get; set; }

    /// <summary>价税合计（<c>total_amount</c>，整型，单位分）。</summary>
    [JsonPropertyName("total_amount")]
    public long? TotalAmount { get; set; }

    /// <summary>税额（<c>tax_amount</c>，整型，单位分）。</summary>
    [JsonPropertyName("tax_amount")]
    public long? TaxAmount { get; set; }

    /// <summary>金额（不含税，<c>amount</c>，整型，单位分）。</summary>
    [JsonPropertyName("amount")]
    public long? Amount { get; set; }

    /// <summary>销售方信息（<c>seller_information</c>），见 <see cref="FapiaoSellerInformation"/>。</summary>
    [JsonPropertyName("seller_information")]
    public FapiaoSellerInformation? SellerInformation { get; set; }

    /// <summary>购买方信息（<c>buyer_information</c>）：与开具请求<b>字段表一致</b> ⇒ 复用 <see cref="FapiaoBuyerInformation"/>。</summary>
    [JsonPropertyName("buyer_information")]
    public FapiaoBuyerInformation? BuyerInformation { get; set; }

    /// <summary>附加信息（<c>extra_information</c>），见 <see cref="FapiaoExtraInformation"/>。</summary>
    [JsonPropertyName("extra_information")]
    public FapiaoExtraInformation? ExtraInformation { get; set; }

    /// <summary>发票明细（<c>items</c>），见 <see cref="FapiaoQueryItem"/>。</summary>
    [JsonPropertyName("items")]
    public List<FapiaoQueryItem>? Items { get; set; }

    /// <summary>备注（<c>remark</c>）：官方渲染中位于 <c>items</c> 之后、与本层级同级。</summary>
    [JsonPropertyName("remark")]
    public string? Remark { get; set; }
}

/// <summary>蓝票 / 红票信息（<c>blue_fapiao</c> / <c>red_fapiao</c>，官方两处字段表一致 ⇒ 共用）。</summary>
[HttpJsonSerializable(SerializerClassName = "NewTaxControlFapiao")]
public class FapiaoBlueRedInformation
{
    /// <summary>发票代码（<c>fapiao_code</c>）。</summary>
    [JsonPropertyName("fapiao_code")]
    public string? FapiaoCode { get; set; }

    /// <summary>发票号码（<c>fapiao_number</c>）。</summary>
    [JsonPropertyName("fapiao_number")]
    public string? FapiaoNumber { get; set; }

    /// <summary>校验码（<c>check_code</c>）。</summary>
    [JsonPropertyName("check_code")]
    public string? CheckCode { get; set; }

    /// <summary>密码区（<c>password</c>）。</summary>
    [JsonPropertyName("password")]
    public string? Password { get; set; }

    /// <summary>开票时间（<c>fapiao_time</c>）。</summary>
    [JsonPropertyName("fapiao_time")]
    public string? FapiaoTime { get; set; }
}

/// <summary>发票卡包信息（<c>card_information</c>）。</summary>
[HttpJsonSerializable(SerializerClassName = "NewTaxControlFapiao")]
public class FapiaoCardInformation
{
    /// <summary>卡券 AppID（<c>card_appid</c>）。</summary>
    [JsonPropertyName("card_appid")]
    public string? CardAppId { get; set; }

    /// <summary>卡券持有者 OpenID（<c>card_openid</c>）。</summary>
    [JsonPropertyName("card_openid")]
    public string? CardOpenId { get; set; }

    /// <summary>卡券 ID（<c>card_id</c>）。</summary>
    [JsonPropertyName("card_id")]
    public string? CardId { get; set; }

    /// <summary>卡券 code（<c>card_code</c>）。</summary>
    [JsonPropertyName("card_code")]
    public string? CardCode { get; set; }

    /// <summary>卡券状态（<c>card_status</c>）：值域未在本轮核验。</summary>
    [JsonPropertyName("card_status")]
    public string? CardStatus { get; set; }
}

/// <summary>销售方信息（<c>seller_information</c>）。</summary>
[HttpJsonSerializable(SerializerClassName = "NewTaxControlFapiao")]
public class FapiaoSellerInformation
{
    /// <summary>销售方名称（<c>name</c>）。</summary>
    [JsonPropertyName("name")]
    public string? Name { get; set; }

    /// <summary>纳税人识别号（<c>taxpayer_id</c>）。</summary>
    [JsonPropertyName("taxpayer_id")]
    public string? TaxpayerId { get; set; }

    /// <summary>地址（<c>address</c>）。</summary>
    [JsonPropertyName("address")]
    public string? Address { get; set; }

    /// <summary>电话（<c>telephone</c>）。</summary>
    [JsonPropertyName("telephone")]
    public string? Telephone { get; set; }

    /// <summary>开户行（<c>bank_name</c>）。</summary>
    [JsonPropertyName("bank_name")]
    public string? BankName { get; set; }

    /// <summary>银行账号（<c>bank_account</c>）。</summary>
    [JsonPropertyName("bank_account")]
    public string? BankAccount { get; set; }
}

/// <summary>附加信息（<c>extra_information</c>）。</summary>
[HttpJsonSerializable(SerializerClassName = "NewTaxControlFapiao")]
public class FapiaoExtraInformation
{
    /// <summary>收款人（<c>payee</c>）。</summary>
    [JsonPropertyName("payee")]
    public string? Payee { get; set; }

    /// <summary>复核人（<c>reviewer</c>）。</summary>
    [JsonPropertyName("reviewer")]
    public string? Reviewer { get; set; }

    /// <summary>开票人（<c>drawer</c>）。</summary>
    [JsonPropertyName("drawer")]
    public string? Drawer { get; set; }
}

/// <summary>查询应答的发票明细项（<c>fapiao_information[].items[]</c>）。</summary>
/// <remarks>
/// <b>与开具请求的明细项不是同一张表</b>：应答版多出 <c>unit_price</c> / <c>amount</c> / <c>tax_amount</c>
/// 且 <c>discount</c> 未标必填、<c>goods_category</c>/<c>goods_id</c>/<c>need_list</c> 不在表内
/// ⇒ 独立类型（同一份事实两处类型会漂移）。
/// </remarks>
[HttpJsonSerializable(SerializerClassName = "NewTaxControlFapiao")]
public class FapiaoQueryItem
{
    /// <summary>税收分类编码（<c>tax_code</c>）。</summary>
    [JsonPropertyName("tax_code")]
    public string? TaxCode { get; set; }

    /// <summary>商品名称（<c>goods_name</c>）。</summary>
    [JsonPropertyName("goods_name")]
    public string? GoodsName { get; set; }

    /// <summary>规格型号（<c>specification</c>）。</summary>
    [JsonPropertyName("specification")]
    public string? Specification { get; set; }

    /// <summary>单位（<c>unit</c>）。</summary>
    [JsonPropertyName("unit")]
    public string? Unit { get; set; }

    /// <summary>数量（<c>quantity</c>）。</summary>
    [JsonPropertyName("quantity")]
    public long? Quantity { get; set; }

    /// <summary>单价（<c>unit_price</c>，整型，单位分）：<b>仅应答版有</b>。</summary>
    [JsonPropertyName("unit_price")]
    public long? UnitPrice { get; set; }

    /// <summary>金额（<c>amount</c>，整型，单位分）：<b>仅应答版有</b>。</summary>
    [JsonPropertyName("amount")]
    public long? Amount { get; set; }

    /// <summary>税额（<c>tax_amount</c>，整型，单位分）：<b>仅应答版有</b>。</summary>
    [JsonPropertyName("tax_amount")]
    public long? TaxAmount { get; set; }

    /// <summary>价税合计（<c>total_amount</c>，整型，单位分）。</summary>
    [JsonPropertyName("total_amount")]
    public long? TotalAmount { get; set; }

    /// <summary>税率（<c>tax_rate</c>）。</summary>
    [JsonPropertyName("tax_rate")]
    public long? TaxRate { get; set; }

    /// <summary>税收优惠标识（<c>tax_prefer_mark</c>）。</summary>
    [JsonPropertyName("tax_prefer_mark")]
    public string? TaxPreferMark { get; set; }

    /// <summary>是否优惠（<c>discount</c>，bool）：应答版未标必填。</summary>
    [JsonPropertyName("discount")]
    public bool? Discount { get; set; }
}

/// <summary>
/// 冲红电子发票请求（<c>POST /v3/new-tax-control-fapiao/fapiao-applications/{fapiao_apply_id}/reverse</c>）。
/// </summary>
/// <remarks>
/// <para>
/// <b>官方文档</b>：<see href="https://pay.weixin.qq.com/docs/merchant/apis/fapiao/fapiao-applications/reverse-fapiao-applications.html"/>
/// （2026-10-09 逐字段核验；更新时间 2025.09.26）。<b>支持商户：【普通商户】</b>。
/// </para>
/// <para>
/// <b>本接口无应答包体</b>（202 Accepted）⇒ 冲红结果只能经回调通知或
/// <see cref="FapiaoQueryResponse"/> 获取。
/// </para>
/// </remarks>
[HttpJsonSerializable(SerializerClassName = "NewTaxControlFapiao")]
public class FapiaoReverseRequest
{
    /// <summary>冲红原因（<c>reverse_reason</c>，必填 string(200)）。</summary>
    [JsonPropertyName("reverse_reason")]
    public string? ReverseReason { get; set; }

    /// <summary>
    /// 需要冲红的发票信息（<c>fapiao_information</c>，<b>选填</b> array），见 <see cref="FapiaoReverseInformation"/>。
    /// </summary>
    /// <remarks>官方标注为<b>选填</b>（与开具接口的 <c>fapiao_information</c> 必填性不同）—— 勿照搬。</remarks>
    [JsonPropertyName("fapiao_information")]
    public List<FapiaoReverseInformation>? FapiaoInformation { get; set; }
}

/// <summary>
/// 冲红目标发票（<c>fapiao_information</c> 项）—— <b>三字段</b>，与其它信息类不可混用。
/// </summary>
/// <remarks>
/// <para>
/// <b>为何不复用 <c>FapiaoBlueRedInformation</c></b>：蓝红票信息是<b>五</b>字段
/// （<c>fapiao_code</c> / <c>fapiao_number</c> / <c>check_code</c> / <c>password</c> / <c>fapiao_time</c>），
/// 而冲红需要的是<b>三</b>字段（<c>fapiao_id</c> / <c>fapiao_code</c> / <c>fapiao_number</c>）——
/// 前者<b>不能</b>定位要冲红的发票（缺 <c>fapiao_id</c>），后者也不该带上无关字段。
/// </para>
/// <para>
/// <b>重试约定</b>：冲红失败重试时须沿用<b>相同</b>的 <c>fapiao_id</c>（官方原文，重试间隔 5 分钟）。
/// </para>
/// </remarks>
[HttpJsonSerializable(SerializerClassName = "NewTaxControlFapiao")]
public class FapiaoReverseInformation
{
    /// <summary>商户发票单号（<c>fapiao_id</c>，必填 string(32)）：须与该发票开票时提交的值<b>一致</b>。</summary>
    [JsonPropertyName("fapiao_id")]
    public string? FapiaoId { get; set; }

    /// <summary>发票代码（<c>fapiao_code</c>，必填 string(12)）。</summary>
    [JsonPropertyName("fapiao_code")]
    public string? FapiaoCode { get; set; }

    /// <summary>发票号码（<c>fapiao_number</c>，必填 string(20)）。</summary>
    [JsonPropertyName("fapiao_number")]
    public string? FapiaoNumber { get; set; }
}

/// <summary>
/// 获取发票下载信息应答（<c>GET /v3/new-tax-control-fapiao/fapiao-applications/{fapiao_apply_id}/fapiao-files</c>）。
/// </summary>
/// <remarks>
/// <b>官方文档</b>：<see href="https://pay.weixin.qq.com/doc/v3/merchant/4012538335"/>
/// （2026-10-09 逐字段核验；更新时间 2025.09.26）。
/// </remarks>
[HttpJsonSerializable(SerializerClassName = "NewTaxControlFapiao")]
public class FapiaoFilesResponse : WechatPayResponse
{
    /// <summary>发票文件下载信息列表（<c>fapiao_download_info_list</c>），见 <see cref="FapiaoDownloadInfo"/>。</summary>
    [JsonPropertyName("fapiao_download_info_list")]
    public List<FapiaoDownloadInfo>? FapiaoDownloadInfoList { get; set; }
}

/// <summary>
/// 单张发票的下载信息（<c>fapiao_download_info_list</c> 项）。
/// </summary>
[HttpJsonSerializable(SerializerClassName = "NewTaxControlFapiao")]
public class FapiaoDownloadInfo
{
    /// <summary>商户发票单号（<c>fapiao_id</c>）。</summary>
    [JsonPropertyName("fapiao_id")]
    public string? FapiaoId { get; set; }

    /// <summary>
    /// 发票文件下载地址（<c>download_url</c>）：官方原文「<b>仅当</b>发票状态为 <c>ISSUED</c> 时存在」
    /// ⇒ 其它状态下为 <c>null</c>。消费侧<b>严禁</b>改写 / 拼接该地址。
    /// </summary>
    [JsonPropertyName("download_url")]
    public string? DownloadUrl { get; set; }

    /// <summary>发票状态（<c>status</c>）：官方原文「仅能下载 <c>ISSUED</c> 状态的发票」。</summary>
    [JsonPropertyName("status")]
    public string? Status { get; set; }
}

/// <summary>
/// 将电子发票插入微信用户卡包请求
/// （<c>POST /v3/new-tax-control-fapiao/fapiao-applications/{fapiao_apply_id}/insert-cards</c>）。
/// </summary>
/// <remarks>
/// <para>
/// <b>官方文档</b>：<see href="https://pay.weixin.qq.com/doc/v3/merchant/4012538365"/>
/// （2026-10-09 逐字段核验；更新时间 2025.09.26）。<b>支持商户：【普通商户】</b>。
/// </para>
/// <para>
/// <b>复用三处子结构</b>（官方字段表逐项一致）：<c>buyer_information</c> →
/// <see cref="FapiaoBuyerInformation"/>、<c>seller_information</c> → <see cref="FapiaoSellerInformation"/>、
/// <c>extra_information</c> → <see cref="FapiaoExtraInformation"/>；卡券明细项与「查询电子发票」的
/// <c>items</c> 字段表亦逐项一致 ⇒ 复用 <see cref="FapiaoQueryItem"/>。
/// </para>
/// </remarks>
[HttpJsonSerializable(SerializerClassName = "NewTaxControlFapiao")]
public class FapiaoInsertCardsRequest
{
    /// <summary>
    /// 场景（<c>scene</c>，必填）。
    /// </summary>
    /// <remarks>
    /// <b>取值未核验</b>：官方本页只标「场景 / 必填」，<b>未列取值表</b> ⇒ 本仓不臆造常量
    /// （开具接口页另有 <c>WITH_WECHATPAY</c> / <c>WITHOUT_WECHATPAY</c> 的取值说明，
    /// 但<b>不得</b>据此推定本接口取值相同）。
    /// </remarks>
    [JsonPropertyName("scene")]
    public string? Scene { get; set; }

    /// <summary>购买方信息（<c>buyer_information</c>，必填）：复用开具接口的 <see cref="FapiaoBuyerInformation"/>。</summary>
    [JsonPropertyName("buyer_information")]
    public FapiaoBuyerInformation? BuyerInformation { get; set; }

    /// <summary>电子发票卡券信息（<c>fapiao_card_information</c>，必填 array，官方标注<b>最多五条</b>）。</summary>
    [JsonPropertyName("fapiao_card_information")]
    public List<FapiaoInsertCardInformation>? FapiaoCardInformation { get; set; }
}

/// <summary>
/// 待插入卡包的发票卡券（<c>fapiao_card_information</c> 项，<b>13 字段</b>）。
/// </summary>
/// <remarks>
/// <para>
/// <b>⚠️ 勿与 <see cref="FapiaoCardInformation"/> 混用</b>：两者<b>字段表完全不同</b> ——
/// 本类（<b>请求</b>侧，商户提供发票要素）是 <c>fapiao_media_id</c> / 号码 / 金额 / 销方 / 明细 / 备注；
/// 而 <see cref="FapiaoCardInformation"/>（<b>应答</b>侧，官方回吐卡券状态）是
/// <c>card_appid</c> / <c>card_openid</c> / <c>card_id</c> / <c>card_code</c> / <c>card_status</c>。
/// 二者都被官方命名为「card_information」，<b>合并即错</b>（守卫 FAP-B5 锁死这条）。
/// </para>
/// <para>
/// <b>与本类的 <see cref="FapiaoBlueRedInformation"/></b> 亦不可复用：蓝红票信息缺金额与销方信息。
/// </para>
/// </remarks>
[HttpJsonSerializable(SerializerClassName = "NewTaxControlFapiao")]
public class FapiaoInsertCardInformation
{
    /// <summary>电子发票文件 ID（<c>fapiao_media_id</c>，必填 string(128)）：来自《上传电子发票文件》，<b>三天内有效</b>。</summary>
    [JsonPropertyName("fapiao_media_id")]
    public string? FapiaoMediaId { get; set; }

    /// <summary>发票号码（<c>fapiao_number</c>，必填）。</summary>
    [JsonPropertyName("fapiao_number")]
    public string? FapiaoNumber { get; set; }

    /// <summary>发票代码（<c>fapiao_code</c>，必填）。</summary>
    [JsonPropertyName("fapiao_code")]
    public string? FapiaoCode { get; set; }

    /// <summary>开票时间（<c>fapiao_time</c>，必填）。</summary>
    [JsonPropertyName("fapiao_time")]
    public string? FapiaoTime { get; set; }

    /// <summary>校验码（<c>check_code</c>，必填）。</summary>
    [JsonPropertyName("check_code")]
    public string? CheckCode { get; set; }

    /// <summary>密码区（<c>password</c>，必填）。</summary>
    [JsonPropertyName("password")]
    public string? Password { get; set; }

    /// <summary>总金额（<c>total_amount</c>，必填，单位分）。</summary>
    [JsonPropertyName("total_amount")]
    public long? TotalAmount { get; set; }

    /// <summary>税额（<c>tax_amount</c>，必填，单位分）。</summary>
    [JsonPropertyName("tax_amount")]
    public long? TaxAmount { get; set; }

    /// <summary>不含税金额（<c>amount</c>，必填，单位分）。</summary>
    [JsonPropertyName("amount")]
    public long? Amount { get; set; }

    /// <summary>销方信息（<c>seller_information</c>，必填）：复用 <see cref="FapiaoSellerInformation"/>。</summary>
    [JsonPropertyName("seller_information")]
    public FapiaoSellerInformation? SellerInformation { get; set; }

    /// <summary>附加信息（<c>extra_information</c>，必填）：复用 <see cref="FapiaoExtraInformation"/>。</summary>
    [JsonPropertyName("extra_information")]
    public FapiaoExtraInformation? ExtraInformation { get; set; }

    /// <summary>发票明细（<c>items</c>，选填 array）：与查询应答的明细字段表一致 ⇒ 复用 <see cref="FapiaoQueryItem"/>。</summary>
    [JsonPropertyName("items")]
    public List<FapiaoQueryItem>? Items { get; set; }

    /// <summary>备注（<c>remark</c>，选填）。</summary>
    [JsonPropertyName("remark")]
    public string? Remark { get; set; }
}

/// <summary>
/// 上传电子发票文件应答（<b>仅 <c>fapiao_media_id</c> 一个字段</b>）。
/// </summary>
/// <remarks>
/// <para>
/// <b>官方文档</b>：<see href="https://pay.weixin.qq.com/docs/merchant/apis/fapiao/fapiao-applications/upload-fapiao-file.html"/>
/// （2026-10-09 逐字段核验；更新时间 2025.09.26）。
/// </para>
/// <para>
/// <b>⚠️ 该 ID <b>三天内有效</b></b>（官方原文）：上传后须<b>尽快</b>调用
/// <see cref="IWechatPayFapiaoService.InsertFapiaoCardsAsync"/> 执行插卡，
/// 过期后只能重新上传。
/// </para>
/// <para>
/// <b>请求侧的 <c>meta</c> 未建为 DTO</b>：它只出现在 <c>multipart</c> 的表单段里，
/// 而本包最低 TFM 为 net6.0（生成的 JsonContext 仅 net8.0+ 可用）⇒ 上传通道用
/// <c>Utf8JsonWriter</c> 手写这 3 个字段（零反射、AOT 安全）。
/// 字段名常量收敛在 <c>WechatPayFapiaoFileService</c> 上并由用例逐字锁定，
/// 其中 <c>digest_alogrithm</c> <b>是官方拼写（少一个 r），必须照录</b>。
/// </para>
/// </remarks>
[HttpJsonSerializable(SerializerClassName = "NewTaxControlFapiao")]
public class FapiaoUploadFileResponse : WechatPayResponse
{
    /// <summary>电子发票文件 ID（<c>fapiao_media_id</c>，必填 string(128)）：<b>三天内有效</b>，用于「将电子发票插入微信用户卡包」。</summary>
    [JsonPropertyName("fapiao_media_id")]
    public string? FapiaoMediaId { get; set; }
}
