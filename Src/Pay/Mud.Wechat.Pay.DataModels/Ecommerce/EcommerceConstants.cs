// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

namespace Mud.Wechat.Pay.DataModels.Ecommerce;

/// <summary>
/// 二级商户账户类型（<c>account_type</c>）取值 —— <b>同名参数、四套取值集合，逐页核对不得合并</b>。
/// </summary>
/// <remarks>
/// <para>
/// <b>本类存在的理由是「不要按域写一套枚举」</b>（2026-10-15 逐页取「可选取值」原文比对）：
/// </para>
/// <list type="bullet">
/// <item><description>实时余额 <see href="https://pay.weixin.qq.com/doc/v3/partner/4012476690"/>：
/// <c>BASIC</c> / <c>FEES</c> / <c>OPERATION</c> / <c>DEPOSIT</c>（四值，选填）。</description></item>
/// <item><description>日终余额 <see href="https://pay.weixin.qq.com/doc/v3/partner/4012476693"/>：
/// <b>只有</b> <c>BASIC</c> / <c>DEPOSIT</c>（两值，选填、不填默认 <c>BASIC</c>）——
/// 手续费与运营账户<b>没有</b>日终余额面，照实时余额页传 <c>FEES</c> 即 <c>PARAM_ERROR</c>。</description></item>
/// <item><description>申请预约提现 <see href="https://pay.weixin.qq.com/doc/v3/partner/4012476652"/>：
/// <c>BASIC</c> / <c>FEES</c> / <c>OPERATION</c>（三值，<b>无</b> <c>DEPOSIT</c>）。</description></item>
/// <item><description>资金流水账单 <see href="https://pay.weixin.qq.com/doc/v3/partner/4012760697"/>：
/// <b>只接受</b> <c>ALL</c>（官方原文「该枚举值只限电商平台下载二级商户资金流水账单场景使用」）。</description></item>
/// </list>
/// <para>
/// 提现查询两支应答里的 <c>account_type</c> 又回到 <c>BASIC</c> / <c>FEES</c> / <c>OPERATION</c> 三值
/// （<see href="https://pay.weixin.qq.com/doc/v3/partner/4012476656"/>）⇒ 「请求侧收窄、应答侧另集」也是本页形态，
/// 反序列化时不要把枚举值当请求枚举的子集校验。
/// </para>
/// </remarks>
public static class EcommerceAccountTypes
{
    /// <summary>基本账户（<c>BASIC</c>）：四页共有的唯一取值。</summary>
    public const string Basic = "BASIC";

    /// <summary>手续费账户（<c>FEES</c>）：<b>日终余额页与账单页不合法</b>。</summary>
    public const string Fees = "FEES";

    /// <summary>运营账户（<c>OPERATION</c>）：<b>日终余额页与账单页不合法</b>。</summary>
    public const string Operation = "OPERATION";

    /// <summary>保证金账户（<c>DEPOSIT</c>）：实时/日终余额页合法，<b>预约提现页不合法</b>。</summary>
    public const string Deposit = "DEPOSIT";

    /// <summary>所有账户（<c>ALL</c>）：<b>仅</b>资金流水账单支使用。</summary>
    public const string All = "ALL";
}

/// <summary>预约提现单状态（<c>status</c>）取值。</summary>
/// <remarks>
/// 出处 <see href="https://pay.weixin.qq.com/doc/v3/partner/4012476656"/> 与
/// <see href="https://pay.weixin.qq.com/doc/v3/partner/4012476665"/>（同一应答表，六值原文照抄：
/// <c>CREATE_SUCCESS</c> / <c>SUCCESS</c> / <c>FAIL</c> / <c>REFUND</c> / <c>CLOSE</c> / <c>INIT</c>）。
/// <b>没有</b> <c>PROCESSING</c> / <c>ACCEPT</c> —— 别套用直连提现或批量转账的状态集。
/// <b>终态只有</b> <c>SUCCESS</c> / <c>FAIL</c> / <c>CLOSE</c>；<c>INIT</c> 与 <c>CREATE_SUCCESS</c> 是在途态，
/// <c>REFUND</c>（退票）是「钱已出账又被银行退回」⇒ <b>既不得按成功记账、也不得按失败重发</b>，须走人工对账。
/// </remarks>
public static class EcommerceWithdrawStatuses
{
    /// <summary>业务单已创建（<c>INIT</c>）。</summary>
    public const string Init = "INIT";

    /// <summary>受理成功（<c>CREATE_SUCCESS</c>）：<b>受理 ≠ 到账</b>，须继续轮询。</summary>
    public const string CreateSuccess = "CREATE_SUCCESS";

    /// <summary>提现成功（<c>SUCCESS</c>）。</summary>
    public const string Success = "SUCCESS";

    /// <summary>提现失败（<c>FAIL</c>）：失败原因见 <c>reason</c>。</summary>
    public const string Fail = "FAIL";

    /// <summary>关单（<c>CLOSE</c>）。</summary>
    public const string Close = "CLOSE";

    /// <summary>提现退票（<c>REFUND</c>）：出款后被银行退回，须人工处置。</summary>
    public const string Refund = "REFUND";
}

/// <summary>账单文件加密算法（<c>algorithm</c>）取值。</summary>
/// <remarks>
/// 出处 <see href="https://pay.weixin.qq.com/doc/v3/partner/4012760697"/>，两值
/// <c>AEAD_AES_256_GCM</c> / <c>SM4_GCM</c>（密钥长度 128bit）。
/// <b><c>algorithm</c> 在收付通账单支是必填 query</b>（直连资金账单无该要求）⇒ 不传即 <c>PARAM_ERROR</c>；
/// 官方<b>没有</b>「不加密」取值。
/// </remarks>
public static class EcommerceBillAlgorithms
{
    /// <summary>AEAD_AES_256_GCM 加密：需以商户证书公钥解出 <c>encrypt_key</c> 后再解密账单。</summary>
    public const string AeadAes256Gcm = "AEAD_AES_256_GCM";

    /// <summary>SM4_GCM 加密（密钥长度 128bit）：国密算法分支。</summary>
    public const string Sm4Gcm = "SM4_GCM";
}

/// <summary>账单压缩类型（<c>tar_type</c>）取值。</summary>
/// <remarks>选填；<b>不填则以不压缩方式返回数据流</b>（官方原文），故不要默认补 <c>GZIP</c>。</remarks>
public static class EcommerceBillTarTypes
{
    /// <summary>GZIP 压缩（<c>GZIP</c>）：返回 <c>.gzip</c> 压缩包账单。</summary>
    public const string Gzip = "GZIP";
}

/// <summary>账单哈希类型（<c>hash_type</c>）取值。</summary>
public static class EcommerceBillHashTypes
{
    /// <summary>Secure Hash Algorithm 1（<c>SHA1</c>）：本域唯一取值。</summary>
    public const string Sha1 = "SHA1";
}

/// <summary>注销申请「是否提取资金」标记（<c>withdraw</c>）取值。</summary>
/// <remarks>
/// 出处 <see href="https://pay.weixin.qq.com/doc/v3/partner/4013892756"/>（请求）与
/// <see href="https://pay.weixin.qq.com/doc/v3/partner/4013892759"/>（应答回显），两值。
/// <b>这是开关、不是状态</b>：填 <c>APPLY_WITHDRAW</c> 才需要收款账号；
/// 与 <see cref="EcommerceCancelWithdrawStates"/>（提现状态三值）无关，勿混用。
/// </remarks>
public static class EcommerceCancelWithdrawFlags
{
    /// <summary>仅注销，不申请提现（<c>NOT_APPLY_WITHDRAW</c>）。</summary>
    public const string NotApplyWithdraw = "NOT_APPLY_WITHDRAW";

    /// <summary>申请提现（<c>APPLY_WITHDRAW</c>）：此时 <c>payee_info</c> 必填。</summary>
    public const string ApplyWithdraw = "APPLY_WITHDRAW";
}

/// <summary>注销提现申请单状态（<c>cancel_state</c>）取值 —— <b>九值</b>。</summary>
/// <remarks>
/// <para>
/// 出处 <see href="https://pay.weixin.qq.com/doc/v3/partner/4013892759"/> 与
/// <see href="https://pay.weixin.qq.com/doc/v3/partner/4013892765"/>（两支应答表逐字段相同）。
/// </para>
/// <para>
/// <b>与电商注销申请族的同名 <c>cancel_state</c> 是两套值集</b>（后者三值，见
/// <see cref="EcommerceCancelApplicationStates"/>）⇒ 判定器不得共用，
/// 这是本域最易写错的一处（2026-10-15 逐页取「可选取值」原文比对所得）。
/// </para>
/// <para>
/// <b>终态只有</b> <c>CANCELED</c>（已注销）与 <c>FINISH</c>（注销完成）；
/// <c>WAITING_MERCHANT_CONFIRM</c> 需要商户超级管理员确认（或平台代确认），
/// <c>FUND_PROCESSING</c> 才有 <c>withdraw_state</c> 与逐账户付款结果。
/// </para>
/// </remarks>
public static class EcommerceCancelStates
{
    /// <summary>已受理（<c>ACCEPTED</c>）。</summary>
    public const string Accepted = "ACCEPTED";

    /// <summary>审核中（<c>REVIEWING</c>）。</summary>
    public const string Reviewing = "REVIEWING";

    /// <summary>审批驳回（<c>REJECTED</c>）。</summary>
    public const string Rejected = "REJECTED";

    /// <summary>等待商户确认（<c>WAITING_MERCHANT_CONFIRM</c>）：须取 <c>confirm_cancel.confirm_cancel_url</c> 引导确认。</summary>
    public const string WaitingMerchantConfirm = "WAITING_MERCHANT_CONFIRM";

    /// <summary>已撤销（<c>REVOKED</c>）。</summary>
    public const string Revoked = "REVOKED";

    /// <summary>系统处理中（<c>SYSTEM_PROCESSING</c>）。</summary>
    public const string SystemProcessing = "SYSTEM_PROCESSING";

    /// <summary>已注销（<c>CANCELED</c>）：注意与「注销完成」<c>FINISH</c> 是两个值。</summary>
    public const string Canceled = "CANCELED";

    /// <summary>资金处理中（<c>FUND_PROCESSING</c>）：此时才有提现状态与逐账户付款结果。</summary>
    public const string FundProcessing = "FUND_PROCESSING";

    /// <summary>注销完成（<c>FINISH</c>）：终态，但<b>不等于每个账户都付款成功</b>。</summary>
    public const string Finish = "FINISH";
}

