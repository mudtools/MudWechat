// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

using Mud.Wechat.Pay.DataModels.Common;

namespace Mud.Wechat.Pay.DataModels.Transfer;

/// <summary>
/// 旧版商家转账·发起批量转账请求体（<b>POST /v3/transfer/batches</b>）。
/// </summary>
/// <remarks>
/// <para><b>官方文档</b>：<see href="https://pay.weixin.qq.com/doc/v3/merchant/4012458841"/>，2026-10-18 逐字段核验；
/// 支持商户：<b>普通商户</b>。这是「商家转账到零钱」的旧版批量接口，与
/// <see cref="TransferBillRequest"/>（新版 <c>/v3/fund-app/mch-transfer/transfer-bills</c>）<b>不是同一产品面</b>。</para>
/// <para><b>本应答的 <c>batch_status</c> 只有 4 值</b>（<c>ACCEPTED</c> / <c>PROCESSING</c> /
/// <c>FINISHED</c> / <c>CLOSED</c>）：与新版转账单的 8 值状态表<b>不可混用</b>。</para>
/// </remarks>
[HttpJsonSerializable(SerializerClassName = "Transfer")]
public class TransferBatchCreateRequest
{
    /// <summary>公众账号 ID（<c>appid</c>，必填 string(32)）。</summary>
    [JsonPropertyName("appid")]
    public string? AppId { get; set; }

    /// <summary>商户批次单号（<c>out_batch_no</c>，必填 string(32)）：<b>业务幂等键</b>，同商户号下唯一。</summary>
    [JsonPropertyName("out_batch_no")]
    public string? OutBatchNo { get; set; }

    /// <summary>批次名称（<c>batch_name</c>，必填 string(32)）。</summary>
    [JsonPropertyName("batch_name")]
    public string? BatchName { get; set; }

    /// <summary>批次备注（<c>batch_remark</c>，必填 string(32)）。</summary>
    [JsonPropertyName("batch_remark")]
    public string? BatchRemark { get; set; }

    /// <summary>总金额（<c>total_amount</c>，必填整型，单位分）：须等于明细金额之和。</summary>
    [JsonPropertyName("total_amount")]
    public long? TotalAmount { get; set; }

    /// <summary>总条数（<c>total_num</c>，必填整型）：须等于明细条数。</summary>
    [JsonPropertyName("total_num")]
    public int? TotalNum { get; set; }

    /// <summary>转账明细列表（<c>transfer_detail_list</c>，必填），见 <see cref="TransferBatchDetailInput"/>。</summary>
    [JsonPropertyName("transfer_detail_list")]
    public List<TransferBatchDetailInput>? TransferDetailList { get; set; }
}

/// <summary>发起批量转账时的单笔明细（<c>transfer_detail_list[]</c>）。</summary>
[HttpJsonSerializable(SerializerClassName = "Transfer")]
public class TransferBatchDetailInput
{
    /// <summary>商户明细单号（<c>out_detail_no</c>，必填 string(32)）：同批次内唯一。</summary>
    [JsonPropertyName("out_detail_no")]
    public string? OutDetailNo { get; set; }

    /// <summary>转账金额（<c>transfer_amount</c>，必填整型，单位分）。</summary>
    [JsonPropertyName("transfer_amount")]
    public long? TransferAmount { get; set; }

    /// <summary>转账备注（<c>transfer_remark</c>，必填 string(32)）：用户可见文案。</summary>
    [JsonPropertyName("transfer_remark")]
    public string? TransferRemark { get; set; }

    /// <summary>收款用户 OpenID（<c>openid</c>，必填 string(64)）。</summary>
    [JsonPropertyName("openid")]
    public string? OpenId { get; set; }

    /// <summary>收款用户姓名（<c>user_name</c>，选填 string(64)）：官方标注为需加密字段（按官方加密指引处理）。</summary>
    [JsonPropertyName("user_name")]
    public string? UserName { get; set; }
}

/// <summary>
/// 发起批量转账应答（<b>仅 4 字段</b>）。
/// </summary>
/// <remarks>
/// <b>官方文档</b>：<see href="https://pay.weixin.qq.com/doc/v3/merchant/4012458841"/>，2026-10-18 核验。
/// <c>batch_status</c> 取值见 <see cref="TransferBatchStatuses"/>；本应答<b>不是</b>终态，
/// 须经批次查询端点轮询到 <c>FINISHED</c> / <c>CLOSED</c> 并逐笔核对明细。
/// </remarks>
[HttpJsonSerializable(SerializerClassName = "Transfer")]
public class TransferBatchCreateResponse : WechatPayResponse
{
    /// <summary>商户批次单号（<c>out_batch_no</c>，必填 string(32)）。</summary>
    [JsonPropertyName("out_batch_no")]
    public string? OutBatchNo { get; set; }

