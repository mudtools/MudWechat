// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

namespace Mud.Wechat.Pay.V2.DataModels.PayV2;

/// <summary>
/// APIv2 机构模式子商户<b>进件</b>（<c>/secapi/mch/addInstitutionsub</c>）请求体（34 行官方字段）。
/// </summary>
/// <remarks>
/// <para><b>主体标识是 <c>app_id</c>，不是 <c>appid</c></b>：本族三支（进件 / 修改 / 查询）全部用
/// <c>app_id</c>，是 APIv2 线里唯一这样拼写的一族；同域别的端点一律 <c>appid</c> ⇒ 不可互相「对齐」。</para>
/// <para><b>本族没有 <c>nonce_str</c>、也没有 <c>sign_type</c></b>：官方进件页在签名行直接写明
/// 「默认加密方式为 MD5，且仅支持 MD5 加密」⇒ 填 <c>sign_type</c> 与官方字段表不符。</para>
/// <para><b>个人信息</b>：<c>director_id_number</c> / <c>principal_id_number</c> /
/// <c>registration_certificate_number</c> / <c>settlement_bank_number</c> / <c>contact_phone</c> /
/// <c>contact_email</c> 官方未标「敏感字段」（仅靠双向 TLS 保护），但 SDK 侧一律按个人信息处理：
/// 不落日志、不入异常消息（PAY-B7 同线口径）。</para>
/// </remarks>
[HttpJsonSerializable(SerializerClassName = "PayV2")]
public class V2InstitutionSubAddRequest
{
    /// <summary>机构在微信公众平台申请服务号对应的 APPID（<b>下划线拼写 <c>app_id</c></b>）。</summary>
    [JsonPropertyName("app_id")] public string? AppId { get; set; }

    /// <summary>微信支付分配的机构商户号。</summary>
    [JsonPropertyName("mch_id")] public string? MchId { get; set; }

    /// <summary>签名，官方本页仅支持 MD5。</summary>
    [JsonPropertyName("sign")] public string? Sign { get; set; }

    /// <summary>机构合作渠道编号，在商户平台创建。</summary>
    [JsonPropertyName("channel_id")] public string? ChannelId { get; set; }

    /// <summary>商户名称，须与注册证件（营业执照／公司注册证书／工商注册证）上的名称完全一致。</summary>
    [JsonPropertyName("merchant_name")] public string? MerchantName { get; set; }

    /// <summary>商户简称，展示在微信订单详情与支付成功页。</summary>
    [JsonPropertyName("merchant_shortname")] public string? MerchantShortname { get; set; }

    /// <summary>
    /// 机构为商户备注的信息，可以是商户在机构系统中的商户编号。<b>进件必填</b>
    /// （修改面的官方字段表已删除该键）。
    /// </summary>
    [JsonPropertyName("merchant_remark")] public string? MerchantRemark { get; set; }

    /// <summary>注册国家或区域代码（详见国家／地区代码表，如 <c>344</c>）。</summary>
    [JsonPropertyName("merchant_country_code")] public string? MerchantCountryCode { get; set; }

    /// <summary>商户类型：<c>ENTERPRISE</c>／<c>INDIVIDUAL</c>（自然人或独资经营者取后者）。</summary>
    [JsonPropertyName("merchant_type")] public string? MerchantType { get; set; }

    /// <summary>业务类目（行业类目列表）。</summary>
    [JsonPropertyName("business_category")] public string? BusinessCategory { get; set; }

    /// <summary>MCC 码，商户行业编码。</summary>
    [JsonPropertyName("mcc")] public string? Mcc { get; set; }

    /// <summary>公司注册文件编号；自然人取董事／负责人证件号，独资经营者取独资经营业务资质证书编号。</summary>
    [JsonPropertyName("registration_certificate_number")] public string? RegistrationCertificateNumber { get; set; }

    /// <summary>注册证件过期时间：日期 <c>YYYY-MM-DD</c>，或字面量 <c>PERMANENT</c>／<c>N/A</c>。</summary>
    [JsonPropertyName("registration_certificate_date")] public string? RegistrationCertificateDate { get; set; }