/// <summary>注销提现的资金状态（<c>withdraw_state</c>）取值。</summary>
/// <remarks>
/// 出处 <see href="https://pay.weixin.qq.com/doc/v3/partner/4013892759"/>，三值，<b>选填</b>
/// （仅申请单进入 <c>FUND_PROCESSING</c> 后返回）。
/// <b><c>WITHDRAW_EXCEPTION</c> 是需人工介入的分支</b>：既非成功也非失败，不得按失败重发提现。
/// 另有「提现单状态」<see cref="EcommerceWithdrawStatuses"/>（六值）是<b>预约提现</b>域的概念，与此处三值无关。
/// </remarks>
public static class EcommerceCancelWithdrawStates
{
    /// <summary>提现处理中（<c>WITHDRAW_PROCESSING</c>）。</summary>
    public const string WithdrawProcessing = "WITHDRAW_PROCESSING";

    /// <summary>提现异常（<c>WITHDRAW_EXCEPTION</c>）：须人工排查，<b>不是</b>失败终态。</summary>
    public const string WithdrawException = "WITHDRAW_EXCEPTION";

    /// <summary>提现成功（<c>WITHDRAW_SUCCEED</c>）：注意官方拼写是 <c>SUCCEED</c> 而非 <c>SUCCESS</c>。</summary>
    public const string WithdrawSucceed = "WITHDRAW_SUCCEED";
}

/// <summary>注销提款的出款子账户类型（<c>out_account_type</c>）取值。</summary>
/// <remarks>
/// 出处 <see href="https://pay.weixin.qq.com/doc/v3/partner/4013892759"/> 与
/// <see href="https://pay.weixin.qq.com/doc/v3/partner/4016420099"/>，四值。
/// <b>与余额/提现族的 <c>account_type</c>（<see cref="EcommerceAccountTypes"/>）是两套词汇</b>：
/// 本处带 <c>_ACCOUNT</c> 后缀且键名为 <c>out_account_type</c>，值不可互换（<c>BASIC</c> ≠ <c>BASIC_ACCOUNT</c>）。
/// </remarks>
public static class EcommerceOutAccountTypes
{
    /// <summary>商户的基本户（<c>BASIC_ACCOUNT</c>）。</summary>
    public const string BasicAccount = "BASIC_ACCOUNT";

    /// <summary>商户的运营账户（<c>OPERATE_ACCOUNT</c>）：注意官方拼写是 <c>OPERATE</c> 而非 <c>OPERATION</c>。</summary>
    public const string OperateAccount = "OPERATE_ACCOUNT";

    /// <summary>商户的保证金户（<c>MARGIN_ACCOUNT</c>）：注意官方拼写是 <c>MARGIN</c> 而非 <c>DEPOSIT</c>。</summary>
    public const string MarginAccount = "MARGIN_ACCOUNT";

    /// <summary>商户的手续费账户（<c>TRADE_FEE_ACCOUNT</c>）：注意官方拼写是 <c>TRADE_FEE</c> 而非 <c>FEES</c>。</summary>
    public const string TradeFeeAccount = "TRADE_FEE_ACCOUNT";
}

/// <summary>注销提现的逐账户付款状态（<c>pay_state</c>）取值。</summary>
/// <remarks>
/// 出处 <see href="https://pay.weixin.qq.com/doc/v3/partner/4013892759"/>，四值。
/// <b><c>BANK_REFUNDED</c>（银行退票）既不得按成功记账、也不得按失败自动重发</b>，须人工对账 ——
/// 与 <see cref="EcommerceWithdrawStatuses.Refund"/> 同属「钱出去又被退回」的分支。
/// </remarks>
public static class EcommercePayStates
{
    /// <summary>处理中（<c>PAY_PROCESSING</c>）。</summary>
    public const string PayProcessing = "PAY_PROCESSING";

    /// <summary>付款成功（<c>PAY_SUCCEED</c>）。</summary>
    public const string PaySucceed = "PAY_SUCCEED";

    /// <summary>付款失败（<c>PAY_FAIL</c>）。</summary>
    public const string PayFail = "PAY_FAIL";

    /// <summary>银行退票（<c>BANK_REFUNDED</c>）：须人工处置。</summary>
    public const string BankRefunded = "BANK_REFUNDED";
}

/// <summary>注销提现收款账户类型（<c>payee_info.account_type</c>）取值。</summary>
/// <remarks>
/// 出处 <see href="https://pay.weixin.qq.com/doc/v3/partner/4013892756"/>，两值。
/// <b>这是「对公/对私」</b>，与余额族 <see cref="EcommerceAccountTypes"/>（基本/手续费/运营/保证金）
/// 和 <see cref="EcommerceOutAccountTypes"/>（出款子账户）<b>三者同名不同义</b> —— 本域 <c>account_type</c> 一词三套词汇，
/// 逐页核对不可互换。
/// </remarks>
public static class EcommercePayeeAccountTypes
{
    /// <summary>对公银行账户（<c>ACCOUNT_TYPE_CORPORATE</c>）：开户名称须与营业执照商户名称一致。</summary>
    public const string Corporate = "ACCOUNT_TYPE_CORPORATE";

    /// <summary>对私银行卡账户（<c>ACCOUNT_TYPE_PERSONAL</c>）：开户名称须与法人身份证姓名一致，且 <c>identity_info</c> 必填。</summary>
    public const string Personal = "ACCOUNT_TYPE_PERSONAL";
}

/// <summary>
/// 注销提现族对私收款方的证件类型（<c>id_doc_type</c>）取值 ——
/// <b>与进件族同名参数的值集不同形</b>。
/// </summary>
/// <remarks>
/// <para>
/// 出处 <see href="https://pay.weixin.qq.com/doc/v3/partner/4013892756"/>（八个值，原文照抄）。
/// </para>
/// <para>
/// <b>本域存在两套身份证/通行证枚举命名，不可互相套用</b>：
/// </para>
/// <list type="table">
/// <listheader><term>族</term><description>身份证 / 港澳来往内地通行证 的官方值</description></listheader>
/// <item><term>注销提现（本类）</term><description><c>IDENTIFICATION_TYPE_ID_CARD</c> / <c>IDENTIFICATION_TYPE_HONGKONG_PASSPORT</c></description></item>
/// <item><term>进件（4012713017、4015616688）</term><description><c>IDENTIFICATION_TYPE_MAINLAND_IDCARD</c> / <c>IDENTIFICATION_TYPE_HONGKONG</c></description></item>
/// </list>
/// <para>
/// 把进件枚举值发给注销接口会得到 <c>PARAM_ERROR</c>；反向亦然。两族各持一套常量是有意设计。
/// </para>
/// </remarks>
public static class EcommerceCancelIdDocTypes
{
    /// <summary>中国大陆居民-身份证（<c>IDENTIFICATION_TYPE_ID_CARD</c>）。</summary>
    public const string IdCard = "IDENTIFICATION_TYPE_ID_CARD";

    /// <summary>其他国家或地区居民-护照（<c>IDENTIFICATION_TYPE_OVERSEA_PASSPORT</c>）。</summary>
    public const string OverseaPassport = "IDENTIFICATION_TYPE_OVERSEA_PASSPORT";

    /// <summary>中国香港居民-来往内地通行证（<c>IDENTIFICATION_TYPE_HONGKONG_PASSPORT</c>）。</summary>
    public const string HongkongPassport = "IDENTIFICATION_TYPE_HONGKONG_PASSPORT";

    /// <summary>中国澳门居民-来往内地通行证（<c>IDENTIFICATION_TYPE_MACAO_PASSPORT</c>）。</summary>
    public const string MacaoPassport = "IDENTIFICATION_TYPE_MACAO_PASSPORT";

    /// <summary>中国台湾居民-来往大陆通行证（<c>IDENTIFICATION_TYPE_TAIWAN_PASSPORT</c>）。</summary>
    public const string TaiwanPassport = "IDENTIFICATION_TYPE_TAIWAN_PASSPORT";

    /// <summary>外国人居留证（<c>IDENTIFICATION_TYPE_FOREIGN_RESIDENT</c>）。</summary>
    public const string ForeignResident = "IDENTIFICATION_TYPE_FOREIGN_RESIDENT";

    /// <summary>港澳居民证（<c>IDENTIFICATION_TYPE_HONGKONG_MACAO_RESIDENT</c>）。</summary>
    public const string HongkongMacaoResident = "IDENTIFICATION_TYPE_HONGKONG_MACAO_RESIDENT";

    /// <summary>台湾居民证（<c>IDENTIFICATION_TYPE_TAIWAN_RESIDENT</c>）。</summary>
    public const string TaiwanResident = "IDENTIFICATION_TYPE_TAIWAN_RESIDENT";
}

/// <summary>注销前置校验里的商户状态（<c>merchant_state</c>）取值。</summary>
/// <remarks>
/// 出处 <see href="https://pay.weixin.qq.com/doc/v3/partner/4016420099"/>，两值。
/// <b>与 <see cref="EcommerceValidateResults"/> 是两个维度</b>：本处说「是不是已注销过」，
/// 后者说「现在能不能发起注销提现」；<c>HAS_BEEN_CANCELLED</c> 时 <c>validate_result</c> 仍会返回值，须自行判读。
/// </remarks>
public static class EcommerceMerchantStates
{
    /// <summary>正常（<c>NORMAL</c>）。</summary>
    public const string Normal = "NORMAL";

    /// <summary>已注销（<c>HAS_BEEN_CANCELLED</c>）。</summary>
    public const string HasBeenCancelled = "HAS_BEEN_CANCELLED";
}

/// <summary>注销提现前置校验结论（<c>validate_result</c>）取值。</summary>
/// <remarks>
/// 出处 <see href="https://pay.weixin.qq.com/doc/v3/partner/4016420099"/>，两值。
/// <b><c>NOT_ALLOW_CANCEL_WITHDRAW</c> 不是接口错误</b>：HTTP 仍是 200、应答仍是正常结构，
/// 原因逐条在 <c>block_reasons</c> 里 ⇒ 不得按失败重试，也不得据此判定调用链路异常。
/// </remarks>
public static class EcommerceValidateResults
{
    /// <summary>可发起注销提现申请（<c>ALLOW_CANCEL_WITHDRAW</c>）。</summary>
    public const string AllowCancelWithdraw = "ALLOW_CANCEL_WITHDRAW";

    /// <summary>不可发起注销提现申请（<c>NOT_ALLOW_CANCEL_WITHDRAW</c>）：原因见 <c>block_reasons</c>。</summary>
    public const string NotAllowCancelWithdraw = "NOT_ALLOW_CANCEL_WITHDRAW";
}

