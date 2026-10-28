// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

using Mud.Wechat.Pay.DataModels.Common;

namespace Mud.Wechat.Pay.DataModels.Bill;

/// <summary>
/// 申请账单应答（微信支付 APIv3；申请交易账单 / 申请资金账单<b>共用同一应答体</b>）。
/// </summary>
/// <remarks>
/// <para>
/// <b>官方文档</b>：申请交易账单 <see href="https://pay.weixin.qq.com/doc/v3/merchant/4012791907"/>
/// （同内容另有 <c>4013071227</c> 入口）；申请资金账单
/// <see href="https://pay.weixin.qq.com/doc/v3/merchant/4012791908"/>（同内容另有 <c>4013071235</c> 入口）
/// （2026-10-09 逐字段核验）。
/// </para>
/// <para>
/// <b>两端点共享本 DTO 的依据</b>：官方对两者给出<b>一致的应答字段表</b>
/// （<c>hash_type</c> / <c>hash_value</c> / <c>download_url</c>），仅请求查询参数不同
/// （交易账单按 <c>bill_type</c>、资金账单按 <c>account_type</c>）。
/// </para>
/// <para>
/// <b>申请 ≠ 下载</b>：本应答给出的是<b>下载地址</b>，账单文件本身须再由
/// <c>/v3/billdownload/file</c> 拉取（见 <c>IWechatPayBillDownloadService</c>）。
/// <c>download_url</c> <b>有效期仅 5 分钟</b>，且只可下载<b>一次</b>。
/// </para>
/// </remarks>
[HttpJsonSerializable(SerializerClassName = "Bill")]
public class BillDownloadInfoResponse : WechatPayResponse
{
    /// <summary>哈希类型（<c>hash_type</c>，必填 string(32)），固定 <c>SHA1</c>。</summary>
    [JsonPropertyName("hash_type")]
    public string? HashType { get; set; }

    /// <summary>哈希值（<c>hash_value</c>，必填 string(1024)），账单文件的 SHA1 摘要，用于下载后校验完整性。</summary>
    [JsonPropertyName("hash_value")]
    public string? HashValue { get; set; }

    /// <summary>账单下载地址（<c>download_url</c>，必填 string(2048)，<b>5 分钟内有效</b>）。</summary>
    [JsonPropertyName("download_url")]
    public string? DownloadUrl { get; set; }
}