    /// <summary>注册证件照片，取值为<b>上传图片接口</b>返回的 <c>media_id</c>（见 <see cref="V2UploadMediaResponse"/>）。</summary>
    [JsonPropertyName("registration_certificate_copy")] public string? RegistrationCertificateCopy { get; set; }

    /// <summary>经营方式：<c>ONLINE</c>／<c>OFFLINE</c>／<c>BOTH</c>。</summary>
    [JsonPropertyName("business_type")] public string? BusinessType { get; set; }

    /// <summary>APP 下载地址。<c>business_type</c> 取 <c>ONLINE</c>／<c>BOTH</c> 时，本项与
    /// <see cref="BusinessWebsite"/>／<see cref="OfficeAccount"/>／<see cref="MiniProgram"/> 四项至少传一项。</summary>
    [JsonPropertyName("app_download")] public string? AppDownload { get; set; }

    /// <summary>
    /// 经营网址。官方字段名是 <c>business_website</c>（正文里偶见 <c>bus_website</c>，属官方文案笔误，
    /// 以其字段表与 XML 示例的 <c>business_website</c> 为准）。
    /// </summary>
    [JsonPropertyName("business_website")] public string? BusinessWebsite { get; set; }

    /// <summary>公众号（服务号／订阅号）信息。条件同上。</summary>
    [JsonPropertyName("office_account")] public string? OfficeAccount { get; set; }

    /// <summary>小程序信息。条件同上。</summary>
    [JsonPropertyName("mini_program")] public string? MiniProgram { get; set; }

    /// <summary>门店地址：<c>business_type</c> 取 <c>OFFLINE</c>／<c>BOTH</c> 时<b>必传</b>。</summary>
    [JsonPropertyName("store_address")] public string? StoreAddress { get; set; }

    /// <summary>
    /// 门店照片，取值为上传图片接口返回的 <c>media_id</c> 列表，<b>JSON 数组文本</b>；
    /// 线下／混合经营建议填写且<b>不少于 3 张</b>。
    /// </summary>
    [JsonPropertyName("store_photos")] public string? StorePhotos { get; set; }

    /// <summary>董事／负责人姓名（商户类型为 <c>ENTERPRISE</c> 时选填）。</summary>
    [JsonPropertyName("director_name")] public string? DirectorName { get; set; }

    /// <summary>董事／负责人证件号（商户类型为 <c>ENTERPRISE</c> 时选填）。<b>个人信息，不入日志。</b></summary>
    [JsonPropertyName("director_id_number")] public string? DirectorIdNumber { get; set; }

    /// <summary>经营者姓名（商户类型为 <c>INDIVIDUAL</c> 时选填）。</summary>
    [JsonPropertyName("principal_name")] public string? PrincipalName { get; set; }

    /// <summary>经营者证件号（商户类型为 <c>INDIVIDUAL</c> 时选填）。<b>个人信息，不入日志。</b></summary>
    [JsonPropertyName("principal_id_number")] public string? PrincipalIdNumber { get; set; }

    /// <summary>客服电话，<b>须带国际前缀</b>（如 <c>+86075586010000</c>）。</summary>
    [JsonPropertyName("office_phone")] public string? OfficePhone { get; set; }

    /// <summary>联系人姓名。</summary>
    [JsonPropertyName("contact_name")] public string? ContactName { get; set; }

    /// <summary>联系人电话。<b>个人信息，不入日志。</b></summary>
    [JsonPropertyName("contact_phone")] public string? ContactPhone { get; set; }

    /// <summary>联系人邮箱。<b>个人信息，不入日志。</b></summary>
    [JsonPropertyName("contact_email")] public string? ContactEmail { get; set; }

    /// <summary>结算银行账户信息。<b>个人／金融信息，不入日志。</b></summary>
    [JsonPropertyName("settlement_bank_number")] public string? SettlementBankNumber { get; set; }

    /// <summary>是否申请 H5 支付权限：<c>YES</c>／<c>NO</c>。</summary>
    [JsonPropertyName("apply_h5_payment")] public string? ApplyH5Payment { get; set; }