/// <summary>不可注销原因类型（<c>block_reasons[].type</c>）取值。</summary>
/// <remarks>
/// 出处 <see href="https://pay.weixin.qq.com/doc/v3/partner/4016420099"/>，四值。
/// <b><c>CONSUMER_COMPLAINT_UNPROCESSED</c> 与 <c>HAS_BLOCKING_CONTROL</c> 都可由平台侧动作解除</b>
/// （处理投诉 / 解除管控），<c>FUNDS_PENDING_PROCESSING</c> 只能等待 ⇒ 分流处置时按类型判断，不看 <c>description</c> 文案。
/// </remarks>
public static class EcommerceBlockReasonTypes
{
    /// <summary>消费者投诉未处理（<c>CONSUMER_COMPLAINT_UNPROCESSED</c>）。</summary>
    public const string ConsumerComplaintUnprocessed = "CONSUMER_COMPLAINT_UNPROCESSED";

    /// <summary>存在不可注销管控（<c>HAS_BLOCKING_CONTROL</c>）。</summary>
    public const string HasBlockingControl = "HAS_BLOCKING_CONTROL";

    /// <summary>商户资金待处理（<c>FUNDS_PENDING_PROCESSING</c>）。</summary>
    public const string FundsPendingProcessing = "FUNDS_PENDING_PROCESSING";

    /// <summary>其他原因（<c>OTHER_REASON</c>）。</summary>
    public const string OtherReason = "OTHER_REASON";
}

/// <summary>电商注销申请的材料类型（<c>application_info[].application_type</c>）取值。</summary>
/// <remarks>
/// <para>
/// 出处 <see href="https://pay.weixin.qq.com/doc/v3/partner/4012476217"/>。官方页该参数的取值表共列出五个值，
/// 其中三个官方原文标注「此枚举值已废弃，请使用新字段」：
/// <c>SP_MERCHANT_APPLICATION</c>、<c>SUB_MERCHANT_APPLICATION</c>、<c>MISSING_OFFICIAL_SEAL_LETTER</c>
/// （最后一个原文写「此材料已废弃，无需上传」）⇒ <b>SDK 只为两个现行值提供常量</b>，
/// 废弃值不出现在契约面上，避免新代码继续沿用。
/// </para>
/// <para>
/// <b>两份申请书是「打印纸质版、填写盖章后拍照」的影像件</b>，且其 <c>media_id</c>
/// 必须来自<b>本族的</b>注销申请材料上传通道（multipart 手工通道），
/// 不是通用媒体上传 <c>/v3/merchant/media/upload</c> 的产物。
/// </para>
/// </remarks>
public static class EcommerceCancelApplicationTypes
{
    /// <summary>电商服务商注销电商子申请书（<c>SP_CANCEL_ACCOUNT_APPLICATION</c>）：需下载模板打印盖章后拍照上传。</summary>
    public const string SpCancelAccountApplication = "SP_CANCEL_ACCOUNT_APPLICATION";

    /// <summary>电商服务商子商户注销申请书（<c>SUB_CANCEL_ACCOUNT_APPLICATION</c>）。</summary>
    public const string SubCancelAccountApplication = "SUB_CANCEL_ACCOUNT_APPLICATION";
}

/// <summary>电商注销申请状态（<c>cancel_state</c>）取值 —— <b>三值</b>。</summary>
/// <remarks>
/// <para>
/// 出处 <see href="https://pay.weixin.qq.com/doc/v3/partner/4012476217"/> 与
/// <see href="https://pay.weixin.qq.com/doc/v3/partner/4012476223"/>，三值原文照抄。
/// </para>
/// <para>
/// <b>本类与 <see cref="EcommerceCancelStates"/>（九值）是同名不同义的两套状态机</b>：
/// 前者属「注销申请书材料审核」族（定位键 <c>out_apply_no</c>），后者属「注销并提现」族
/// （定位键 <c>out_request_no</c>）。<b>两族的终态拼写还不一样</b>：本族是 <c>CANCEL_SUCCESS</c>、
/// 那族是 <c>FINISH</c> ⇒ 一个判定器覆盖两族必然出错，守卫按族分别锁定。
/// </para>
/// </remarks>
public static class EcommerceCancelApplicationStates
{
    /// <summary>审核中（<c>REVIEWING</c>）。</summary>
    public const string Reviewing = "REVIEWING";

    /// <summary>审核驳回（<c>REJECTED</c>）：驳回原因见应答的 <c>reject_reason</c>。</summary>
    public const string Rejected = "REJECTED";

    /// <summary>注销成功（<c>CANCEL_SUCCESS</c>）：本族终态。</summary>
    public const string CancelSuccess = "CANCEL_SUCCESS";
}

/// <summary>
/// 授权/签约状态（<c>auth_state</c>）取值 —— 本域两支个人收款方查询接口共用。
/// </summary>
/// <remarks>
/// <para>
/// 出处 <see href="https://pay.weixin.qq.com/doc/v3/partner/4012739528"/> 与
/// <see href="https://pay.weixin.qq.com/doc/v3/partner/4015851492"/>，两支应答侧均只列
/// <c>AUTHORIZED</c> / <c>UNAUTHORIZED</c> 两值。
/// </para>
/// <para>
/// <b>商家转账授权支（4013504208/4013504209）的 <c>authorization_state</c> 是三值</b>，
/// 多出一个 <c>AUTHORIZE_PROCESSING</c>（处理中）⇒ 那是<b>另一个字段名、另一套值集</b>，
/// 见 <c>EcommerceTransferAuthorizationStates</c>，不得复用本类。
/// </para>
/// </remarks>
public static class EcommerceAuthStates
{
    /// <summary>已授权（<c>AUTHORIZED</c>）。</summary>
    public const string Authorized = "AUTHORIZED";

    /// <summary>未授权（<c>UNAUTHORIZED</c>）。</summary>
    public const string Unauthorized = "UNAUTHORIZED";
}

/// <summary>
/// 主体名下子商户列表的主体类型（<c>organization_type</c>）取值。
/// </summary>
/// <remarks>
/// <para>
/// 出处 <see href="https://pay.weixin.qq.com/doc/v3/partner/4033742105"/>，六值原文照抄。
/// </para>
/// <para>
/// <b><c>SUBJECT_TYPE_INSTITUTIONS_CLONED</c> 是官方原文的拼写</b>（事业单位），
/// 名字里的 <c>_CLONED</c> 看着像笔误但<b>必须照抄</b> —— 本仓 DTO 一律沿用官方键名与值，不做「顺手修正」。
/// </para>
/// <para>
/// <b>与进件族的 <c>subject_type</c> 概念相邻但字段名不同</b>（本处为 <c>organization_type</c>），
/// 进件族的主体类型另见进件 DTO；不要跨族共用常量类。
/// </para>
/// </remarks>
public static class EcommerceSubjectOrganizationTypes
{
    /// <summary>企业（<c>SUBJECT_TYPE_ENTERPRISE</c>）：营业执照主体类型一般为有限公司、有限责任公司。</summary>
    public const string Enterprise = "SUBJECT_TYPE_ENTERPRISE";

    /// <summary>事业单位（<c>SUBJECT_TYPE_INSTITUTIONS_CLONED</c>）：含医疗、教育、学校等单位。<b>官方拼写含 <c>_CLONED</c>，照抄</b>。</summary>
    public const string InstitutionsCloned = "SUBJECT_TYPE_INSTITUTIONS_CLONED";

    /// <summary>个体工商户（<c>SUBJECT_TYPE_INDIVIDUAL</c>）。</summary>
    public const string Individual = "SUBJECT_TYPE_INDIVIDUAL";

    /// <summary>社会组织（<c>SUBJECT_TYPE_OTHERS</c>）：社会团体、民办非企业、基金会、基层群众性自治组织、农村集体经济组织等。</summary>
    public const string Others = "SUBJECT_TYPE_OTHERS";

    /// <summary>政府机关（<c>SUBJECT_TYPE_GOVERNMENT</c>）：机关党委、税务、民政、人社、工商、商务、市监等。</summary>
    public const string Government = "SUBJECT_TYPE_GOVERNMENT";

    /// <summary>小微商户或个人卖家（<c>SUBJECT_TYPE_MICRO</c>）：无营业执照、免工商注册登记的实体商户。</summary>
    public const string Micro = "SUBJECT_TYPE_MICRO";
}

/// <summary>分账接收方类型（<c>type</c>）取值。</summary>
/// <remarks>
/// 出处 <see href="https://pay.weixin.qq.com/doc/v3/partner/4012691594"/>（请求分账）与
/// <see href="https://pay.weixin.qq.com/doc/v3/partner/4012477758"/>（添加接收方），同一两值集合。
/// <b>官方按本类型分流「名称」字段的必填性</b>：<c>MERCHANT_ID</c> 时商户全称必传（小微/个体户为开户人姓名）、
/// <c>PERSONAL_OPENID</c> 时选传但传了会校验实名匹配；<c>appid</c> 同样在接收方含 <c>PERSONAL_OPENID</c> 时转为必填。
/// </remarks>
public static class EcommerceReceiverTypes
{
    /// <summary>商户号（<c>MERCHANT_ID</c>）：接收方账号填商户号。</summary>
    public const string MerchantId = "MERCHANT_ID";

    /// <summary>个人 OpenID（<c>PERSONAL_OPENID</c>）：用户在商户 appid 下的唯一标识；此时 <c>appid</c> 必填。</summary>
    public const string PersonalOpenId = "PERSONAL_OPENID";
}

/// <summary>单个接收方的分账结果（<c>receivers[].result</c>）取值。</summary>
/// <remarks>
/// 出处 <see href="https://pay.weixin.qq.com/doc/v3/partner/4012691594"/> 与
/// <see href="https://pay.weixin.qq.com/doc/v3/partner/4012477734"/>（同一行原文）。
/// <b>只有 <c>SUCCESS</c> / <c>CLOSED</c> 是终态</b>，<c>PENDING</c> 须继续查询；
/// 且该字段与单级 <c>status</c>（<see cref="EcommerceSplitOrderStatuses"/>）<b>不同层</b>：
/// <c>status=FINISHED</c> 只说明动账执行完毕，逐接收方结果必须看本枚举。
/// </remarks>
public static class EcommerceProfitSharingResults
{
    /// <summary>待分账（<c>PENDING</c>）：非终态。</summary>
    public const string Pending = "PENDING";

    /// <summary>分账成功（<c>SUCCESS</c>）：终态。</summary>
    public const string Success = "SUCCESS";

    /// <summary>已关闭（<c>CLOSED</c>）：终态，此时应答带 <c>fail_reason</c>。</summary>
    public const string Closed = "CLOSED";
}

