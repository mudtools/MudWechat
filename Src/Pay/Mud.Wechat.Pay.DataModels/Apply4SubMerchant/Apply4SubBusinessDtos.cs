// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

namespace Mud.Wechat.Pay.DataModels.Apply4SubMerchant;

/// <summary>
/// 经营资料（进件 <c>business_info</c>）。
/// </summary>
/// <remarks>字段表照官方「提交申请单」页原文（<see href="https://pay.weixin.qq.com/doc/v3/partner/4012719997"/>，2026-10-14 逐字段核验）。</remarks>
[HttpJsonSerializable(SerializerClassName = "Apply4SubMerchant")]
public class Apply4SubBusinessInfo
{
    /// <summary>商户简称（<c>merchant_shortname</c>，必填 string(64)）：收款页面向用户展示的名称，须与实际经营内容相符。</summary>
    [JsonPropertyName("merchant_shortname")]
    public string? MerchantShortName { get; set; }

    /// <summary>客服电话（<c>service_phone</c>，必填 string(32)）：官方要求提供真实可接通的售后电话。</summary>
    [JsonPropertyName("service_phone")]
    public string? ServicePhone { get; set; }

    /// <summary>经营场景（<c>sales_info</c>，必填对象），见 <see cref="Apply4SubSalesInfo"/>。</summary>
    [JsonPropertyName("sales_info")]
    public Apply4SubSalesInfo? SalesInfo { get; set; }
}

/// <summary>
/// 经营场景信息（进件 <c>business_info.sales_info</c>）。
/// </summary>
/// <remarks>
/// <para>
/// <b>场景数组与各场景资料是「声明即必填」的对应关系</b>（官方原文）：
/// <see cref="SalesScenesType"/> 里出现的每个场景，其对应子对象即必填 ——
/// 取 <c>SALES_SCENES_STORE</c> 须填 <see cref="BizStoreInfo"/>，取 <c>SALES_SCENES_MP</c> 须填 <see cref="MpInfo"/>，
/// 取 <c>SALES_SCENES_MINI_PROGRAM</c> 须填 <see cref="MiniProgramInfo"/>，取 <c>SALES_SCENES_APP</c> 须填 <see cref="AppInfo"/>，
/// 取 <c>SALES_SCENES_WEB</c> 须填 <see cref="WebInfo"/>，取 <c>SALES_SCENES_WEWORK</c> 须填 <see cref="WeWorkInfo"/>。
/// </para>
/// <para>
/// <b>审核通过后的能力由场景决定</b>：如 <c>SALES_SCENES_STORE</c> 通过后服务商可代商户发起付款码支付与 JSAPI 支付，
/// <c>SALES_SCENES_WEWORK</c> 通过后开通企业微信专区自有交易权限并绑定 CorpID。
/// </para>
/// </remarks>
[HttpJsonSerializable(SerializerClassName = "Apply4SubMerchant")]
public class Apply4SubSalesInfo
{
    /// <summary>经营场景类型（<c>sales_scenes_type</c>，必填数组）：取值见 <see cref="Apply4SubSalesScenesTypes"/>。</summary>
    [JsonPropertyName("sales_scenes_type")]
    public List<string>? SalesScenesType { get; set; }

    /// <summary>线下场所信息（<c>biz_store_info</c>，条件必填），见 <see cref="Apply4SubBizStoreInfo"/>。</summary>
    [JsonPropertyName("biz_store_info")]
    public Apply4SubBizStoreInfo? BizStoreInfo { get; set; }

    /// <summary>服务号 / 公众号信息（<c>mp_info</c>，条件必填），见 <see cref="Apply4SubMpInfo"/>。</summary>
    [JsonPropertyName("mp_info")]
    public Apply4SubMpInfo? MpInfo { get; set; }

    /// <summary>小程序信息（<c>mini_program_info</c>，条件必填），见 <see cref="Apply4SubMiniProgramInfo"/>。</summary>
    [JsonPropertyName("mini_program_info")]
    public Apply4SubMiniProgramInfo? MiniProgramInfo { get; set; }

    /// <summary>APP 信息（<c>app_info</c>，条件必填），见 <see cref="Apply4SubAppInfo"/>。</summary>
    [JsonPropertyName("app_info")]
    public Apply4SubAppInfo? AppInfo { get; set; }

    /// <summary>互联网网站信息（<c>web_info</c>，条件必填），见 <see cref="Apply4SubWebInfo"/>。</summary>
    [JsonPropertyName("web_info")]
    public Apply4SubWebInfo? WebInfo { get; set; }

    /// <summary>企业微信信息（<c>wework_info</c>，条件必填），见 <see cref="Apply4SubWeWorkInfo"/>。</summary>
    [JsonPropertyName("wework_info")]
    public Apply4SubWeWorkInfo? WeWorkInfo { get; set; }
}