    /// <summary>H5 经营网址：<c>apply_h5_payment = YES</c> 时必填（主营业务、可售商品／服务及价格、可在线下单网址）。</summary>
    [JsonPropertyName("h5_website_url")] public string? H5WebsiteUrl { get; set; }

    /// <summary>
    /// H5 支付域名列表，<b>JSON 数组文本</b>且<b>不超过 5 个</b>；<c>apply_h5_payment = YES</c> 时必填。
    /// <b>官方原文警告：提交的修改的域名列表会覆盖已有支付域名列表</b> ⇒ 须全量上传，不做增量。
    /// </summary>
    [JsonPropertyName("h5_domains")] public string? H5Domains { get; set; }

    /// <summary>额外补充信息，如 <c>{"tid":"xxx","mid":"yyy"}</c>（修改面的官方字段表已删除该键）。</summary>
    [JsonPropertyName("attach")] public string? Attach { get; set; }
}

/// <summary>
/// APIv2 机构模式子商户<b>修改</b>（<c>/secapi/mch/modifyInstitutionsub</c>）请求体。
/// </summary>
/// <remarks>
/// <para><b>与进件面的三处官方差异</b>（照抄两支各自字段表，勿共用 DTO）：
/// ① 新增 <c>sub_mch_id</c>；② 字段表<b>删除</b> <c>merchant_remark</c> 与 <c>attach</c>
/// （官方修改页的 XML 示例仍发了 <c>merchant_remark</c>，属示例与字段表不一致，以字段表为准）；
/// ③ <c>app_id</c>／<c>mch_id</c>／<c>sub_mch_id</c> 三键官方标注「不支持修改」，仍须上传以定位主体。</para>
/// <para><b>签名面与进件页同为「无 <c>nonce_str</c>、无 <c>sign_type</c>，MD5」</b>。</para>
/// </remarks>
[HttpJsonSerializable(SerializerClassName = "PayV2")]
public class V2InstitutionSubModifyRequest
{
    /// <summary>机构的 APPID（<c>app_id</c> 拼写）。官方标注「不支持修改」，仍需上传。</summary>
    [JsonPropertyName("app_id")] public string? AppId { get; set; }

    /// <summary>机构商户号。官方标注「不支持修改」，仍需上传。</summary>
    [JsonPropertyName("mch_id")] public string? MchId { get; set; }

    /// <summary>子商户号（定位键）。官方标注「不支持修改」。</summary>
    [JsonPropertyName("sub_mch_id")] public string? SubMchId { get; set; }

    /// <summary>签名，MD5。</summary>
    [JsonPropertyName("sign")] public string? Sign { get; set; }

    /// <summary>商户名称，须与注册证件一致。</summary>
    [JsonPropertyName("merchant_name")] public string? MerchantName { get; set; }

    /// <summary>机构合作渠道编号。</summary>
    [JsonPropertyName("channel_id")] public string? ChannelId { get; set; }

    /// <summary>商户简称。</summary>
    [JsonPropertyName("merchant_shortname")] public string? MerchantShortname { get; set; }

    /// <summary>注册国家或区域代码。</summary>
    [JsonPropertyName("merchant_country_code")] public string? MerchantCountryCode { get; set; }

    /// <summary>商户类型：<c>ENTERPRISE</c>／<c>INDIVIDUAL</c>。</summary>
    [JsonPropertyName("merchant_type")] public string? MerchantType { get; set; }

    /// <summary>业务类目。</summary>
    [JsonPropertyName("business_category")] public string? BusinessCategory { get; set; }

    /// <summary>MCC 码。</summary>
    [JsonPropertyName("mcc")] public string? Mcc { get; set; }

    /// <summary>公司注册文件编号。</summary>
    [JsonPropertyName("registration_certificate_number")] public string? RegistrationCertificateNumber { get; set; }

    /// <summary>注册证件过期时间：<c>YYYY-MM-DD</c> 或 <c>PERMANENT</c>／<c>N/A</c>。</summary>
    [JsonPropertyName("registration_certificate_date")] public string? RegistrationCertificateDate { get; set; }