/// <summary>分账失败原因（<c>receivers[].fail_reason</c>）取值。</summary>
/// <remarks>
/// 出处 <see href="https://pay.weixin.qq.com/doc/v3/partner/4012691594"/> /
/// <see href="https://pay.weixin.qq.com/doc/v3/partner/4012477734"/>，八值原文照抄，
/// <b>仅当 <c>result</c> 为 <c>CLOSED</c> 时返回</b>。处理方案官方指向「分账失败处理指引」
/// （<c>4015504955</c>）：<c>NO_RELATION</c> / <c>NO_AUTH</c> 属关系类（需重新维护接收方），
/// <c>RECEIVER_REAL_NAME_NOT_VERIFIED</c> 属实名类，<b>均不是可直接重试的瞬时错误</b>。
/// </remarks>
public static class EcommerceProfitSharingFailReasons
{
    /// <summary>分账接收账户异常（<c>ACCOUNT_ABNORMAL</c>）。</summary>
    public const string AccountAbnormal = "ACCOUNT_ABNORMAL";

    /// <summary>分账关系已解除（<c>NO_RELATION</c>）。</summary>
    public const string NoRelation = "NO_RELATION";

    /// <summary>高风险接收方（<c>RECEIVER_HIGH_RISK</c>）。</summary>
    public const string ReceiverHighRisk = "RECEIVER_HIGH_RISK";

    /// <summary>接收方未实名（<c>RECEIVER_REAL_NAME_NOT_VERIFIED</c>）。</summary>
    public const string ReceiverRealNameNotVerified = "RECEIVER_REAL_NAME_NOT_VERIFIED";

    /// <summary>分账权限已解除（<c>NO_AUTH</c>）。</summary>
    public const string NoAuth = "NO_AUTH";

    /// <summary>超出用户月收款限额（<c>RECEIVER_RECEIPT_LIMIT</c>）。</summary>
    public const string ReceiverReceiptLimit = "RECEIVER_RECEIPT_LIMIT";

    /// <summary>分出方账户异常（<c>PAYER_ACCOUNT_ABNORMAL</c>）。</summary>
    public const string PayerAccountAbnormal = "PAYER_ACCOUNT_ABNORMAL";

    /// <summary>描述参数设置失败（<c>INVALID_REQUEST</c>）。</summary>
    public const string InvalidRequest = "INVALID_REQUEST";
}

/// <summary>分账单状态（<c>status</c>）取值。</summary>
/// <remarks>
/// 出处 <see href="https://pay.weixin.qq.com/doc/v3/partner/4012477734"/>（必填）与
/// <see href="https://pay.weixin.qq.com/doc/v3/partner/4012691594"/>（选填）。
/// <b>官方对 <c>FINISHED</c> 的措辞两支不同</b>：GET 页写「分账完成，终态」、POST 页写「处理完成，终态」，
/// 两处均紧跟「仅代表分账动账执行完毕，每个接收方的分账结果请查看 receivers 中的 result 字段」⇒
/// 单状态不可当业务成功。<b>本域分账单没有失败态</b>（失败落在逐接收方 <c>CLOSED</c>）。
/// </remarks>
public static class EcommerceSplitOrderStatuses
{
    /// <summary>处理中（<c>PROCESSING</c>）：非终态，存在非终态分账结果，可稍后再查直到 <c>FINISHED</c>。</summary>
    public const string Processing = "PROCESSING";

    /// <summary>分账完成 / 处理完成（<c>FINISHED</c>）：终态，仅代表动账执行完毕。</summary>
    public const string Finished = "FINISHED";
}

/// <summary>分账回退结果（<c>result</c>）取值。</summary>
/// <remarks>
/// 出处 <see href="https://pay.weixin.qq.com/doc/v3/partner/4012477737"/> 与
/// <see href="https://pay.weixin.qq.com/doc/v3/partner/4012477740"/>（两支同一行原文）。
/// <b>官方资金风险警示（原文）</b>：「如果返回为处理中，请勿变更商户回退单号，使用相同的参数再次发起分账回退，
/// 否则会出现资金风险」；处理中的回退单 5 天未成功会因超时被置为 <c>FAILED</c>。
/// </remarks>
public static class EcommerceProfitSharingReturnResults
{
    /// <summary>处理中（<c>PROCESSING</c>）：非终态，禁止换单号重发。</summary>
    public const string Processing = "PROCESSING";

    /// <summary>已成功（<c>SUCCESS</c>）：终态。</summary>
    public const string Success = "SUCCESS";

    /// <summary>已失败（<c>FAILED</c>）：终态，此时应答带 <c>fail_reason</c>。</summary>
    public const string Failed = "FAILED";
}

/// <summary>分账回退失败原因（<c>fail_reason</c>）取值 —— <b>请求支三值、查询支四值，不得合并</b>。</summary>
/// <remarks>
/// <see href="https://pay.weixin.qq.com/doc/v3/partner/4012477737"/>（请求分账回退）只列
/// <c>ACCOUNT_ABNORMAL</c> / <c>TIME_OUT_CLOSED</c> / <c>PAYER_ACCOUNT_ABNORMAL</c>；
/// <see href="https://pay.weixin.qq.com/doc/v3/partner/4012477740"/>（查询回退结果）额外列
/// <c>INVALID_REQUEST</c> ⇒ 反序列化侧按四值收，请求支不会给出第四值。
/// 注意与 <see cref="EcommerceProfitSharingFailReasons"/>（分账失败八值）是两套词表，
/// 仅 <c>ACCOUNT_ABNORMAL</c> / <c>PAYER_ACCOUNT_ABNORMAL</c> 同名同义。
/// </remarks>
public static class EcommerceProfitSharingReturnFailReasons
{
    /// <summary>原分账接收方账户异常（<c>ACCOUNT_ABNORMAL</c>）。</summary>
    public const string AccountAbnormal = "ACCOUNT_ABNORMAL";

    /// <summary>超时关单（<c>TIME_OUT_CLOSED</c>）：处理中超 5 天被置失败。</summary>
    public const string TimeOutClosed = "TIME_OUT_CLOSED";

    /// <summary>原分账分出方账户异常（<c>PAYER_ACCOUNT_ABNORMAL</c>）。</summary>
    public const string PayerAccountAbnormal = "PAYER_ACCOUNT_ABNORMAL";

    /// <summary>描述参数设置失败（<c>INVALID_REQUEST</c>）：<b>仅查询支列出</b>。</summary>
    public const string InvalidRequest = "INVALID_REQUEST";
}

/// <summary>分账接收方与分账方的关系类型（<c>relation_type</c>）取值。</summary>
/// <remarks>
/// 出处 <see href="https://pay.weixin.qq.com/doc/v3/partner/4012477758"/>（添加分账接收方，必填）。
/// <b>官方拼写为 <c>SERVICE_PROVIDER</c></b>；五值中 <c>OTHERS</c> 是官方提供的「其他」兜底值，
/// 存在即要求宿主显式选择，官方不提供「关系类型可不填」。
/// </remarks>
public static class EcommerceRelationTypes
{
    /// <summary>供应商（<c>SUPPLIER</c>）。</summary>
    public const string Supplier = "SUPPLIER";

    /// <summary>分销商（<c>DISTRIBUTOR</c>）。</summary>
    public const string Distributor = "DISTRIBUTOR";

    /// <summary>服务商（<c>SERVICE_PROVIDER</c>）。</summary>
    public const string ServiceProvider = "SERVICE_PROVIDER";

    /// <summary>平台（<c>PLATFORM</c>）。</summary>
    public const string Platform = "PLATFORM";

    /// <summary>其他（<c>OTHERS</c>）。</summary>
    public const string Others = "OTHERS";
}

/// <summary>补差单结果（<c>result</c>，发起补差支）取值。</summary>
/// <remarks>
/// 出处 <see href="https://pay.weixin.qq.com/doc/v3/partner/4012477631"/>，三值
/// <c>SUCCESS</c> / <c>FAIL</c> / <c>REFUND</c>。<b>只有本支有 <c>REFUND</c>（全额回退补差）</b>，
/// 与回退支（<see cref="EcommerceSubsidyReturnResults"/>）、取消支（<see cref="EcommerceSubsidyCancelResults"/>）
/// 的两值集合不同 —— 同名 <c>result</c> 字段在三页各自成表，勿共用一套常量。
/// </remarks>
public static class EcommerceSubsidyCreateResults
{
    /// <summary>补差成功（<c>SUCCESS</c>）。</summary>
    public const string Success = "SUCCESS";

    /// <summary>补差失败（<c>FAIL</c>）。</summary>
    public const string Fail = "FAIL";

    /// <summary>全额回退补差（<c>REFUND</c>）。</summary>
    public const string Refund = "REFUND";
}

/// <summary>补差回退结果（<c>result</c>，补差回退支）取值。</summary>
/// <remarks>出处 <see href="https://pay.weixin.qq.com/doc/v3/partner/4012477636"/>，两值。</remarks>
public static class EcommerceSubsidyReturnResults
{
    /// <summary>已成功（<c>SUCCESS</c>）。</summary>
    public const string Success = "SUCCESS";

    /// <summary>已失败（<c>FAIL</c>）。</summary>
    public const string Fail = "FAIL";
}

/// <summary>取消补差结果（<c>result</c>，取消补差支）取值。</summary>
/// <remarks>
/// 出处 <see href="https://pay.weixin.qq.com/doc/v3/partner/4012477639"/>，两值。
/// <b>本域同时存在 <c>FAIL</c> 与 <c>FAILED</c> 两种失败拼写</b>（补差三支用 <c>FAIL</c>、
/// 分账回退用 <c>FAILED</c>、注销审批用 <c>REJECTED</c>）⇒ 照页取值，不做「顺手统一」。
/// </remarks>
public static class EcommerceSubsidyCancelResults
{
    /// <summary>成功（<c>SUCCESS</c>）。</summary>
    public const string Success = "SUCCESS";

    /// <summary>失败（<c>FAIL</c>）。</summary>
    public const string Fail = "FAIL";
}

/// <summary>出资账户类型（<c>from[].account</c> / <c>amount.from[].account</c>）取值 —— 补差回退与退款出资两支共用。</summary>
/// <remarks>
/// 出处 <see href="https://pay.weixin.qq.com/doc/v3/partner/4012477636"/>（补差回退）与
/// <see href="https://pay.weixin.qq.com/doc/v3/partner/4012476892"/>（申请退款，<c>amount.from</c>），
/// 两支该行取值集合相同（<c>AVAILABLE</c> / <c>UNAVAILABLE</c>）⇒ 共用一个常量类。
/// 补差支官方说明「当前仅支持从一个账户出资」且「本次回退金额不超过指定账户的实际可用资金」；
/// 退款支另有约束「基本账户可用 + 不可用出资金额之和 = 退款金额」且「账户类型不能重复」。
/// <b>本枚举与 <see cref="EcommerceAccountTypes"/>（<c>BASIC</c>/<c>FEES</c>/<c>OPERATION</c>/<c>DEPOSIT</c>）
/// 不是同一套概念</b>：前者是账户资金状态、后者是账户种类。
/// </remarks>
public static class EcommerceFundingAccountTypes
{
    /// <summary>可用余额（<c>AVAILABLE</c>）。</summary>
    public const string Available = "AVAILABLE";

