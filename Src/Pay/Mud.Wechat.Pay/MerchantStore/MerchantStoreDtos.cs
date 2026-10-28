// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

using Mud.Wechat.Pay.DataModels.Common;

namespace Mud.Wechat.Pay.DataModels.MerchantStore;

/// <summary>
/// 门店基础信息（官方 <c>store_basics</c>）：创建 / 更新 / 查询三支共用同一棵子树。
/// </summary>
/// <remarks>
/// <para><b>官方文档</b>：<see href="https://pay.weixin.qq.com/doc/v3/merchant/4014085766"/>，2026-10-14 逐字段核验。</para>
/// <para><b>「必填」落在叶子而非包装层</b>：创建支 <c>store_basics</c> 本身必填，更新支该对象<b>选填</b>
/// （不传即不改），但一旦传入，<c>brand_name</c> / <c>store_name</c> 仍是对象内的必填字段 ——
/// 官方两页的叶子必填性逐行一致 ⇒ 一份类型承载两形态，条件必填由官方以 <c>INVALID_REQUEST</c> 拒绝，
/// SDK 不做客户端校验。</para>
/// <para><b>品牌名须先完成微信支付品牌认证</b>（官方原文），且 <c>store_name</c> 须与门头照一致、
/// <b>不得含分店信息</b>（分店写 <c>branch_name</c>）；若填 <c>brand_name</c> 则须与认证品牌一致，
/// 更新支未填时官方默认落到「其他品牌」。</para>
/// </remarks>
[HttpJsonSerializable(SerializerClassName = "MerchantStore")]
public class MerchantStoreBasics
{
    /// <summary>商家门店编号（<c>store_reference_id</c>，选填 string(32)）：商家系统内唯一的门店编号。</summary>
    [JsonPropertyName("store_reference_id")]
    public string? StoreReferenceId { get; set; }

    /// <summary>品牌名称（<c>brand_name</c>，必填 string(32)）：须已完成微信支付品牌认证。</summary>
    [JsonPropertyName("brand_name")]
    public string? BrandName { get; set; }

    /// <summary>门店名称（<c>store_name</c>，必填 string(32)）：须与门头照一致且不含分店信息。</summary>
    [JsonPropertyName("store_name")]
    public string? StoreName { get; set; }

    /// <summary>分店名称（<c>branch_name</c>，选填 string(20)）：如「南山店」。</summary>
    [JsonPropertyName("branch_name")]
    public string? BranchName { get; set; }

    /// <summary>品牌标识（<c>brand_id</c>，选填 integer）：微信支付分配的全局唯一品牌 ID。</summary>
    [JsonPropertyName("brand_id")]
    public long? BrandId { get; set; }
}

/// <summary>
/// 门店地址信息（官方 <c>store_address</c>）：创建 / 更新 / 查询三支共用。
/// </summary>
/// <remarks>
/// <para><b>官方文档</b>：<see href="https://pay.weixin.qq.com/doc/v3/merchant/4014085766"/>，2026-10-14 逐字段核验。</para>
/// <para><b>修改地址会触发核实流程</b>（官方在更新支页内明写）⇒ 不是「PATCH 即生效」，
/// 结果须经查询支读 <c>store_state</c>（含 <c>address_failed_reason</c>）才能闭环。</para>
/// <para><b>长度两说</b>：<c>address_detail</c> 参数表是 string(100)，同页散文又要求「4-64 个字符」
/// 且「不要重复填写省市区」⇒ 本 SDK 不校验，宿主按散文口径写更安全。</para>
/// <para><b>经纬度是 string 不是数值</b>（官方 <c>longitude</c> / <c>latitude</c> 均 string(32)）：
/// 建成 <c>double?</c> 会在反序列化含度分秒文本的历史数据时抛异常。</para>
/// </remarks>
[HttpJsonSerializable(SerializerClassName = "MerchantStore")]
public class MerchantStoreAddress
{
    /// <summary>省市编码（<c>address_code</c>，必填 string(20)）：微信支付行政区划编码，<b>只能为数字</b>。</summary>
    [JsonPropertyName("address_code")]
    public string? AddressCode { get; set; }

    /// <summary>详细地址（<c>address_detail</c>，必填 string(100)）：精确到门牌号，官方散文要求 4-64 字符且不重复省市区。</summary>
    [JsonPropertyName("address_detail")]
    public string? AddressDetail { get; set; }