    /// <summary>注册证件照片的 <c>media_id</c>。</summary>
    [JsonPropertyName("registration_certificate_copy")] public string? RegistrationCertificateCopy { get; set; }

    /// <summary>经营方式：<c>ONLINE</c>／<c>OFFLINE</c>／<c>BOTH</c>。</summary>
    [JsonPropertyName("business_type")] public string? BusinessType { get; set; }

    /// <summary>APP 下载地址（<c>ONLINE</c>／<c>BOTH</c> 时四项至少选一）。</summary>
    [JsonPropertyName("app_download")] public string? AppDownload { get; set; }

    /// <summary>经营网址（官方字段名 <c>business_website</c>）。</summary>
    [JsonPropertyName("business_website")] public string? BusinessWebsite { get; set; }

    /// <summary>公众号信息（条件同上）。</summary>
    [JsonPropertyName("office_account")] public string? OfficeAccount { get; set; }

    /// <summary>小程序信息（条件同上）。</summary>
    [JsonPropertyName("mini_program")] public string? MiniProgram { get; set; }

    /// <summary>门店地址（<c>OFFLINE</c>／<c>BOTH</c> 时必传）。</summary>
    [JsonPropertyName("store_address")] public string? StoreAddress { get; set; }

    /// <summary>门店照片 <c>media_id</c> 列表（JSON 数组文本，建议 ≥3 张）。</summary>
    [JsonPropertyName("store_photos")] public string? StorePhotos { get; set; }

    /// <summary>董事／负责人姓名（<c>ENTERPRISE</c> 时选填）。</summary>
    [JsonPropertyName("director_name")] public string? DirectorName { get; set; }

    /// <summary>董事／负责人证件号。<b>个人信息，不入日志。</b></summary>
    [JsonPropertyName("director_id_number")] public string? DirectorIdNumber { get; set; }

    /// <summary>经营者姓名（<c>INDIVIDUAL</c> 时选填）。</summary>
    [JsonPropertyName("principal_name")] public string? PrincipalName { get; set; }

    /// <summary>经营者证件号。<b>个人信息，不入日志。</b></summary>
    [JsonPropertyName("principal_id_number")] public string? PrincipalIdNumber { get; set; }

    /// <summary>公司电话（须带前缀）。</summary>
    [JsonPropertyName("office_phone")] public string? OfficePhone { get; set; }

    /// <summary>联系人姓名。</summary>
    [JsonPropertyName("contact_name")] public string? ContactName { get; set; }

    /// <summary>联系人电话。<b>个人信息，不入日志。</b></summary>
    [JsonPropertyName("contact_phone")] public string? ContactPhone { get; set; }

    /// <summary>联系人邮箱。<b>个人信息，不入日志。</b></summary>
    [JsonPropertyName("contact_email")] public string? ContactEmail { get; set; }

    /// <summary>结算银行账户信息。<b>金融信息，不入日志。</b></summary>
    [JsonPropertyName("settlement_bank_number")] public string? SettlementBankNumber { get; set; }

    /// <summary>
    /// 是否申请 H5 支付权限：<c>YES</c>／<c>NO</c>。官方原文
    /// 「如已有 H5 支付权限但需要修改 H5 支付域名，需要填写为 YES」。
    /// </summary>
    [JsonPropertyName("apply_h5_payment")] public string? ApplyH5Payment { get; set; }

    /// <summary>H5 经营网址（<c>apply_h5_payment = YES</c> 时必填）。</summary>
    [JsonPropertyName("h5_website_url")] public string? H5WebsiteUrl { get; set; }

    /// <summary>
    /// H5 支付域名列表，JSON 数组文本、不超过 5 个（<c>apply_h5_payment = YES</c> 时必填）。
    /// <b>覆盖语义取自进件页原文</b>「提交的修改的域名列表会覆盖已有支付域名列表」；
    /// 修改页本行的描述文字在上游文档里被误粘成了「子商户的结算银行账户信息」，且全页未复述覆盖警告
    /// —— 两者皆属官方源文缺陷，本 SDK 按进件页的覆盖口径处理。
    /// </summary>
    [JsonPropertyName("h5_domains")] public string? H5Domains { get; set; }
}

