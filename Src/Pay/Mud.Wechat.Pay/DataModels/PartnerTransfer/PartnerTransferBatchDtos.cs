// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

using Mud.Wechat.Pay.DataModels.Common;

namespace Mud.Wechat.Pay.DataModels.PartnerTransfer;

/// <summary>
/// 服务商「发起批量转账」请求（<b>POST /v3/partner-transfer/batches</b> 的包体）。
/// </summary>
/// <remarks>
/// <para><b>官方文档</b>：<see href="https://pay.weixin.qq.com/doc/v3/partner/4012465738"/>，2026-10-14 逐字段核验。</para>
/// <para><b>服务商三标识形态</b>：<c>sub_mchid</c> <b>必填</b>、<c>sub_appid</c> / <c>sp_appid</c> 选填 ——
/// 与 <see cref="PartnerTransactions"/> 族的 <c>sp_mchid</c> 显式入体不同，本域服务商商户号
/// <b>不在报文里</b>（由签名主体给出），故字段表只有子商户两节 + <c>sp_appid</c>。</para>
/// <para><b><c>authorization_type</c> 必填且影响 <c>openid</c> 归属</b>：
/// <see cref="PartnerTransferAuthorizationTypes.InformationAuthorizationType"/> 用特约商户用户信息、
/// 出款方为服务商（官方注明<b>自 2026-06-29 起停止支持</b>）；
/// <see cref="PartnerTransferAuthorizationTypes.FundAuthorizationType"/> 出款方为特约商户、
/// 用户信息为服务商 <c>appid</c> 下的 openid ⇒ 选错类型会让全部明细以
/// <c>OPENID_INVALID</c> 失败，本字段无默认值可推。</para>
/// <para><b>计数与总额必须自洽</b>：官方 <c>total_num</c>「一个转账批次单最多发起<b>一千</b>笔转账」
/// （银行面是三千），<c>total_amount</c> 须等于 <c>transfer_detail_list[].transfer_amount</c> 之和 ——
/// 本 SDK 不做客户端求和校验，错配由官方以 <c>INVALID_REQUEST</c> 拒绝。</para>
/// <para><b>本域 <c>transfer_detail_list</c> 是必填</b>（银行同名字段为选填）⇒ 两域容器形态不可互换。</para>
/// <para><b>敏感字段加密</b>：<c>user_name</c> / <c>user_id_card</c> 须以微信支付公钥或平台证书加密
/// （<c>Wechatpay-Serial</c> 请求头随之必填），与进件域同一套加密面；加密头由传输层与宿主协同，
/// DTO 只承载<b>密文字符串</b>，不做任何转换。</para>
/// </remarks>
[HttpJsonSerializable(SerializerClassName = "PartnerTransfer")]
public class PartnerTransferBatchCreateRequest
{
    /// <summary>特约商户号（<c>sub_mchid</c>，必填 string(32)）：微信支付分配的特约商户号。</summary>
    [JsonPropertyName("sub_mchid")]
    public string? SubMchId { get; set; }

    /// <summary>特约商户 AppID（<c>sub_appid</c>，选填 string(32)）：微信分配的特约商户公众号 AppID。</summary>
    [JsonPropertyName("sub_appid")]
    public string? SubAppId { get; set; }

    /// <summary>特约商户授权类型（<c>authorization_type</c>，必填，3 值）：见 <see cref="PartnerTransferAuthorizationTypes"/>。</summary>
    [JsonPropertyName("authorization_type")]
    public string? AuthorizationType { get; set; }

    /// <summary>商家批次单号（<c>out_batch_no</c>，必填 string(32)）：商户系统内部唯一，<b>发起后不可更换</b>。</summary>
    [JsonPropertyName("out_batch_no")]
    public string? OutBatchNo { get; set; }

    /// <summary>批次名称（<c>batch_name</c>，必填 string(32)）：该笔批量转账的名称。</summary>
    [JsonPropertyName("batch_name")]
    public string? BatchName { get; set; }

    /// <summary>批次备注（<c>batch_remark</c>，必填 string(32)）：转账说明，UTF8 编码，最多 32 字符。</summary>
    [JsonPropertyName("batch_remark")]
    public string? BatchRemark { get; set; }

