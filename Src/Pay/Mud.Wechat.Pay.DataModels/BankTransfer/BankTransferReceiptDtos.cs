// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

using Mud.Wechat.Pay.DataModels.Common;

namespace Mud.Wechat.Pay.DataModels.BankTransfer;

/// <summary>
/// 从业机构（银行）「受理汇总电子回单」请求（<b>POST /v3/bank-batch-transfer/receipt/summary-receipts</b> 的包体）。
/// </summary>
/// <remarks>
/// <para><b>官方文档</b>：<see href="https://pay.weixin.qq.com/doc/v3/partner/4012470746"/>，2026-10-14 逐字段核验。</para>
/// <para><b>受理与查询共用一条路由</b>：本域是支付线里<b>唯一</b>「同一张表同时有 POST 与 GET」的形态
/// （POST 发起受理、GET 查受理结果）⇒ 端点计数须按 <b>(方法, 路由)</b> 二元组，否则 9 支会被数成 7 支（守卫 BT-B1）。</para>
/// <para>回单生成为<b>异步</b>：受理成功只回 <c>receipt_state = ACCEPTED</c>，文件字段须轮询查询支才出现。</para>
/// </remarks>
[HttpJsonSerializable(SerializerClassName = "BankTransfer")]
public class BankTransferSummaryReceiptRequest
{
    /// <summary>银行批次单号（<c>out_batch_no</c>，必填 string(32)）：须为该银行服务商已完成的批次。</summary>
    [JsonPropertyName("out_batch_no")]
    public string? OutBatchNo { get; set; }
}

/// <summary>
/// 从业机构（银行）汇总电子回单应答（<b>POST 受理支与 GET 查询支共用</b>本类型）。
/// </summary>
/// <remarks>
/// <para><b>官方文档</b>：受理 <see href="https://pay.weixin.qq.com/doc/v3/partner/4012470746"/>、
/// 查询受理结果 <see href="https://pay.weixin.qq.com/doc/v3/partner/4012470761"/>，
/// 2026-10-14 逐页比对字段表<b>完全相同</b> ⇒ 共用一型（守卫 BT-B6）。</para>
/// <para><b>除 <c>out_batch_no</c> 与 <c>receipt_no</c> 外全部选填</b>：
/// <c>hash_type</c> / <c>hash_value</c> / <c>download_url</c> 官方注明仅在 <c>receipt_state = FINISHED</c> 时返回
/// ⇒ 受理后立即读 <c>download_url</c> 会拿到 <c>null</c>，须按状态轮询。</para>
/// <para><b><c>download_url</c> 不进生成式接口</b>：回单文件下载走该动态路由且响应<b>非 JSON</b>，
/// 与账单下载通道同款形态 ⇒ 本域只建模「取 URL」，不做取文件（守卫 BT-B2 零声明断言）。</para>
/// </remarks>
[HttpJsonSerializable(SerializerClassName = "BankTransfer")]
public class BankTransferSummaryReceiptResponse : WechatPayResponse
{
    /// <summary>银行批次单号（<c>out_batch_no</c>，必填 string(32)）。</summary>
    [JsonPropertyName("out_batch_no")]
    public string? OutBatchNo { get; set; }

    /// <summary>电子回单编号（<c>receipt_no</c>，必填 string(256)）：微信侧回单唯一标识。</summary>
    [JsonPropertyName("receipt_no")]
    public string? ReceiptNo { get; set; }

    /// <summary>电子回单状态（<c>receipt_state</c>，选填，2 值）：见 <see cref="BankTransferReceiptStates"/>。</summary>
    [JsonPropertyName("receipt_state")]
    public string? ReceiptState { get; set; }

    /// <summary>回单文件哈希方法（<c>hash_type</c>，选填 string(20)）：官方注明当前版本为 SHA256。</summary>
    [JsonPropertyName("hash_type")]
    public string? HashType { get; set; }

    /// <summary>回单文件哈希值（<c>hash_value</c>，选填 string(1000)）：用于下载后校验文件完整性。</summary>
    [JsonPropertyName("hash_value")]
    public string? HashValue { get; set; }

    /// <summary>回单文件下载地址（<c>download_url</c>，选填 string(3000)）：<b>动态路由 + 非 JSON</b>，不经生成式接口。</summary>
    [JsonPropertyName("download_url")]
    public string? DownloadUrl { get; set; }