    /// <summary>微信批次单号（<c>batch_id</c>，必填 string(64)）：微信侧批次唯一标识。</summary>
    [JsonPropertyName("batch_id")]
    public string? BatchId { get; set; }

    /// <summary>批次创建时间（<c>create_time</c>，必填 string，rfc3339）。</summary>
    [JsonPropertyName("create_time")]
    public string? CreateTime { get; set; }

    /// <summary>批次状态（<c>batch_status</c>，必填 string）：取值见 <see cref="TransferBatchStatuses"/>。</summary>
    [JsonPropertyName("batch_status")]
    public string? BatchStatus { get; set; }
}

/// <summary>
/// 批量转账批次查询应答（<b>包装形态</b> <c>{transfer_batch, transfer_detail_list}</c>）。
/// </summary>
/// <remarks>
/// <para><b>官方文档</b>：<see href="https://pay.weixin.qq.com/doc/v3/merchant/4012458868"/>（按微信批次单号）、
/// <see href="https://pay.weixin.qq.com/doc/v3/merchant/4012458877"/>（按商户批次单号），
/// 2026-10-18 逐页比对：两页应答表<b>逐项一致</b> ⇒ 共用本类型。</para>
/// <para><b><c>need_query_detail</c> 是必填 query</b>：取 <c>true</c> 才会返回 <c>transfer_detail_list</c>；
/// 取 <c>false</c> 时明细列表为空 —— 不要据此判定「批次没有明细」。</para>
/// <para><b>本应答是快照而非终态</b>：须以 <c>batch_status</c> 到达 <c>FINISHED</c>/<c>CLOSED</c>
/// 且 <c>success_num + fail_num = total_num</c> 才能做资金结论。</para>
/// </remarks>
[HttpJsonSerializable(SerializerClassName = "Transfer")]
public class TransferBatchQueryResponse : WechatPayResponse
{
    /// <summary>批次概要（<c>transfer_batch</c>，必填），见 <see cref="TransferBatchInfo"/>。</summary>
    [JsonPropertyName("transfer_batch")]
    public TransferBatchInfo? TransferBatch { get; set; }

    /// <summary>明细列表（<c>transfer_detail_list</c>，仅在 <c>need_query_detail = true</c> 时返回），见 <see cref="TransferBatchDetailItem"/>。</summary>
    [JsonPropertyName("transfer_detail_list")]
    public List<TransferBatchDetailItem>? TransferDetailList { get; set; }
}

/// <summary>批次概要（<c>transfer_batch</c>）。</summary>
[HttpJsonSerializable(SerializerClassName = "Transfer")]
public class TransferBatchInfo
{
    /// <summary>商户号（<c>mchid</c>，<b>无下划线</b>——与新版转账查询的 <c>mch_id</c> 不同）。</summary>
    [JsonPropertyName("mchid")]
    public string? MchId { get; set; }

    /// <summary>商户批次单号（<c>out_batch_no</c>）。</summary>
    [JsonPropertyName("out_batch_no")]
    public string? OutBatchNo { get; set; }

    /// <summary>微信批次单号（<c>batch_id</c>）。</summary>
    [JsonPropertyName("batch_id")]
    public string? BatchId { get; set; }

    /// <summary>公众账号 ID（<c>appid</c>）。</summary>
    [JsonPropertyName("appid")]
    public string? AppId { get; set; }

    /// <summary>批次状态（<c>batch_status</c>）：取值见 <see cref="TransferBatchStatuses"/>。</summary>
    [JsonPropertyName("batch_status")]
    public string? BatchStatus { get; set; }

    /// <summary>批次类型（<c>batch_type</c>，如 <c>API</c>）。</summary>
    [JsonPropertyName("batch_type")]
    public string? BatchType { get; set; }

    /// <summary>批次名称（<c>batch_name</c>）。</summary>
    [JsonPropertyName("batch_name")]
    public string? BatchName { get; set; }

    /// <summary>批次备注（<c>batch_remark</c>）。</summary>
    [JsonPropertyName("batch_remark")]
    public string? BatchRemark { get; set; }

    /// <summary>批次关闭原因（<c>close_reason</c>）：批次关闭时返回。</summary>
    [JsonPropertyName("close_reason")]
    public string? CloseReason { get; set; }

