// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

using Mud.Wechat.Pay.DataModels.Common;

namespace Mud.Wechat.Pay.DataModels.Ecommerce;

/// <summary>
/// 申请二级商户商家转账授权（<c>POST /v3/ecommerce/mch-transfer/authorizations</c>）请求体。
/// </summary>
/// <remarks>
/// <para>
/// <b>官方文档</b>：<see href="https://pay.weixin.qq.com/doc/v3/partner/4013504208"/>
/// （2026-10-15 逐字段核验，全表 5 行；支持商户：【平台商户】）。
/// </para>
/// <para>
/// <b>本接口无 <c>Wechatpay-Serial</c> 头</b>（官方 Header 表只有 Authorization/Accept/Content-Type）
/// ⇒ 与「发起转账」不同，授权申请侧没有加密字段，无须上送公钥序列号。
/// </para>
/// <para>
/// <b>协议列表是必填数组</b>：官方把 <c>agreement_list</c> 与其内层四字段全标必填 ⇒ 至少要提交一份协议签署记录，
/// <see cref="EcommerceTransferAgreement.AgreementCode"/> 官方只给一个取值 <c>1</c>。
/// </para>
/// </remarks>
[HttpJsonSerializable(SerializerClassName = "Ecommerce")]
public class EcommerceTransferAuthorizationRequest
{
    /// <summary>二级商户号（<c>sub_mchid</c>，必填 string(32)）：需授权商家转账功能的商户号。</summary>
    [JsonPropertyName("sub_mchid")]
    public string? SubMchid { get; set; }

    /// <summary>签约协议列表（<c>agreement_list</c>，必填数组）：提交二级商户已签署的微信支付协议记录，见 <see cref="EcommerceTransferAgreement"/>。</summary>
    [JsonPropertyName("agreement_list")]
    public IReadOnlyList<EcommerceTransferAgreement>? AgreementList { get; set; }
}

/// <summary>
/// 商家转账授权申请里的单份签约协议（<c>agreement_list[]</c> 的元素形态，四字段官方全标必填）。
/// </summary>
/// <remarks>
/// 出处 <see href="https://pay.weixin.qq.com/doc/v3/partner/4013504208"/>。
/// <see cref="AgreementVersion"/> 官方口径为「协议发布时间」，形如 <c>20230101</c> ⇒ 不是自增版本号，
/// 传协议正文里的发布日期；<see cref="SignOrganization"/> 须为二级商户主体名称。
/// </remarks>
[HttpJsonSerializable(SerializerClassName = "Ecommerce")]
public class EcommerceTransferAgreement
{
    /// <summary>协议编号（<c>agreement_code</c>，必填 string(16)）：官方本页只提供一个取值 <c>1</c>，见 <see cref="EcommerceTransferAgreementCodes"/>。</summary>
    [JsonPropertyName("agreement_code")]
    public string? AgreementCode { get; set; }

    /// <summary>协议版本号（<c>agreement_version</c>，必填 string(16)）：版本号为协议发布时间，格式如 <c>20230101</c>。</summary>
    [JsonPropertyName("agreement_version")]
    public string? AgreementVersion { get; set; }

    /// <summary>签约主体（<c>sign_organization</c>，必填 string(128)）：二级商户的主体名称。</summary>
    [JsonPropertyName("sign_organization")]
    public string? SignOrganization { get; set; }

    /// <summary>签约时间（<c>sign_time</c>，必填 string(32)）：rfc3339 格式 <c>YYYY-MM-DDTHH:mm:ss+TIMEZONE</c>。</summary>
    [JsonPropertyName("sign_time")]
    public string? SignTime { get; set; }
}

/// <summary>
/// 商家转账授权应答 —— <b>申请（<see href="https://pay.weixin.qq.com/doc/v3/partner/4013504208"/>）
/// 与查询（<see href="https://pay.weixin.qq.com/doc/v3/partner/4013504209"/>）共用</b>：
/// 2026-10-15 比对为「字段名与类型全同，查询页少文档化一行 <c>accept_time</c>」。
/// </summary>
/// <remarks>
/// <para>
/// <b>申请成功 ≠ 已授权</b>：官方值集含中间态 <see cref="EcommerceTransferAuthorizationStates.AuthorizeProcessing"/>
/// （「授权申请正在处理中，请稍后查询授权结果」）⇒ 本接口回 200 只表示申请受理，
/// 必须用查询方法轮询到 <c>AUTHORIZED</c> 才能发起转账。
/// </para>
/// <para>
/// <see cref="AcceptTime"/> 官方只在申请页文档化（且标必填），查询页未列出该字段
/// ⇒ 反序列化时可能为 null，勿以其存在与否判断调用的是哪一支。
/// </para>
/// <para>
/// 本面 <c>authorization_state</c> 是三值集，<b>与个人收款方签约面的两值 <c>auth_state</c> 不同</b>
/// （见 <see cref="EcommerceAuthStates"/>），字段名与值集都不得互换。
/// </para>
/// </remarks>
[HttpJsonSerializable(SerializerClassName = "Ecommerce")]
public class EcommerceTransferAuthorizationResponse : WechatPayResponse
{
    /// <summary>二级商户号（<c>sub_mchid</c>，必填 string(32)）：微信支付分配的商户号。</summary>
    [JsonPropertyName("sub_mchid")]
    public string? SubMchid { get; set; }