    /// <summary>不可用余额（<c>UNAVAILABLE</c>）。</summary>
    public const string Unavailable = "UNAVAILABLE";
}

/// <summary>退款单状态（<c>status</c>）取值。</summary>
/// <remarks>
/// 出处 <see href="https://pay.weixin.qq.com/doc/v3/partner/4012476908"/>（查单两支）与
/// <see href="https://pay.weixin.qq.com/doc/v3/partner/4012650253"/>（个人收款方查单），四值同形。
/// <b>两支对 <c>ABNORMAL</c> 的处置指引不同</b>：二级商户支指向商户平台交易中心人工处理，
/// 个人收款方支指向「发起异常退款」API（<c>4012649274</c>）⇒ 同一状态值的后续动作按身份分流。
/// <b>申请退款的受理应答里没有 <c>status</c></b>（两支均无该字段）⇒ 不可把受理返回当退款成功。
/// </remarks>
public static class EcommerceRefundStatuses
{
    /// <summary>退款成功（<c>SUCCESS</c>）：此时应答带 <c>success_time</c>。</summary>
    public const string Success = "SUCCESS";

    /// <summary>退款关闭（<c>CLOSED</c>）。</summary>
    public const string Closed = "CLOSED";

    /// <summary>退款处理中（<c>PROCESSING</c>）：非终态。</summary>
    public const string Processing = "PROCESSING";

    /// <summary>退款异常（<c>ABNORMAL</c>）：原路退银行卡失败（卡作废/冻结）。</summary>
    public const string Abnormal = "ABNORMAL";
}

/// <summary>退款渠道（<c>channel</c>）取值。</summary>
/// <remarks>
/// 出处 <see href="https://pay.weixin.qq.com/doc/v3/partner/4012476908"/>，<b>选填</b>四值。
/// <c>OTHER_BALANCE</c> / <c>OTHER_BANKCARD</c> 是「原账户异常后退到其他账户」⇒ 与
/// <see cref="EcommerceRefundStatuses"/> 的 <c>ABNORMAL</c> 属不同维度（前者是资金去向、后者是单据状态）。
/// </remarks>
public static class EcommerceRefundChannels
{
    /// <summary>原路退款（<c>ORIGINAL</c>）。</summary>
    public const string Original = "ORIGINAL";

    /// <summary>退回到余额（<c>BALANCE</c>）。</summary>
    public const string Balance = "BALANCE";

    /// <summary>原账户异常退到其他余额账户（<c>OTHER_BALANCE</c>）。</summary>
    public const string OtherBalance = "OTHER_BALANCE";

    /// <summary>原银行卡异常退到其他银行卡（<c>OTHER_BANKCARD</c>）。</summary>
    public const string OtherBankcard = "OTHER_BANKCARD";
}

/// <summary>退款出资商户（<c>refund_account</c>）取值。</summary>
/// <remarks>
/// 出处 <see href="https://pay.weixin.qq.com/doc/v3/partner/4012476892"/>（请求）与
/// <see href="https://pay.weixin.qq.com/doc/v3/partner/4012476908"/>（应答）。
/// <b>官方两处警示（照抄语义）</b>：请求侧「传入垫付值仅代表可以使用垫付退款，实际出款账户以受理结果或查单为准」；
/// 应答侧该字段是<b>选填</b>，「申请退款时可能暂未扣除退款资金，此时不返回本字段，需查单确认」。
/// <c>REFUND_SOURCE_SUB_MERCHANT</c> 为默认值。
/// </remarks>
public static class EcommerceRefundSources
{
    /// <summary>电商平台垫付（<c>REFUND_SOURCE_PARTNER_ADVANCE</c>）：需向微信支付申请开通。</summary>
    public const string PartnerAdvance = "REFUND_SOURCE_PARTNER_ADVANCE";

    /// <summary>二级商户出资（<c>REFUND_SOURCE_SUB_MERCHANT</c>）：默认值。</summary>
    public const string SubMerchant = "REFUND_SOURCE_SUB_MERCHANT";
}

/// <summary>退款资金来源账户（请求侧 <c>funds_account</c>）取值 —— <b>二级商户支三值、个人收款方支只有 <c>UNSETTLED</c>，不得合并</b>。</summary>
/// <remarks>
/// <para>
/// <see href="https://pay.weixin.qq.com/doc/v3/partner/4012476892"/>（二级商户申请退款，<b>选填</b>）列
/// <c>AVAILABLE</c>（可用余额）、<c>UNSETTLED</c>（未结算资金，个人收款订单未结算时）、<c>PREPAID</c>（未结算资金，预付订单未结算时）。
/// </para>
/// <para>
/// <see href="https://pay.weixin.qq.com/doc/v3/partner/4012649457"/>（个人收款方申请退款）该字段是<b>必填</b>
/// 且官方只列 <c>UNSETTLED</c> ⇒ 「同一字段在两支的必填性与取值集同时不同」，本域退款族最易踩的形态差异。
/// </para>
/// <para>
/// <b>请求侧与应答侧不是同一套词表</b>：应答 <c>funds_account</c> 另有 <c>OPERATION</c> / <c>BASIC</c> /
/// <c>ECNY_BASIC</c>（见 <see cref="EcommerceRefundFundsAccountStates"/>），而 <c>PREPAID</c> 不出现在应答里。
/// </para>
/// </remarks>
public static class EcommerceRefundFundingAccounts
{
    /// <summary>可用余额（<c>AVAILABLE</c>）：待分账订单退款时可指定。</summary>
    public const string Available = "AVAILABLE";

    /// <summary>未结算资金（<c>UNSETTLED</c>）：个人收款方支的<b>唯一且必填</b>取值。</summary>
    public const string Unsettled = "UNSETTLED";

    /// <summary>未结算资金（<c>PREPAID</c>）：预付订单未结算时指定；<b>仅二级商户支</b>。</summary>
    public const string Prepaid = "PREPAID";
}

/// <summary>退款单应答的资金账户（<c>funds_account</c>）取值。</summary>
/// <remarks>
/// 出处 <see href="https://pay.weixin.qq.com/doc/v3/partner/4012476908"/> 与
/// <see href="https://pay.weixin.qq.com/doc/v3/partner/4012650253"/>（两支该行取值集合相同，<b>选填</b>六值）。
/// 与请求侧 <see cref="EcommerceRefundFundingAccounts"/> 是两套词表（详见其 remarks）。
/// </remarks>
public static class EcommerceRefundFundsAccountStates
{
    /// <summary>未结算资金（<c>UNSETTLED</c>）。</summary>
    public const string Unsettled = "UNSETTLED";

    /// <summary>可用余额（<c>AVAILABLE</c>）。</summary>
    public const string Available = "AVAILABLE";

    /// <summary>不可用余额（<c>UNAVAILABLE</c>）。</summary>
    public const string Unavailable = "UNAVAILABLE";

    /// <summary>运营户（<c>OPERATION</c>）。</summary>
    public const string Operation = "OPERATION";

    /// <summary>基本账户（<c>BASIC</c>）：含可用余额和不可用余额。</summary>
    public const string Basic = "BASIC";

    /// <summary>数字人民币基本账户（<c>ECNY_BASIC</c>）。</summary>
    public const string EcnyBasic = "ECNY_BASIC";
}

/// <summary>优惠范围（<c>promotion_detail[].scope</c>）取值。</summary>
/// <remarks>出处 <see href="https://pay.weixin.qq.com/doc/v3/partner/4012476892"/> 等各支 <c>promotion_detail</c> 表，两值。</remarks>
public static class EcommercePromotionScopes
{
    /// <summary>全场代金券（<c>GLOBAL</c>）。</summary>
    public const string Global = "GLOBAL";

    /// <summary>单品优惠（<c>SINGLE</c>）。</summary>
    public const string Single = "SINGLE";
}

/// <summary>优惠类型（<c>promotion_detail[].type</c>）取值。</summary>
/// <remarks>
/// 出处同上，两值。<b>官方以「是否走结算资金」区分</b>：<c>COUPON</c> 是充值型代金券、
/// <c>DISCOUNT</c> 是免充值型优惠券 ⇒ 退款时两者的资金处理路径不同。
/// </remarks>
public static class EcommercePromotionTypes
{
    /// <summary>代金券（<c>COUPON</c>）：需要走结算资金的充值型。</summary>
    public const string Coupon = "COUPON";

    /// <summary>优惠券（<c>DISCOUNT</c>）：不走结算资金的免充值型。</summary>
    public const string Discount = "DISCOUNT";
}

/// <summary>垫付回补的出/入账账户类型（<c>payer_account</c> / <c>payee_account</c>）取值。</summary>
/// <remarks>
/// 出处 <see href="https://pay.weixin.qq.com/doc/v3/partner/4012476916"/>，<b>两支同表且均必填</b>，两值
/// <c>BASIC</c> / <c>OPERATION</c>。<b>与 <see cref="EcommerceAccountTypes"/> 的值集不同</b>：
/// 后者是 <c>BASIC</c>/<c>FEES</c>/<c>OPERATION</c>/<c>DEPOSIT</c>，回补面只有两值。
/// </remarks>
public static class EcommerceAdvanceAccountTypes
{
    /// <summary>基本账户（<c>BASIC</c>）。</summary>
    public const string Basic = "BASIC";

    /// <summary>运营账户（<c>OPERATION</c>）。</summary>
    public const string Operation = "OPERATION";
}

/// <summary>垫付回补结果（<c>result</c>）取值。</summary>
/// <remarks>
/// 出处 <see href="https://pay.weixin.qq.com/doc/v3/partner/4012476916"/> 与
/// <see href="https://pay.weixin.qq.com/doc/v3/partner/4012476927"/>（同一应答表）。
/// 官方注明 <c>FAILED</c> 的成因是「出款方账户余额不足」⇒ 补余额后可再发起；<c>PROCESSING</c> 非终态。
/// <b>本域失败拼写在此为 <c>FAILED</c></b>（补差支为 <c>FAIL</c>，见 <see cref="EcommerceSubsidyReturnResults"/>）。
/// </remarks>
public static class EcommerceReturnAdvanceResults
{
    /// <summary>回补成功（<c>SUCCESS</c>）。</summary>
    public const string Success = "SUCCESS";