    /// <summary>总金额（<c>total_amount</c>，单位分）。</summary>
    [JsonPropertyName("total_amount")]
    public long? TotalAmount { get; set; }

    /// <summary>总条数（<c>total_num</c>）。</summary>
    [JsonPropertyName("total_num")]
    public int? TotalNum { get; set; }

    /// <summary>批次创建时间（<c>create_time</c>，rfc3339）。</summary>
    [JsonPropertyName("create_time")]
    public string? CreateTime { get; set; }

    /// <summary>批次更新时间（<c>update_time</c>，rfc3339）。</summary>
    [JsonPropertyName("update_time")]
    public string? UpdateTime { get; set; }

    /// <summary>成功金额（<c>success_amount</c>，单位分）。</summary>
    [JsonPropertyName("success_amount")]
    public long? SuccessAmount { get; set; }

    /// <summary>成功条数（<c>success_num</c>）。</summary>
    [JsonPropertyName("success_num")]
    public int? SuccessNum { get; set; }

    /// <summary>失败金额（<c>fail_amount</c>，单位分）。</summary>
    [JsonPropertyName("fail_amount")]
    public long? FailAmount { get; set; }

    /// <summary>失败条数（<c>fail_num</c>）。</summary>
    [JsonPropertyName("fail_num")]
    public int? FailNum { get; set; }

    /// <summary>转账场景 ID（<c>transfer_scene_id</c>）。</summary>
    [JsonPropertyName("transfer_scene_id")]
    public string? TransferSceneId { get; set; }
}

/// <summary>批次查询应答里的明细项（<c>transfer_detail_list[]</c>，<b>仅 3 字段</b>）。</summary>
/// <remarks>
/// <b>本列表是索引而非明细详情</b>：要拿金额、收款人、失败原因等完整字段，须再调
/// 「按微信明细单号查询」或「按商户明细单号查询」端点。
/// </remarks>
[HttpJsonSerializable(SerializerClassName = "Transfer")]
public class TransferBatchDetailItem
{
    /// <summary>微信明细单号（<c>detail_id</c>）。</summary>
    [JsonPropertyName("detail_id")]
    public string? DetailId { get; set; }

    /// <summary>商户明细单号（<c>out_detail_no</c>）。</summary>
    [JsonPropertyName("out_detail_no")]
    public string? OutDetailNo { get; set; }

    /// <summary>明细状态（<c>detail_status</c>）：取值须按官方明细状态表判定。</summary>
    [JsonPropertyName("detail_status")]
    public string? DetailStatus { get; set; }
}

/// <summary>
/// 批量转账单笔明细查询应答（<b>平铺明细表</b>）。
/// </summary>
/// <remarks>
/// <para><b>官方文档</b>：<see href="https://pay.weixin.qq.com/doc/v3/merchant/4012458886"/>（按微信明细单号）、
/// <see href="https://pay.weixin.qq.com/doc/v3/merchant/4012458902"/>（按商户明细单号），
/// 2026-10-18 逐页比对：两页应答表<b>逐项一致</b> ⇒ 共用本类型。</para>
/// <para><b>字段名是 <c>user_name</c></b>（明文姓名，选填）—— 本页<b>没有</b> <c>user_name_encrypted</c>，
/// 勿按其它产品线的加密字段命名补造。</para>
/// </remarks>
[HttpJsonSerializable(SerializerClassName = "Transfer")]
public class TransferBatchDetailResponse : WechatPayResponse
{
    /// <summary>商户号（<c>mchid</c>）。</summary>
    [JsonPropertyName("mchid")]
    public string? MchId { get; set; }

    /// <summary>商户批次单号（<c>out_batch_no</c>）。</summary>
    [JsonPropertyName("out_batch_no")]
    public string? OutBatchNo { get; set; }

    /// <summary>微信批次单号（<c>batch_id</c>）。</summary>
    [JsonPropertyName("batch_id")]
    public string? BatchId { get; set; }

    /// <summary>公众账号 ID（<c>appid</c>）。</summary>
    [JsonPropertyName("appid")]
    public string? AppId { get; set; }

    /// <summary>商户明细单号（<c>out_detail_no</c>）。</summary>
    [JsonPropertyName("out_detail_no")]
    public string? OutDetailNo { get; set; }

    /// <summary>微信明细单号（<c>detail_id</c>）。</summary>
    [JsonPropertyName("detail_id")]
    public string? DetailId { get; set; }