    /// <summary>授权状态（<c>authorization_state</c>，必填）：见 <see cref="EcommerceTransferAuthorizationStates"/>，<c>AUTHORIZE_PROCESSING</c> 为非终态须回查。</summary>
    [JsonPropertyName("authorization_state")]
    public string? AuthorizationState { get; set; }

    /// <summary>授权申请受理时间（<c>accept_time</c>，必填 string(32)）：rfc3339 格式；<b>仅申请页文档化本字段</b>，查询页未列出。</summary>
    [JsonPropertyName("accept_time")]
    public string? AcceptTime { get; set; }

    /// <summary>授权时间（<c>authorize_time</c>，选填 string(32)）：rfc3339 格式；未完成授权时不返回。</summary>
    [JsonPropertyName("authorize_time")]
    public string? AuthorizeTime { get; set; }
}

/// <summary>
/// 发起商家转账（<c>POST /v3/ecommerce/mch-transfer/transfer-bills</c>）请求体。
/// </summary>
/// <remarks>
/// <para>
/// <b>官方文档</b>：<see href="https://pay.weixin.qq.com/doc/v3/partner/4013504211"/>
/// （2026-10-15 逐字段核验，全表 9 行；支持商户：【平台商户】）。
/// </para>
/// <para>
/// <b>本接口 Header 必填 <c>Wechatpay-Serial</c></b>：官方要求用<b>微信支付公钥</b>（推荐）或平台证书公钥
/// 加密敏感字段后上送公钥 ID / 证书序列号 ⇒ 唯一的敏感字段是 <see cref="UserName"/>
/// （<c>string(1024)</c> 密文，标准 RSA 与国密均支持，公钥由微信侧提供）。
/// 明文直传该字段会导致转账失败或泄露收款人姓名，SDK 不代为加密（加密属宿主证书资产）。
/// </para>
/// <para>
/// <b><see cref="UserName"/> 的条件必填</b>：官方原文「转账金额 &gt;= 2,000 元时，该笔明细必须填写」
/// ⇒ 阈值以<b>元</b>表述而 <see cref="TransferAmount"/> 以<b>分</b>传递，判断须换算为 200000 分。
/// 传入后微信支付会校验收款用户与姓名是否一致，并提供电子回单。
/// </para>
/// <para>
/// <b><see cref="Appid"/> 是服务商 AppID</b>，且必须是 <see cref="Openid"/> 所属的那个 AppID
/// （官方：「请填写收款用户 OpenID 对应的 AppID」）⇒ AppID 与 OpenID 不匹配时官方直接校验失败。
/// </para>
/// <para>
/// <b><see cref="OutBillNo"/> 字符集比退款单号严格</b>：官方要求「只能由数字、大小写字母组成」，
/// 不含 <c>- _ | * @</c>；同域退款支的 <c>out_refund_no</c> 允许这些符号 ⇒ 复用同一套单号生成器会在此报错。
/// </para>
/// <para>
/// <see cref="TransferSceneId"/> 须在「平台商家转账 - 产品设置」中已获取；
/// <see cref="NotifyUrl"/> 官方要求公网可访问、<b>必须 HTTPS</b>、不能携带参数。
/// </para>
/// </remarks>
[HttpJsonSerializable(SerializerClassName = "Ecommerce")]
public class EcommerceTransferBillCreateRequest
{
    /// <summary>二级商户号（<c>sub_mchid</c>，必填 string(32)）：出资商户号。</summary>
    [JsonPropertyName("sub_mchid")]
    public string? SubMchid { get; set; }

    /// <summary>服务商 AppID（<c>appid</c>，必填 string(32)）：必须是 <see cref="Openid"/> 对应的 AppID。</summary>
    [JsonPropertyName("appid")]
    public string? Appid { get; set; }

