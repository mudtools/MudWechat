// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

using Mud.Wechat.Pay.DataModels.Common;

namespace Mud.Wechat.Pay.DataModels.Bill;

/// <summary>
/// 「账单下载信息列表」应答（微信支付 APIv3；服务商查询子商户资金账单 / 支付分商户账单<b>共用</b>）。
/// </summary>
/// <remarks>
/// <para>
/// <b>官方文档</b>：服务商查询子商户资金账单
/// <see href="https://pay.weixin.qq.com/doc/v3/partner/4012760249"/>（2026-10-18 逐字段核验，
/// 支持商户【服务商】/【平台商户】）；支付分商户账单
/// <see href="https://pay.weixin.qq.com/wiki/doc/apiv3/apis/chapter6_1_29.shtml"/>（2026-10-18 逐字段核验，
/// 支持商户【普通商户】）。
/// </para>
/// <para>
/// <b>与直连单文件账单是两套应答形态</b>：<c>/v3/bill/tradebill</c> 与 <c>/v3/bill/fundflowbill</c> 回
/// <b>单文件</b>三字段（<see cref="BillDownloadInfoResponse"/>）；本表的两个端点回
/// <b>文件分片列表</b>（<c>download_bill_count</c> + <c>download_bill_list[]</c>）⇒ 必须按
/// <c>bill_sequence</c> 升序把多个分片合并成一份完整账单，直连那套「下载一个文件即账单」的处理逻辑照搬过来会<b>丢数据</b>。
/// </para>
/// <para>
/// <b>分片是加密的</b>：每个分片自带 <c>encrypt_key</c> 与 <c>nonce</c>，须按官方指定的
/// <c>AEAD_AES_256_GCM</c> 逐片解密后再拼接。<c>encrypt_key</c> 属凭据类材料，
/// <b>不得</b>写入日志、遥测或异常消息。
/// </para>
/// <para>
/// <b>两端点共享本 DTO 的依据</b>：官方两页的应答字段表<b>逐项一致</b>（<c>download_bill_count</c> /
/// <c>download_bill_list</c>，元素同为 6 字段）⇒ 同表共用一型，防两处字段各自漂移
/// （同本仓「表相同则共用、表不同则分建」口径）。
/// </para>
/// <para>
/// <b>申请 ≠ 下载</b>：<c>download_url</c> <b>5 分钟内有效</b>且只可下载<b>一次</b>；
/// 下载后须用 <c>hash_value</c>（SHA1）校验完整性。
/// </para>
/// </remarks>
[HttpJsonSerializable(SerializerClassName = "Bill")]
public class BillDownloadInfoListResponse : WechatPayResponse
{
    /// <summary>下载信息总数（<c>download_bill_count</c>，必填 integer）：即分片数，也是「合并几个文件」的权威计数。</summary>
    [JsonPropertyName("download_bill_count")]
    public int? DownloadBillCount { get; set; }

    /// <summary>下载信息明细（<c>download_bill_list</c>，必填数组）：按
    /// <see cref="BillDownloadInfoItem.BillSequence"/> 升序合并为完整账单。</summary>
    [JsonPropertyName("download_bill_list")]
    public List<BillDownloadInfoItem>? DownloadBillList { get; set; }
}

/// <summary>
/// 账单下载信息的<b>单个文件分片</b>（<c>download_bill_list[]</c> 元素，6 字段，2026-10-18 逐表核验）。
/// </summary>
/// <remarks>
/// <c>bill_sequence</c> <b>起始值为 1</b>（不是 0）⇒ 宿主按序号拼文件时不要以 0 为基。
/// 每片自带 <c>hash_value</c> 与 <c>encrypt_key</c>，逐片校验、逐片解密后再拼接。
/// </remarks>
[HttpJsonSerializable(SerializerClassName = "Bill")]
public class BillDownloadInfoItem
{
    /// <summary>文件分片序号（<c>bill_sequence</c>，必填 integer，<b>起始值为 1</b>）。</summary>
    [JsonPropertyName("bill_sequence")]
    public int? BillSequence { get; set; }

    /// <summary>哈希类型（<c>hash_type</c>，必填 string(32)），固定 <c>SHA1</c>。</summary>
    [JsonPropertyName("hash_type")]
    public string? HashType { get; set; }

    /// <summary>哈希值（<c>hash_value</c>，必填 string(1024)），分片解密前的 SHA1 摘要，用于校验完整性。</summary>
    [JsonPropertyName("hash_value")]
    public string? HashValue { get; set; }

    /// <summary>分片下载地址（<c>download_url</c>，必填 string(2048)，<b>5 分钟内有效且只可下载一次</b>）。</summary>
    [JsonPropertyName("download_url")]
    public string? DownloadUrl { get; set; }

    /// <summary>解密密钥（<c>encrypt_key</c>，必填 string(1024)，<c>AEAD_AES_256_GCM</c> 的 Base64 密钥材料）。
    /// <b>属凭据类材料，不得写入日志、遥测或异常消息</b>。</summary>
    [JsonPropertyName("encrypt_key")]
    public string? EncryptKey { get; set; }

    /// <summary>解密随机串（<c>nonce</c>，必填 string(32)，<c>AEAD_AES_256_GCM</c> 的 nonce）。</summary>
    [JsonPropertyName("nonce")]
    public string? Nonce { get; set; }
}