/// <summary>
/// APIv2 机构模式子商户<b>查询</b>（<c>/secapi/mch/queryInstitutionsub</c>）请求体。
/// </summary>
/// <remarks><para>官方请求面就 4 行、全部必填：<c>app_id</c>／<c>mch_id</c>／<c>sub_mch_id</c>／<c>sign</c>。</para></remarks>
[HttpJsonSerializable(SerializerClassName = "PayV2")]
public class V2InstitutionSubQueryRequest
{
    /// <summary>机构的 APPID（<c>app_id</c> 拼写）。</summary>
    [JsonPropertyName("app_id")] public string? AppId { get; set; }

    /// <summary>机构商户号。</summary>
    [JsonPropertyName("mch_id")] public string? MchId { get; set; }

    /// <summary>子商户号。</summary>
    [JsonPropertyName("sub_mch_id")] public string? SubMchId { get; set; }

    /// <summary>签名，MD5。</summary>
    [JsonPropertyName("sign")] public string? Sign { get; set; }
}

/// <summary>
/// APIv2 机构模式子商户<b>修改</b>应答：信封 + <c>sub_mch_id</c> + <c>h5_authorization_state</c>。
/// </summary>
/// <remarks>
/// <para><b>本支应答没有 <c>verification_status</c></b>：官方只在进件与查询两页回该键
/// （修改页字段表就 8 行）⇒ 修改后的审核状态须另发查询确认。</para>
/// </remarks>
[HttpJsonSerializable(SerializerClassName = "PayV2")]
public class V2InstitutionSubResponse
{
    [JsonPropertyName("return_code")] public string? ReturnCode { get; set; }

    /// <summary>返回信息，非空时为错误原因（签名失败、参数格式校验错误等）。</summary>
    [JsonPropertyName("return_msg")] public string? ReturnMsg { get; set; }

    /// <summary>业务结果：SUCCESS / FAIL。</summary>
    [JsonPropertyName("result_code")] public string? ResultCode { get; set; }

    [JsonPropertyName("err_code")] public string? ErrCode { get; set; }
    [JsonPropertyName("err_code_des")] public string? ErrCodeDes { get; set; }

    /// <summary>微信返回的签名（官方必填，证书收发器会做验签）。</summary>
    [JsonPropertyName("sign")] public string? Sign { get; set; }

    /// <summary>微信支付为该商户创建的子商户号。</summary>
    [JsonPropertyName("sub_mch_id")] public string? SubMchId { get; set; }

    /// <summary>
    /// H5 支付权限状态，完整枚举：<c>APPROVED</c> 已有权限／<c>UNAUTHORIZED</c> 未获得也未申请过／
    /// <c>UNDER_REVIEW</c> 审核中／<c>REJECTED</c> 被驳回／<c>UNDER_PUNISHMENT</c> 曾有权限但已被处罚（须联系 BD）／
    /// <c>APPLICATION_FAILED</c> 申请单创建失败。
    /// </summary>
    [JsonPropertyName("h5_authorization_state")] public string? H5AuthorizationState { get; set; }
}

/// <summary>
/// APIv2 机构模式子商户<b>进件</b>应答：在 <see cref="V2InstitutionSubResponse"/> 之上多两键。
/// </summary>
/// <remarks>
/// <para><b><c>verification_status</c> 的取值不是大写枚举</b>：官方为自由文本
/// <c>Under review</c>（审核中，子商户暂无交易权限）／<c>Approved</c>（审核通过，可发起正常交易），
/// 且同页表格里写 <c>Under review</c>、示例里写 <c>Under Review</c>（大小写不一致属上游缺陷）
/// ⇒ 判定须走「忽略大小写 + 去空格」，不得做成精确字符串比较。</para>
/// </remarks>
[HttpJsonSerializable(SerializerClassName = "PayV2")]
public class V2InstitutionSubAddResponse : V2InstitutionSubResponse
{
    /// <summary>审核状态：<c>Under review</c>／<c>Approved</c>（大小写不稳定，见类型注释）。</summary>
    [JsonPropertyName("verification_status")] public string? VerificationStatus { get; set; }