    /// <summary>回单受理时间（<c>create_time</c>，选填 string(32)）：rfc3339。</summary>
    [JsonPropertyName("create_time")]
    public string? CreateTime { get; set; }

    /// <summary>回单状态变更时间（<c>update_time</c>，选填 string(32)）：rfc3339。</summary>
    [JsonPropertyName("update_time")]
    public string? UpdateTime { get; set; }
}

/// <summary>
/// 从业机构（银行）「受理明细电子回单」请求（<b>POST /v3/bank-batch-transfer/receipt/detail-receipts</b> 的包体）。
/// </summary>
/// <remarks>
/// <para><b>官方文档</b>：<see href="https://pay.weixin.qq.com/doc/v3/partner/4012470770"/>，2026-10-14 逐字段核验。</para>
/// <para><b>定位键两支合用</b>：POST 走包体、GET 走 query，参数名同为 <c>out_batch_no</c> + <c>out_detail_no</c>
/// （汇总支只需 <c>out_batch_no</c>）⇒ 与本域转账族「按微信单号 / 按商家单号」四支分列的定位方式同源。</para>
/// </remarks>
[HttpJsonSerializable(SerializerClassName = "BankTransfer")]
public class BankTransferDetailReceiptRequest
{
    /// <summary>银行批次单号（<c>out_batch_no</c>，必填 string(32)）。</summary>
    [JsonPropertyName("out_batch_no")]
    public string? OutBatchNo { get; set; }

    /// <summary>银行明细单号（<c>out_detail_no</c>，必填 string(32)）：批次单下唯一。</summary>
    [JsonPropertyName("out_detail_no")]
    public string? OutDetailNo { get; set; }
}

/// <summary>
/// 从业机构（银行）明细电子回单应答（<b>POST 受理支与 GET 查询支共用</b>本类型）。
/// </summary>
/// <remarks>
/// <para><b>官方文档</b>：受理 <see href="https://pay.weixin.qq.com/doc/v3/partner/4012470770"/>、
/// 查询受理结果 <see href="https://pay.weixin.qq.com/doc/v3/partner/4012470775"/>，
/// 2026-10-14 逐页比对字段表<b>完全相同</b> ⇒ 共用一型（守卫 BT-B6）。</para>
/// <para><b>与汇总应答不是同一形态</b>：本应答多出 <c>out_detail_no</c>、但<b>没有</b>
/// <c>create_time</c> / <c>update_time</c> ⇒ 「表相同则共用，表不同则分建」，两型不互为别名。</para>
/// </remarks>
[HttpJsonSerializable(SerializerClassName = "BankTransfer")]
public class BankTransferDetailReceiptResponse : WechatPayResponse
{
    /// <summary>银行批次单号（<c>out_batch_no</c>，必填 string(32)）。</summary>
    [JsonPropertyName("out_batch_no")]
    public string? OutBatchNo { get; set; }

    /// <summary>银行明细单号（<c>out_detail_no</c>，必填 string(32)）。</summary>
    [JsonPropertyName("out_detail_no")]
    public string? OutDetailNo { get; set; }

    /// <summary>电子回单编号（<c>receipt_no</c>，必填 string(256)）。</summary>
    [JsonPropertyName("receipt_no")]
    public string? ReceiptNo { get; set; }

    /// <summary>电子回单状态（<c>receipt_state</c>，选填，2 值）：见 <see cref="BankTransferReceiptStates"/>。</summary>
    [JsonPropertyName("receipt_state")]
    public string? ReceiptState { get; set; }

    /// <summary>回单文件哈希方法（<c>hash_type</c>，选填 string(20)）：<c>FINISHED</c> 时返回。</summary>
    [JsonPropertyName("hash_type")]
    public string? HashType { get; set; }

    /// <summary>回单文件哈希值（<c>hash_value</c>，选填 string(1000)）。</summary>
    [JsonPropertyName("hash_value")]
    public string? HashValue { get; set; }

    /// <summary>回单文件下载地址（<c>download_url</c>，选填 string(3000)）：<b>动态路由 + 非 JSON</b>，不经生成式接口。</summary>
    [JsonPropertyName("download_url")]
    public string? DownloadUrl { get; set; }
}
