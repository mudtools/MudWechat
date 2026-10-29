// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

using Mud.Wechat.Pay.DataModels.Common;

namespace Mud.Wechat.Pay.DataModels.Ecommerce;

/// <summary>
/// 申请二级商户资金流水账单（<c>GET /v3/ecommerce/bill/fundflowbill</c>）。
/// </summary>
/// <remarks>
/// <para>
/// <b>官方文档</b>：<see href="https://pay.weixin.qq.com/doc/v3/partner/4012760697"/>
/// （2026-10-15 逐字段核验；更新时间 2024.10.23；支持商户：【平台商户】）。
/// </para>
/// <para>
/// <b>与直连资金账单是两套应答形态</b>：直连 <c>GET /v3/bill/fundflowbill</c> 回<b>单文件</b>三字段
/// （<c>hash_type</c>/<c>hash_value</c>/<c>download_url</c>），本支回<b>文件分片列表</b>
/// （<c>download_bill_count</c> + <c>download_bill_list[]</c>）⇒ 必须按 <c>bill_sequence</c> 顺序
/// 把多个文件合并成一份完整账单，直连那套「下载一个文件即账单」的处理逻辑照搬过来会<b>丢数据</b>。
/// </para>
/// <para>
/// <b><c>account_type</c> 只接受 <c>ALL</c>、<c>algorithm</c> 必填</b>（官方原文「本接口只支持填ALL」）：
/// 这两点都与直连不同（直连按账户类型分列、无加密算法必填要求）。
/// </para>
/// <para>
/// <b><c>download_url</c> 五分钟内有效</b>且 <c>encrypt_key</c> 是<b>用商户证书公钥加密后再 Base64</b> 的密钥
/// ⇒ 取到地址须立刻下载，且必须先以自身 API 证书私钥解出密钥才能解密账单内容。
/// </para>
/// <para>
/// <b><c>tar_type</c> 选填</b>：不填即返回<b>未压缩</b>数据流，SDK 不代为补默认值（补 <c>GZIP</c> 会让宿主
/// 按 gzip 解压而失败）。
/// </para>
/// </remarks>
[HttpJsonSerializable(SerializerClassName = "Ecommerce")]
public class EcommerceFundFlowBillResponse : WechatPayResponse
{
    /// <summary>下载信息总数（<c>download_bill_count</c>，必填 integer）：即 <c>download_bill_list</c> 的分片数，也是「合并几个文件」的权威计数。</summary>
    [JsonPropertyName("download_bill_count")]
    public int? DownloadBillCount { get; set; }

    /// <summary>下载信息明细（<c>download_bill_list</c>，<b>选填</b>数组）：按 <see cref="EcommerceDownloadBillItem.BillSequence"/> 升序合并为完整账单。</summary>
    [JsonPropertyName("download_bill_list")]
    public IReadOnlyList<EcommerceDownloadBillItem>? DownloadBillList { get; set; }
}

/// <summary>
/// 资金流水账单的<b>单个文件分片</b>（<c>download_bill_list[]</c> 的元素形态）。
/// </summary>
/// <remarks>
/// 官方注明 <c>bill_sequence</c> <b>起始值为 1</b>（不是 0）⇒ 宿主按序号拼文件时不要以 0 为基。
/// 每片自带 <c>hash_value</c> 与 <c>encrypt_key</c>，逐片校验、逐片解密后再拼接。
/// </remarks>
[HttpJsonSerializable(SerializerClassName = "Ecommerce")]
public class EcommerceDownloadBillItem
{
    /// <summary>账单文件序号（<c>bill_sequence</c>，必填 integer）：<b>从 1 起</b>，多个文件按序号合并为完整账单。</summary>
    [JsonPropertyName("bill_sequence")]
    public int? BillSequence { get; set; }

    /// <summary>哈希类型（<c>hash_type</c>，必填）：本页唯一取值 <c>SHA1</c>，见 <see cref="EcommerceBillHashTypes"/>。</summary>
    [JsonPropertyName("hash_type")]
    public string? HashType { get; set; }

    /// <summary>哈希值（<c>hash_value</c>，必填 string(1024)）：原始账单（gzip 需先解压）的摘要，用于校验完整性。</summary>
    [JsonPropertyName("hash_value")]
    public string? HashValue { get; set; }

    /// <summary>下载地址（<c>download_url</c>，必填 string(2048)）：<b>5 分钟内有效</b>。</summary>
    [JsonPropertyName("download_url")]
    public string? DownloadUrl { get; set; }

    /// <summary>
    /// 加密密钥（<c>encrypt_key</c>，必填 string(512)）：以<b>商户证书公钥</b>加密后 Base64 编码，
    /// 须用自身 API 客户端私钥解出后才是账单的对称密钥。
    /// </summary>
    [JsonPropertyName("encrypt_key")]
    public string? EncryptKey { get; set; }

    /// <summary>随机字符串（<c>nonce</c>，必填 string(16)）：解密账单时与 <see cref="EncryptKey"/> 配套使用。</summary>
    [JsonPropertyName("nonce")]
    public string? Nonce { get; set; }
}