    /// <summary>回补失败（<c>FAILED</c>）：出款方账户余额不足。</summary>
    public const string Failed = "FAILED";

    /// <summary>处理中（<c>PROCESSING</c>）：非终态。</summary>
    public const string Processing = "PROCESSING";
}

/// <summary>异常退款处理方式（<c>type</c>）取值。</summary>
/// <remarks>
/// 出处 <see href="https://pay.weixin.qq.com/doc/v3/partner/4012649274"/>（必填）。
/// <b>官方只提供一个取值</b>：<c>USER_BANK_CARD</c>（退款到用户银行卡），原文「个人收款只支持退款至用户银行账户」
/// ⇒ 无「退回零钱」分支；且该页 <c>bank_type</c> 银行范围官方限定为 15 家银行的借记卡。
/// </remarks>
public static class EcommerceAbnormalRefundTypes
{
    /// <summary>退款到用户银行卡（<c>USER_BANK_CARD</c>）：官方在本页提供的唯一取值。</summary>
    public const string UserBankCard = "USER_BANK_CARD";
}

/// <summary>
/// 进件主体类型（<c>organization_type</c>）取值 —— <b>官方用纯数字字符串，不是 <c>SUBJECT_TYPE_*</c></b>。
/// </summary>
/// <remarks>
/// <para>
/// 出处 <see href="https://pay.weixin.qq.com/doc/v3/partner/4012713017"/>（标准进件，<c>string(4)</c> 必填）与
/// <see href="https://pay.weixin.qq.com/doc/v3/partner/4015616688"/>（账户托管进件，七值同集）。
/// </para>
/// <para>
/// <b>与「主体名下子商户」查询应答的 <c>organization_type</c> 是两套值</b>（那边为
/// <c>SUBJECT_TYPE_ENTERPRISE</c> 等字符串，见 <see cref="EcommerceSubjectOrganizationTypes"/>）⇒
/// 从进件请求回读到查询应答、或反向比对时须先做值映射，直接 <c>==</c> 比对恒不等。
/// </para>
/// <para>
/// 官方注明「非小微的主体类型需与营业执照/登记证书上一致」；<c>2500</c>（个人卖家）另要求
/// 在 <c>business_addition_desc</c> 写明经营满 6 个月、收入累计超 20 万的声明。
/// </para>
/// </remarks>
public static class EcommerceApplymentOrganizationTypes
{
    /// <summary>小微商户（<c>2401</c>）：无营业执照的个人商家。</summary>
    public const string Micro = "2401";

    /// <summary>个人卖家（<c>2500</c>）：无营业执照，已持续从事电子商务经营活动满 6 个月且期间经营收入累计超过 20 万元。</summary>
    public const string PersonalSeller = "2500";

    /// <summary>个体工商户（<c>4</c>）：营业执照主体类型一般为个体户、个体工商户、个体经营。</summary>
    public const string IndividualBusiness = "4";

    /// <summary>企业（<c>2</c>）：营业执照主体类型一般为有限公司、有限责任公司。</summary>
    public const string Enterprise = "2";

    /// <summary>事业单位（<c>3</c>）：国内各类事业单位，如医疗、教育、学校等单位。</summary>
    public const string Institution = "3";

    /// <summary>政府机关（<c>2502</c>）：各级、各类政府机关，如机关党委、税务、民政、人社、工商、商务、市监等。</summary>
    public const string Government = "2502";

    /// <summary>社会组织（<c>1708</c>）：社会团体、民办非企业、基金会、基层群众性自治组织、农村集体经济组织等。</summary>
    public const string SocialOrganization = "1708";
}

/// <summary>
/// 进件登记证书类型（<c>business_license_info.cert_type</c>）取值。
/// </summary>
/// <remarks>
/// <para>
/// 出处 <see href="https://pay.weixin.qq.com/doc/v3/partner/4012713017"/>、
/// <see href="https://pay.weixin.qq.com/doc/v3/partner/4015616688"/>，十值同集、原文照抄。
/// </para>
/// <para>
/// <b>主体为「个体工商户/企业」时不填本字段</b>（官方原文），它只服务于「政府机关/事业单位/社会组织」的登记证书；
/// 且官方按主体限定可选子集：<c>2388</c> 仅事业单位，<c>2389</c> 为政府机关与社会组织共用，
/// 其余 <c>2394/2395/2396/2399/2400/2520/2521/2522</c> 仅社会组织。
/// </para>
/// </remarks>
public static class EcommerceApplymentCertTypes
{
    /// <summary>事业单位法人证书（<c>CERTIFICATE_TYPE_2388</c>）：主体为事业单位时选择。</summary>
    public const string InstitutionLegalPerson = "CERTIFICATE_TYPE_2388";

    /// <summary>统一社会信用代码证书（<c>CERTIFICATE_TYPE_2389</c>）：主体为政府机关或社会组织时选择。</summary>
    public const string UnifiedSocialCredit = "CERTIFICATE_TYPE_2389";

    /// <summary>社会团体法人登记证书（<c>CERTIFICATE_TYPE_2394</c>）：主体为社会组织时可选。</summary>
    public const string SocialGroup = "CERTIFICATE_TYPE_2394";

    /// <summary>民办非企业单位登记证书（<c>CERTIFICATE_TYPE_2395</c>）：主体为社会组织时可选。</summary>
    public const string PrivateNonEnterprise = "CERTIFICATE_TYPE_2395";

    /// <summary>基金会法人登记证书（<c>CERTIFICATE_TYPE_2396</c>）：主体为社会组织时可选。</summary>
    public const string Foundation = "CERTIFICATE_TYPE_2396";

    /// <summary>宗教活动场所登记证（<c>CERTIFICATE_TYPE_2399</c>）：主体为社会组织时可选。</summary>
    public const string ReligiousVenue = "CERTIFICATE_TYPE_2399";

    /// <summary>政府部门下发的其他有效证明文件（<c>CERTIFICATE_TYPE_2400</c>）：主体为社会组织时可选。</summary>
    public const string OtherGovernmentDocument = "CERTIFICATE_TYPE_2400";

    /// <summary>执业许可证/执业证（<c>CERTIFICATE_TYPE_2520</c>）：主体为社会组织时可选。</summary>
    public const string PracticeLicense = "CERTIFICATE_TYPE_2520";

    /// <summary>基层群众性自治组织特别法人统一社会信用代码证（<c>CERTIFICATE_TYPE_2521</c>）：主体为社会组织时可选。</summary>
    public const string GrassrootsAutonomyCreditCode = "CERTIFICATE_TYPE_2521";

    /// <summary>农村集体经济组织登记证（<c>CERTIFICATE_TYPE_2522</c>）：主体为社会组织时可选。</summary>
    public const string RuralCollectiveEconomic = "CERTIFICATE_TYPE_2522";
}

/// <summary>
/// 进件证件（身份/国籍/居住类）类型取值 —— 进件族三处字段共用同一八值集合。
/// </summary>
/// <remarks>
/// <para>
/// 出处 <see href="https://pay.weixin.qq.com/doc/v3/partner/4012713017"/>，共用本集合的三处：
/// <c>id_doc_type</c>（经营者/法定代表人证件类型）、<c>ubo_info_list[].ubo_id_doc_type</c>（受益人证件类型）、
/// <c>contact_info.contact_id_doc_type</c>（超级管理员证件类型）；
/// <see href="https://pay.weixin.qq.com/doc/v3/partner/4015616688"/> 的三处值集与之逐字相同 ⇒ 共用本类。
/// </para>
/// <para>
/// <b>与注销族的 <c>id_doc_type</c> 不是同一套</b>（那边为 <c>IDENTITY_NO</c> 形态的短值，
/// 见 <see cref="EcommerceCancelIdDocTypes"/>）⇒ 跨族复用常量会写出官方不接受的值。
/// </para>
/// <para>
/// 官方约束：<c>id_doc_type</c> 选填，未填时系统默认按身份证处理；主体为「政府机关」时仅支持身份证；
/// 来往通行证 / 外国人居留证 / 港澳居民居住证 / 台湾居民居住证需补证件反面照片，护照不需。
/// </para>
/// </remarks>
public static class EcommerceApplymentIdDocTypes
{
    /// <summary>中国大陆居民-身份证（<c>IDENTIFICATION_TYPE_MAINLAND_IDCARD</c>）。</summary>
    public const string MainlandIdCard = "IDENTIFICATION_TYPE_MAINLAND_IDCARD";

    /// <summary>其他国家或地区居民-护照（<c>IDENTIFICATION_TYPE_OVERSEA_PASSPORT</c>）。</summary>
    public const string OverseaPassport = "IDENTIFICATION_TYPE_OVERSEA_PASSPORT";

    /// <summary>中国香港居民-来往内地通行证（<c>IDENTIFICATION_TYPE_HONGKONG</c>）。</summary>
    public const string HongKong = "IDENTIFICATION_TYPE_HONGKONG";

    /// <summary>中国澳门居民-来往内地通行证（<c>IDENTIFICATION_TYPE_MACAO</c>）。</summary>
    public const string Macao = "IDENTIFICATION_TYPE_MACAO";

    /// <summary>中国台湾居民-来往大陆通行证（<c>IDENTIFICATION_TYPE_TAIWAN</c>）。</summary>
    public const string Taiwan = "IDENTIFICATION_TYPE_TAIWAN";

    /// <summary>外国人居留证（<c>IDENTIFICATION_TYPE_FOREIGN_RESIDENT</c>）。</summary>
    public const string ForeignResident = "IDENTIFICATION_TYPE_FOREIGN_RESIDENT";

    /// <summary>港澳居民居住证（<c>IDENTIFICATION_TYPE_HONGKONG_MACAO_RESIDENT</c>）。</summary>
    public const string HongKongMacaoResident = "IDENTIFICATION_TYPE_HONGKONG_MACAO_RESIDENT";

    /// <summary>台湾居民居住证（<c>IDENTIFICATION_TYPE_TAIWAN_RESIDENT</c>）。</summary>
    public const string TaiwanResident = "IDENTIFICATION_TYPE_TAIWAN_RESIDENT";
}