    /// <summary>转账总金额（<c>total_amount</c>，必填 integer）：单位为<b>分</b>，须等于明细金额之和。</summary>
    [JsonPropertyName("total_amount")]
    public int? TotalAmount { get; set; }

    /// <summary>转账总笔数（<c>total_num</c>，必填 integer）：官方上限<b>一千</b>笔（银行面为三千）。</summary>
    [JsonPropertyName("total_num")]
    public int? TotalNum { get; set; }

    /// <summary>转账明细列表（<c>transfer_detail_list</c>，<b>必填</b> array）：见 <see cref="PartnerTransferDetailInput"/>。</summary>
    [JsonPropertyName("transfer_detail_list")]
    public List<PartnerTransferDetailInput>? TransferDetailList { get; set; }

    /// <summary>服务商 AppID（<c>sp_appid</c>，选填 string(32)）：微信分配的服务商公众号 AppID。</summary>
    [JsonPropertyName("sp_appid")]
    public string? SpAppId { get; set; }

    /// <summary>批量转账用途（<c>transfer_purpose</c>，选填，6 值）：见 <see cref="PartnerTransferPurposes"/>。</summary>
    [JsonPropertyName("transfer_purpose")]
    public string? TransferPurpose { get; set; }

    /// <summary>转账场景（<c>transfer_scene</c>，选填，2 值）：见 <see cref="PartnerTransferScenes"/>，缺省为普通转账。</summary>
    [JsonPropertyName("transfer_scene")]
    public string? TransferScene { get; set; }
}

/// <summary>
/// 转账明细入参（官方构件名 <c>TransferDetailInput</c>，服务商面）。
/// </summary>
/// <remarks>
/// <para><b>与银行面同名但必填性不同</b>：本面 <c>user_name</c> <b>选填</b>，
/// 银行面 <c>user_name</c> <b>必填</b>（务工卡场景须实名）⇒ 两域各建一套构件类型，
/// 字段名照抄官方原文共用（守卫 BT/PTF 系列锁定）。</para>
/// <para><c>user_name</c> / <c>user_id_card</c> 为<b>密文</b>字段（string(1024)），须先加密再赋值。</para>
/// </remarks>
[HttpJsonSerializable(SerializerClassName = "PartnerTransfer")]
public class PartnerTransferDetailInput
{
    /// <summary>商家明细单号（<c>out_detail_no</c>，必填 string(32)）：批次单下唯一。</summary>
    [JsonPropertyName("out_detail_no")]
    public string? OutDetailNo { get; set; }

    /// <summary>转账金额（<c>transfer_amount</c>，必填 integer）：单位为<b>分</b>。</summary>
    [JsonPropertyName("transfer_amount")]
    public int? TransferAmount { get; set; }

    /// <summary>转账备注（<c>transfer_remark</c>，必填 string(32)）：微信用户会收到该备注，UTF8 编码。</summary>
    [JsonPropertyName("transfer_remark")]
    public string? TransferRemark { get; set; }

    /// <summary>收款用户 OpenID（<c>openid</c>，必填 string(64)）：归属由 <c>authorization_type</c> 决定（服务商或特约商户 AppID 下）。</summary>
    [JsonPropertyName("openid")]
    public string? OpenId { get; set; }

    /// <summary>收款用户姓名（<c>user_name</c>，<b>本面选填</b> string(1024)）：<b>密文</b>，须加密后传入。</summary>
    [JsonPropertyName("user_name")]
    public string? UserName { get; set; }

    /// <summary>收款用户证件号（<c>user_id_card</c>，选填 string(1024)）：<b>密文</b>，务工卡场景须加密传入。</summary>
    [JsonPropertyName("user_id_card")]
    public string? UserIdCard { get; set; }
}

/// <summary>
/// 服务商「发起批量转账」应答（200 报文，仅三字段）。
/// </summary>
/// <remarks>
/// <para><b>受理 ≠ 转账成功</b>：本应答只给出批次号与受理时间，批次状态须走
/// <c>QueryBatchByBatchIdAsync</c> 轮询；官方明令<b>批量转账一旦发起不允许撤销</b>，
/// 未查到明确批次结果前<b>不得换 <c>out_batch_no</c> 重提</b>。</para>
/// <para>官方文档：<see href="https://pay.weixin.qq.com/doc/v3/partner/4012465738"/>。</para>
/// </remarks>
[HttpJsonSerializable(SerializerClassName = "PartnerTransfer")]
public class PartnerTransferBatchCreateResponse : WechatPayResponse
{
    /// <summary>商家批次单号（<c>out_batch_no</c>，必填 string(32)）：原样回显入参。</summary>
    [JsonPropertyName("out_batch_no")]
    public string? OutBatchNo { get; set; }