    /// <summary>辅助位置（<c>address_complements</c>，选填 string(50)）：周边标志建筑等补充描述。</summary>
    [JsonPropertyName("address_complements")]
    public string? AddressComplements { get; set; }

    /// <summary>经度（<c>longitude</c>，选填 <b>string</b>(32)）：取值 [-180,180]，官方类型为字符串。</summary>
    [JsonPropertyName("longitude")]
    public string? Longitude { get; set; }

    /// <summary>纬度（<c>latitude</c>，选填 <b>string</b>(32)）：取值 [-90,90]，官方类型为字符串。</summary>
    [JsonPropertyName("latitude")]
    public string? Latitude { get; set; }
}

/// <summary>
/// 门店经营信息（官方 <c>store_business</c>）：创建 / 更新 / 查询三支共用。
/// </summary>
/// <remarks>
/// <para><b>官方文档</b>：<see href="https://pay.weixin.qq.com/doc/v3/merchant/4014085766"/>，2026-10-14 逐字段核验。</para>
/// <para><b>服务电话口径两页不同</b>：创建页写「仅数字和 -」，更新页写「最多两个、以 ; 分隔」
/// ⇒ 两处均照页记录，SDK 侧不做格式校验（守卫 MST-B3 锁定两页口径分歧这一事实）。</para>
/// <para><b>经营时间有固定格式</b>：官方要求形如「周一至周五 09:00-18:00|周六至周日 10:00-17:00」
/// 且<b>最多两段</b>，用竖线分隔。</para>
/// </remarks>
[HttpJsonSerializable(SerializerClassName = "MerchantStore")]
public class MerchantStoreBusinessInfo
{
    /// <summary>服务电话（<c>service_phone</c>，选填 string(32)）：仅数字与连字符，更新页另许可最多两个以 ; 分隔。</summary>
    [JsonPropertyName("service_phone")]
    public string? ServicePhone { get; set; }

    /// <summary>经营时间（<c>business_hours</c>，选填 string(256)）：官方固定格式，<b>最多两段</b>并以竖线分隔。</summary>
    [JsonPropertyName("business_hours")]
    public string? BusinessHours { get; set; }
}

/// <summary>
/// 门店收款商户号信息（官方 <c>store_recipient</c> 数组项，亦名官方类型 <c>StoreRecipientInfoWithCompanyName</c>）。
/// </summary>
/// <remarks>
/// <para><b>官方文档</b>：<see href="https://pay.weixin.qq.com/doc/v3/merchant/4014085771"/>，2026-10-14 逐字段核验。</para>
/// <para><b>一个门店最多绑定五个收款商户号</b>（官方原文即有「绑定绑定」叠字），超数以
/// <c>INVALID_REQUEST</c> 拒绝；绑定主体须与品牌主体或既有收款方一致。</para>
/// <para><b>本类型被四处共用</b>：创建（必填数组）、绑定（必填数组）、解绑（<b>选填</b>数组）、查询应答。
/// 官方四页字段表逐行相同 ⇒ 一份类型，包装层的必填性差异见各请求 DTO。</para>
/// </remarks>
[HttpJsonSerializable(SerializerClassName = "MerchantStore")]
public class MerchantStoreRecipientInfo
{
    /// <summary>收款商户号（<c>mchid</c>，必填 string(16)）：门店收款微信支付商户号。</summary>
    [JsonPropertyName("mchid")]
    public string? MchId { get; set; }

    /// <summary>收款主体名称（<c>company_name</c>，选填 string(256)）。</summary>
    [JsonPropertyName("company_name")]
    public string? CompanyName { get; set; }
}

/// <summary>
/// 门店状态（官方 <c>store_state</c>）：<b>只出现在查询应答</b>，创建 / 更新两支的请求表内无此节点。
/// </summary>
/// <remarks>
/// <para><b>官方文档</b>：<see href="https://pay.weixin.qq.com/doc/v3/merchant/4014085769"/>，2026-10-14 逐字段核验。</para>
/// <para><b>修改地址后的核实结果就在这里闭环</b>：<c>address_failed_reason</c> 是地址核实失败原因，
/// 更新支是 204 无包体 ⇒ 唯一的结果确认路径是查询支读本节点。</para>
/// </remarks>
[HttpJsonSerializable(SerializerClassName = "MerchantStore")]
public class MerchantStoreState
{
    /// <summary>门店状态值（<c>store_state_value</c>，选填 string）：官方取值见 <see cref="MerchantStoreStateValues"/>。</summary>
    [JsonPropertyName("store_state_value")]
    public string? StoreStateValue { get; set; }

