// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

namespace Mud.Wechat.MiniProgram.DataModels.XPay;

/// <summary>
/// 「绑定广告金充值账户」请求体（<c>POST /xpay/bind_transfer_accout</c>）。
/// </summary>
/// <remarks>
/// <para><b>路由名官方拼写为 <c>accout</c>（缺字母 n）</b> ⇒ 路由照官方原文，SDK 类型与方法名用正确拼写 <c>BindTransferAccountAsync</c>；该拼写由守卫 XP1 逐字锁定，<b>不得</b>「顺手修正」路由。</para>
/// <para><b>三支字段官方全部标注非必填</b>（本域唯一），但绑定操作至少需定位一个账户 ⇒ 语义必填由官方正文承担、表格不承担，SDK 不校验。</para>
/// <para>官方文档：<see href="https://developers.weixin.qq.com/miniprogram/dev/server/API/VirtualPayment/api_bind_transfer_accout.html"/></para>
/// </remarks>
[HttpJsonSerializable(SerializerClassName = "XPay")]
public class WxaXPayBindTransferAccountRequest
{
    /// <summary>transfer_account_uid（官方类型 <c>number</c>）：充值账户 uid</summary>
    [JsonPropertyName("transfer_account_uid")]
    public long? TransferAccountUid { get; set; }

    /// <summary>transfer_account_org_name（官方类型 <c>string</c>）：充值账户主体名称</summary>
    [JsonPropertyName("transfer_account_org_name")]
    public string? TransferAccountOrgName { get; set; }

    /// <summary>env（官方类型 <c>number</c>）：0-正式环境 1-沙箱环境（仅作为签名校验，查询的结果都是正式环境的）</summary>
    [JsonPropertyName("env")]
    public long? Env { get; set; }
}