    /// <summary>明细状态（<c>detail_status</c>）：取值须按官方明细状态表判定。</summary>
    [JsonPropertyName("detail_status")]
    public string? DetailStatus { get; set; }

    /// <summary>转账金额（<c>transfer_amount</c>，单位分）。</summary>
    [JsonPropertyName("transfer_amount")]
    public long? TransferAmount { get; set; }

    /// <summary>转账备注（<c>transfer_remark</c>）。</summary>
    [JsonPropertyName("transfer_remark")]
    public string? TransferRemark { get; set; }

    /// <summary>失败原因（<c>fail_reason</c>）：明细失败时返回。</summary>
    [JsonPropertyName("fail_reason")]
    public string? FailReason { get; set; }

    /// <summary>收款用户 OpenID（<c>openid</c>）。</summary>
    [JsonPropertyName("openid")]
    public string? OpenId { get; set; }

    /// <summary>收款用户姓名（<c>user_name</c>）。</summary>
    [JsonPropertyName("user_name")]
    public string? UserName { get; set; }

    /// <summary>明细发起时间（<c>initiate_time</c>，rfc3339）。</summary>
    [JsonPropertyName("initiate_time")]
    public string? InitiateTime { get; set; }

    /// <summary>明细更新时间（<c>update_time</c>，rfc3339）。</summary>
    [JsonPropertyName("update_time")]
    public string? UpdateTime { get; set; }
}

/// <summary>
/// 申请批次转账电子回单请求体（<b>POST /v3/transfer/bill-receipt</b>）。
/// </summary>
/// <remarks>
/// <b>官方文档</b>：<see href="https://pay.weixin.qq.com/doc/v3/merchant/4012458946"/>，2026-10-18 核验。
/// 请求体<b>只有 <c>out_batch_no</c></b>；回单按<b>批次</b>生成（本族另有按明细生成的回单面）。
/// </remarks>
[HttpJsonSerializable(SerializerClassName = "Transfer")]
public class TransferBillReceiptRequest
{
    /// <summary>商户批次单号（<c>out_batch_no</c>，必填 string(32)）。</summary>
    [JsonPropertyName("out_batch_no")]
    public string? OutBatchNo { get; set; }
}

/// <summary>
/// 批次转账电子回单应答（申请与查询<b>共用</b>）。
/// </summary>
/// <remarks>
/// <para><b>官方文档</b>：<see href="https://pay.weixin.qq.com/doc/v3/merchant/4012458946"/>（申请）、
/// <see href="https://pay.weixin.qq.com/doc/v3/merchant/4012458950"/>（查询），
/// 2026-10-18 逐页比对：两页应答表<b>逐项一致</b> ⇒ 共用本类型。</para>
/// <para><b>两段式</b>：申请后 <c>signature_status</c> 通常为 <c>ACCEPTED</c>，
/// 须轮询查询端点直到 <c>FINISHED</c> 才具备 <c>hash_value</c> / <c>download_url</c>。</para>
/// <para><b><c>download_url</c> 不得自行拼接</b>，且须用 <c>hash_type</c>/<c>hash_value</c> 校验下载文件摘要。</para>
/// </remarks>
[HttpJsonSerializable(SerializerClassName = "Transfer")]
public class TransferBillReceiptResponse : WechatPayResponse
{
    /// <summary>商户批次单号（<c>out_batch_no</c>）。</summary>
    [JsonPropertyName("out_batch_no")]
    public string? OutBatchNo { get; set; }

    /// <summary>电子回单申请单号（<c>signature_no</c>）。</summary>
    [JsonPropertyName("signature_no")]
    public string? SignatureNo { get; set; }

    /// <summary>电子回单状态（<c>signature_status</c>）：取值见 <see cref="TransferSignatureStatuses"/>。</summary>
    [JsonPropertyName("signature_status")]
    public string? SignatureStatus { get; set; }

    /// <summary>回单文件摘要类型（<c>hash_type</c>）：仅已完成时返回。</summary>
    [JsonPropertyName("hash_type")]
    public string? HashType { get; set; }

    /// <summary>回单文件摘要值（<c>hash_value</c>）：仅已完成时返回，须与下载文件摘要比对。</summary>
    [JsonPropertyName("hash_value")]
    public string? HashValue { get; set; }

    /// <summary>回单文件下载地址（<c>download_url</c>）：仅已完成时返回，须原样使用、勿自行拼接。</summary>
    [JsonPropertyName("download_url")]
    public string? DownloadUrl { get; set; }