    /// <summary>门店状态描述（<c>store_state_message</c>，选填 string）。</summary>
    [JsonPropertyName("store_state_message")]
    public string? StoreStateMessage { get; set; }

    /// <summary>地址核实失败原因（<c>address_failed_reason</c>，选填 string）。</summary>
    [JsonPropertyName("address_failed_reason")]
    public string? AddressFailedReason { get; set; }
}

/// <summary>
/// 创建品牌门店请求（<b>POST /v3/merchant-store/stores</b> 的包体；商户面与服务商面<b>共用本类型</b>）。
/// </summary>
/// <remarks>
/// <para><b>官方文档</b>：商户面 <see href="https://pay.weixin.qq.com/doc/v3/merchant/4014085766"/>、
/// 服务商面 <see href="https://pay.weixin.qq.com/doc/v3/merchant/4013948546"/>，2026-10-14 两页逐字段比对。</para>
/// <para><b>两页字段表只差一个 <c>sub_mchid</c></b>（服务商面多一行选填 string(16)，描述注明服务商调用场景须传），
/// 四棵子树逐字段同名同必填性同长度 ⇒ 按「表相同则共用」归并（守卫 MST-B6 断言两支共用同一请求类型）。</para>
/// <para><b>应答只回 <c>store_id</c></b>：官方为 integer 且标注必填，后续更新 / 查询 / 绑定三支全部以它定位。</para>
/// </remarks>
[HttpJsonSerializable(SerializerClassName = "MerchantStore")]
public class MerchantStoreCreateRequest
{
    /// <summary>特约商户号（<c>sub_mchid</c>，官方标选填 string(16)）：<b>服务商调用场景须传</b>，直连商户留空。</summary>
    [JsonPropertyName("sub_mchid")]
    public string? SubMchId { get; set; }

    /// <summary>门店基础信息（<c>store_basics</c>，必填 object）：见 <see cref="MerchantStoreBasics"/>。</summary>
    [JsonPropertyName("store_basics")]
    public MerchantStoreBasics? StoreBasics { get; set; }

    /// <summary>门店地址信息（<c>store_address</c>，必填 object）：见 <see cref="MerchantStoreAddress"/>。</summary>
    [JsonPropertyName("store_address")]
    public MerchantStoreAddress? StoreAddress { get; set; }

    /// <summary>门店经营信息（<c>store_business</c>，必填 object）：见 <see cref="MerchantStoreBusinessInfo"/>。</summary>
    [JsonPropertyName("store_business")]
    public MerchantStoreBusinessInfo? StoreBusiness { get; set; }

    /// <summary>收款商户号列表（<c>store_recipient</c>，必填 array，<b>最多五个</b>）：见 <see cref="MerchantStoreRecipientInfo"/>。</summary>
    [JsonPropertyName("store_recipient")]
    public List<MerchantStoreRecipientInfo>? StoreRecipient { get; set; }
}

/// <summary>
/// 创建品牌门店应答：<b>只回微信支付分配的全局门店 ID</b>（商户面与服务商面共用）。
/// </summary>
[HttpJsonSerializable(SerializerClassName = "MerchantStore")]
public class MerchantStoreCreateResponse : WechatPayResponse
{
    /// <summary>微信支付门店 ID（<c>store_id</c>，必填 integer）：全局唯一，后续三支均以它定位。</summary>
    [JsonPropertyName("store_id")]
    public long? StoreId { get; set; }
}