    /// <summary>商户单号（<c>out_bill_no</c>，必填 string(32)）：商户系统内部唯一，<b>官方限定只能由数字、大小写字母组成</b>。</summary>
    [JsonPropertyName("out_bill_no")]
    public string? OutBillNo { get; set; }

    /// <summary>转账场景 ID（<c>transfer_scene_id</c>，必填 string(16)）：可在「平台商家转账 - 产品设置」查看已获得场景和申请新场景。</summary>
    [JsonPropertyName("transfer_scene_id")]
    public string? TransferSceneId { get; set; }

    /// <summary>收款用户 OpenID（<c>openid</c>，必填 string(64)）：服务商 AppID 下收款用户的 OpenID；传入后官方校验其与确认收款用户的微信是否一致，不一致无法转账成功。</summary>
    [JsonPropertyName("openid")]
    public string? Openid { get; set; }

    /// <summary>收款用户真实姓名（<c>user_name</c>，选填 string(1024)）：<b>需加密</b>且请求头上送 <c>Wechatpay-Serial</c>；转账金额 ≥ 2,000 元时必填。</summary>
    [JsonPropertyName("user_name")]
    public string? UserName { get; set; }

    /// <summary>转账金额（<c>transfer_amount</c>，必填 integer）：单位为<b>分</b>；≥ 200000 分（2,000 元）时 <see cref="UserName"/> 必填。</summary>
    [JsonPropertyName("transfer_amount")]
    public long? TransferAmount { get; set; }

    /// <summary>转账备注（<c>transfer_remark</c>，必填 string(32)）：微信用户收款时可见，最多 32 个字符。</summary>
    [JsonPropertyName("transfer_remark")]
    public string? TransferRemark { get; set; }

    /// <summary>商户回调地址（<c>notify_url</c>，选填 string(256)）：异步接收用户确认收款及转账结果；须公网可访问、<b>必须 HTTPS</b>、不能携带参数。</summary>
    [JsonPropertyName("notify_url")]
    public string? NotifyUrl { get; set; }
}

/// <summary>
/// 发起商家转账应答（<c>POST /v3/ecommerce/mch-transfer/transfer-bills</c>，7 行）。
/// </summary>
/// <remarks>
/// <para>
/// 与查询应答 <see cref="EcommerceMchTransferBillQueryResponse"/> <b>不共用</b>（逐页比对实测）：
/// 本支独有 <see cref="PackageInfo"/>，查询支独有 <c>mchid</c>/<c>appid</c>/<c>transfer_scene_id</c>/
/// <c>transfer_amount</c>/<c>transfer_remark</c>/<c>openid</c>/<c>user_name</c>/<c>update_time</c> ⇒ 双侧各有独占字段。
/// </para>
/// <para>
/// <b><see cref="PackageInfo"/> 24 小时内有效</b>（官方原文）且只在待用户确认收款时使用：
/// 把它交给小程序/APP 侧拉起微信收款确认页；过期后须按查询到的状态决定是否重新处理，
/// 官方对 <c>TRANSFERING</c> 的指引是「可拉起微信收款确认页面再次重试确认收款」。
/// </para>
/// </remarks>
[HttpJsonSerializable(SerializerClassName = "Ecommerce")]
public class EcommerceTransferBillCreateResponse : WechatPayResponse
{
    /// <summary>二级商户号（<c>sub_mchid</c>，必填 string(32)）：出资商户号。</summary>
    [JsonPropertyName("sub_mchid")]
    public string? SubMchid { get; set; }

    /// <summary>商户单号（<c>out_bill_no</c>，必填 string(32)）：商户系统内部的商家单号。</summary>
    [JsonPropertyName("out_bill_no")]
    public string? OutBillNo { get; set; }

    /// <summary>微信转账单号（<c>transfer_bill_no</c>，必填 string(64)）：微信商家转账系统返回的唯一标识。</summary>
    [JsonPropertyName("transfer_bill_no")]
    public string? TransferBillNo { get; set; }

    /// <summary>单据创建时间（<c>create_time</c>，必填 string(32)）：单据受理成功时返回，rfc3339 格式。</summary>
    [JsonPropertyName("create_time")]
    public string? CreateTime { get; set; }

    /// <summary>单据状态（<c>state</c>，必填）：见 <see cref="EcommerceMchTransferBillStates"/>；刚受理通常为 <c>ACCEPTED</c> 或 <c>WAIT_USER_CONFIRM</c>，非终态须回查。</summary>
    [JsonPropertyName("state")]
    public string? State { get; set; }

    /// <summary>失败原因（<c>fail_reason</c>，选填 string(64)）：订单已失败或已退资金时返回。</summary>
    [JsonPropertyName("fail_reason")]
    public string? FailReason { get; set; }