/// <summary>
/// 进件证件持有人类型（<c>id_holder_type</c>）取值。
/// </summary>
/// <remarks>
/// <para>
/// 出处 <see href="https://pay.weixin.qq.com/doc/v3/partner/4012713017"/>、
/// <see href="https://pay.weixin.qq.com/doc/v3/partner/4015616688"/>，两值同集。
/// </para>
/// <para>
/// <b>两页对 <c>LEGAL</c> 的中文标签不同</b>（标准进件页作「经营者/法定代表人」，账户托管页作「法定代表人」），
/// 值本身一致 ⇒ 共用本类，语义差异记在字段注释里即可。
/// </para>
/// <para>
/// 官方只在「主体为政府机关/事业单位且无法提供法定代表人证件」时才需要传 <c>SUPER</c>（经办人）；
/// 小微/个人卖家/企业/个体户/社会组织默认经营者/法定代表人，无须传本字段。
/// 选 <c>SUPER</c> 时必须上传 <c>authorize_letter_copy</c>（法定代表人说明函）。
/// </para>
/// </remarks>
public static class EcommerceApplymentIdHolderTypes
{
    /// <summary>经营者/法定代表人（<c>LEGAL</c>）。</summary>
    public const string Legal = "LEGAL";

    /// <summary>经办人（<c>SUPER</c>）：经商户授权办理微信支付业务的人员。</summary>
    public const string Super = "SUPER";
}

/// <summary>
/// 金融机构类型（<c>finance_institution_info.finance_type</c>）取值。
/// </summary>
/// <remarks>
/// 出处 <see href="https://pay.weixin.qq.com/doc/v3/partner/4012713017"/>、
/// <see href="https://pay.weixin.qq.com/doc/v3/partner/4015616688"/>，六值同集、必填（当 <c>finance_institution=true</c> 时）。
/// 官方要求「金融机构类型需与营业执照/登记证书上一致」，且许可证图片须为申请商家主体所属，
/// 可授权使用总公司/分公司的特殊资质，最多 5 张。
/// </remarks>
public static class EcommerceFinanceTypes
{
    /// <summary>银行业（<c>BANK_AGENT</c>）：商业银行、政策性银行、农村合作银行、村镇银行、开发性金融机构等。</summary>
    public const string BankAgent = "BANK_AGENT";

    /// <summary>支付机构（<c>PAYMENT_AGENT</c>）：非银行类支付机构。</summary>
    public const string PaymentAgent = "PAYMENT_AGENT";

    /// <summary>保险业（<c>INSURANCE</c>）：保险、保险中介、保险代理、保险经纪等保险类业务。</summary>
    public const string Insurance = "INSURANCE";

    /// <summary>交易及结算类金融机构（<c>TRADE_AND_SETTLE</c>）：交易所、登记结算类机构、银行卡清算机构、资金清算中心等。</summary>
    public const string TradeAndSettle = "TRADE_AND_SETTLE";

    /// <summary>小额贷款机构（<c>SMALL_LOAN</c>）：取得小额贷款业务经营资质、不吸收公众存款、主要经营小额贷款业务的地方金融组织。</summary>
    public const string SmallLoan = "SMALL_LOAN";

    /// <summary>其他金融机构（<c>OTHER</c>）：财务公司、信托、金融资产管理、金融租赁、汽车金融、货币经纪、消费金融、证券、担保、商业保理、典当等。</summary>
    public const string Other = "OTHER";
}

/// <summary>
/// 进件结算账户类型（<c>account_info.bank_account_type</c>）取值 —— <b>官方用两位数字串</b>。
/// </summary>
/// <remarks>
/// 出处 <see href="https://pay.weixin.qq.com/doc/v3/partner/4012713017"/>、
/// <see href="https://pay.weixin.qq.com/doc/v3/partner/4015616688"/>（<c>string(2)</c> 必填）。
/// 官方按主体分流：企业/政府机关/事业单位/社会组织只可 <c>74</c>；小微/个人卖家只可 <c>75</c>；
/// 个体工商户两者皆可。
/// <b>与「二级商户账户类型」<c>BASIC/FEES/OPERATION/DEPOSIT</c>（见 <see cref="EcommerceAccountTypes"/>）
/// 是完全不同的概念</b>，勿按字段名相近共用常量。
/// </remarks>
public static class EcommerceBankAccountTypes
{
    /// <summary>对公账户（<c>74</c>）：开户名称必须与营业执照上的「商户名称」一致。</summary>
    public const string Corporate = "74";

    /// <summary>对私账户（<c>75</c>）：开户名称必须与经营者身份证姓名一致。</summary>
    public const string Personal = "75";
}

/// <summary>
/// 进件超级管理员类型（<c>contact_info.contact_type</c>）取值 —— <b>官方用两位数字串</b>。
/// </summary>
/// <remarks>
/// 出处 <see href="https://pay.weixin.qq.com/doc/v3/partner/4012713017"/>、
/// <see href="https://pay.weixin.qq.com/doc/v3/partner/4015616688"/>（<c>string(2)</c> 必填）。
/// 官方按主体分流：小微/个人卖家只可 <c>65</c>；个体工商户/企业/政府机关/事业单位/社会组织可 <c>65</c> 或 <c>66</c>。
/// 选 <c>66</c>（经办人）时须补超级管理员证件类型、证件照片、有效期等一组字段。
/// </remarks>
public static class EcommerceContactTypes
{
    /// <summary>经营者/法定代表人（<c>65</c>）。</summary>
    public const string Legal = "65";

    /// <summary>经办人（<c>66</c>）：经商户授权办理微信支付业务的人员。</summary>
    public const string Agent = "66";
}

/// <summary>
/// 进件申请单状态（<c>applyment_state</c>）取值。
/// </summary>
/// <remarks>
/// <para>
/// 出处 <see href="https://pay.weixin.qq.com/doc/v3/partner/4012691469"/>、
/// <see href="https://pay.weixin.qq.com/doc/v3/partner/4012691376"/>、
/// <see href="https://pay.weixin.qq.com/doc/v3/partner/4015616714"/>、
/// <see href="https://pay.weixin.qq.com/doc/v3/partner/4015616708"/>，四页应答同八值集合（已逐页比对）。
/// </para>
/// <para>
/// <b>进件是异步审核</b>：提交成功只代表申请单创建，须轮询本状态到 <see cref="Finish"/> 才拿到 <c>sub_mchid</c>；
/// 各状态分别决定应答里哪几组信息才出现 —— <c>ACCOUNT_NEED_VERIFY</c> 出 <c>account_validation</c>
/// 与 <c>legal_validation_url</c>，<c>NEED_SIGN</c> 出 <c>sign_url</c>，<c>REJECTED</c>/<c>FROZEN</c> 出 <c>audit_detail</c>，
/// <c>NEED_SIGN</c>/<c>FINISH</c> 才出 <c>sub_mchid</c> ⇒ 这些字段全为选填，勿按「查单必返」处理。
/// </para>
/// </remarks>
public static class EcommerceApplymentStates
{
    /// <summary>资料校验中（<c>CHECKING</c>）。</summary>
    public const string Checking = "CHECKING";

    /// <summary>待账户验证（<c>ACCOUNT_NEED_VERIFY</c>）：需按 <c>account_validation</c> 指引汇款，或让法人扫 <c>legal_validation_url</c>。</summary>
    public const string AccountNeedVerify = "ACCOUNT_NEED_VERIFY";

    /// <summary>审核中（<c>AUDITING</c>）。</summary>
    public const string Auditing = "AUDITING";

    /// <summary>已驳回（<c>REJECTED</c>）：返回 <c>audit_detail</c>；平台代理签约模式下此状态也可能返回 <c>sign_url</c>。</summary>
    public const string Rejected = "REJECTED";

    /// <summary>待签约（<c>NEED_SIGN</c>）：返回 <c>sign_url</c> 与 <c>sub_mchid</c>。</summary>
    public const string NeedSign = "NEED_SIGN";

    /// <summary>完成（<c>FINISH</c>）：终态，返回 <c>sub_mchid</c>，可发起支付与资金操作。</summary>
    public const string Finish = "FINISH";

    /// <summary>已冻结（<c>FROZEN</c>）：返回 <c>audit_detail</c>。</summary>
    public const string Frozen = "FROZEN";

    /// <summary>已作废（<c>CANCELED</c>）：撤销申请单后的终态。</summary>
    public const string Canceled = "CANCELED";
}

/// <summary>
/// 进件签约状态（<c>sign_state</c>，进件查询应答中<b>选填</b>）取值。
/// </summary>
/// <remarks>
/// 出处 <see href="https://pay.weixin.qq.com/doc/v3/partner/4012691469"/> 等四页进件查询应答（值集相同）。
/// 官方警示：<b>申请单被驳回后若修改了商户主体名称、法人名称、超级管理员信息、主体类型，则需重新签约</b>
/// ⇒ 不能把此前已出现的 <c>SIGNED</c> 当作永久结论，改过上述资料后要再查一次。
/// </remarks>
public static class EcommerceApplymentSignStates
{
    /// <summary>未签约（<c>UNSIGNED</c>）：电商平台可取签约链接引导超级管理员完成签约。</summary>
    public const string Unsigned = "UNSIGNED";

    /// <summary>已签约（<c>SIGNED</c>）：超级管理员已完成签约。</summary>
    public const string Signed = "SIGNED";

    /// <summary>不可签约（<c>NOT_SIGNABLE</c>）：一般为已驳回、已冻结、机器校验中，暂不支持超级管理员签约。</summary>
    public const string NotSignable = "NOT_SIGNABLE";
}

/// <summary>
/// 平台代理核身方式（<c>proxy_verify_identity_info.verify_identity_type</c>）取值。
/// </summary>
/// <remarks>
/// 出处 <see href="https://pay.weixin.qq.com/doc/v3/partner/4015616722"/>（提交签约核身信息，必填）。
/// 该对象整体仅在申请单「走平台代理签约和代理核身入驻模式」（<c>proxy_sign_verify_mode=true</c>）时必填。
/// </remarks>
public static class EcommerceVerifyIdentityTypes
{
    /// <summary>面对面核身（<c>VERIFY_IDENTITY_TYPE_FACE_TO_FACE</c>）。</summary>
    public const string FaceToFace = "VERIFY_IDENTITY_TYPE_FACE_TO_FACE";

    /// <summary>远程核身（<c>VERIFY_IDENTITY_TYPE_REMOTE</c>）。</summary>
    public const string Remote = "VERIFY_IDENTITY_TYPE_REMOTE";
}