    /// <summary>微信支付批次单号（<c>batch_id</c>，必填 string(64)）：微信侧唯一标识。</summary>
    [JsonPropertyName("batch_id")]
    public string? BatchId { get; set; }

    /// <summary>批次受理时间（<c>create_time</c>，必填 string(32)）：rfc3339，保持 <c>string</c> 不做往返。</summary>
    [JsonPropertyName("create_time")]
    public string? CreateTime { get; set; }
}

/// <summary>
/// 服务商「查询批次单」应答（两支按 <c>batch_id</c> / <c>out_batch_no</c> 定位的 GET <b>共用</b>本类型）。
/// </summary>
/// <remarks>
/// <para><b>官方文档</b>：微信支付批次单号查单 <see href="https://pay.weixin.qq.com/doc/v3/partner/4012465772"/>、
/// 商家批次单号查单 <see href="https://pay.weixin.qq.com/doc/v3/partner/4012465790"/>，
/// 2026-10-14 逐页比对<b>字段表完全相同</b> ⇒ 按「表相同则共用」建一型（守卫 PTF-B6 断言两方法同返回类型）。</para>
/// <para><b>与银行面不同名</b>：本面用 <c>total_num</c> / <c>success_num</c> / <c>fail_num</c>、
/// 批次状态字段名 <c>batch_status</c>；银行面是 <c>total_count</c> / <c>success_count</c> / <c>fail_count</c>
/// 与 <c>batch_state</c> ⇒ 两域类型不可共用。</para>
/// <para><b>应答多出三节</b>：<c>sp_appid</c>、<c>transfer_purpose</c>、<c>transfer_scene</c>
/// 官方列在 <c>transfer_detail_list</c> <b>之后</b>（示例报文亦以顶层键收尾）⇒ 它们是批次单字段而非明细字段，
/// 勿误挂进 <see cref="PartnerTransferBatchDetailItem"/>。</para>
/// <para><b>明细列表只在批次 <c>FINISHED</c> 时返回</b>（官方原文），批次处理中须改用明细查询端点。</para>
/// </remarks>
[HttpJsonSerializable(SerializerClassName = "PartnerTransfer")]
public class PartnerTransferBatchQueryResponse : WechatPayResponse
{
    /// <summary>服务商商户号（<c>sp_mchid</c>，必填 string(32)）。</summary>
    [JsonPropertyName("sp_mchid")]
    public string? SpMchId { get; set; }

    /// <summary>特约商户号（<c>sub_mchid</c>，必填 string(32)）。</summary>
    [JsonPropertyName("sub_mchid")]
    public string? SubMchId { get; set; }

    /// <summary>商家批次单号（<c>out_batch_no</c>，必填 string(32)）。</summary>
    [JsonPropertyName("out_batch_no")]
    public string? OutBatchNo { get; set; }

    /// <summary>微信支付批次单号（<c>batch_id</c>，必填 string(64)）。</summary>
    [JsonPropertyName("batch_id")]
    public string? BatchId { get; set; }

    /// <summary>特约商户 AppID（<c>sub_appid</c>，选填 string(32)）。</summary>
    [JsonPropertyName("sub_appid")]
    public string? SubAppId { get; set; }

    /// <summary>批次状态（<c>batch_status</c>，必填 string(32)，5 值）：见 <see cref="PartnerTransferBatchStates"/>。</summary>
    [JsonPropertyName("batch_status")]
    public string? BatchStatus { get; set; }

    /// <summary>批次类型（<c>batch_type</c>，必填 string(32)，2 值）：见 <see cref="PartnerTransferBatchTypes"/>。</summary>
    [JsonPropertyName("batch_type")]
    public string? BatchType { get; set; }

    /// <summary>特约商户授权类型（<c>authorization_type</c>，必填，3 值）：见 <see cref="PartnerTransferAuthorizationTypes"/>。</summary>
    [JsonPropertyName("authorization_type")]
    public string? AuthorizationType { get; set; }