/// <summary>线下场所（门店）信息（进件 <c>sales_info.biz_store_info</c>）。</summary>
[HttpJsonSerializable(SerializerClassName = "Apply4SubMerchant")]
public class Apply4SubBizStoreInfo
{
    /// <summary>门店名称（<c>biz_store_name</c>，必填 string(50)）。</summary>
    [JsonPropertyName("biz_store_name")]
    public string? BizStoreName { get; set; }

    /// <summary>门店省市编码（<c>biz_address_code</c>，必填 string(128)）：至少精确到市，详见官方「省市区编号对照表」。</summary>
    [JsonPropertyName("biz_address_code")]
    public string? BizAddressCode { get; set; }

    /// <summary>门店地址（<c>biz_store_address</c>，必填 string(512)）。</summary>
    [JsonPropertyName("biz_store_address")]
    public string? BizStoreAddress { get; set; }

    /// <summary>门店门头照片（<c>store_entrance_pic</c>，必填数组）：门头正面照（招牌与门框完整、清晰可辨识），填 MediaID。</summary>
    [JsonPropertyName("store_entrance_pic")]
    public List<string>? StoreEntrancePic { get; set; }

    /// <summary>店内环境照片（<c>indoor_pic</c>，必填数组）：可辨识经营内容；停车场等无固定门头场所可传停车场内部照片，填 MediaID。</summary>
    [JsonPropertyName("indoor_pic")]
    public List<string>? IndoorPic { get; set; }

    /// <summary>线下场所对应的商家 AppID（<c>biz_sub_appid</c>，选填 string(256)）。</summary>
    [JsonPropertyName("biz_sub_appid")]
    public string? BizSubAppId { get; set; }
}

/// <summary>服务号 / 公众号信息（进件 <c>sales_info.mp_info</c>）。</summary>
[HttpJsonSerializable(SerializerClassName = "Apply4SubMerchant")]
public class Apply4SubMpInfo
{
    /// <summary>服务商公众号 / 小程序 AppID（<c>mp_appid</c>，选填 string(256)）。</summary>
    [JsonPropertyName("mp_appid")]
    public string? MpAppId { get; set; }

    /// <summary>商户公众号 AppID（<c>mp_sub_appid</c>，选填 string(256)）：须为与商家主体一致且<b>已认证</b>的服务号、或政府 / 媒体类型公众号。</summary>
    [JsonPropertyName("mp_sub_appid")]
    public string? MpSubAppId { get; set; }

    /// <summary>公众号页面截图（<c>mp_pics</c>，必填数组，<b>最多 5 张</b>）：展示商品 / 服务的页面截图或设计稿；公众号未建设完善或未上线时<b>必须</b>提供，填 MediaID。</summary>
    [JsonPropertyName("mp_pics")]
    public List<string>? MpPics { get; set; }
}

/// <summary>小程序信息（进件 <c>sales_info.mini_program_info</c>）。</summary>
[HttpJsonSerializable(SerializerClassName = "Apply4SubMerchant")]
public class Apply4SubMiniProgramInfo
{
    /// <summary>服务商小程序 AppID（<c>mini_program_appid</c>，选填 string(256)）。</summary>
    [JsonPropertyName("mini_program_appid")]
    public string? MiniProgramAppId { get; set; }

    /// <summary>商户小程序 AppID（<c>mini_program_sub_appid</c>，选填 string(256)）：须与商家主体一致且已认证。</summary>
    [JsonPropertyName("mini_program_sub_appid")]
    public string? MiniProgramSubAppId { get; set; }

    /// <summary>小程序页面截图（<c>mini_program_pics</c>，选填数组，<b>最多 5 张</b>）：小程序未建设完善或未上线时须提供，填 MediaID。</summary>
    [JsonPropertyName("mini_program_pics")]
    public List<string>? MiniProgramPics { get; set; }
}

/// <summary>APP 信息（进件 <c>sales_info.app_info</c>）。</summary>
[HttpJsonSerializable(SerializerClassName = "Apply4SubMerchant")]
public class Apply4SubAppInfo
{
    /// <summary>服务商应用 AppID（<c>app_appid</c>，选填 string(256)）。</summary>
    [JsonPropertyName("app_appid")]
    public string? AppAppId { get; set; }

    /// <summary>商户应用 AppID（<c>app_sub_appid</c>，选填 string(256)）：须与商家主体一致且已认证。</summary>
    [JsonPropertyName("app_sub_appid")]
    public string? AppSubAppId { get; set; }

    /// <summary>APP 截图（<c>app_pics</c>，必填数组）：官方要求 APP 首页、尾页、应用内、支付页<b>各 1 张</b>，填 MediaID。</summary>
    [JsonPropertyName("app_pics")]
    public List<string>? AppPics { get; set; }
}