    /// <summary>跳转领取页面的 package 信息（<c>package_info</c>，选填 string(1024)）：拉起微信支付收款页用，<b>官方注明 24 小时内有效</b>。</summary>
    [JsonPropertyName("package_info")]
    public string? PackageInfo { get; set; }
}

/// <summary>
/// 撤销商家转账（<c>POST /v3/ecommerce/mch-transfer/transfer-bills/out-bill-no/{out_bill_no}/cancel</c>）请求体。
/// </summary>
/// <remarks>
/// <para>
/// <b>官方文档</b>：<see href="https://pay.weixin.qq.com/doc/v3/partner/4013504212"/>
/// （2026-10-15 逐字段核验，Body 仅 1 行）。
/// </para>
/// <para>
/// <b>单号走 Path、Body 只剩出资商户号</b>：与发起转账「全字段进 Body」相反 ⇒
/// 商户单号在本接口是路径参数 <c>out_bill_no</c>（string(32)，官方限定数字与大小写字母），
/// 请求体只承载 <see cref="SubMchid"/>。
/// </para>
/// <para>
/// 官方对撤销的语义警示：<b>返回成功仅表示撤销请求已受理</b>，退款等操作异步处理，
/// 以最终查询单据返回状态为准 ⇒ 不得把本应答的 <c>CANCELING</c> 当作已撤销。
/// 撤销窗口是「用户确认收款之前」。
/// </para>
/// </remarks>
[HttpJsonSerializable(SerializerClassName = "Ecommerce")]
public class EcommerceTransferBillCancelRequest
{
    /// <summary>二级商户号（<c>sub_mchid</c>，必填 string(32)）：出资商户号。</summary>
    [JsonPropertyName("sub_mchid")]
    public string? SubMchid { get; set; }
}

/// <summary>
/// 撤销商家转账应答（5 行）：只回官方本页文档化的撤销态字段。
/// </summary>
/// <remarks>
/// <para>
/// <b><see cref="State"/> 官方在本页只列两值</b>（<c>CANCELING</c> 撤销中、<c>CANCELLED</c> 已撤销），
/// 属 <see cref="EcommerceMchTransferBillStates"/> 的子集 ⇒ 撤销同步应答不会出现 <c>SUCCESS</c>/<c>FAIL</c>。
/// </para>
/// <para>
/// 官方：本接口成功仅表示撤销请求已受理，最终结果须以查询单据状态为准
/// （<see href="https://pay.weixin.qq.com/doc/v3/partner/4013504213"/> 或
/// <see href="https://pay.weixin.qq.com/doc/v3/partner/4013504214"/>）。
/// </para>
/// </remarks>
[HttpJsonSerializable(SerializerClassName = "Ecommerce")]
public class EcommerceTransferBillCancelResponse : WechatPayResponse
{
    /// <summary>二级商户号（<c>sub_mchid</c>，必填 string(32)）：出资商户号。</summary>
    [JsonPropertyName("sub_mchid")]
    public string? SubMchid { get; set; }

    /// <summary>商户单号（<c>out_bill_no</c>，必填 string(32)）：商户系统内部的商家单号。</summary>
    [JsonPropertyName("out_bill_no")]
    public string? OutBillNo { get; set; }

    /// <summary>微信转账单号（<c>transfer_bill_no</c>，必填 string(64)）：微信商家转账系统返回的唯一标识。</summary>
    [JsonPropertyName("transfer_bill_no")]
    public string? TransferBillNo { get; set; }

    /// <summary>单据状态（<c>state</c>，必填）：本页官方只给 <c>CANCELING</c>（撤销中）/ <c>CANCELLED</c>（已撤销）两值。</summary>
    [JsonPropertyName("state")]
    public string? State { get; set; }

    /// <summary>状态变更时间（<c>update_time</c>，必填）：最后一次状态变更时间，rfc3339 格式。</summary>
    [JsonPropertyName("update_time")]
    public string? UpdateTime { get; set; }
}