/// <summary>
/// 更新品牌门店请求（<b>PATCH /v3/merchant-store/stores/{store_id}</b> 的包体；两形态共用）。
/// </summary>
/// <remarks>
/// <para><b>官方文档</b>：商户面 <see href="https://pay.weixin.qq.com/doc/v3/merchant/4014085768"/>、
/// 服务商面 <see href="https://pay.weixin.qq.com/doc/v3/merchant/4013948551"/>，2026-10-14 两页逐字段比对。</para>
/// <para><b>本类型没有 <c>store_recipient</c></b>：收款商户号的增删走独立的 bind / unbind 两支
/// ⇒ 把创建请求 DTO 照抄过来当更新请求会多出一个官方不接受的字段（守卫 MST-B3 锁定三棵 vs 四棵子树之差）。</para>
/// <para><b>成功应答是 204 无包体</b>：与同域的创建 / 查询（均 200 + JSON）不同，接口侧声明为 <c>Task</c>。</para>
/// <para><b>三棵子树均为选填 = 不传即不改</b>，但传入后其内部叶子（<c>brand_name</c> / <c>store_name</c> /
/// <c>address_code</c> / <c>address_detail</c>）仍是必填 —— 官方更新页的叶子必填性与创建页逐行一致。</para>
/// </remarks>
[HttpJsonSerializable(SerializerClassName = "MerchantStore")]
public class MerchantStoreUpdateRequest
{
    /// <summary>特约商户号（<c>sub_mchid</c>，官方标选填 string(16)）：服务商调用场景须传。</summary>
    [JsonPropertyName("sub_mchid")]
    public string? SubMchId { get; set; }

    /// <summary>门店基础信息（<c>store_basics</c>，选填 object）：不传即不改；见 <see cref="MerchantStoreBasics"/>。</summary>
    [JsonPropertyName("store_basics")]
    public MerchantStoreBasics? StoreBasics { get; set; }

    /// <summary>门店地址信息（<c>store_address</c>，选填 object）：<b>修改会触发地址核实流程</b>；见 <see cref="MerchantStoreAddress"/>。</summary>
    [JsonPropertyName("store_address")]
    public MerchantStoreAddress? StoreAddress { get; set; }

    /// <summary>门店经营信息（<c>store_business</c>，选填 object）：不传即不改；见 <see cref="MerchantStoreBusinessInfo"/>。</summary>
    [JsonPropertyName("store_business")]
    public MerchantStoreBusinessInfo? StoreBusiness { get; set; }
}

/// <summary>
/// 查询品牌门店应答（商户面与服务商面<b>逐字段相同</b>，共用本类型）。
/// </summary>
/// <remarks>
/// <para><b>官方文档</b>：商户面 <see href="https://pay.weixin.qq.com/doc/v3/merchant/4014085769"/>、
/// 服务商面 <see href="https://pay.weixin.qq.com/doc/v3/merchant/4013948577"/>，2026-10-14 两页逐字段比对：
/// 请求面唯一差异是服务商多一个<b>查询参数</b> <c>sub_mchid</c>（不在包体里），应答表零差异。</para>
/// <para><b>本应答含 <c>store_state</c></b>（请求侧没有）：地址核实的成败只在查询应答里可见，
/// 故更新支（204）之后必须查这一支确认，不能把 204 当作「已生效」。</para>
/// </remarks>
[HttpJsonSerializable(SerializerClassName = "MerchantStore")]
public class MerchantStoreQueryResponse : WechatPayResponse
{
    /// <summary>门店基础信息（<c>store_basics</c>，选填 object）：见 <see cref="MerchantStoreBasics"/>。</summary>
    [JsonPropertyName("store_basics")]
    public MerchantStoreBasics? StoreBasics { get; set; }

    /// <summary>门店地址信息（<c>store_address</c>，选填 object）：见 <see cref="MerchantStoreAddress"/>。</summary>
    [JsonPropertyName("store_address")]
    public MerchantStoreAddress? StoreAddress { get; set; }

    /// <summary>门店经营信息（<c>store_business</c>，选填 object）：见 <see cref="MerchantStoreBusinessInfo"/>。</summary>
    [JsonPropertyName("store_business")]
    public MerchantStoreBusinessInfo? StoreBusiness { get; set; }

    /// <summary>收款商户号列表（<c>store_recipient</c>，选填 array）：见 <see cref="MerchantStoreRecipientInfo"/>。</summary>
    [JsonPropertyName("store_recipient")]
    public List<MerchantStoreRecipientInfo>? StoreRecipient { get; set; }

    /// <summary>门店状态（<c>store_state</c>，选填 object）：<b>仅本应答有</b>，见 <see cref="MerchantStoreState"/>。</summary>
    [JsonPropertyName("store_state")]
    public MerchantStoreState? StoreState { get; set; }
}