/// <summary>互联网网站信息（进件 <c>sales_info.web_info</c>）。</summary>
/// <remarks>
/// <b>官方现版已无「网站 URL 备案截图列表」字段</b>（第三方 SDK 转述的 <c>url_record_list</c> 不在现行页面内）
/// —— 现行面只有域名、网站授权函与 AppID 三项，勿凭旧版补字段。
/// </remarks>
[HttpJsonSerializable(SerializerClassName = "Apply4SubMerchant")]
public class Apply4SubWebInfo
{
    /// <summary>互联网网站域名（<c>domain</c>，必填 string(1024)）：PC 商城 / 智能终端等场景可上传官网链接，域名须 ICP 备案。</summary>
    [JsonPropertyName("domain")]
    public string? Domain { get; set; }

    /// <summary>网站授权函（<c>web_authorisation</c>，选填 string(1024)）：<b>备案主体与申请主体不同</b>时必须上传加盖公章的网站授权函，填 MediaID。</summary>
    [JsonPropertyName("web_authorisation")]
    public string? WebAuthorisation { get; set; }

    /// <summary>服务商网站 AppID（<c>web_appid</c>，选填 string(256)）。</summary>
    [JsonPropertyName("web_appid")]
    public string? WebAppId { get; set; }
}

/// <summary>企业微信信息（进件 <c>sales_info.wework_info</c>）。</summary>
[HttpJsonSerializable(SerializerClassName = "Apply4SubMerchant")]
public class Apply4SubWeWorkInfo
{
    /// <summary>商家企业微信 CorpID（<c>sub_corp_id</c>，必填 string(256)）：须与商家主体一致且已认证；审核通过后开通企业微信专区自有交易权限并完成商户号与该 CorpID 的绑定。</summary>
    [JsonPropertyName("sub_corp_id")]
    public string? SubCorpId { get; set; }

    /// <summary>企业微信页面截图（<c>wework_pics</c>，必填数组，<b>最多 5 张</b>）：填 MediaID。</summary>
    [JsonPropertyName("wework_pics")]
    public List<string>? WeWorkPics { get; set; }
}

/// <summary>结算规则（进件 <c>settlement_info</c>）。</summary>
/// <remarks>
/// <para>
/// <b>费率字段有硬性时间线（官方原文）</b>：2023-09-18 起，填写 <c>activities_id</c> 时<b>只能</b>分别传入
/// <see cref="DebitActivitiesRate"/> 与 <see cref="CreditActivitiesRate"/>，只传
/// <see cref="ActivitiesRate"/> 会直接报错 ⇒ 本类型三个字段都保留（照官方原文），但新接入方应只填借贷分离两值。
/// </para>
/// <para>结算规则 ID 与所属行业须查官方「费率结算规则对照表」；特殊资质图片仅在该行业要求时提交。</para>
/// </remarks>
[HttpJsonSerializable(SerializerClassName = "Apply4SubMerchant")]
public class Apply4SubSettlementInfo
{
    /// <summary>入驻结算规则 ID（<c>settlement_id</c>，必填）：见官方费率结算规则对照表。</summary>
    [JsonPropertyName("settlement_id")]
    public string? SettlementId { get; set; }

    /// <summary>所属行业（<c>qualification_type</c>，必填）：填写指定行业名称，见官方费率结算规则对照表。</summary>
    [JsonPropertyName("qualification_type")]
    public string? QualificationType { get; set; }

    /// <summary>特殊资质图片（<c>qualifications</c>，选填数组，<b>最多 5 张</b>）：仅所选行业要求时提交，填 MediaID。</summary>
    [JsonPropertyName("qualifications")]
    public List<string>? Qualifications { get; set; }

    /// <summary>优惠费率活动 ID（<c>activities_id</c>，选填 string(32)）：拟报名优惠费率活动时<b>必填</b>。</summary>
    [JsonPropertyName("activities_id")]
    public string? ActivitiesId { get; set; }

    /// <summary>优惠费率活动值（<c>activities_rate</c>，选填）：两位以内小数，须在活动 ID 指定费率范围内。</summary>
    [JsonPropertyName("activities_rate")]
    public string? ActivitiesRate { get; set; }

    /// <summary>优惠费率活动补充材料（<c>activities_additions</c>，选填数组，<b>最多 5 张</b>）：按所选活动要求提供，填 MediaID。</summary>
    [JsonPropertyName("activities_additions")]
    public List<string>? ActivitiesAdditions { get; set; }

    /// <summary>非信用卡活动费率值（<c>debit_activities_rate</c>，选填）：填写了 <see cref="ActivitiesId"/> 时<b>必填</b>，两位以内小数。</summary>
    [JsonPropertyName("debit_activities_rate")]
    public string? DebitActivitiesRate { get; set; }

