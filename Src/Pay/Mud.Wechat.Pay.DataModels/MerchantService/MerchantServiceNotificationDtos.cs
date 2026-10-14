// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

using Mud.Wechat.Pay.DataModels.Common;

namespace Mud.Wechat.Pay.DataModels.MerchantService;

/// <summary>
/// 投诉通知回调地址写入请求（官方 <c>url</c>）：<b>创建（POST）与更新（PUT）两支共用</b>。
/// </summary>
/// <remarks>
/// <para><b>官方文档</b>：<see href="https://pay.weixin.qq.com/doc/v3/merchant/4012458679"/>（创建）与
/// <see href="https://pay.weixin.qq.com/doc/v3/merchant/4012459282"/>（更新），2026-10-14 两页逐行比对：
/// 请求体仅 <c>url string(255) 必填</c> 一行，<b>两页完全一致</b> ⇒ 共用一型（守卫 MC-B6）。</para>
/// <para><b>POST 是「创建」而非「 upsert」</b>：官方把创建与更新做成两支不同动词，
/// 已存在回调地址时重复 POST 的行为官方未述 ⇒ 需要改地址走 <c>Update</c>，不要靠 POST 兜。</para>
/// <para><b>仅支持 HTTPS</b>（官方原文）⇒ <c>http://</c> 会被拒；本 SDK 不做 URL 形态校验，
/// 但宿主注意本值<b>不经过</b> <c>WechatApiHosts</c> 白名单（那是出向请求的防线，这里是入向推送地址）。</para>
/// </remarks>
[HttpJsonSerializable(SerializerClassName = "MerchantService")]
public class MerchantServiceComplaintNotificationUrlRequest
{
    /// <summary>通知地址（<c>url</c>，必填 string(255)）：仅支持 HTTPS。</summary>
    [JsonPropertyName("url")]
    public string? Url { get; set; }
}

/// <summary>
/// 投诉通知回调地址应答（官方 <c>{mchid, url}</c>）：<b>创建 / 更新 / 查询三支共用</b>。
/// </summary>
/// <remarks>
/// <para><b>官方文档</b>：<see href="https://pay.weixin.qq.com/doc/v3/merchant/4012458679"/>、
/// <see href="https://pay.weixin.qq.com/doc/v3/merchant/4012459282"/>、
/// <see href="https://pay.weixin.qq.com/doc/v3/merchant/4012459014"/>，2026-10-14 三页逐行比对：
/// 应答均为两键且均必填，仅描述措辞不同 ⇒ 共用一型（守卫 MC-B6）。</para>
/// <para><b>删除支（DELETE）无应答包体</b>（204）⇒ 本型不挂在删除支的返回上；
/// 要确认删除生效只能再调查询支。</para>
/// <para><b>查询支是「配置读回」而非列表</b>：官方返回单个 <c>{mchid, url}</c>，
/// 即一个商户号只有一个回调地址，无分页、无多条。</para>
/// </remarks>
[HttpJsonSerializable(SerializerClassName = "MerchantService")]
public class MerchantServiceComplaintNotificationResponse : WechatPayResponse
{
    /// <summary>商户号（<c>mchid</c>，必填 string(64)）：由微信支付生成并下发。</summary>
    [JsonPropertyName("mchid")]
    public string? MchId { get; set; }

    /// <summary>通知地址（<c>url</c>，必填 string(255)）：当前生效的回调地址。</summary>
    [JsonPropertyName("url")]
    public string? Url { get; set; }
}