    /// <summary>批次名称（<c>batch_name</c>，必填 string(32)）。</summary>
    [JsonPropertyName("batch_name")]
    public string? BatchName { get; set; }

    /// <summary>批次备注（<c>batch_remark</c>，必填 string(32)）。</summary>
    [JsonPropertyName("batch_remark")]
    public string? BatchRemark { get; set; }

    /// <summary>批次关闭原因（<c>close_reason</c>，选填，官方注明<b>取值可能新增</b>）：见 <see cref="PartnerTransferCloseReasons"/>。</summary>
    [JsonPropertyName("close_reason")]
    public string? CloseReason { get; set; }

    /// <summary>转账总金额（<c>total_amount</c>，必填 integer）：单位为分。</summary>
    [JsonPropertyName("total_amount")]
    public int? TotalAmount { get; set; }

    /// <summary>转账总笔数（<c>total_num</c>，必填 integer）：本域字段名为 <c>total_num</c>，非银行的 <c>total_count</c>。</summary>
    [JsonPropertyName("total_num")]
    public int? TotalNum { get; set; }

    /// <summary>批次创建时间（<c>create_time</c>，选填 string(32)）：批次受理成功时返回，rfc3339。</summary>
    [JsonPropertyName("create_time")]
    public string? CreateTime { get; set; }

    /// <summary>批次更新时间（<c>update_time</c>，选填 string(32)）：最近一次状态变更时间，rfc3339。</summary>
    [JsonPropertyName("update_time")]
    public string? UpdateTime { get; set; }

    /// <summary>转账成功金额（<c>success_amount</c>，选填 integer）：<c>PROCESSING</c> 期间随时变化。</summary>
    [JsonPropertyName("success_amount")]
    public int? SuccessAmount { get; set; }

    /// <summary>转账成功笔数（<c>success_num</c>，选填 integer）。</summary>
    [JsonPropertyName("success_num")]
    public int? SuccessNum { get; set; }

    /// <summary>转账失败金额（<c>fail_amount</c>，选填 integer）。</summary>
    [JsonPropertyName("fail_amount")]
    public int? FailAmount { get; set; }

    /// <summary>转账失败笔数（<c>fail_num</c>，选填 integer）。</summary>
    [JsonPropertyName("fail_num")]
    public int? FailNum { get; set; }

    /// <summary>转账明细列表（<c>transfer_detail_list</c>，选填 array）：仅批次 <c>FINISHED</c> 时返回。</summary>
    [JsonPropertyName("transfer_detail_list")]
    public List<PartnerTransferBatchDetailItem>? TransferDetailList { get; set; }

    /// <summary>服务商 AppID（<c>sp_appid</c>，选填 string(32)）。</summary>
    [JsonPropertyName("sp_appid")]
    public string? SpAppId { get; set; }

    /// <summary>批量转账用途（<c>transfer_purpose</c>，选填，6 值）。</summary>
    [JsonPropertyName("transfer_purpose")]
    public string? TransferPurpose { get; set; }

    /// <summary>转账场景（<c>transfer_scene</c>，选填，2 值）。</summary>
    [JsonPropertyName("transfer_scene")]
    public string? TransferScene { get; set; }
}

/// <summary>
/// 批次单应答内嵌的转账明细概要（<c>transfer_detail_list[]</c>，服务商面，官方 3 字段）。
/// </summary>
/// <remarks>
/// <b>银行面此处多出 <c>fail_reason</c></b>（4 字段）⇒ 两域概要类型分建。
/// 明细状态字段本面为 <c>detail_status</c>，银行面为 <c>detail_state</c>。
/// </remarks>
[HttpJsonSerializable(SerializerClassName = "PartnerTransfer")]
public class PartnerTransferBatchDetailItem
{
    /// <summary>微信支付明细单号（<c>detail_id</c>，必填 string(64)）。</summary>
    [JsonPropertyName("detail_id")]
    public string? DetailId { get; set; }

    /// <summary>商家明细单号（<c>out_detail_no</c>，必填 string(32)）。</summary>
    [JsonPropertyName("out_detail_no")]
    public string? OutDetailNo { get; set; }

    /// <summary>明细状态（<c>detail_status</c>，必填 string(32)）：本面字段名为 <c>detail_status</c>。</summary>
    [JsonPropertyName("detail_status")]
    public string? DetailStatus { get; set; }
}