/// <summary>
/// 商家转账授权状态（<c>authorization_state</c>）取值 —— <b>三值，比通用授权面多一个中间态</b>。
/// </summary>
/// <remarks>
/// <para>
/// 出处 <see href="https://pay.weixin.qq.com/doc/v3/partner/4013504208"/>（申请授权）与
/// <see href="https://pay.weixin.qq.com/doc/v3/partner/4013504209"/>（查询授权状态），两页值集相同。
/// </para>
/// <para>
/// <b>不得复用 <see cref="EcommerceAuthStates"/></b>（那是个人收款方签约面的两值集
/// <c>AUTHORIZED</c>/<c>UNAUTHORIZED</c>）：本面官方明文「授权申请正在处理中，请稍后查询授权结果」
/// ⇒ <see cref="AuthorizeProcessing"/> 非终态，申请成功回 <c>AUTHORIZE_PROCESSING</c> 时<b>不能</b>当已授权发起转账，
/// 必须回查。
/// </para>
/// </remarks>
public static class EcommerceTransferAuthorizationStates
{
    /// <summary>授权申请处理中（<c>AUTHORIZE_PROCESSING</c>）：非终态，须稍后回查授权结果。</summary>
    public const string AuthorizeProcessing = "AUTHORIZE_PROCESSING";

    /// <summary>已授权（<c>AUTHORIZED</c>）：二级商户商家转账授权已完成。</summary>
    public const string Authorized = "AUTHORIZED";

    /// <summary>未授权（<c>UNAUTHORIZED</c>）。</summary>
    public const string Unauthorized = "UNAUTHORIZED";
}

/// <summary>
/// 商家转账协议编号（<c>agreement_list[].agreement_code</c>）取值。
/// </summary>
/// <remarks>
/// 出处 <see href="https://pay.weixin.qq.com/doc/v3/partner/4013504208"/>：官方「协议编号可选取值」
/// <b>只列一个值</b> <c>1</c>（微信支付运营功能服务协议（平台二级商户））⇒ 不是自由编号。
/// 配套 <c>agreement_version</c> 官方要求填协议发布时间（形如 <c>20230101</c>）、
/// <c>sign_organization</c> 填二级商户主体名称、<c>sign_time</c> 用 rfc3339 格式。
/// </remarks>
public static class EcommerceTransferAgreementCodes
{
    /// <summary>微信支付运营功能服务协议（平台二级商户）（<c>1</c>）：官方在本页提供的唯一取值。</summary>
    public const string OperationServiceAgreement = "1";
}

/// <summary>
/// 商家转账单据状态（<c>state</c>）取值 —— <b>八值，含两个「结果未明确」态</b>。
/// </summary>
/// <remarks>
/// <para>
/// 出处 <see href="https://pay.weixin.qq.com/doc/v3/partner/4013504211"/>（转账）与
/// <see href="https://pay.weixin.qq.com/doc/v3/partner/4013504213"/>、
/// <see href="https://pay.weixin.qq.com/doc/v3/partner/4013504214"/>（查询），三页值集逐字相同。
/// </para>
/// <para>
/// <b>撤销页（<see href="https://pay.weixin.qq.com/doc/v3/partner/4013504212"/>）官方只列两值</b>
/// （<c>CANCELING</c> 撤销中、<c>CANCELLED</c> 已撤销）⇒ 撤销接口返回的状态是本集合的子集，
/// 不要把撤销应答按八值校验。
/// </para>
/// <para>
/// <b><c>PROCESSING</c> 与 <c>TRANSFERING</c> 都不等于失败</b>：官方对 <c>PROCESSING</c> 提示
/// 「如一直处于此状态，建议检查账户余额是否足够」，对 <c>TRANSFERING</c> 提示
/// 「可拉起微信收款确认页面再次重试确认收款」⇒ 终态只有 <c>SUCCESS</c>/<c>FAIL</c>/<c>CANCELLED</c>，
/// 轮询到中间态不得判失败。
/// </para>
/// </remarks>
public static class EcommerceMchTransferBillStates
{
    /// <summary>转账已受理（<c>ACCEPTED</c>）。</summary>
    public const string Accepted = "ACCEPTED";

    /// <summary>转账处理中、结果尚未明确（<c>PROCESSING</c>）：长期停留时官方提示检查账户余额。</summary>
    public const string Processing = "PROCESSING";

    /// <summary>待收款用户确认（<c>WAIT_USER_CONFIRM</c>）：可用 <c>package_info</c> 拉起微信收款确认页。</summary>
    public const string WaitUserConfirm = "WAIT_USER_CONFIRM";

    /// <summary>转账结果尚未明确（<c>TRANSFERING</c>）：可拉起收款确认页再次重试确认收款。</summary>
    public const string Transfering = "TRANSFERING";

    /// <summary>转账成功（<c>SUCCESS</c>）：终态。</summary>
    public const string Success = "SUCCESS";

    /// <summary>转账失败（<c>FAIL</c>）：终态，配合 <c>fail_reason</c>。</summary>
    public const string Fail = "FAIL";

    /// <summary>撤销受理成功、正在撤销中（<c>CANCELING</c>）：非终态。</summary>
    public const string Canceling = "CANCELING";

    /// <summary>转账撤销完成（<c>CANCELLED</c>）：终态。</summary>
    public const string Cancelled = "CANCELLED";
}

/// <summary>
/// 委托代扣协议状态（<c>contract_state</c>）取值。
/// </summary>
/// <remarks>
/// 出处 <see href="https://pay.weixin.qq.com/doc/v3/partner/4012884060"/>（电商服务商查询签约协议，必填字段）。
/// <b>只有 <c>TERMINATED</c> 时 <c>contract_terminate_info</c> 才有效</b>（官方原文）⇒ 解约信息判空要先看本协议状态，
/// 不要反过来用「有没有解约信息」推断状态。
/// </remarks>
public static class EcommercePapayContractStates
{
    /// <summary>未签约（<c>NOT_SIGNED</c>）。</summary>
    public const string NotSigned = "NOT_SIGNED";

    /// <summary>已签约（<c>SIGNED</c>）。</summary>
    public const string Signed = "SIGNED";

    /// <summary>已解约（<c>TERMINATED</c>）：此时才返回解约信息。</summary>
    public const string Terminated = "TERMINATED";
}

/// <summary>
/// 委托代扣协议解约方式（<c>contract_terminate_info.contract_termination_mode</c>）取值。
/// </summary>
/// <remarks>
/// 出处 <see href="https://pay.weixin.qq.com/doc/v3/partner/4012884060"/>，五值原文照抄。
/// 五值区分了「谁发起的解约」⇒ 排查免密扣款失败原因时用它判别是用户侧、商户侧还是微信支付系统侧动作，
/// 其中 <c>WEPAY_WEB_TERMINATE</c> 的官方原文拼写即为 <c>WEPAY</c>（非 <c>WXPAY</c>），照抄勿改。
/// </remarks>
public static class EcommercePapayTerminationModes
{
    /// <summary>用户主动解约（<c>USER_TERMINATE</c>）。</summary>
    public const string UserTerminate = "USER_TERMINATE";

    /// <summary>商户通过 API 发起的解约（<c>MCH_API_TERMINATE</c>）。</summary>
    public const string MchApiTerminate = "MCH_API_TERMINATE";

    /// <summary>商户在商户平台发起的解约（<c>WEPAY_WEB_TERMINATE</c>）：官方拼写为 <c>WEPAY</c>。</summary>
    public const string WepayWebTerminate = "WEPAY_WEB_TERMINATE";

    /// <summary>用户联系微信支付客服发起的解约（<c>CUSTOM_SERVICE_TERMINATE</c>）。</summary>
    public const string CustomServiceTerminate = "CUSTOM_SERVICE_TERMINATE";

    /// <summary>微信支付系统主动发起的解约（<c>SYSTEM_TERMINATE</c>）。</summary>
    public const string SystemTerminate = "SYSTEM_TERMINATE";
}

/// <summary>
/// 电商代扣合单子单交易状态（<c>sub_orders[].trade_state</c>）取值。
/// </summary>
/// <remarks>
/// <para>
/// 出处 <see href="https://pay.weixin.qq.com/doc/v3/partner/4012884121"/>（合单支付）与
/// <see href="https://pay.weixin.qq.com/doc/v3/partner/4012884130"/>（合单查询），两页应答逐字段实测<b>完全一致</b>、值集相同。
/// </para>
/// <para>
/// <b><c>REFUND</c> 在这里是交易状态而非终态</b>（官方语义「转入退款」）；
/// <c>USERPAYING</c>（用户支付中）与 <c>NOTPAY</c> 都需继续查单 ⇒
/// 支付侧终态只认 <c>SUCCESS</c>/<c>CLOSED</c>/<c>PAYERROR</c>。
/// </para>
/// </remarks>
public static class EcommercePapayTradeStates
{
    /// <summary>支付成功（<c>SUCCESS</c>）。</summary>
    public const string Success = "SUCCESS";

    /// <summary>转入退款（<c>REFUND</c>）。</summary>
    public const string Refund = "REFUND";

    /// <summary>未支付（<c>NOTPAY</c>）。</summary>
    public const string NotPay = "NOTPAY";

    /// <summary>已关闭（<c>CLOSED</c>）。</summary>
    public const string Closed = "CLOSED";

    /// <summary>用户支付中（<c>USERPAYING</c>）。</summary>
    public const string UserPaying = "USERPAYING";

    /// <summary>支付失败，其他原因如银行返回失败（<c>PAYERROR</c>）。</summary>
    public const string PayError = "PAYERROR";
}

/// <summary>
/// 电商代扣合单子单交易类型（<c>sub_orders[].trade_type</c>）取值。
/// </summary>
/// <remarks>
/// 出处 <see href="https://pay.weixin.qq.com/doc/v3/partner/4012884121"/>、
/// <see href="https://pay.weixin.qq.com/doc/v3/partner/4012884130"/>：官方「枚举值：PAP：委托代扣」
/// ⇒ 本域应答<b>只会出现这一个值</b>，与直连合单的 <c>JSAPI/NATIVE/APP</c> 形态不同。
/// </remarks>
public static class EcommercePapayTradeTypes
{
    /// <summary>委托代扣（<c>PAP</c>）：官方在本域应答提供的唯一取值。</summary>
    public const string Pap = "PAP";
}

/// <summary>
/// 个人收款方权限类型（<c>permission_type</c>）取值。
/// </summary>
/// <remarks>
/// 出处 <see href="https://pay.weixin.qq.com/doc/v3/partner/4012739528"/>（<b>必填</b> query）。
/// 官方「可选取值」<b>只提供一个</b>：<c>INDIVIDUAL_PAYEE</c>（个人收款方权限）⇒ 该参数在本域是
/// 「必填但只有一个合法值」的形态，与 <see cref="EcommercePapayTradeTypes"/>、
/// <see cref="EcommerceAbnormalRefundTypes"/> 同属「照页保留单值类」而非冗余建模。
/// </remarks>
public static class EcommercePermissionTypes
{
    /// <summary>个人收款方权限（<c>INDIVIDUAL_PAYEE</c>）：官方在本页提供的唯一取值。</summary>
    public const string IndividualPayee = "INDIVIDUAL_PAYEE";
}
