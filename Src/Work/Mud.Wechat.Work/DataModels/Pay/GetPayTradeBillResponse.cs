// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

namespace Mud.Wechat.Work.DataModels.Pay;

/// <summary>
/// 交易账单申请响应体（<c>/cgi-bin/miniapppay/get_bill</c>，交易账单域）。
/// <para>
/// 官方业务限制：账单明细数据在下载的文件里（明细数据表头/明细数据内容/汇总数据表头/汇总数据
/// 四部分，字段以英文逗号分隔）；下载地址 30 秒内有效；下载账单文件时以 <see cref="Auth"/>
/// 作为 https 校验的 Authorization 请求头。
/// </para>
/// </summary>
[HttpJsonSerializable(SerializerClassName = "Pay")]
public class GetPayTradeBillResponse : WechatWorkResponse
{
    /// <summary>
    /// 获取或设置哈希类型（官方必填）：SHA1 - SHA1 值。
    /// </summary>
    [JsonPropertyName("hash_type")]
    public string? HashType { get; set; }

    /// <summary>
    /// 获取或设置哈希值（官方必填，1~1024 个字符）：
    /// 原始账单（gzip 需要解压缩）的摘要值，用于校验文件的完整性。
    /// </summary>
    [JsonPropertyName("hash_value")]
    public string? HashValue { get; set; }

    /// <summary>
    /// 获取或设置账单下载地址（官方必填，1~2048 个字符）：
    /// 供下一步请求账单文件的下载地址，该地址 30 秒内有效。
    /// </summary>
    [JsonPropertyName("download_url")]
    public string? DownloadUrl { get; set; }

    /// <summary>
    /// 获取或设置校验头（官方可选，1~2048 个字符）：
    /// https 请求校验头，用于作为 https 校验的 Authorization。
    /// </summary>
    [JsonPropertyName("auth")]
    public string? Auth { get; set; }
}