    /// <summary>信用卡活动费率值（<c>credit_activities_rate</c>，选填）：填写了 <see cref="ActivitiesId"/> 时<b>必填</b>，两位以内小数。</summary>
    [JsonPropertyName("credit_activities_rate")]
    public string? CreditActivitiesRate { get; set; }
}

/// <summary>结算银行账户（进件 <c>bank_account_info</c>）。</summary>
/// <remarks>
/// <para>
/// <b>开户名称一致性（官方硬约束）</b>：<c>BANK_ACCOUNT_TYPE_PERSONAL</c> 时须与「经营者证件姓名」一致；
/// <c>BANK_ACCOUNT_TYPE_CORPORATE</c> 时须与营业执照「商户名称」一致。
/// </para>
/// <para>
/// <b><c>bank_name</c> 与 <c>bank_branch_id</c> 二选一</b>：是否需要支行由「查询支持个人 / 对公业务的银行列表 API」
/// 的返回决定；其它银行为「开户银行全称（含支行）」与「联行号」二选一。
/// <c>bank_address_code</c> 官方明示即将下线、无需传入。
/// </para>
/// </remarks>
[HttpJsonSerializable(SerializerClassName = "Apply4SubMerchant")]
public class Apply4SubBankAccountInfo
{
    /// <summary>账户类型（<c>bank_account_type</c>，必填）：取值见 <see cref="Apply4SubBankAccountTypes"/>。</summary>
    [JsonPropertyName("bank_account_type")]
    public string? BankAccountType { get; set; }

    /// <summary>开户名称（<c>account_name</c>，必填 string(2048)，<b>需加密</b>）：一致性约束见本类型 remarks。</summary>
    [JsonPropertyName("account_name")]
    public string? AccountName { get; set; }

    /// <summary>开户银行（<c>account_bank</c>，必填 string(128)）：对私取「查询支持个人业务的银行列表」、对公取「查询支持对公业务的银行列表」的返回值。</summary>
    [JsonPropertyName("account_bank")]
    public string? AccountBank { get; set; }

    /// <summary>开户银行省市编码（<c>bank_address_code</c>，选填 string(128)）：<b>官方明示即将下线、无需传入</b>；无对应省市区编号时可向上取市级 / 省级编号。</summary>
    [JsonPropertyName("bank_address_code")]
    public string? BankAddressCode { get; set; }

    /// <summary>开户银行联行号（<c>bank_branch_id</c>，选填 string(128)）：与 <see cref="BankName"/> 二选一，取自「查询支行列表 API」。</summary>
    [JsonPropertyName("bank_branch_id")]
    public string? BankBranchId { get; set; }

    /// <summary>开户银行全称（含支行）（<c>bank_name</c>，选填 string(128)）：与 <see cref="BankBranchId"/> 二选一，取自「查询支行列表 API」。</summary>
    [JsonPropertyName("bank_name")]
    public string? BankName { get; set; }

    /// <summary>银行账号（<c>account_number</c>，必填 string(2048)，<b>需加密</b>）：长度须符合官方「卡号长度要求表」。</summary>
    [JsonPropertyName("account_number")]
    public string? AccountNumber { get; set; }
}

/// <summary>补充材料（进件 <c>addition_info</c>，整对象选填）。</summary>
/// <remarks>
/// <b>驳回后的定向补件</b>：官方说明「资金来源」与「资金用途」被要求补充时，按指引依次写入
/// <see cref="BusinessAdditionMsg"/>（多值以中文分号分隔）。视频用<b>视频上传 API</b>（非图片上传 API）取 MediaID。
/// </remarks>
[HttpJsonSerializable(SerializerClassName = "Apply4SubMerchant")]
public class Apply4SubAdditionInfo
{
    /// <summary>法定代表人开户承诺函（<c>legal_person_commitment</c>，选填 string(1024)）：亲笔签署的扫描件，填 MediaID。</summary>
    [JsonPropertyName("legal_person_commitment")]
    public string? LegalPersonCommitment { get; set; }

    /// <summary>法定代表人开户意愿视频（<c>legal_person_video</c>，选填 string(1024)）：5M 内，avi / wmv / mpeg / mp4 / mov / mkv / flv / f4v / m4v / rmvb，填视频上传 API 的 MediaID。</summary>
    [JsonPropertyName("legal_person_video")]
    public string? LegalPersonVideo { get; set; }

    /// <summary>补充材料（<c>business_addition_pics</c>，选填数组，<b>最多 5 个</b>文件，支持照片或 PDF），填 MediaID。</summary>
    [JsonPropertyName("business_addition_pics")]
    public List<string>? BusinessAdditionPics { get; set; }

    /// <summary>补充说明（<c>business_addition_msg</c>，选填 string(512)）：被要求补充资金来源 / 资金用途时依次传入。</summary>
    [JsonPropertyName("business_addition_msg")]
    public string? BusinessAdditionMsg { get; set; }
}