    /// <summary>审核状态描述。</summary>
    [JsonPropertyName("description")] public string? Description { get; set; }
}

/// <summary>
/// APIv2 机构模式子商户<b>查询</b>应答（官方 40 行，本族最宽的一支）。
/// </summary>
/// <remarks>
/// <para><b>状态字段换了一套</b>：查询页不回 <c>verification_status</c>，改为
/// <c>sub_mch_status</c>（必填）+ <c>application_status</c>；两者的取值与进件页的
/// <c>verification_status</c> 不可互换判定。</para>
/// <para><b>反直觉类型</b>：本支 <c>result_code</c> 官方标 string(16)（进件／修改两支标 string(32)）、
/// <c>business_category</c> 标 string(10)（请求面标 string（3），全角括号），字段名与门控仍照抄。</para>
/// <para><b>返回个人信息</b>：法人／负责人证件号、联系人手机邮箱、结算银行账户均以明文回传，
/// 官方未标「敏感字段」⇒ SDK 侧一律不落日志、不入异常消息（PAY-B7 同线口径）。</para>
/// </remarks>
[HttpJsonSerializable(SerializerClassName = "PayV2")]
public class V2InstitutionSubQueryResponse
{
    [JsonPropertyName("return_code")] public string? ReturnCode { get; set; }
    [JsonPropertyName("return_msg")] public string? ReturnMsg { get; set; }

    /// <summary>业务结果：SUCCESS / FAIL（官方本页标注 string(16)）。</summary>
    [JsonPropertyName("result_code")] public string? ResultCode { get; set; }

    [JsonPropertyName("err_code")] public string? ErrCode { get; set; }
    [JsonPropertyName("err_code_des")] public string? ErrCodeDes { get; set; }

    /// <summary>微信返回的签名（官方必填，须验签）。</summary>
    [JsonPropertyName("sign")] public string? Sign { get; set; }

    /// <summary>子商户号。</summary>
    [JsonPropertyName("sub_mch_id")] public string? SubMchId { get; set; }

    /// <summary>商户名称。</summary>
    [JsonPropertyName("merchant_name")] public string? MerchantName { get; set; }

    /// <summary>商户简称。</summary>
    [JsonPropertyName("merchant_shortname")] public string? MerchantShortname { get; set; }

    /// <summary>注册国家或区域代码。</summary>
    [JsonPropertyName("merchant_country_code")] public string? MerchantCountryCode { get; set; }

    /// <summary>业务类目。</summary>
    [JsonPropertyName("business_category")] public string? BusinessCategory { get; set; }

    /// <summary>MCC 码。</summary>
    [JsonPropertyName("mcc")] public string? Mcc { get; set; }

    /// <summary>客服电话。<b>个人信息，不入日志。</b></summary>
    [JsonPropertyName("office_phone")] public string? OfficePhone { get; set; }

    /// <summary>联系人姓名。</summary>
    [JsonPropertyName("contact_name")] public string? ContactName { get; set; }

    /// <summary>联系人电话。<b>个人信息，不入日志。</b></summary>
    [JsonPropertyName("contact_phone")] public string? ContactPhone { get; set; }

    /// <summary>联系人邮箱。<b>个人信息，不入日志。</b></summary>
    [JsonPropertyName("contact_email")] public string? ContactEmail { get; set; }

    /// <summary>机构为商户备注的信息（进件时上传的 <c>merchant_remark</c>）。</summary>
    [JsonPropertyName("merchant_remark")] public string? MerchantRemark { get; set; }

    /// <summary>经营方式：<c>ONLINE</c>／<c>OFFLINE</c>／<c>BOTH</c>。</summary>
    [JsonPropertyName("business_type")] public string? BusinessType { get; set; }