    /// <summary>申请单创建时间（<c>create_time</c>，rfc3339）。</summary>
    [JsonPropertyName("create_time")]
    public string? CreateTime { get; set; }

    /// <summary>申请单更新时间（<c>update_time</c>，rfc3339）。</summary>
    [JsonPropertyName("update_time")]
    public string? UpdateTime { get; set; }
}

/// <summary>
/// 申请转账明细电子回单请求体（<b>POST /v3/transfer-detail/electronic-receipts</b>）。
/// </summary>
/// <remarks>
/// <para><b>官方文档</b>：<see href="https://pay.weixin.qq.com/doc/v3/merchant/4012458967"/>，2026-10-18 逐字段核验。</para>
/// <para><b><c>accept_type</c> 决定 <c>out_batch_no</c> 是否必填</b>：官方把
/// <c>BATCH_TRANSFER</c> 归为「批次转账」、<c>TRANSFER_TO_POCKET</c> / <c>TRANSFER_TO_BANK</c> 归为
/// 「转账到零钱/银行卡」；后者场景下 <c>out_batch_no</c> 的必填性按官方页面为准（本类型按选填承载）。</para>
/// </remarks>
[HttpJsonSerializable(SerializerClassName = "Transfer")]
public class TransferElectronicReceiptRequest
{
    /// <summary>回单类型（<c>accept_type</c>，必填 string）：取值见 <see cref="TransferReceiptAcceptTypes"/>。</summary>
    [JsonPropertyName("accept_type")]
    public string? AcceptType { get; set; }

    /// <summary>商户批次单号（<c>out_batch_no</c>，条件必填）：批次转账场景需上传。</summary>
    [JsonPropertyName("out_batch_no")]
    public string? OutBatchNo { get; set; }

    /// <summary>商户明细单号（<c>out_detail_no</c>，必填 string(32)）。</summary>
    [JsonPropertyName("out_detail_no")]
    public string? OutDetailNo { get; set; }
}

/// <summary>
/// 转账明细电子回单应答（申请与查询<b>共用</b>）。
/// </summary>
/// <remarks>
/// <para><b>官方文档</b>：<see href="https://pay.weixin.qq.com/doc/v3/merchant/4012458967"/>（申请）、
/// <see href="https://pay.weixin.qq.com/doc/v3/merchant/4012458972"/>（查询），
/// 2026-10-18 逐页比对：两页应答表<b>逐项一致</b> ⇒ 共用本类型。</para>
/// <para><b>与批次回单应答的差异</b>：本表多 <c>accept_type</c> / <c>out_detail_no</c>，
/// 且<b>没有</b> <c>create_time</c> / <c>update_time</c> ⇒ 两个回单族不共用类型。</para>
/// </remarks>
[HttpJsonSerializable(SerializerClassName = "Transfer")]
public class TransferElectronicReceiptResponse : WechatPayResponse
{
    /// <summary>回单类型（<c>accept_type</c>）：取值见 <see cref="TransferReceiptAcceptTypes"/>。</summary>
    [JsonPropertyName("accept_type")]
    public string? AcceptType { get; set; }

    /// <summary>商户批次单号（<c>out_batch_no</c>）。</summary>
    [JsonPropertyName("out_batch_no")]
    public string? OutBatchNo { get; set; }

    /// <summary>商户明细单号（<c>out_detail_no</c>）。</summary>
    [JsonPropertyName("out_detail_no")]
    public string? OutDetailNo { get; set; }

    /// <summary>电子回单申请单号（<c>signature_no</c>）。</summary>
    [JsonPropertyName("signature_no")]
    public string? SignatureNo { get; set; }

    /// <summary>电子回单状态（<c>signature_status</c>）：取值见 <see cref="TransferSignatureStatuses"/>。</summary>
    [JsonPropertyName("signature_status")]
    public string? SignatureStatus { get; set; }

    /// <summary>回单文件摘要类型（<c>hash_type</c>）：仅已完成时返回。</summary>
    [JsonPropertyName("hash_type")]
    public string? HashType { get; set; }

    /// <summary>回单文件摘要值（<c>hash_value</c>）：仅已完成时返回，须与下载文件摘要比对。</summary>
    [JsonPropertyName("hash_value")]
    public string? HashValue { get; set; }

    /// <summary>回单文件下载地址（<c>download_url</c>）：仅已完成时返回，须原样使用、勿自行拼接。</summary>
    [JsonPropertyName("download_url")]
    public string? DownloadUrl { get; set; }
}