/// <summary>
/// 商家转账订单查询应答（14 行）—— <b>两支查询共用同一形态</b>：
/// 按商户单号（<c>GET /v3/ecommerce/mch-transfer/transfer-bills/out-bill-no/{out_bill_no}</c>，
/// <see href="https://pay.weixin.qq.com/doc/v3/partner/4013504213"/>）与
/// 按微信转账单号（<c>GET /v3/ecommerce/mch-transfer/transfer-bills/transfer-bill-no/{transfer_bill_no}</c>，
/// <see href="https://pay.weixin.qq.com/doc/v3/partner/4013504214"/>），
/// 2026-10-15 逐字段比对结果为<b>完全一致</b>（两支都要求 <c>sub_mchid</c> 走 query）。
/// </summary>
/// <remarks>
/// <para>
/// <b>本应答比发起转账多回 <c>mchid</c>（服务商商户号）</b>：发起应答里没有该字段，
/// 需拿服务商商户号时只能走查询 ⇒ 别指望从发起应答里读 <c>mchid</c>。
/// </para>
/// <para>
/// <b>本应答没有 <c>package_info</c></b>：官方只在发起支返回它 ⇒ 过了 24 小时窗口或未保存该值时，
/// 无法从查询重新取得拉起收款页的数据。
/// </para>
/// <para>
/// <see cref="UserName"/> 在本应答里是<b>密文形态透传</b>（官方描述与请求侧一致，
/// 支持标准 RSA 与国密、公钥由微信侧提供）⇒ 需要明文时由宿主用自身私钥解密，
/// 且该值不得进日志、遥测或异常消息。
/// </para>
/// </remarks>
[HttpJsonSerializable(SerializerClassName = "Ecommerce")]
public class EcommerceMchTransferBillQueryResponse : WechatPayResponse
{
    /// <summary>服务商商户号（<c>mchid</c>，必填 string(32)）：微信支付分配的商户号；<b>仅查询应答返回，发起应答无此字段</b>。</summary>
    [JsonPropertyName("mchid")]
    public string? Mchid { get; set; }

    /// <summary>二级商户号（<c>sub_mchid</c>，必填 string(32)）：出资商户号。</summary>
    [JsonPropertyName("sub_mchid")]
    public string? SubMchid { get; set; }

    /// <summary>商户单号（<c>out_bill_no</c>，必填 string(32)）：商户系统内部的商家单号。</summary>
    [JsonPropertyName("out_bill_no")]
    public string? OutBillNo { get; set; }

    /// <summary>微信转账单号（<c>transfer_bill_no</c>，必填 string(64)）：微信商家转账系统返回的唯一标识。</summary>
    [JsonPropertyName("transfer_bill_no")]
    public string? TransferBillNo { get; set; }

    /// <summary>服务商 AppID（<c>appid</c>，必填 string(32)）：与收款用户 OpenID 对应的 AppID。</summary>
    [JsonPropertyName("appid")]
    public string? Appid { get; set; }

    /// <summary>转账场景 ID（<c>transfer_scene_id</c>，必填 string(16)）：该笔转账使用的转账场景。</summary>
    [JsonPropertyName("transfer_scene_id")]
    public string? TransferSceneId { get; set; }

    /// <summary>单据状态（<c>state</c>，必填）：见 <see cref="EcommerceMchTransferBillStates"/>；终态只有 <c>SUCCESS</c>/<c>FAIL</c>/<c>CANCELLED</c>。</summary>
    [JsonPropertyName("state")]
    public string? State { get; set; }

    /// <summary>转账金额（<c>transfer_amount</c>，必填 integer）：单位为<b>分</b>。</summary>
    [JsonPropertyName("transfer_amount")]
    public long? TransferAmount { get; set; }

    /// <summary>转账备注（<c>transfer_remark</c>，必填 string(32)）：UTF-8 编码，最多 32 个字符。</summary>
    [JsonPropertyName("transfer_remark")]
    public string? TransferRemark { get; set; }

    /// <summary>失败原因（<c>fail_reason</c>，选填 string(64)）：订单已失败或已退资金时返回。</summary>
    [JsonPropertyName("fail_reason")]
    public string? FailReason { get; set; }

    /// <summary>收款用户 OpenID（<c>openid</c>，选填 string(64)）：服务商 AppID 下收款用户的 OpenID。</summary>
    [JsonPropertyName("openid")]
    public string? Openid { get; set; }

    /// <summary>收款用户姓名（<c>user_name</c>，选填 string(1024)）：<b>密文</b>，需宿主以自身私钥解密；不得进日志与异常消息。</summary>
    [JsonPropertyName("user_name")]
    public string? UserName { get; set; }

    /// <summary>单据创建时间（<c>create_time</c>，必填 string(32)）：单据受理成功时返回，rfc3339 格式。</summary>
    [JsonPropertyName("create_time")]
    public string? CreateTime { get; set; }

    /// <summary>状态变更时间（<c>update_time</c>，必填 string(32)）：最后一次状态变更时间，rfc3339 格式。</summary>
    [JsonPropertyName("update_time")]
    public string? UpdateTime { get; set; }
}