    /// <summary>商户类型：<c>ENTERPRISE</c>／<c>INDIVIDUAL</c>。</summary>
    [JsonPropertyName("merchant_type")] public string? MerchantType { get; set; }

    /// <summary>公司注册文件编号。<b>个人信息，不入日志。</b></summary>
    [JsonPropertyName("registration_certificate_number")] public string? RegistrationCertificateNumber { get; set; }

    /// <summary>注册证件过期时间。</summary>
    [JsonPropertyName("registration_certificate_date")] public string? RegistrationCertificateDate { get; set; }

    /// <summary>注册证件照片的 <c>media_id</c>。</summary>
    [JsonPropertyName("registration_certificate_copy")] public string? RegistrationCertificateCopy { get; set; }

    /// <summary>门店照片 <c>media_id</c> 列表（JSON 数组文本）。</summary>
    [JsonPropertyName("store_photos")] public string? StorePhotos { get; set; }

    /// <summary>APP 下载地址。</summary>
    [JsonPropertyName("app_download")] public string? AppDownload { get; set; }

    /// <summary>经营网址。</summary>
    [JsonPropertyName("business_website")] public string? BusinessWebsite { get; set; }

    /// <summary>公众号信息。</summary>
    [JsonPropertyName("office_account")] public string? OfficeAccount { get; set; }

    /// <summary>小程序信息。</summary>
    [JsonPropertyName("mini_program")] public string? MiniProgram { get; set; }

    /// <summary>门店地址。</summary>
    [JsonPropertyName("store_address")] public string? StoreAddress { get; set; }

    /// <summary>董事／负责人姓名。</summary>
    [JsonPropertyName("director_name")] public string? DirectorName { get; set; }

    /// <summary>董事／负责人证件号。<b>个人信息，不入日志。</b></summary>
    [JsonPropertyName("director_id_number")] public string? DirectorIdNumber { get; set; }

    /// <summary>经营者姓名。</summary>
    [JsonPropertyName("principal_name")] public string? PrincipalName { get; set; }

    /// <summary>经营者证件号。<b>个人信息，不入日志。</b></summary>
    [JsonPropertyName("principal_id_number")] public string? PrincipalIdNumber { get; set; }

    /// <summary>结算银行账户信息。<b>金融信息，不入日志。</b></summary>
    [JsonPropertyName("settlement_bank_number")] public string? SettlementBankNumber { get; set; }

    /// <summary>机构合作渠道编号。</summary>
    [JsonPropertyName("channel_id")] public string? ChannelId { get; set; }

    /// <summary>H5 支付权限状态（六值枚举见 <see cref="V2InstitutionSubResponse.H5AuthorizationState"/>）。</summary>
    [JsonPropertyName("h5_authorization_state")] public string? H5AuthorizationState { get; set; }

    /// <summary>H5 权限被驳回／申请失败时的驳回详情。</summary>
    [JsonPropertyName("h5_audit_reject_detail")] public string? H5AuditRejectDetail { get; set; }

    /// <summary>
    /// 子商户状态，完整枚举：<c>OPERATING</c> 正常／<c>PENDING</c> 待处理／<c>DEACTIVATED</c> 已停用／
    /// <c>CLOSED</c> 已关闭／<c>INCOMPLETE_APPLICATION</c> 未完成入驻。
    /// </summary>
    [JsonPropertyName("sub_mch_status")] public string? SubMchStatus { get; set; }

    /// <summary>入驻申请状态：<c>UNDER_REVIEW</c> 审核中／<c>REJECTED</c> 已驳回。</summary>
    [JsonPropertyName("application_status")] public string? ApplicationStatus { get; set; }

    /// <summary>
    /// 入驻申请被驳回的原因，<b>JSON 数组文本</b>，每项为 <c>{field, reason}</c>；本 SDK 原样交回字符串。
    /// </summary>
    [JsonPropertyName("application_reject_detail")] public string? ApplicationRejectDetail { get; set; }

    /// <summary>商户提供的额外补充信息原文。</summary>
    [JsonPropertyName("attach")] public string? Attach { get; set; }
}
